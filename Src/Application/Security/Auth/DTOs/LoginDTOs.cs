namespace RhSensoERP.Application.Security.Auth.DTOs;

/// <summary>
/// DTO para requisição de login - suporta todos os modos de autenticação
/// </summary>
public record LoginRequestDto(
    string CdUsuario,
    string Senha,
    string? Domain = null)
{
    /// <summary>
    /// Para compatibilidade com Windows Auth: aceita DOMAIN\user no campo CdUsuario
    /// </summary>
    public (string User, string? Password, string? Domain) GetAuthData()
    {
        // Se o usuário contém barra invertida, é formato DOMAIN\user
        if (!string.IsNullOrWhiteSpace(CdUsuario) && CdUsuario.Contains('\\'))
        {
            var parts = CdUsuario.Split('\\', 2);
            if (parts.Length == 2)
            {
                return (parts[1], Senha, parts[0]);
            }
        }

        // Senão, usa os campos separados
        return (CdUsuario, Senha, Domain);
    }
};

/// <summary>
/// DTO para resposta de login
/// </summary>
public record LoginResponseDto(
    string AccessToken,
    UserSessionData UserData,
    List<UserGroup> Groups,
    List<UserPermission> Permissions);