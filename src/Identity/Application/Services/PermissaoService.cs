// ============================================================================
// ARQUIVO ATUALIZADO - FASE 3:
// src/Identity/Application/Services/PermissaoService.cs
// ============================================================================
//
// Este arquivo implementa a lógica de agregação das permissões do usuário
// usando o repositório de permissões do legado.
//
// ATUALIZAÇÃO:
// - Adicionado método ValidarPermissaoAsync para validação detalhada
// - Melhoria no tratamento de erros e logging
// ============================================================================

using Microsoft.Extensions.Logging;
using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Identity.Application.DTOs.Permissoes;
using RhSensoERP.Identity.Infrastructure.Repositories;

namespace RhSensoERP.Identity.Application.Services;

/// <summary>
/// Implementação do serviço de permissões, responsável por
/// carregar funções e botões do usuário a partir das tabelas legadas.
/// </summary>
public sealed class PermissaoService : IPermissaoService
{
    private readonly IPermissaoRepository _permissaoRepository;
    private readonly ILogger<PermissaoService> _logger;

    public PermissaoService(
        IPermissaoRepository permissaoRepository,
        ILogger<PermissaoService> logger)
    {
        _permissaoRepository = permissaoRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<UserPermissionsDto> CarregarPermissoesAsync(
        string cdUsuario,
        string? cdSistema = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(cdUsuario))
            throw new ArgumentException("cdUsuario é obrigatório.", nameof(cdUsuario));

        _logger.LogInformation(
            "🔐 Carregando permissões para usuário {User} (Sistema: {Sistema})",
            cdUsuario,
            cdSistema ?? "TODOS");

        // Busca as funções + botões do usuário no legado
        List<FuncaoPermissaoDto> funcoes =
            await _permissaoRepository.GetPermissoesDoUsuarioAsync(cdUsuario, cdSistema, ct);

        var result = new UserPermissionsDto();

        // ==========================
        // Funções (telas/módulos)
        // ==========================
        result.Funcoes = funcoes
            .Select(f => new UserFuncaoDto
            {
                CdFuncao = f.CdFuncao,
                DcFuncao = f.DcFuncao,
                CdSistema = f.CdSistema,
                // Ações vêm do DTO do repositório (ex: "IAEC")
                CdAcoes = f.Acoes,
                // Restrição ainda não vem do legado → default neutro
                CdRestric = 'N'
            })
            .ToList();

        // ==========================
        // Botões por função
        // ==========================
        result.Botoes = funcoes
            .SelectMany(f => f.Botoes.Select(b => new UserBotaoDto
            {
                CdFuncao = f.CdFuncao,
                // Hoje o DTO de botão tem NmBotao, não CdBotao.
                // Usamos NmBotao como identificador lógico.
                CdBotao = b.NmBotao,
                DcBotao = b.DcBotao
            }))
            .ToList();

        // Grupos ainda não estão sendo carregados pelo repositório atual.
        // Quando as tabelas de grupos forem mapeadas no repositório,
        // basta preencher result.Grupos aqui.

        _logger.LogInformation(
            "✅ Permissões carregadas. Funções: {Funcoes}, Botões: {Botoes}",
            result.Funcoes.Count,
            result.Botoes.Count);

        return result;
    }

    /// <inheritdoc />
    public async Task<bool> TemPermissaoAsync(
        string cdUsuario,
        string cdFuncao,
        char acao,
        string? cdSistema = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(cdUsuario))
            throw new ArgumentException("cdUsuario é obrigatório.", nameof(cdUsuario));

        if (string.IsNullOrWhiteSpace(cdFuncao))
            throw new ArgumentException("cdFuncao é obrigatório.", nameof(cdFuncao));

        var funcoes = await _permissaoRepository.GetPermissoesDoUsuarioAsync(cdUsuario, cdSistema, ct);

        var funcao = funcoes.FirstOrDefault(f =>
            f.CdFuncao == cdFuncao &&
            (cdSistema == null || f.CdSistema == cdSistema));

        if (funcao is null)
            return false;

