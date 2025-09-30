using Microsoft.EntityFrameworkCore;
using RhSensoERP.Application.Common.Interfaces.Repositories.SEG;
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Infrastructure.Persistence.Repositories.SEG
{
    internal sealed class SistemaRepository : ISistemaRepository
    {
        private readonly DbContext _ctx;

        public SistemaRepository(DbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<Sistema>> GetAllAsync(CancellationToken ct = default)
            => await _ctx.Set<Sistema>().AsNoTracking().OrderBy(x => x.CdSistema).ToListAsync(ct);

        public async Task<Sistema?> GetByIdAsync(string cdSistema, CancellationToken ct = default)
            => await _ctx.Set<Sistema>().AsNoTracking().FirstOrDefaultAsync(x => x.CdSistema == cdSistema, ct);

        public async Task<bool> ExistsAsync(string cdSistema, CancellationToken ct = default)
            => await _ctx.Set<Sistema>().AnyAsync(x => x.CdSistema == cdSistema, ct);

        public async Task AddAsync(Sistema entity, CancellationToken ct = default)
            => await _ctx.Set<Sistema>().AddAsync(entity, ct);

        public Task UpdateAsync(Sistema entity, CancellationToken ct = default)
        {
            _ctx.Set<Sistema>().Update(entity);
            return Task.CompletedTask;
        }

        public async Task<bool> DeleteAsync(string cdSistema, CancellationToken ct = default)
        {
            var existing = await _ctx.Set<Sistema>().FirstOrDefaultAsync(x => x.CdSistema == cdSistema, ct);
            if (existing is null) return false;
            _ctx.Set<Sistema>().Remove(existing);
            return true;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _ctx.SaveChangesAsync(ct);
    }
}