namespace RhSensoERP.Application.Security.Auth.DTOs;

/// <summary>
/// DTO para requisição de login
/// </summary>
public record LoginRequestDto(string CdUsuario, string Senha);

/// <summary>
/// DTO para resposta de login
/// </summary>
public record LoginResponseDto(
    string AccessToken,
    UserSessionData UserData,
    List<UserGroup> Groups,
    List<UserPermission> Permissions);