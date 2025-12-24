// ============================================================================
// FASE 3 - Active Directory
// src/Identity/Application/Services/IActiveDirectoryService.cs
// ============================================================================

namespace RhSensoERP.Identity.Application.Services;

/// <summary>
/// Serviço para autenticação via Active Directory / LDAP.
/// </summary>
public interface IActiveDirectoryService
{
    /// <summary>
    /// Autentica um usuário no Active Directory.
    /// </summary>
    /// <param name="username">Nome de usuário (cdusuario)</param>
    /// <param name="password">Senha do AD</param>
    /// <param name="domain">Domínio do AD (opcional, usa configuração padrão se null)</param>
    /// <param name="ct">CancellationToken</param>
    /// <returns>True se autenticado com sucesso, False caso contrário</returns>
    Task<bool> AuthenticateAsync(
        string username,
        string password,
        string? domain = null,
        CancellationToken ct = default);

    /// <summary>
    /// Verifica se o serviço de AD está configurado e disponível.
    /// </summary>
    /// <returns>True se AD está disponível</returns>
    bool IsAvailable();

    /// <summary>
    /// Obtém informações do usuário do Active Directory.
    /// </summary>
    /// <param name="username">Nome de usuário</param>
    /// <param name="ct">CancellationToken</param>
    /// <returns>Informações do usuário ou null se não encontrado</returns>
    Task<ActiveDirectoryUserInfo?> GetUserInfoAsync(
        string username,
        CancellationToken ct = default);
}

/// <summary>
/// Informações do usuário obtidas do Active Directory.
/// </summary>
public sealed class ActiveDirectoryUserInfo
{
    public string Username { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public string? Department { get; set; }
    public string? Title { get; set; }
    public bool IsActive { get; set; }
}
