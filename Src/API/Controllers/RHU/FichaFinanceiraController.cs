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
using RhSensoERP.Application.RHU.DTOs;

namespace RhSensoERP.API.Controllers.RHU
{
    [ApiController]
    [Route("api/v1/rhu/[controller]")]
    [Produces("application/json")]
    public class FichaFinanceiraController : ControllerBase
    {
        private readonly AppDbContext _db;
        public FichaFinanceiraController(AppDbContext db) => _db = db;

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

/// <summary>Lista FichaFinanceira com paginação.</summary>
[HttpGet]
public async Task<ActionResult<ApiResponse<PagedResult<FichaFinanceiraListItemDto>>>> List(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? sort = null, string? noMatric = null, int? cdEmpresa = null, int? cdFilial = null, string? cdConta = null, DateTime? de = null, DateTime? ate = null)
{
    var q = _db.Set<FichaFinanceira>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(noMatric)) q = q.Where(x => x.NoMatric == noMatric);
    if (cdEmpresa.HasValue) q = q.Where(x => x.CdEmpresa == cdEmpresa.Value);
    if (cdFilial.HasValue) q = q.Where(x => x.CdFilial == cdFilial.Value);
    if (!string.IsNullOrWhiteSpace(cdConta)) q = q.Where(x => x.CdConta == cdConta);
    if (de.HasValue) q = q.Where(x => x.DtConta >= de.Value);
    if (ate.HasValue) q = q.Where(x => x.DtConta <= ate.Value);
    var ordered = ApplySort(q, sort);
    var total = await ordered.CountAsync();
    var items = await ordered
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new FichaFinanceiraListItemDto { NoMatric = x.NoMatric, CdEmpresa = x.CdEmpresa, CdFilial = x.CdFilial, CdConta = x.CdConta, DtConta = x.DtConta, VlConta = x.VlConta, QtConta = x.QtConta })
        .ToListAsync();
    var resp = new PagedResult<FichaFinanceiraListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    return Ok(ApiResponse<PagedResult<FichaFinanceiraListItemDto>>.Ok(resp));
}

/// <summary>Exporta FichaFinanceira para CSV (projeção em DTO de listagem).</summary>
[HttpPost("export")]
public async Task<IActionResult> Export([FromQuery] string? sort = null, string? noMatric = null, int? cdEmpresa = null, int? cdFilial = null, string? cdConta = null, DateTime? de = null, DateTime? ate = null)
{
    var q = _db.Set<FichaFinanceira>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(noMatric)) q = q.Where(x => x.NoMatric == noMatric);
    if (cdEmpresa.HasValue) q = q.Where(x => x.CdEmpresa == cdEmpresa.Value);
    if (cdFilial.HasValue) q = q.Where(x => x.CdFilial == cdFilial.Value);
    if (!string.IsNullOrWhiteSpace(cdConta)) q = q.Where(x => x.CdConta == cdConta);
    if (de.HasValue) q = q.Where(x => x.DtConta >= de.Value);
    if (ate.HasValue) q = q.Where(x => x.DtConta <= ate.Value);
    var ordered = ApplySort(q, sort);
    var rows = await ordered.Select(x => new FichaFinanceiraListItemDto { NoMatric = x.NoMatric, CdEmpresa = x.CdEmpresa, CdFilial = x.CdFilial, CdConta = x.CdConta, DtConta = x.DtConta, VlConta = x.VlConta, QtConta = x.QtConta }).ToListAsync();
    var bytes = ToCsv(rows);
    return File(bytes, "text/csv", "ficha1_export.csv");
}

/// <summary>Obtém FichaFinanceira por chave.</summary>
[HttpGet("{noMatric}/{cdEmpresa:int}/{cdFilial:int}/{cdConta}/{dtConta:datetime}")]
public async Task<ActionResult<ApiResponse<FichaFinanceira>>> Get(string noMatric, int cdEmpresa, int cdFilial, string cdConta, DateTime dtConta)
{
    var entity = await _db.Set<FichaFinanceira>().AsNoTracking().FirstOrDefaultAsync(x => x.NoMatric == noMatric && x.CdEmpresa == cdEmpresa && x.CdFilial == cdFilial && x.CdConta == cdConta && x.DtConta == dtConta);
    if (entity == null) return NotFound(ApiResponse<FichaFinanceira>.Fail("Registro não encontrado."));
    return Ok(ApiResponse<FichaFinanceira>.Ok(entity));
}

/// <summary>Cria um FichaFinanceira.</summary>
[HttpPost]
public async Task<ActionResult<ApiResponse<FichaFinanceira>>> Create([FromBody] FichaFinanceira model)
{
    _db.Set<FichaFinanceira>().Add(model);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<FichaFinanceira>.Ok(model, "Criado com sucesso."));
}

/// <summary>Atualiza um FichaFinanceira pela chave.</summary>
[HttpPut("{noMatric}/{cdEmpresa:int}/{cdFilial:int}/{cdConta}/{dtConta:datetime}")]
public async Task<ActionResult<ApiResponse<object>>> Update(string noMatric, int cdEmpresa, int cdFilial, string cdConta, DateTime dtConta, [FromBody] FichaFinanceira model)
{
    var exists = await _db.Set<FichaFinanceira>().AnyAsync(x => x.NoMatric == noMatric && x.CdEmpresa == cdEmpresa && x.CdFilial == cdFilial && x.CdConta == cdConta && x.DtConta == dtConta);
    if (!exists) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para atualização."));
    _db.Entry(model).State = EntityState.Modified;
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Atualizado com sucesso."));
}

/// <summary>Exclui um FichaFinanceira pela chave.</summary>
[HttpDelete("{noMatric}/{cdEmpresa:int}/{cdFilial:int}/{cdConta}/{dtConta:datetime}")]
public async Task<ActionResult<ApiResponse<object>>> Delete(string noMatric, int cdEmpresa, int cdFilial, string cdConta, DateTime dtConta)
{
    var e = await _db.Set<FichaFinanceira>().FirstOrDefaultAsync(x => x.NoMatric == noMatric && x.CdEmpresa == cdEmpresa && x.CdFilial == cdFilial && x.CdConta == cdConta && x.DtConta == dtConta);
    if (e == null) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para exclusão."));
    _db.Remove(e);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Excluído com sucesso."));
}

/// <summary>Exclui em lote por chaves.</summary>
[HttpPost("bulk-delete")]
public async Task<ActionResult<ApiResponse<object>>> BulkDelete([FromBody] IEnumerable<FichaFinanceiraKeyDto> keys)
{
    var set = _db.Set<FichaFinanceira>();
    foreach (var k in keys)
    {
        var e = await set.FirstOrDefaultAsync(x => x.NoMatric == k.NoMatric && x.CdEmpresa == k.CdEmpresa && x.CdFilial == k.CdFilial && x.CdConta == k.CdConta && x.DtConta == k.DtConta);
        if (e != null) _db.Remove(e);
    }
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Registros excluídos."));
}

    }
}
