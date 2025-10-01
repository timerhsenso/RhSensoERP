namespace RhSensoWeb.Configuration;

/// <summary>
/// Configurações de autenticação e autorização
/// </summary>
public class AuthSettings
{
    public const string SectionName = "Authentication";

    /// <summary>
    /// Nome do cookie de autenticação
    /// </summary>
    public string CookieName { get; set; } = "RhSensoAuth";

    /// <summary>
    /// Tempo de expiração do cookie (formato TimeSpan)
    /// </summary>
    public TimeSpan ExpireTimeSpan { get; set; } = TimeSpan.FromHours(8);

    /// <summary>
    /// Habilitar expiração deslizante (renova automaticamente)
    /// </summary>
    public bool SlidingExpiration { get; set; } = true;

    /// <summary>
    /// Caminho para página de login
    /// </summary>
    public string LoginPath { get; set; } = "/Auth/Login";

    /// <summary>
    /// Caminho para página de logout
    /// </summary>
    public string LogoutPath { get; set; } = "/Auth/Logout";

    /// <summary>
    /// Caminho para página de acesso negado
    /// </summary>
    public string AccessDeniedPath { get; set; } = "/Auth/AccessDenied";

    /// <summary>
    /// Domínio do cookie (opcional)
    /// </summary>
    public string? CookieDomain { get; set; }

    /// <summary>
    /// Usar cookies seguros (HTTPS only)
    /// </summary>
    public bool SecureCookies { get; set; } = true;

    /// <summary>
    /// Política SameSite do cookie
    /// </summary>
    public string SameSiteMode { get; set; } = "Lax";

    /// <summary>
    /// Chave para criptografia do JWT no cookie
    /// </summary>
    public string? EncryptionKey { get; set; }
}
