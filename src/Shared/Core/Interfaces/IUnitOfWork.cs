namespace RhSensoERP.Shared.Core.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
