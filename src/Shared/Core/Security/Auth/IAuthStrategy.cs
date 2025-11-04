namespace RhSensoERP.Shared.Core.Security.Auth;

/// <summary>
/// Contrato para estratégias de autenticação
/// Suporta SaaS, OnPrem (legacy) e Windows Authentication
/// </summary>
public interface IAuthStrategy
{
    /// <summary>
    /// Modo de autenticação desta estratégia
    /// </summary>
    AuthMode Mode { get; }

    /// <summary>
    /// Executa autenticação usando a estratégia específica
    /// </summary>
    /// <param name="request">Dados de login (usuário/senha ou contexto)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Resultado da autenticação com dados do usuário e token</returns>
    Task<AuthStrategyResult> AuthenticateAsync(LoginRequest request, CancellationToken ct = default);
}

/// <summary>
/// Modos de autenticação suportados
/// </summary>
public enum AuthMode
{
    /// <summary>
    /// Autenticação SaaS - email/senha em tabela dedicada
    /// </summary>
    SaaS,

    /// <summary>
    /// Autenticação On-Premises - tabela tuse1 legacy
    /// </summary>
    OnPrem,

    /// <summary>
    /// Autenticação Windows - Kerberos/NTLM via domínio
    /// </summary>
    Windows
}

/// <summary>
/// Request unificado para todas as estratégias
/// </summary>
public record LoginRequest(
    string? UserOrEmail,
    string? Password,
    string? Domain = null)
{
    /// <summary>
    /// Para Windows Auth: combina domínio e usuário no formato DOMAIN\user
    /// </summary>
    public string GetFullUsername()
    {
        if (!string.IsNullOrWhiteSpace(Domain) && !string.IsNullOrWhiteSpace(UserOrEmail))
        {
            return $"{Domain}\\{UserOrEmail}";
        }
        return UserOrEmail ?? string.Empty;
    }

    /// <summary>
    /// Cria LoginRequest a partir de string no formato DOMAIN\user
    /// </summary>
    public static LoginRequest FromDomainUser(string domainUser, string? password = null)
    {
        if (string.IsNullOrWhiteSpace(domainUser))
            return new LoginRequest(null, password);

        var parts = domainUser.Split('\\', 2);
        if (parts.Length == 2)
        {
            return new LoginRequest(parts[1], password, parts[0]);
        }

        return new LoginRequest(domainUser, password);
    }
};

/// <summary>
/// Resultado da autenticação por estratégia
/// Independente da infraestrutura - apenas dados essenciais
/// </summary>
public record AuthStrategyResult(
    bool Success,
    string? ErrorMessage,
    string? UserKey,           // Chave canônica do usuário (cdusuario, email, etc)
    string? DisplayName,
    string? Provider,          // "OnPrem", "SaaS", "Windows"
    Guid? TenantId = null,
    Dictionary<string, object>? AdditionalData = null);