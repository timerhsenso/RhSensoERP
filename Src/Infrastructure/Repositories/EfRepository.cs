using Microsoft.EntityFrameworkCore;
using RhSensoERP.Core.Abstractions.Entities;
using RhSensoERP.Core.Abstractions.Interfaces;
using RhSensoERP.Core.Abstractions.Paging;
using RhSensoERP.Infrastructure.Persistence;

namespace RhSensoERP.Infrastructure.Repositories;

public class EfRepository<T> : IRepository<T> where T : BaseEntity, ISoftDeletable
{
    private readonly AppDbContext _db;
    public EfRepository(AppDbContext db) => _db = db;

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<PagedResult<T>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.Set<T>().AsNoTracking();
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<T> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    }

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await _db.Set<T>().AddAsync(entity, ct);
    }

    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _db.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        entity.IsDeleted = true; // soft delete
        _db.Set<T>().Update(entity);
        return Task.CompletedTask;
    }
}
