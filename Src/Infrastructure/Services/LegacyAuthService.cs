using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RhSensoERP.Infrastructure.Persistence;
using RhSensoERP.Application.Security.Auth.DTOs;
using RhSensoERP.Application.Security.Auth.Services;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Infrastructure.Auth;

// ALIAS PARA RESOLVER AMBIGUIDADE
using UserGroupEntity = RhSensoERP.Core.Security.Entities.UserGroup;
using UserGroupDto = RhSensoERP.Application.Security.Auth.DTOs.UserGroup;

namespace RhSensoERP.Infrastructure.Services;

/// <summary>
/// Serviço de autenticação para sistema legacy
/// Implementa validação contra tabelas tuse1, usrh1 e hbrh1
/// </summary>
public class LegacyAuthService : ILegacyAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtTokenService _jwtService;
    private readonly IConfiguration _config;

    public LegacyAuthService(AppDbContext context, JwtTokenService jwtService, IConfiguration config)
    {
        _context = context;
        _jwtService = jwtService;
        _config = config;
    }

    /// <summary>
    /// Autentica usuário no sistema legacy (tabela tuse1)
    /// </summary>
    public async Task<AuthResult> AuthenticateAsync(string cdusuario, string senha, CancellationToken ct = default)
    {
        try
        {
            // 1️⃣ BUSCA USUÁRIO ATIVO NO BANCO
            var user = await _context.Set<User>()
                .Where(u => u.FlAtivo == 'S' &&
                           u.CdUsuario == cdusuario &&
                           u.SenhaUser == senha)
                .FirstOrDefaultAsync(ct);

            if (user == null)
                return new AuthResult(false, "Usuário não encontrado, inativo ou senha incorreta", null, null);

            // 2️⃣ MONTA DADOS DA SESSÃO
            var userData = new UserSessionData(
                user.CdUsuario,
                user.DcUsuario,
                user.NmImpCche,
                user.TpUsuario,
                user.NoMatric,
                user.CdEmpresa,
                user.CdFilial,
                user.NoUser,
                user.EmailUsuario,
                user.FlAtivo,
                user.Id,
                user.NormalizedUsername,
                user.IdFuncionario,
                user.FlNaoRecebeEmail);

            // 3️⃣ CARREGA PERMISSÕES COMPLETAS (GRUPOS + FUNÇÕES)
            var permissions = await GetUserPermissionsAsync(cdusuario, ct);
            permissions.UserData = userData;

            // 4️⃣ CALCULA TENANT ID BASEADO NA EMPRESA
            var tenantId = user.CdEmpresa.HasValue ?
                Guid.Parse($"{user.CdEmpresa:D8}-0000-0000-0000-000000000000") :
                Guid.Parse("00000001-0000-0000-0000-000000000000");

            // 5️⃣ GERA JWT COM PERMISSÕES NO FORMATO "SISTEMA.FUNCAO.ACOES"
            var permissionClaims = permissions.Permissions
                .Select(p => $"{p.CdSistema.Trim()}.{p.CdFuncao.Trim()}.{p.CdAcoes.Trim()}")
                .Distinct()
                .ToList();

            var token = _jwtService.CreateAccessToken(
                user.CdUsuario,
                tenantId,
                permissionClaims);

            return new AuthResult(true, null, userData, token);
        }
        catch (Exception ex)
        {
            return new AuthResult(false, $"Erro interno: {ex.Message}", null, null);
        }
    }

    /// <summary>
    /// Carrega GRUPOS e PERMISSÕES completas do usuário
    /// Busca em usrh1 (grupos) e hbrh1 (permissões dos grupos)
    /// </summary>
    public async Task<UserPermissions> GetUserPermissionsAsync(string cdusuario, CancellationToken ct = default)
    {
        try
        {
            // ========================================
            // 1️⃣ BUSCA GRUPOS DO USUÁRIO (tabela usrh1)
            // ========================================
            var grupos = await _context.Set<UserGroupEntity>()
                .Where(ug => ug.CdUsuario == cdusuario &&
                            (ug.DtFimVal == null || ug.DtFimVal > DateTime.Now)) // ✅ Grupos ativos
                .Select(ug => new
                {
                    CdGrUser = ug.CdGrUser,
                    CdSistema = ug.CdSistema ?? string.Empty
                })
                .Distinct()
                .ToListAsync(ct);

            if (!grupos.Any())
            {
                // Usuário sem grupos = sem permissões
                return new UserPermissions
                {
                    Groups = new List<UserGroupDto>(),
                    Permissions = new List<UserPermission>()
                };
            }

            // ========================================
            // 2️⃣ BUSCA PERMISSÕES DOS GRUPOS (tabela hbrh1)
            // ========================================
            var permissions = new List<UserPermission>();

            foreach (var grupo in grupos)
            {
                var permsGrupo = await _context.Set<GrupoFuncao>()
                    .Where(gf => gf.CdGrUser == grupo.CdGrUser &&
                                (string.IsNullOrEmpty(grupo.CdSistema) || gf.CdSistema == grupo.CdSistema))
                    .Select(gf => new UserPermission(
                        gf.CdSistema ?? string.Empty,
                        gf.CdGrUser,
                        gf.CdFuncao,
                        gf.CdAcoes,
                        gf.CdRestric))
                    .ToListAsync(ct);

                permissions.AddRange(permsGrupo);
            }

            // ========================================
            // 3️⃣ REMOVE DUPLICATAS (mesmo usuário pode ter múltiplos grupos)
            // ========================================
            var uniquePermissions = permissions
                .GroupBy(p => new { p.CdSistema, p.CdFuncao })
                .Select(g => new UserPermission(
                    g.Key.CdSistema,
                    g.First().CdGrUser,
                    g.Key.CdFuncao,
                    MergeAcoes(g.Select(x => x.CdAcoes)), // ✅ Mescla ações (IAEC)
                    g.Max(x => x.CdRestric)))              // ✅ Pega a restrição mais permissiva
                .ToList();

            // ========================================
            // 4️⃣ MONTA LISTA DE GRUPOS (DTO)
            // ========================================
            var gruposDto = grupos
                .Select(g => new UserGroupDto(g.CdGrUser, g.CdSistema))
                .Distinct()
                .ToList();

            return new UserPermissions
            {
                Groups = gruposDto,
                Permissions = uniquePermissions
            };
        }
        catch (Exception ex)
        {
            // Em caso de erro, retorna permissões vazias (usuário não terá acesso)
            return new UserPermissions
            {
                Groups = new List<UserGroupDto>(),
                Permissions = new List<UserPermission>()
            };
        }
    }

    /// <summary>
    /// Verifica se usuário tem acesso a uma função (habilitação)
    /// </summary>
    public bool CheckHabilitacao(string cdsistema, string cdfuncao, UserPermissions permissions)
    {
        return permissions.Permissions.Any(p =>
            p.CdSistema.Trim().Equals(cdsistema.Trim(), StringComparison.OrdinalIgnoreCase) &&
            p.CdFuncao.Trim().Equals(cdfuncao.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Verifica se usuário tem permissão para uma ação específica em um botão
    /// Ações: I=Incluir, A=Alterar, E=Excluir, C=Consultar
    /// </summary>
    public bool CheckBotao(string cdsistema, string cdfuncao, char acao, UserPermissions permissions)
    {
        var permission = permissions.Permissions.FirstOrDefault(p =>
            p.CdSistema.Trim().Equals(cdsistema.Trim(), StringComparison.OrdinalIgnoreCase) &&
            p.CdFuncao.Trim().Equals(cdfuncao.Trim(), StringComparison.OrdinalIgnoreCase));

        return permission != null && permission.CdAcoes.Contains(acao, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Retorna nível de restrição do usuário na função
    /// S = Sem restrição (acesso total)
    /// N = Normal (restrição padrão)
    /// </summary>
    public char CheckRestricao(string cdsistema, string cdfuncao, UserPermissions permissions)
    {
        var permission = permissions.Permissions.FirstOrDefault(p =>
            p.CdSistema.Trim().Equals(cdsistema.Trim(), StringComparison.OrdinalIgnoreCase) &&
            p.CdFuncao.Trim().Equals(cdfuncao.Trim(), StringComparison.OrdinalIgnoreCase));

        return permission?.CdRestric ?? 'N';
    }

    // ========================================
    // MÉTODOS AUXILIARES PRIVADOS
    // ========================================

    /// <summary>
    /// Mescla ações de múltiplas permissões (união de IAEC)
    /// Exemplo: "IAC" + "IEC" = "IAEC"
    /// </summary>
    private static string MergeAcoes(IEnumerable<string> acoes)
    {
        var uniqueChars = new HashSet<char>();

        foreach (var acao in acoes)
        {
            if (!string.IsNullOrWhiteSpace(acao))
            {
                foreach (var c in acao.Trim().ToUpperInvariant())
                {
                    if ("IAEC".Contains(c))
                        uniqueChars.Add(c);
                }
            }
        }

        // Retorna na ordem padrão: I, A, E, C
        var result = new System.Text.StringBuilder();
        if (uniqueChars.Contains('I')) result.Append('I');
        if (uniqueChars.Contains('A')) result.Append('A');
        if (uniqueChars.Contains('E')) result.Append('E');
        if (uniqueChars.Contains('C')) result.Append('C');

        return result.ToString();
    }
}