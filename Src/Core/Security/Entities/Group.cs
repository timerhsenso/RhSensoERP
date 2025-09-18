using RhSensoERP.Core.Abstractions.Entities;

public class Group : AuditableEntity
{
    public string Name { get; set; } = string.Empty;        // ex: nome_grupo, group_name
    public string Description { get; set; } = string.Empty; // ex: descricao, description
    public bool Active { get; set; } = true;

    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    public virtual ICollection<GroupRole> GroupRoles { get; set; } = new List<GroupRole>();
}