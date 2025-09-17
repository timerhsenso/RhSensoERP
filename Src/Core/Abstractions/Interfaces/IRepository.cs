using RhSensoERP.Core.Abstractions.Entities;
using RhSensoERP.Core.Abstractions.Paging;

namespace RhSensoERP.Core.Abstractions.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<T>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default); // soft delete handled by infra
}
