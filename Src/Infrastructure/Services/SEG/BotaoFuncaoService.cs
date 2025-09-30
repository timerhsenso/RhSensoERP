using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using RhSensoERP.Application.SEG.DTOs;
using RhSensoERP.Application.SEG.Interfaces;
using RhSensoERP.Core.Security.Entities;

namespace RhSensoERP.Infrastructure.Services.SEG
{
    public sealed class BotaoFuncaoService : IBotaoFuncaoService
    {
        private readonly DbContext _db; // injeta DbContext para evitar problema de namespace do ApplicationDbContext

        public BotaoFuncaoService(DbContext db)
        {
            _db = db;
        }

        public async Task<PagedResult<BotaoFuncaoDto>> GetAsync(BotaoFuncaoQuery query, CancellationToken ct)
        {
            var q = _db.Set<BotaoFuncao>().AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.CdSistema))
                q = q.Where(x => x.CdSistema == query.CdSistema);

            if (!string.IsNullOrWhiteSpace(query.CdFuncao))
                q = q.Where(x => x.CdFuncao == query.CdFuncao);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim();
                q = q.Where(x => x.NmBotao.Contains(s) || x.DcBotao.Contains(s));
            }

            var total = await q.LongCountAsync(ct);

            var page = query.Page <= 0 ? 1 : query.Page;
            var size = query.PageSize <= 0 ? 20 : query.PageSize;

            var items = await q
                .OrderBy(x => x.CdSistema)
                .ThenBy(x => x.CdFuncao)
                .ThenBy(x => x.NmBotao)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(x => new BotaoFuncaoDto
                {
                    CdSistema = x.CdSistema,
                    CdFuncao = x.CdFuncao,
                    NmBotao = x.NmBotao,
                    DcBotao = x.DcBotao,
                    CdAcao = x.CdAcao
                })
                .ToListAsync(ct);

            return new PagedResult<BotaoFuncaoDto>
            {
                Page = page,
                PageSize = size,
                Total = total,
                Items = items
            };
        }

        public async Task<BotaoFuncaoDto?> GetByIdAsync(string cdSistema, string cdFuncao, string nmBotao, CancellationToken ct)
        {
            var e = await _db.Set<BotaoFuncao>().AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CdSistema == cdSistema &&
                    x.CdFuncao == cdFuncao &&
                    x.NmBotao == nmBotao, ct);

            if (e is null) return null;

            return new BotaoFuncaoDto
            {
                CdSistema = e.CdSistema,
                CdFuncao = e.CdFuncao,
                NmBotao = e.NmBotao,
                DcBotao = e.DcBotao,
                CdAcao = e.CdAcao
            };
        }

        public async Task<BotaoFuncaoDto> CreateAsync(BotaoFuncaoDto dto, CancellationToken ct)
        {
            var exists = await _db.Set<BotaoFuncao>().AnyAsync(x =>
                x.CdSistema == dto.CdSistema &&
                x.CdFuncao == dto.CdFuncao &&
                x.NmBotao == dto.NmBotao, ct);

            if (exists)
                throw new DbUpdateException($"Já existe o botão '{dto.NmBotao}' para a função '{dto.CdFuncao}' do sistema '{dto.CdSistema}'.");

            var e = new BotaoFuncao
            {
                CdSistema = dto.CdSistema.Trim(),
                CdFuncao = dto.CdFuncao.Trim(),
                NmBotao = dto.NmBotao.Trim(),
                DcBotao = dto.DcBotao.Trim(),
                CdAcao = char.ToUpperInvariant(dto.CdAcao)
            };

            _db.Add(e);
            await _db.SaveChangesAsync(ct);

            return new BotaoFuncaoDto
            {
                CdSistema = e.CdSistema,
                CdFuncao = e.CdFuncao,
                NmBotao = e.NmBotao,
                DcBotao = e.DcBotao,
                CdAcao = e.CdAcao
            };
        }

        public async Task<BotaoFuncaoDto> UpdateAsync(string cdSistema, string cdFuncao, string nmBotao, BotaoFuncaoDto dto, CancellationToken ct)
        {
            var e = await _db.Set<BotaoFuncao>().FirstOrDefaultAsync(x =>
                x.CdSistema == cdSistema &&
                x.CdFuncao == cdFuncao &&
                x.NmBotao == nmBotao, ct);

            if (e is null)
                throw new KeyNotFoundException("Registro não encontrado.");

            // PK composta não muda
            e.DcBotao = dto.DcBotao.Trim();
            e.CdAcao = char.ToUpperInvariant(dto.CdAcao);

            await _db.SaveChangesAsync(ct);

            return new BotaoFuncaoDto
            {
                CdSistema = e.CdSistema,
                CdFuncao = e.CdFuncao,
                NmBotao = e.NmBotao,
                DcBotao = e.DcBotao,
                CdAcao = e.CdAcao
            };
        }

        public async Task<bool> DeleteAsync(string cdSistema, string cdFuncao, string nmBotao, CancellationToken ct)
        {
            var e = await _db.Set<BotaoFuncao>().FirstOrDefaultAsync(x =>
                x.CdSistema == cdSistema &&
                x.CdFuncao == cdFuncao &&
                x.NmBotao == nmBotao, ct);

            if (e is null) return false;

            _db.Remove(e);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<int> DeleteManyAsync(IEnumerable<BotaoFuncaoKeyDto> keys, CancellationToken ct)
        {
            // Para performance em lote, monta um HashSet com a PK composta
            var keySet = new HashSet<(string CdSistema, string CdFuncao, string NmBotao)>(
                keys.Select(k => (k.CdSistema, k.CdFuncao, k.NmBotao)));

            var set = _db.Set<BotaoFuncao>();
            var toDelete = await set
                .Where(x => keySet.Contains(new(x.CdSistema, x.CdFuncao, x.NmBotao)))
                .ToListAsync(ct);

            if (toDelete.Count == 0) return 0;

            set.RemoveRange(toDelete);
            return await _db.SaveChangesAsync(ct);
        }
    }
}
