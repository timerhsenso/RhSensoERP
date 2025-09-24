using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Infrastructure.Persistence;
using RhSensoERP.Core.Shared;
using RhSensoERP.Core.Abstractions.Paging;
using RhSensoERP.Core.RHU.Entities;
using RhSensoERP.Core.RHU.DTOs;

namespace RhSensoERP.API.Controllers.RHU
{
    [ApiController]
    [Route("api/v1/rhu/[controller]")]
    [Produces("application/json")]
    public class AfastamentoController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AfastamentoController(AppDbContext db) => _db = db;

        private static IQueryable<T> ApplySort<T>(IQueryable<T> q, string? sort)
        {
            if (string.IsNullOrWhiteSpace(sort)) return q;
            var desc = sort.StartsWith("-");
            var prop = desc ? sort.Substring(1) : sort;
            return desc ? q.OrderByDescending(e => EF.Property<object>(e, prop))
                        : q.OrderBy(e => EF.Property<object>(e, prop));
        }

        private static byte[] ToCsv<T>(IEnumerable<T> rows, string sep = ";")
        {
            var props = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var lines = new List<string>();
            lines.Add(string.Join(sep, props.Select(p => Escape(p.Name, sep))));
            foreach (var r in rows)
                lines.Add(string.Join(sep, props.Select(p => Escape(p.GetValue(r), sep))));
            return System.Text.Encoding.UTF8.GetBytes(string.Join("\n", lines));
        }
        private static string Escape(object? v, string sep)
        {
            if (v is null) return string.Empty;
            var s = Convert.ToString(v) ?? string.Empty;
            if (s.Contains(sep) || s.Contains('\n') || s.Contains('\r') || s.Contains('"'))
                s = "\""+ s.Replace("\"", "\"\"") + "\"";
            return s;
        }

/// <summary>Lista Afastamento com paginação.</summary>
[HttpGet]
public async Task<ActionResult<ApiResponse<PagedResult<AfastamentoListItemDto>>>> List(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? sort = null, string? noMatric = null, int? cdEmpresa = null, int? cdFilial = null, DateTime? de = null, DateTime? ate = null)
{
    var q = _db.Set<Afastamento>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(noMatric)) q = q.Where(x => x.NoMatric == noMatric);
    if (cdEmpresa.HasValue) q = q.Where(x => x.CdEmpresa == cdEmpresa.Value);
    if (cdFilial.HasValue) q = q.Where(x => x.CdFilial == cdFilial.Value);
    if (de.HasValue) q = q.Where(x => x.DtAfast >= de.Value);
    if (ate.HasValue) q = q.Where(x => x.DtAfast <= ate.Value);
    var ordered = ApplySort(q, sort);
    var total = await ordered.CountAsync();
    var items = await ordered
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new AfastamentoListItemDto { NoMatric = x.NoMatric, CdEmpresa = x.CdEmpresa, CdFilial = x.CdFilial, DtAfast = x.DtAfast, CdMotAfas = x.CdMotAfas, CdSituacao = x.CdSituacao })
        .ToListAsync();
    var resp = new PagedResult<AfastamentoListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    return Ok(ApiResponse<PagedResult<AfastamentoListItemDto>>.Ok(resp));
}

/// <summary>Exporta Afastamento para CSV (projeção em DTO de listagem).</summary>
[HttpPost("export")]
public async Task<IActionResult> Export([FromQuery] string? sort = null, string? noMatric = null, int? cdEmpresa = null, int? cdFilial = null, DateTime? de = null, DateTime? ate = null)
{
    var q = _db.Set<Afastamento>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(noMatric)) q = q.Where(x => x.NoMatric == noMatric);
    if (cdEmpresa.HasValue) q = q.Where(x => x.CdEmpresa == cdEmpresa.Value);
    if (cdFilial.HasValue) q = q.Where(x => x.CdFilial == cdFilial.Value);
    if (de.HasValue) q = q.Where(x => x.DtAfast >= de.Value);
    if (ate.HasValue) q = q.Where(x => x.DtAfast <= ate.Value);
    var ordered = ApplySort(q, sort);
    var rows = await ordered.Select(x => new AfastamentoListItemDto { NoMatric = x.NoMatric, CdEmpresa = x.CdEmpresa, CdFilial = x.CdFilial, DtAfast = x.DtAfast, CdMotAfas = x.CdMotAfas, CdSituacao = x.CdSituacao }).ToListAsync();
    var bytes = ToCsv(rows);
    return File(bytes, "text/csv", "afas1_export.csv");
}

/// <summary>Obtém Afastamento por chave.</summary>
[HttpGet("{noMatric}/{cdEmpresa:int}/{cdFilial:int}/{dtAfast:datetime}")]
public async Task<ActionResult<ApiResponse<Afastamento>>> Get(string noMatric, int cdEmpresa, int cdFilial, DateTime dtAfast)
{
    var entity = await _db.Set<Afastamento>().AsNoTracking().FirstOrDefaultAsync(x => x.NoMatric == noMatric && x.CdEmpresa == cdEmpresa && x.CdFilial == cdFilial && x.DtAfast == dtAfast);
    if (entity == null) return NotFound(ApiResponse<Afastamento>.Fail("Registro não encontrado."));
    return Ok(ApiResponse<Afastamento>.Ok(entity));
}

/// <summary>Cria um Afastamento.</summary>
[HttpPost]
public async Task<ActionResult<ApiResponse<Afastamento>>> Create([FromBody] Afastamento model)
{
    _db.Set<Afastamento>().Add(model);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<Afastamento>.Ok(model, "Criado com sucesso."));
}

/// <summary>Atualiza um Afastamento pela chave.</summary>
[HttpPut("{noMatric}/{cdEmpresa:int}/{cdFilial:int}/{dtAfast:datetime}")]
public async Task<ActionResult<ApiResponse<object>>> Update(string noMatric, int cdEmpresa, int cdFilial, DateTime dtAfast, [FromBody] Afastamento model)
{
    var exists = await _db.Set<Afastamento>().AnyAsync(x => x.NoMatric == noMatric && x.CdEmpresa == cdEmpresa && x.CdFilial == cdFilial && x.DtAfast == dtAfast);
    if (!exists) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para atualização."));
    _db.Entry(model).State = EntityState.Modified;
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Atualizado com sucesso."));
}

/// <summary>Exclui um Afastamento pela chave.</summary>
[HttpDelete("{noMatric}/{cdEmpresa:int}/{cdFilial:int}/{dtAfast:datetime}")]
public async Task<ActionResult<ApiResponse<object>>> Delete(string noMatric, int cdEmpresa, int cdFilial, DateTime dtAfast)
{
    var e = await _db.Set<Afastamento>().FirstOrDefaultAsync(x => x.NoMatric == noMatric && x.CdEmpresa == cdEmpresa && x.CdFilial == cdFilial && x.DtAfast == dtAfast);
    if (e == null) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para exclusão."));
    _db.Remove(e);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Excluído com sucesso."));
}

/// <summary>Exclui em lote por chaves.</summary>
[HttpPost("bulk-delete")]
public async Task<ActionResult<ApiResponse<object>>> BulkDelete([FromBody] IEnumerable<AfastamentoKeyDto> keys)
{
    var set = _db.Set<Afastamento>();
    foreach (var k in keys)
    {
        var e = await set.FirstOrDefaultAsync(x => x.NoMatric == k.NoMatric && x.CdEmpresa == k.CdEmpresa && x.CdFilial == k.CdFilial && x.DtAfast == k.DtAfast);
        if (e != null) _db.Remove(e);
    }
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Registros excluídos."));
}

    }
}
