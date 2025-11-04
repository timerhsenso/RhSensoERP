namespace RhSensoERP.Shared.Core.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}
