using Microsoft.EntityFrameworkCore;
using RhSensoERP.Application.SEG.DTOs;
using RhSensoERP.Application.SEG.Interfaces;
using RhSensoERP.Core.Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RhSensoERP.Infrastructure.Services.SEG
{
    internal sealed class SistemaService : ISistemaService
    {
        private readonly DbContext _ctx;

        public SistemaService(DbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IEnumerable<SistemaDto>> GetAllAsync(CancellationToken ct = default)
        {
            return await _ctx.Set<Sistema>()
                .AsNoTracking()
                .OrderBy(x => x.CdSistema)
                .Select(x => new SistemaDto
                {
                    CdSistema = x.CdSistema,
                    DcSistema = x.DcSistema,
                    Ativo = x.Ativo
                })
                .ToListAsync(ct);
        }

        public async Task<SistemaDto?> GetByIdAsync(string cdSistema, CancellationToken ct = default)
        {
            var e = await _ctx.Set<Sistema>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CdSistema == cdSistema, ct);

            return e is null ? null : new SistemaDto
            {
                CdSistema = e.CdSistema,
                DcSistema = e.DcSistema,
                Ativo = e.Ativo
            };
        }

        public async Task<SistemaDto> CreateAsync(SistemaUpsertDto dto, CancellationToken ct = default)
        {
            // não permitir duplicidade de chave
            var exists = await _ctx.Set<Sistema>().AnyAsync(x => x.CdSistema == dto.CdSistema, ct);
            if (exists)
                throw new InvalidOperationException($"Já existe um sistema com código '{dto.CdSistema}'.");

            var e = new Sistema
            {
                CdSistema = dto.CdSistema.Trim(),
                DcSistema = dto.DcSistema.Trim(),
                Ativo = dto.Ativo
            };

            await _ctx.Set<Sistema>().AddAsync(e, ct);
            await _ctx.SaveChangesAsync(ct);

            return new SistemaDto { CdSistema = e.CdSistema, DcSistema = e.DcSistema, Ativo = e.Ativo };
        }

        public async Task<SistemaDto?> UpdateAsync(string cdSistema, SistemaUpsertDto dto, CancellationToken ct = default)
        {
            var e = await _ctx.Set<Sistema>().FirstOrDefaultAsync(x => x.CdSistema == cdSistema, ct);
            if (e is null) return null;

            e.DcSistema = dto.DcSistema.Trim();
            e.Ativo = dto.Ativo;

            _ctx.Set<Sistema>().Update(e);
            await _ctx.SaveChangesAsync(ct);

            return new SistemaDto { CdSistema = e.CdSistema, DcSistema = e.DcSistema, Ativo = e.Ativo };
        }

        public async Task<bool> DeleteAsync(string cdSistema, CancellationToken ct = default)
        {
            var e = await _ctx.Set<Sistema>().FirstOrDefaultAsync(x => x.CdSistema == cdSistema, ct);
            if (e is null) return false;

            _ctx.Set<Sistema>().Remove(e);

            try
            {
                await _ctx.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateException ex)
            {
                // bloqueia exclusão quando há relacionamentos (ex.: Funções vinculadas)
                throw new InvalidOperationException(
                    $"Não foi possível excluir o sistema '{cdSistema}'. Verifique relacionamentos.", ex);
            }
        }
    }
}
