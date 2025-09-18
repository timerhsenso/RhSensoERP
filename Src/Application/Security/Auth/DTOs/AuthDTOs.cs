namespace RhSensoERP.Application.Security.Auth.DTOs;

public record AuthResult(
    bool Success,
    string? ErrorMessage,
    UserSessionData? UserData,
    string? AccessToken);

public record UserSessionData(
    string CdUsuario,
    string DcUsuario,
    string? NmImpCche,
    char TpUsuario,
    string? NoMatric,
    int? CdEmpresa,
    int? CdFilial,
    int NoUser,
    string? EmailUsuario,
    char FlAtivo,
    Guid Id,
    string? NormalizedUsername,
    Guid? IdFuncionario,
    char? FlNaoRecebeEmail);

public record UserGroup(string CdGrUser, string CdSistema);

public record UserPermission(
    string CdSistema,
    string CdGrUser, 
    string CdFuncao,
    string CdAcoes,
    char CdRestric);

public class UserPermissions
{
    public UserSessionData? UserData { get; set; }
    public List<UserGroup> Groups { get; set; } = new();
    public List<UserPermission> Permissions { get; set; } = new();
}