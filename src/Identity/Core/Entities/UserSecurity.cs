// RhSensoERP.Identity/Domain/Entities/UserSecurity.cs

namespace RhSensoERP.Identity.Core.Entities;

/// <summary>
/// Entidade de segurança do usuário (1:1 com tuse1).
/// Agregado raiz para todas as entidades de segurança.
/// </summary>
public class UserSecurity
{
    // ========================================
    // IDENTIDADE
    // ========================================

    public Guid Id { get; private set; }
    public Guid IdUsuario { get; private set; }
    public Guid? IdSaaS { get; private set; }

    // ========================================
    // SENHA
    // ========================================

    public string PasswordHash { get; private set; } = string.Empty;
    public string PasswordSalt { get; private set; } = string.Empty;
    public string PasswordAlgorithm { get; private set; } = "PBKDF2-SHA512";
    public int PasswordVersion { get; private set; } = 1;
    public DateTime PasswordChangedAt { get; private set; }
    public bool MustChangePassword { get; private set; }
    public string? ForcePasswordChangeReason { get; private set; }

    // ========================================
    // LOCKOUT
    // ========================================

    public int AccessFailedCount { get; private set; }
    public DateTime? LockoutEnd { get; private set; }
    public bool LockoutEnabled { get; private set; } = true;
    public string? AccountLockedReason { get; private set; }

    // ========================================
    // TWO-FACTOR AUTHENTICATION
    // ========================================

    public bool TwoFactorEnabled { get; private set; }
    public string? TwoFactorSecret { get; private set; }
    public string? TwoFactorType { get; private set; }
    public DateTime? TwoFactorActivatedAt { get; private set; }

    // ========================================
    // CONFIRMAÇÕES
    // ========================================

    public bool EmailConfirmed { get; private set; }
    public DateTime? EmailConfirmedAt { get; private set; }
    public string? PhoneNumber { get; private set; }
    public bool PhoneNumberConfirmed { get; private set; }
    public DateTime? PhoneNumberConfirmedAt { get; private set; }

    // ========================================
    // SECURITY STAMP
    // ========================================

    public string SecurityStamp { get; private set; } = Guid.NewGuid().ToString();
    public string ConcurrencyStamp { get; private set; } = Guid.NewGuid().ToString();

    // ========================================
    // CACHE
    // ========================================

    public DateTime? LastLoginAt { get; private set; }
    public string? LastLoginIP { get; private set; }

    // ========================================
    // AUDITORIA
    // ========================================

    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    // ========================================
    // NAVEGAÇÃO
    // ========================================

    public virtual ICollection<PasswordHistory> PasswordHistories { get; private set; } = new List<PasswordHistory>();
    public virtual ICollection<LoginAuditLog> LoginAuditLogs { get; private set; } = new List<LoginAuditLog>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    public virtual ICollection<TwoFactorRecoveryCode> RecoveryCodes { get; private set; } = new List<TwoFactorRecoveryCode>();
    public virtual ICollection<UserSecurityToken> SecurityTokens { get; private set; } = new List<UserSecurityToken>();

    // ========================================
    // CONSTRUTORES
    // ========================================

    private UserSecurity() { } // EF Core

    public UserSecurity(Guid idUsuario, Guid? idSaaS, string passwordHash, string passwordSalt)
    {
        Id = Guid.NewGuid();
        IdUsuario = idUsuario;
        IdSaaS = idSaaS;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        PasswordChangedAt = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        SecurityStamp = Guid.NewGuid().ToString();
        ConcurrencyStamp = Guid.NewGuid().ToString();
    }

    // ========================================
    // MÉTODOS DE NEGÓCIO
    // ========================================

    public void ChangePassword(string newPasswordHash, string newSalt, string? reason = null, string? ipAddress = null)
    {
        var history = new PasswordHistory(Id, PasswordHash, PasswordAlgorithm, reason ?? "Manual", ipAddress);
        PasswordHistories.Add(history);

        PasswordHash = newPasswordHash;
        PasswordSalt = newSalt;
        PasswordChangedAt = DateTime.UtcNow;
        MustChangePassword = false;
        ForcePasswordChangeReason = null;

        RegenerateSecurityStamp();
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementAccessFailedCount()
    {
        AccessFailedCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ResetAccessFailedCount()
    {
        AccessFailedCount = 0;
        UpdatedAt = DateTime.UtcNow;
    }

    public void LockUntil(DateTime lockoutEnd, string? reason = null)
    {
        LockoutEnd = lockoutEnd;
        AccountLockedReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unlock()
    {
        LockoutEnd = null;
        AccountLockedReason = null;
        AccessFailedCount = 0;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ForcePasswordChange(string reason)
    {
        MustChangePassword = true;
        ForcePasswordChangeReason = reason;
        RegenerateSecurityStamp();
        UpdatedAt = DateTime.UtcNow;
    }

    public void EnableTwoFactor(string secret, string type)
    {
        TwoFactorEnabled = true;
        TwoFactorSecret = secret;
        TwoFactorType = type;
        TwoFactorActivatedAt = DateTime.UtcNow;
        RegenerateSecurityStamp();
        UpdatedAt = DateTime.UtcNow;
    }

    public void DisableTwoFactor()
    {
        TwoFactorEnabled = false;
        TwoFactorSecret = null;
        TwoFactorType = null;
        TwoFactorActivatedAt = null;
        RecoveryCodes.Clear();
        RegenerateSecurityStamp();
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        EmailConfirmedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfirmPhoneNumber(string phoneNumber)
    {
        PhoneNumber = phoneNumber;
        PhoneNumberConfirmed = true;
        PhoneNumberConfirmedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RegisterSuccessfulLogin(string ipAddress)
    {
        LastLoginAt = DateTime.UtcNow;
        LastLoginIP = ipAddress;
        AccessFailedCount = 0;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RegenerateSecurityStamp()
    {
        SecurityStamp = Guid.NewGuid().ToString();
        ConcurrencyStamp = Guid.NewGuid().ToString();
    }

    public void SetActiveStatus(bool isActive)
    {
        IsActive = isActive;
        if (!isActive)
        {
            RegenerateSecurityStamp();
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        IsActive = false;
        RegenerateSecurityStamp();
    }
}