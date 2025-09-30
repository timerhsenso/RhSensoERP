using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Core.SEG.Entities; // <- usa diretamente o namespace da entidade
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RhSensoERP.Application.Common.Interfaces.Repositories.SEG
{
    public interface ISistemaRepository
    {
        Task<List<Sistema>> GetAllAsync(CancellationToken ct = default);
        Task<Sistema?> GetByIdAsync(string cdSistema, CancellationToken ct = default);
        Task<bool> ExistsAsync(string cdSistema, CancellationToken ct = default);
        Task AddAsync(Sistema entity, CancellationToken ct = default);
        Task UpdateAsync(Sistema entity, CancellationToken ct = default);
        Task<bool> DeleteAsync(string cdSistema, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
