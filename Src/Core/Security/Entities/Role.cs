using RhSensoERP.Core.Abstractions.Entities;

namespace RhSensoERP.Core.Security.Entities;

public class Role : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Active { get; set; } = true;

    public virtual ICollection<GroupRole> GroupRoles { get; set; } = new List<GroupRole>();
}