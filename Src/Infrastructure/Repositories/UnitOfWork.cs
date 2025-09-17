using RhSensoERP.Core.Abstractions.Interfaces;
using RhSensoERP.Infrastructure.Persistence;

namespace RhSensoERP.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    public UnitOfWork(AppDbContext db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
