namespace RhSensoERP.Shared.Core.Entities;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
