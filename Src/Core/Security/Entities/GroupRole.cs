using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.Security.Entities;

public class GroupRole : BaseEntity
{
    public Guid GroupId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Group Group { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
}