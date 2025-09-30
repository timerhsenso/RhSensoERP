using RhSensoERP.Application.SEG.DTOs;

namespace RhSensoERP.Application.SEG.Services.Interfaces
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