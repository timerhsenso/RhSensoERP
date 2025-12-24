// RhSensoERP.Identity/Domain/Entities/TwoFactorRecoveryCode.cs

namespace RhSensoERP.Identity.Core.Entities;

/// <summary>
/// Códigos de recuperação para 2FA (backup).
/// </summary>
public class TwoFactorRecoveryCode
{
    public Guid Id { get; private set; }
    public Guid IdUserSecurity { get; private set; }

    public string CodeHash { get; private set; } = string.Empty;
    public DateTime GeneratedAt { get; private set; }

    public bool IsUsed { get; private set; }
    public DateTime? UsedAt { get; private set; }
    public string? UsedFromIp { get; private set; }

    // Navegação
    public virtual UserSecurity? UserSecurity { get; private set; }

    private TwoFactorRecoveryCode() { } // EF Core

    public TwoFactorRecoveryCode(Guid idUserSecurity, string codeHash)
    {
        Id = Guid.NewGuid();
        IdUserSecurity = idUserSecurity;
        CodeHash = codeHash;
        GeneratedAt = DateTime.UtcNow;
    }

    public void MarkAsUsed(string? usedFromIp)
    {
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
        UsedFromIp = usedFromIp;
    }
}