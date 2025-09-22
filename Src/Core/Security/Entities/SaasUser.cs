using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.Security.Entities;

/// <summary>
/// Entidade para usuários SaaS - completamente separada do sistema legacy
/// </summary>
public class SaasUser : BaseEntity
{
    // Autenticação
    public string Email { get; set; } = string.Empty;
    public string EmailNormalized { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    // Controle de acesso
    public bool EmailConfirmed { get; set; }
    public bool IsActive { get; set; } = true;

    // Tokens de segurança
    public string? EmailConfirmationToken { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }

    // Multi-tenant
    public Guid TenantId { get; set; }

    // Auditoria e controle
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public int LoginAttempts { get; set; }
    public DateTime? LockedUntil { get; set; }

    // Metadados de sessão
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }

    // Relacionamentos
    public virtual SaasTenant Tenant { get; set; } = null!;
    public virtual ICollection<SaasInvitation> SentInvitations { get; set; } = new List<SaasInvitation>();

    // Propriedades de conveniência
    public string Username => Email;
    public string DisplayName => FullName;
    public bool IsLocked => LockedUntil.HasValue && LockedUntil > DateTime.UtcNow;
    public bool CanLogin => IsActive && EmailConfirmed && !IsLocked;

    // Métodos de negócio
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        LoginAttempts = 0; // Reset em login bem-sucedido
        LockedUntil = null;
    }

    public void IncrementLoginAttempts()
    {
        LoginAttempts++;
        UpdatedAt = DateTime.UtcNow;

        // Lockout após 5 tentativas
        if (LoginAttempts >= 5)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(15);
        }
    }

    public void ResetLoginAttempts()
    {
        LoginAttempts = 0;
        LockedUntil = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        EmailConfirmationToken = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void GenerateEmailConfirmationToken()
    {
        EmailConfirmationToken = Guid.NewGuid().ToString("N");
        UpdatedAt = DateTime.UtcNow;
    }

    public void GeneratePasswordResetToken()
    {
        PasswordResetToken = Guid.NewGuid().ToString("N");
        PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsPasswordResetTokenValid(string token)
    {
        return !string.IsNullOrEmpty(PasswordResetToken) &&
               PasswordResetToken == token &&
               PasswordResetTokenExpiry.HasValue &&
               PasswordResetTokenExpiry > DateTime.UtcNow;
    }

    public void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetTokenExpiry = null;
        UpdatedAt = DateTime.UtcNow;
    }
}