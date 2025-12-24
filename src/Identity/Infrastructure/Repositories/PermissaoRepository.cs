// =============================================================================
// src/Identity/Infrastructure/Repositories/PermissaoRepository.cs
// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RhSensoERP.Identity.Application.DTOs.Permissoes;
using RhSensoERP.Identity.Core.Entities;
using RhSensoERP.Identity.Infrastructure.Persistence.Contexts;

namespace RhSensoERP.Identity.Infrastructure.Repositories;

public interface IPermissaoRepository
{
    Task<List<FuncaoPermissaoDto>> GetPermissoesDoUsuarioAsync(
        string cdUsuario,
        string? cdSistema,
        CancellationToken ct);

    /// <summary>
    /// Atualiza as ações (IAEC) de uma permissão de grupo para uma função.
    /// </summary>
    Task<TogglePermissaoResponse> TogglePermissaoAsync(
        string cdUsuario,
        string cdSistema,
        string cdFuncao,
        char acao,
        bool enabled,
        CancellationToken ct);
}

public sealed class PermissaoRepository : IPermissaoRepository
{
    private readonly IdentityDbContext _db;
    private readonly ILogger<PermissaoRepository> _logger;

    public PermissaoRepository(
        IdentityDbContext db,
        ILogger<PermissaoRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<FuncaoPermissaoDto>> GetPermissoesDoUsuarioAsync(
        string cdUsuario,
        string? cdSistema,
        CancellationToken ct)
    {
        // ================================================================
        // PASSO 1: Buscar grupos do usuário
        // ================================================================
        var grupos = _db.Set<UserGroup>().AsNoTracking()
            .Where(ug => ug.CdUsuario == cdUsuario);

        if (!string.IsNullOrWhiteSpace(cdSistema))
        {
            grupos = grupos.Where(ug => ug.CdSistema == cdSistema);
        }

        // ================================================================
        // PASSO 2: Join com permissões e funções (traduzível para SQL)
        // ================================================================
        var queryFuncoes =
            from ug in grupos
            join gf in _db.Set<GrupoFuncao>().AsNoTracking()
                on new { ug.CdSistema, ug.CdGrUser }
                equals new { CdSistema = gf.CdSistema!, gf.CdGrUser }
            join f in _db.Funcoes.AsNoTracking()
                on new { gf.CdSistema, gf.CdFuncao }
                equals new { f.CdSistema, f.CdFuncao }
            select new
            {
                gf.CdSistema,
                gf.CdFuncao,
                f.DcFuncao,
                f.DcModulo,
                gf.CdAcoes
            };

        var funcoes = await queryFuncoes
            .Distinct()
            .ToListAsync(ct);

        // ================================================================
        // PASSO 3: Buscar botões
        // ================================================================
        var chaves = funcoes
            .Select(x => new { x.CdSistema, x.CdFuncao })
            .Distinct()
            .ToHashSet();

        var todosBotoesDaQuery = cdSistema != null
            ? await _db.Set<BotaoFuncao>()
                .AsNoTracking()
                .Where(b => b.CdSistema == cdSistema)
                .ToListAsync(ct)
            : await _db.Set<BotaoFuncao>()
                .AsNoTracking()
                .ToListAsync(ct);

        var botoes = todosBotoesDaQuery
            .Where(b => chaves.Contains(new { b.CdSistema, b.CdFuncao }))
            .Select(b => new
            {
                b.CdSistema,
                b.CdFuncao,
                b.NmBotao,
                b.DcBotao,
                b.CdAcao
            })
            .ToList();

        // ================================================================
        // PASSO 4: Montar DTO final
        // ================================================================
        var result = funcoes
            .GroupBy(x => new
            {
                x.CdSistema,
                x.CdFuncao,
                x.DcFuncao,
                x.DcModulo
            })
            .Select(g => new FuncaoPermissaoDto
            {
                CdSistema = g.Key.CdSistema!,
                CdFuncao = g.Key.CdFuncao,
                DcFuncao = g.Key.DcFuncao,
                DcModulo = g.Key.DcModulo,
                Acoes = string.Concat(
                    g.Select(z => z.CdAcoes)
                     .Where(s => !string.IsNullOrWhiteSpace(s))
                     .SelectMany(s => s!)
                     .Distinct()
                     .OrderBy(c => c)),
                Botoes = botoes
                    .Where(b => b.CdSistema == g.Key.CdSistema &&
                                b.CdFuncao == g.Key.CdFuncao)
                    .Select(b => new BotaoDto
                    {
                        NmBotao = b.NmBotao,
                        DcBotao = b.DcBotao,
                        CdAcao = b.CdAcao.ToString()
                    })
                    .OrderBy(b => b.NmBotao)
                    .ToList()
            })
            .OrderBy(r => r.CdSistema)
            .ThenBy(r => r.DcModulo)
            .ThenBy(r => r.CdFuncao)
            .ToList();

        return result;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Usa SQL raw para compatibilidade com tabela legada hbrh1 
    /// que não possui campos de auditoria (CreatedAt, UpdatedAt, etc.)
    /// </remarks>
    public async Task<TogglePermissaoResponse> TogglePermissaoAsync(
        string cdUsuario,
        string cdSistema,
        string cdFuncao,
        char acao,
        bool enabled,
        CancellationToken ct)
    {
        try
        {
            // ================================================================
            // PASSO 1: Buscar grupo e CdAcoes usando SQL raw
            // (evita mapeamento de campos de auditoria inexistentes)
            // ================================================================
            var selectSql = @"
                SELECT TOP 1 
                    u.cdgruser AS CdGrUser,
                    h.cdsistema AS CdSistema,
                    h.cdfuncao AS CdFuncao,
                    ISNULL(h.cdacoes, '') AS CdAcoes
                FROM usrh1 u
                INNER JOIN hbrh1 h 
                    ON u.cdsistema = h.cdsistema 
                    AND u.cdgruser = h.cdgruser
                WHERE u.cdusuario = @cdUsuario 
                  AND h.cdsistema = @cdSistema 
                  AND h.cdfuncao = @cdFuncao";

            string? cdGrUser = null;
            string? cdSistemaDb = null;
            string? cdFuncaoDb = null;
            string cdAcoesAtual = string.Empty;

            var connection = _db.Database.GetDbConnection();

            try
            {
                await connection.OpenAsync(ct);
            }
            catch (InvalidOperationException)
            {
                // Conexão já está aberta
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = selectSql;
                command.CommandTimeout = 60;

                var paramUsuario = command.CreateParameter();
                paramUsuario.ParameterName = "@cdUsuario";
                paramUsuario.Value = cdUsuario;
                command.Parameters.Add(paramUsuario);

                var paramSistema = command.CreateParameter();
                paramSistema.ParameterName = "@cdSistema";
                paramSistema.Value = cdSistema;
                command.Parameters.Add(paramSistema);

                var paramFuncao = command.CreateParameter();
                paramFuncao.ParameterName = "@cdFuncao";
                paramFuncao.Value = cdFuncao;
                command.Parameters.Add(paramFuncao);

                using var reader = await command.ExecuteReaderAsync(ct);
                if (await reader.ReadAsync(ct))
                {
                    cdGrUser = reader["CdGrUser"]?.ToString()?.Trim();
                    cdSistemaDb = reader["CdSistema"]?.ToString();
                    cdFuncaoDb = reader["CdFuncao"]?.ToString();
                    cdAcoesAtual = reader["CdAcoes"]?.ToString()?.Trim() ?? string.Empty;
                }
            }

            // Verifica se encontrou
            if (string.IsNullOrEmpty(cdGrUser))
            {
                _logger.LogWarning(
                    "Toggle permissão falhou: Usuário {CdUsuario} não tem acesso à função {CdFuncao}/{CdSistema}",
                    cdUsuario, cdFuncao, cdSistema);

                return new TogglePermissaoResponse
                {
                    Success = false,
                    Message = $"Usuário '{cdUsuario}' não possui acesso à função '{cdFuncao}' no sistema '{cdSistema?.Trim()}'."
                };
            }

            // ================================================================
            // PASSO 2: Calcular novo CdAcoes
            // ================================================================
            var acaoUpper = char.ToUpper(acao);
            string novasAcoes;

            if (enabled)
            {
                // Adicionar ação se não existir
                if (!cdAcoesAtual.Contains(acaoUpper))
                {
                    novasAcoes = OrdenarAcoes(cdAcoesAtual + acaoUpper);
                }
                else
                {
                    // Já está habilitado
                    return new TogglePermissaoResponse
                    {
                        Success = true,
                        Message = $"Permissão '{GetDescricaoAcao(acaoUpper)}' já está habilitada.",
                        CdAcoesAtualizado = cdAcoesAtual,
                        CdGrUser = cdGrUser
                    };
                }
            }
            else
            {
                // Remover ação
                if (cdAcoesAtual.Contains(acaoUpper))
                {
                    novasAcoes = cdAcoesAtual.Replace(acaoUpper.ToString(), string.Empty);
                }
                else
                {
                    // Já está desabilitado
                    return new TogglePermissaoResponse
                    {
                        Success = true,
                        Message = $"Permissão '{GetDescricaoAcao(acaoUpper)}' já está desabilitada.",
                        CdAcoesAtualizado = cdAcoesAtual,
                        CdGrUser = cdGrUser
                    };
                }
            }

            // ================================================================
            // PASSO 3: Atualizar no banco usando SQL raw
            // ================================================================
            var updateSql = @"
                UPDATE hbrh1 
                SET cdacoes = @novasAcoes 
                WHERE cdsistema = @cdSistema 
                  AND cdgruser = @cdGrUser 
                  AND cdfuncao = @cdFuncao";

            using (var updateCommand = connection.CreateCommand())
            {
                updateCommand.CommandText = updateSql;
                updateCommand.CommandTimeout = 60;

                var paramNovasAcoes = updateCommand.CreateParameter();
                paramNovasAcoes.ParameterName = "@novasAcoes";
                paramNovasAcoes.Value = novasAcoes;
                updateCommand.Parameters.Add(paramNovasAcoes);

                var paramSistemaUpd = updateCommand.CreateParameter();
                paramSistemaUpd.ParameterName = "@cdSistema";
                paramSistemaUpd.Value = cdSistemaDb!; // Usar valor exato do banco
                updateCommand.Parameters.Add(paramSistemaUpd);

                var paramGrUser = updateCommand.CreateParameter();
                paramGrUser.ParameterName = "@cdGrUser";
                paramGrUser.Value = cdGrUser;
                updateCommand.Parameters.Add(paramGrUser);

                var paramFuncaoUpd = updateCommand.CreateParameter();
                paramFuncaoUpd.ParameterName = "@cdFuncao";
                paramFuncaoUpd.Value = cdFuncaoDb!; // Usar valor exato do banco
                updateCommand.Parameters.Add(paramFuncaoUpd);

                var rowsAffected = await updateCommand.ExecuteNonQueryAsync(ct);

                if (rowsAffected > 0)
                {
                    var operacao = enabled ? "habilitada" : "desabilitada";

                    _logger.LogInformation(
                        "✅ Permissão '{Acao}' {Operacao} para {CdUsuario} na função {CdFuncao}/{CdSistema}. Grupo: {CdGrUser}, Novas ações: {NovasAcoes}",
                        GetDescricaoAcao(acaoUpper), operacao, cdUsuario, cdFuncao, cdSistema?.Trim(), cdGrUser, novasAcoes);

                    return new TogglePermissaoResponse
                    {
                        Success = true,
                        Message = $"Permissão '{GetDescricaoAcao(acaoUpper)}' {operacao} com sucesso.",
                        CdAcoesAtualizado = novasAcoes,
                        CdGrUser = cdGrUser
                    };
                }
                else
                {
                    _logger.LogWarning(
                        "⚠️ Toggle permissão: Nenhum registro atualizado para {CdUsuario}/{CdFuncao}/{CdSistema}",
                        cdUsuario, cdFuncao, cdSistema);

                    return new TogglePermissaoResponse
                    {
                        Success = false,
                        Message = "Nenhum registro foi atualizado. Verifique se a função existe."
                    };
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ Erro ao toggle permissão: Usuário={CdUsuario}, Função={CdFuncao}, Sistema={CdSistema}, Ação={Acao}",
                cdUsuario, cdFuncao, cdSistema, acao);

            throw;
        }
    }

    #region Métodos Auxiliares

    /// <summary>
    /// Ordena as ações no padrão IAEC
    /// </summary>
    private static string OrdenarAcoes(string acoes)
    {
        const string ordem = "IAEC";
        return string.Concat(ordem.Where(c => acoes.Contains(c)));
    }

    /// <summary>
    /// Retorna a descrição da ação
    /// </summary>
    private static string GetDescricaoAcao(char acao) => acao switch
    {
        'I' => "Incluir",
        'A' => "Alterar",
        'E' => "Excluir",
        'C' => "Consultar",
        _ => acao.ToString()
    };

    #endregion
}