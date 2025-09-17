namespace RhSensoERP.Core.Abstractions.Entities;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
