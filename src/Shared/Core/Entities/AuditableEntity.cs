namespace RhSensoERP.Shared.Core.Entities;

public abstract class AuditableEntity : BaseEntity, ISoftDeletable
{
    public Guid TenantId { get; set; }
    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
