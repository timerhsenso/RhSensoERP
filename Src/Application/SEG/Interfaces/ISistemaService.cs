using RhSensoERP.Application.SEG.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RhSensoERP.Application.SEG.Interfaces
{
    public interface ISistemaService
    {
        Task<IEnumerable<SistemaDto>> GetAllAsync(CancellationToken ct = default);
        Task<SistemaDto?> GetByIdAsync(string cdSistema, CancellationToken ct = default);
        Task<SistemaDto> CreateAsync(SistemaUpsertDto dto, CancellationToken ct = default);
        Task<SistemaDto?> UpdateAsync(string cdSistema, SistemaUpsertDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(string cdSistema, CancellationToken ct = default);
    }
}
