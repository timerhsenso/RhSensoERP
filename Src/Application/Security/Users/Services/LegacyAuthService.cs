// src/Application/Security/Auth/Services/LegacyAuthService.cs
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Application.Security.Auth.DTOs;
using RhSensoERP.Application.Security.Users.Services;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Infrastructure.Auth;
using RhSensoERP.Infrastructure.Persistence;
using System;

namespace RhSensoERP.Application.Security.Auth.Services;

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
            // Validar usuário conforme sua consulta SQL
            var user = await _context.Set<User>()
                .Where(u => u.FlAtivo == 'S' &&
                           u.CdUsuario == cdusuario)
                .FirstOrDefaultAsync(ct);

            if (user == null)
                return new AuthResult(false, "Usuário não encontrado ou inativo", null, null);

            // Verificar senha (adaptar conforme appsettings - banco ou windows)
            var useWindowsAuth = _config.GetValue<bool>("Auth:UseWindowsAuth", false);

            if (!useWindowsAuth)
            {
                // Autenticação por banco
                if (user.SenhaUser != senha)
                    return new AuthResult(false, "Senha incorreta", null, null);
            }
            else
            {
                // Implementar autenticação Windows aqui se necessário
                // Usar WindowsIdentity.GetCurrent() ou similar
            }

            // Criar dados de sessão
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

            // Buscar permissões
            var permissions = await GetUserPermissionsAsync(cdusuario, ct);
            permissions.UserData = userData;

            // Gerar JWT com permissões
            var token = _jwtService.CreateAccessToken(
                user.CdUsuario,
                user.CdEmpresa ?? 1, // Default tenant
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
        // Implementar suas consultas SQL usando EF Core

        // 1. Buscar grupos do usuário
        var groups = await _context.Set<UserGroup>()
            .Where(ug => ug.CdUsuario == cdusuario &&
                        ug.DtFimVal == null &&
                        _context.Set<Sistema>().Any(s => s.CdSistema == ug.CdSistema && s.Ativo))
            .Select(ug => new UserGroup(ug.CdGrUser, ug.CdSistema!))
            .ToListAsync(ct);

        // 2. Buscar permissões (sua consulta complexa)
        var permissions = await (
            from ug in _context.Set<UserGroup>()
            join gf in _context.Set<GrupoFuncao>()
                on new { ug.CdGrUser, ug.CdSistema } equals new { gf.CdGrUser, gf.CdSistema }
            join s in _context.Set<Sistema>()
                on ug.CdSistema equals s.CdSistema
            where ug.CdUsuario == cdusuario &&
                  ug.DtFimVal == null &&
                  s.Ativo
            orderby ug.CdSistema, ug.CdGrUser, gf.CdFuncao
            select new UserPermission(
                ug.CdSistema!,
                ug.CdGrUser,
                gf.CdFuncao,
                gf.CdAcoes,
                gf.CdRestric)
        ).ToListAsync(ct);

        return new UserPermissions
        {
            Groups = groups,
            Permissions = permissions
        };
    }

    public bool CheckHabilitacao(string cdsistema, string cdfuncao, UserPermissions permissions)
    {
        return permissions.Permissions.Any(p =>
            p.CdSistema.Trim() == cdsistema.Trim() &&
            p.CdFuncao.Trim() == cdfuncao.Trim());
    }

    public bool CheckBotao(string cdsistema, string cdfuncao, char acao, UserPermissions permissions)
    {
        var permission = permissions.Permissions.FirstOrDefault(p =>
            p.CdSistema.Trim() == cdsistema.Trim() &&
            p.CdFuncao.Trim() == cdfuncao.Trim());

        return permission != null && permission.CdAcoes.Contains(acao);
    }

    public char CheckRestricao(string cdsistema, string cdfuncao, UserPermissions permissions)
    {
        var permission = permissions.Permissions.FirstOrDefault(p =>
            p.CdSistema.Trim() == cdsistema.Trim() &&
            p.CdFuncao.Trim() == cdfuncao.Trim());

        return permission?.CdRestric ?? 'N'; // N = Não tem acesso
    }
}