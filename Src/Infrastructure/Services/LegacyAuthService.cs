using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RhSensoERP.Infrastructure.Persistence;
using RhSensoERP.Application.Security.Auth.DTOs;
using RhSensoERP.Application.Security.Auth.Services;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Infrastructure.Auth;

// ADICIONAR ALIAS PARA RESOLVER AMBIGUIDADE
using UserGroupEntity = RhSensoERP.Core.Security.Entities.UserGroup;
using UserGroupDto = RhSensoERP.Application.Security.Auth.DTOs.UserGroup;

namespace RhSensoERP.Infrastructure.Services;

/// <summary>
/// Serviço de autenticação para sistema legacy
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

    public async Task<AuthResult> AuthenticateAsync(string cdusuario, string senha, CancellationToken ct = default)
    {
        try
        {
            var user = await _context.Set<User>()
                .Where(u => u.FlAtivo == 'S' &&
                           u.CdUsuario == cdusuario &&
                           u.SenhaUser == senha)
                .FirstOrDefaultAsync(ct);

            if (user == null)
                return new AuthResult(false, "Usuário não encontrado, inativo ou senha incorreta", null, null);

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

            var permissions = await GetUserPermissionsAsync(cdusuario, ct);
            permissions.UserData = userData;

            var tenantId = user.CdEmpresa.HasValue ?
                Guid.Parse($"{user.CdEmpresa:D8}-0000-0000-0000-000000000000") :
                Guid.Parse("00000001-0000-0000-0000-000000000000");

            var token = _jwtService.CreateAccessToken(
                user.CdUsuario,
                tenantId,
                permissions.Permissions.Select(p => $"{p.CdSistema}.{p.CdFuncao}.{p.CdAcoes}"));

            return new AuthResult(true, null, userData, token);
        }
        catch (Exception ex)
        {
            return new AuthResult(false, $"Erro interno: {ex.Message}", null, null);
        }
    }

    public async Task<UserPermissions> GetUserPermissionsAsync(string cdusuario, CancellationToken ct = default)
    {
        try
        {
            // CORREÇÃO: Usar alias explícito para evitar ambiguidade
            var grupos = await _context.Set<UserGroupEntity>()
                .Where(ug => ug.CdUsuario == cdusuario &&
                            ug.DtFimVal == null)
                .Select(ug => new UserGroupDto(ug.CdGrUser, ug.CdSistema!))  // LINHA 113 CORRIGIDA
                .ToListAsync(ct);

            var permissions = new List<UserPermission>();

            return new UserPermissions
            {
                Groups = grupos,
                Permissions = permissions
            };
        }
        catch (Exception)
        {
            return new UserPermissions
            {
                Groups = new List<UserGroupDto>(),
                Permissions = new List<UserPermission>()
            };
        }
    }

    public bool CheckHabilitacao(string cdsistema, string cdfuncao, UserPermissions permissions)
    {
        return permissions.Permissions.Any(p =>
            p.CdSistema.Trim().Equals(cdsistema.Trim(), StringComparison.OrdinalIgnoreCase) &&
            p.CdFuncao.Trim().Equals(cdfuncao.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public bool CheckBotao(string cdsistema, string cdfuncao, char acao, UserPermissions permissions)
    {
        var permission = permissions.Permissions.FirstOrDefault(p =>
            p.CdSistema.Trim().Equals(cdsistema.Trim(), StringComparison.OrdinalIgnoreCase) &&
            p.CdFuncao.Trim().Equals(cdfuncao.Trim(), StringComparison.OrdinalIgnoreCase));

        return permission != null && permission.CdAcoes.Contains(acao);
    }

    public char CheckRestricao(string cdsistema, string cdfuncao, UserPermissions permissions)
    {
        var permission = permissions.Permissions.FirstOrDefault(p =>
            p.CdSistema.Trim().Equals(cdsistema.Trim(), StringComparison.OrdinalIgnoreCase) &&
            p.CdFuncao.Trim().Equals(cdfuncao.Trim(), StringComparison.OrdinalIgnoreCase));

        return permission?.CdRestric ?? 'N';
    }
}