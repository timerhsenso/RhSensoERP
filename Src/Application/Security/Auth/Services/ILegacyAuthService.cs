using RhSensoERP.Application.Security.Auth.DTOs;

namespace RhSensoERP.Application.Security.Auth.Services;

public interface ILegacyAuthService
{
    Task<AuthResult> AuthenticateAsync(string cdusuario, string senha, CancellationToken ct = default);
    Task<UserPermissions> GetUserPermissionsAsync(string cdusuario, CancellationToken ct = default);
    bool CheckHabilitacao(string cdsistema, string cdfuncao, UserPermissions permissions);
    bool CheckBotao(string cdsistema, string cdfuncao, char acao, UserPermissions permissions);
    char CheckRestricao(string cdsistema, string cdfuncao, UserPermissions permissions);
}