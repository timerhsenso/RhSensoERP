using RhSensoERP.Application.SEG.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RhSensoERP.Application.SEG.Interfaces
{
    public interface IBotaoFuncaoService
    {
        Task<PagedResult<BotaoFuncaoDto>> GetAsync(BotaoFuncaoQuery query, CancellationToken ct);
        Task<BotaoFuncaoDto?> GetByIdAsync(string cdSistema, string cdFuncao, string nmBotao, CancellationToken ct);
        Task<BotaoFuncaoDto> CreateAsync(BotaoFuncaoDto dto, CancellationToken ct);
        Task<BotaoFuncaoDto> UpdateAsync(string cdSistema, string cdFuncao, string nmBotao, BotaoFuncaoDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(string cdSistema, string cdFuncao, string nmBotao, CancellationToken ct);
        Task<int> DeleteManyAsync(IEnumerable<BotaoFuncaoKeyDto> keys, CancellationToken ct);
    }
}
