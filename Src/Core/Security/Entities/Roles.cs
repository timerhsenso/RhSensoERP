using RhSensoERP.Core.Abstractions.Entities;
using RhSensoERP.Core.Security.Entities;

public class Role : AuditableEntity
{
    public string Name { get; set; } = string.Empty;        // ex: nome_funcao, role_name
    public string Description { get; set; } = string.Empty;
    public bool Active { get; set; } = true;

    public virtual ICollection<GroupRole> GroupRoles { get; set; } = new List<GroupRole>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

// Tabelas de relacionamento (muitos-para-muitos)
public class UserGroup
{
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Group Group { get; set; } = null!;
}

public class GroupRole
{
    public Guid GroupId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Group Group { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
}