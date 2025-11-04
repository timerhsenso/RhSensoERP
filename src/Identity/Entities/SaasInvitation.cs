using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.Security.Entities;

/// <summary>
/// Entidade para convites SaaS
/// </summary>
public class SaasInvitation : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string EmailNormalized { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public Guid InvitedById { get; set; }

    // Token e controle
    public string InvitationToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public bool IsAccepted { get; set; }

    // Metadados
    public string Role { get; set; } = "User";
    public string? Message { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual SaasTenant Tenant { get; set; } = null!;
    public virtual SaasUser InvitedBy { get; set; } = null!;

    // Propriedades de conveniência
    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool IsValid => !IsAccepted && !IsExpired;
    public TimeSpan TimeUntilExpiry => ExpiresAt - DateTime.UtcNow;

    // Métodos de negócio
    public static SaasInvitation Create(string email, Guid tenantId, Guid invitedById, string? message = null, string role = "User")
    {
        return new SaasInvitation
        {
            Email = email.Trim(),
            EmailNormalized = email.Trim().ToUpperInvariant(),
            TenantId = tenantId,
            InvitedById = invitedById,
            InvitationToken = Guid.NewGuid().ToString("N"),
            ExpiresAt = DateTime.UtcNow.AddDays(7), // Convite expira em 7 dias
            Role = role,
            Message = message?.Trim()
        };
    }

    public void Accept()
    {
        if (!IsValid)
            throw new InvalidOperationException("Convite inválido ou expirado");

        IsAccepted = true;
        AcceptedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Extend(int days = 7)
    {
        if (IsAccepted)
            throw new InvalidOperationException("Não é possível estender convite já aceito");

        ExpiresAt = DateTime.UtcNow.AddDays(days);
        UpdatedAt = DateTime.UtcNow;
    }
}