        return !string.IsNullOrEmpty(funcao.Acoes) &&
               funcao.Acoes.Contains(acao);
    }

    /// <inheritdoc />
    public async Task<ValidarPermissaoResponse> ValidarPermissaoAsync(
        ValidarPermissaoRequest request,
        CancellationToken ct = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.CdUsuario))
            throw new ArgumentException("cdUsuario é obrigatório.", nameof(request.CdUsuario));

        if (string.IsNullOrWhiteSpace(request.CdSistema))
            throw new ArgumentException("cdSistema é obrigatório.", nameof(request.CdSistema));

        if (string.IsNullOrWhiteSpace(request.CdFuncao))
            throw new ArgumentException("cdFuncao é obrigatório.", nameof(request.CdFuncao));

        _logger.LogInformation(
            "🔍 Validando permissão: Usuário={User}, Sistema={Sistema}, Função={Funcao}, Ação={Acao}",
            request.CdUsuario,
            request.CdSistema,
            request.CdFuncao,
            request.Acao);

        var response = new ValidarPermissaoResponse
        {
            CdUsuario = request.CdUsuario,
            CdSistema = request.CdSistema,
            CdFuncao = request.CdFuncao,
            Acao = request.Acao,
            DescricaoAcao = ObterDescricaoAcao(request.Acao)
        };

        try
        {
            // Busca as permissões do usuário para o sistema especificado
            var funcoes = await _permissaoRepository.GetPermissoesDoUsuarioAsync(
                request.CdUsuario,
                request.CdSistema,
                ct);

            // Busca a função específica
            var funcao = funcoes.FirstOrDefault(f =>
                f.CdFuncao.Equals(request.CdFuncao, StringComparison.OrdinalIgnoreCase) &&
                f.CdSistema.Equals(request.CdSistema, StringComparison.OrdinalIgnoreCase));

            if (funcao == null)
            {
                response.TemPermissao = false;
                response.Motivo = $"Usuário não possui acesso à função '{request.CdFuncao}' no sistema '{request.CdSistema}'";
                response.AcoesDisponiveis = string.Empty;

                _logger.LogWarning(
                    "❌ Permissão negada: Usuário {User} não tem acesso à função {Funcao}",
                    request.CdUsuario,
                    request.CdFuncao);

                return response;
            }

            // Armazena as ações disponíveis
            response.AcoesDisponiveis = funcao.Acoes ?? string.Empty;

            // Verifica se a ação específica está presente
            if (string.IsNullOrEmpty(funcao.Acoes) || !funcao.Acoes.Contains(request.Acao))
            {
                response.TemPermissao = false;
                response.Motivo = $"Usuário não possui permissão de '{response.DescricaoAcao}' para esta função. " +
                                  $"Ações disponíveis: {FormatarAcoesDisponiveis(funcao.Acoes)}";

                _logger.LogWarning(
                    "❌ Permissão negada: Usuário {User} não tem ação '{Acao}' na função {Funcao}. Ações disponíveis: {Acoes}",
                    request.CdUsuario,
                    request.Acao,
                    request.CdFuncao,
                    funcao.Acoes);

                return response;
            }

            // Permissão concedida
            response.TemPermissao = true;

            _logger.LogInformation(
                "✅ Permissão concedida: Usuário {User} tem ação '{Acao}' na função {Funcao}",
                request.CdUsuario,
                request.Acao,
                request.CdFuncao);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ Erro ao validar permissão para usuário {User}, função {Funcao}",
                request.CdUsuario,
                request.CdFuncao);

            response.TemPermissao = false;
            response.Motivo = "Erro ao validar permissão. Contate o administrador do sistema.";
            return response;
        }
    }

    /// <inheritdoc />
    public async Task<List<string>> ObterFuncoesPermitidasAsync(
        string cdUsuario,
        string? cdSistema = null,
        CancellationToken ct = default)
    {
        var funcoes = await _permissaoRepository.GetPermissoesDoUsuarioAsync(cdUsuario, cdSistema, ct);

        return funcoes
            .Select(f => f.CdFuncao)
            .Distinct()
            .OrderBy(f => f)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<List<string>> ObterBotoesPermitidosAsync(
        string cdUsuario,
        string cdFuncao,
        string? cdSistema = null,
        CancellationToken ct = default)
    {
        var funcoes = await _permissaoRepository.GetPermissoesDoUsuarioAsync(cdUsuario, cdSistema, ct);

        var funcao = funcoes.FirstOrDefault(f =>
            f.CdFuncao == cdFuncao &&
            (cdSistema == null || f.CdSistema == cdSistema));

        if (funcao is null)
            return new List<string>();

        return funcao.Botoes
            .Select(b => b.NmBotao)
            .Distinct()
            .OrderBy(b => b)
            .ToList();
    }

    #region Métodos Auxiliares

    /// <summary>
    /// Obtém a descrição amigável da ação
    /// </summary>
    private static string ObterDescricaoAcao(char acao) => acao switch
    {
        'I' => "Incluir",
        'A' => "Alterar",
        'E' => "Excluir",
        'C' => "Consultar",
        _ => acao.ToString()
    };

    /// <summary>
    /// Formata as ações disponíveis de forma amigável
    /// </summary>
    private static string FormatarAcoesDisponiveis(string? acoes)
    {
        if (string.IsNullOrEmpty(acoes))
            return "Nenhuma";

        var descricoes = acoes
            .Select(a => ObterDescricaoAcao(a))
            .ToList();

        return string.Join(", ", descricoes);
    }

    #endregion

    /// <inheritdoc />
    public async Task<TogglePermissaoResponse> TogglePermissaoAsync(
        TogglePermissaoRequest request,
        CancellationToken ct = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.CdUsuario))
            throw new ArgumentException("cdUsuario é obrigatório.", nameof(request.CdUsuario));

        if (string.IsNullOrWhiteSpace(request.CdSistema))
            throw new ArgumentException("cdSistema é obrigatório.", nameof(request.CdSistema));

        if (string.IsNullOrWhiteSpace(request.CdFuncao))
            throw new ArgumentException("cdFuncao é obrigatório.", nameof(request.CdFuncao));

        _logger.LogInformation(
            "🔄 Toggle permissão: Usuário={User}, Sistema={Sistema}, Função={Funcao}, Ação={Acao}, Enabled={Enabled}",
            request.CdUsuario,
            request.CdSistema,
            request.CdFuncao,
            request.Acao,
            request.Enabled);

        try
        {
            var result = await _permissaoRepository.TogglePermissaoAsync(
                request.CdUsuario,
                request.CdSistema,
                request.CdFuncao,
                request.Acao,
                request.Enabled,
                ct);

            if (result.Success)
            {
                _logger.LogInformation(
                    "✅ Permissão atualizada: Grupo={Grupo}, Ações={Acoes}",
                    result.CdGrUser,
                    result.CdAcoesAtualizado);
            }
            else
            {
                _logger.LogWarning("⚠️ Falha ao toggle permissão: {Message}", result.Message);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao toggle permissão para usuário {User}", request.CdUsuario);
            return new TogglePermissaoResponse
            {
                Success = false,
                Message = "Erro ao atualizar permissão: " + ex.Message
            };
        }
    }
}
