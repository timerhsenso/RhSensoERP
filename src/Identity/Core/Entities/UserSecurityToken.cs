// RhSensoERP.Identity/Domain/Entities/UserSecurityToken.cs

namespace RhSensoERP.Identity.Core.Entities;

/// <summary>
/// Tokens de propósito específico: confirmação de email, reset de senha, etc.
/// </summary>
public class UserSecurityToken
{
    public Guid Id { get; private set; }
    public Guid IdUserSecurity { get; private set; }

    public string TokenType { get; private set; } = string.Empty;
    public string TokenHash { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public string? RequestedFromIp { get; private set; }

    public bool IsUsed { get; private set; }
    public DateTime? UsedAt { get; private set; }
    public string? UsedFromIp { get; private set; }

    // Navegação
    public virtual UserSecurity? UserSecurity { get; private set; }

    private UserSecurityToken() { } // EF Core

    public UserSecurityToken(
        Guid idUserSecurity,
        string tokenType,
        string tokenHash,
        DateTime expiresAt,
        string? requestedFromIp = null)
    {
        Id = Guid.NewGuid();
        IdUserSecurity = idUserSecurity;
        TokenType = tokenType;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        RequestedFromIp = requestedFromIp;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsUsed(string? usedFromIp)
    {
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
        UsedFromIp = usedFromIp;
    }

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;
    public bool IsValid() => !IsUsed && !IsExpired();
}