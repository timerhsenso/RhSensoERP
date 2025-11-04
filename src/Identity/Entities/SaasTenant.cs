using RhSensoERP.Shared.Core.Entities;

namespace RhSensoERP.Identity.Entities;

/// <summary>
/// Entidade para empresas/tenants SaaS
/// </summary>
public class SaasTenant : BaseEntity
{
    public string CompanyName { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public bool IsActive { get; set; } = true;

    // Configurações de plano
    public int MaxUsers { get; set; } = 10;
    public string PlanType { get; set; } = "Basic"; // Basic, Pro, Enterprise

    // Auditoria
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Relacionamentos
  //  public virtual ICollection<SaasUser> Users { get; set; } = new List<SaasUser>();
  //  public virtual ICollection<SaasInvitation> Invitations { get; set; } = new List<SaasInvitation>();

    // Propriedades de conveniência
  //  public int ActiveUsersCount => Users.Count(u => u.IsActive);
 //   public bool CanAddMoreUsers => ActiveUsersCount < MaxUsers;
   // public int AvailableUserSlots => Math.Max(0, MaxUsers - ActiveUsersCount);

    // Métodos de negócio
    //public bool CanInviteUser()
    //{
     //   return IsActive && CanAddMoreUsers;
    //}

    public void UpdateSettings(string companyName, string? domain, int maxUsers, string planType)
    {
        CompanyName = companyName;
        Domain = domain;
        MaxUsers = maxUsers;
        PlanType = planType;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}