namespace RhSensoERP.Application.Security.Auth.DTOs;

/// <summary>
/// DTO para requisição de login - suporta todos os modos de autenticação
/// IMPORTANTE: O campo Domain não é enviado pelo frontend
/// Para Windows Auth, o domínio é obtido da configuração (DefaultDomain)
/// </summary>
public record LoginRequestDto(
    string CdUsuario,
    string Senha,
    string? Domain = null) // Backend pode receber null - usará DefaultDomain da config
{
    /// <summary>
    /// Extrai dados de autenticação
    /// Para Windows Auth: aceita formato DOMAIN\user OU usa DefaultDomain da configuração
    /// Para OnPrem/SaaS: ignora domínio
    /// </summary>
    public (string User, string? Password, string? Domain) GetAuthData()
    {
        // Se o usuário contém barra invertida, é formato DOMAIN\user (raro, mas suportado)
        if (!string.IsNullOrWhiteSpace(CdUsuario) && CdUsuario.Contains('\\'))
        {
            var parts = CdUsuario.Split('\\', 2);
            if (parts.Length == 2)
            {
                return (parts[1], Senha, parts[0]);
            }
        }

        // Retorna null no Domain - WindowsAuthStrategy usará DefaultDomain da config
        return (CdUsuario, Senha, Domain);
    }
}

/// <summary>
/// DTO para resposta de login
/// </summary>
public record LoginResponseDto(
    string AccessToken,
    UserSessionData UserData,
    List<UserGroup> Groups,
    List<UserPermission> Permissions);