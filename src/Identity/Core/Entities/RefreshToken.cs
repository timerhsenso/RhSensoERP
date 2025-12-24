// RhSensoERP.Identity/Domain/Entities/RefreshToken.cs

namespace RhSensoERP.Identity.Core.Entities;

/// <summary>
/// Refresh tokens para renovação de JWT. Suporta múltiplos dispositivos.
/// </summary>
public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid IdUserSecurity { get; private set; }

    public string TokenHash { get; private set; } = string.Empty;
    public string? DeviceId { get; private set; }
    public string? DeviceName { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public string CreatedByIp { get; private set; } = string.Empty;

    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedByIp { get; private set; }
    public string? RevokedReason { get; private set; }

    public Guid? ReplacedByTokenId { get; private set; }

    // Navegação
    public virtual UserSecurity? UserSecurity { get; private set; }
    public virtual RefreshToken? ReplacedByToken { get; private set; }

    private RefreshToken() { } // EF Core

    public RefreshToken(
        Guid idUserSecurity,
        string tokenHash,
        DateTime expiresAt,
        string createdByIp,
        string? deviceId = null,
        string? deviceName = null)
    {
        Id = Guid.NewGuid();
        IdUserSecurity = idUserSecurity;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        CreatedByIp = createdByIp;
        DeviceId = deviceId;
        DeviceName = deviceName;
        CreatedAt = DateTime.UtcNow;
    }

    public void Revoke(string? revokedByIp, string reason)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevokedByIp = revokedByIp;
        RevokedReason = reason;
    }

    public void SetReplacedBy(Guid newTokenId)
    {
        ReplacedByTokenId = newTokenId;
        Revoke(null, "TokenRotation");
    }

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive() => !IsRevoked && !IsExpired();
}