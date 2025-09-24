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
    public class CentroCustoController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CentroCustoController(AppDbContext db) => _db = db;

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

/// <summary>Lista CentroCusto com paginação.</summary>
[HttpGet]
public async Task<ActionResult<ApiResponse<PagedResult<CentroCustoListItemDto>>>> List(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? sort = null, string? cdCcusto = null, string? descricao = null)
{
    var q = _db.Set<CentroCusto>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(cdCcusto)) q = q.Where(x => x.CdCcusto == cdCcusto);
    if (!string.IsNullOrWhiteSpace(descricao)) q = q.Where(x => (x.DcCcusto ?? "").Contains(descricao));
    var ordered = ApplySort(q, sort);
    var total = await ordered.CountAsync();
    var items = await ordered
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new CentroCustoListItemDto { CdCcusto = x.CdCcusto, DcCcusto = x.DcCcusto, FlAtivo = x.FlAtivo })
        .ToListAsync();
    var resp = new PagedResult<CentroCustoListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    return Ok(ApiResponse<PagedResult<CentroCustoListItemDto>>.Ok(resp));
}

/// <summary>Exporta CentroCusto para CSV (projeção em DTO de listagem).</summary>
[HttpPost("export")]
public async Task<IActionResult> Export([FromQuery] string? sort = null, string? cdCcusto = null, string? descricao = null)
{
    var q = _db.Set<CentroCusto>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(cdCcusto)) q = q.Where(x => x.CdCcusto == cdCcusto);
    if (!string.IsNullOrWhiteSpace(descricao)) q = q.Where(x => (x.DcCcusto ?? "").Contains(descricao));
    var ordered = ApplySort(q, sort);
    var rows = await ordered.Select(x => new CentroCustoListItemDto { CdCcusto = x.CdCcusto, DcCcusto = x.DcCcusto, FlAtivo = x.FlAtivo }).ToListAsync();
    var bytes = ToCsv(rows);
    return File(bytes, "text/csv", "tcus1_export.csv");
}

/// <summary>Obtém CentroCusto por chave.</summary>
[HttpGet("{cdCcusto}")]
public async Task<ActionResult<ApiResponse<CentroCusto>>> Get(string cdCcusto)
{
    var entity = await _db.Set<CentroCusto>().AsNoTracking().FirstOrDefaultAsync(x => x.CdCcusto == cdCcusto);
    if (entity == null) return NotFound(ApiResponse<CentroCusto>.Fail("Registro não encontrado."));
    return Ok(ApiResponse<CentroCusto>.Ok(entity));
}

/// <summary>Cria um CentroCusto.</summary>
[HttpPost]
public async Task<ActionResult<ApiResponse<CentroCusto>>> Create([FromBody] CentroCusto model)
{
    _db.Set<CentroCusto>().Add(model);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<CentroCusto>.Ok(model, "Criado com sucesso."));
}

/// <summary>Atualiza um CentroCusto pela chave.</summary>
[HttpPut("{cdCcusto}")]
public async Task<ActionResult<ApiResponse<object>>> Update(string cdCcusto, [FromBody] CentroCusto model)
{
    var exists = await _db.Set<CentroCusto>().AnyAsync(x => x.CdCcusto == cdCcusto);
    if (!exists) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para atualização."));
    _db.Entry(model).State = EntityState.Modified;
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Atualizado com sucesso."));
}

/// <summary>Exclui um CentroCusto pela chave.</summary>
[HttpDelete("{cdCcusto}")]
public async Task<ActionResult<ApiResponse<object>>> Delete(string cdCcusto)
{
    var e = await _db.Set<CentroCusto>().FirstOrDefaultAsync(x => x.CdCcusto == cdCcusto);
    if (e == null) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para exclusão."));
    _db.Remove(e);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Excluído com sucesso."));
}

/// <summary>Exclui em lote por chaves.</summary>
[HttpPost("bulk-delete")]
public async Task<ActionResult<ApiResponse<object>>> BulkDelete([FromBody] IEnumerable<CentroCustoKeyDto> keys)
{
    var set = _db.Set<CentroCusto>();
    foreach (var k in keys)
    {
        var e = await set.FirstOrDefaultAsync(x => x.CdCcusto == k.CdCcusto);
        if (e != null) _db.Remove(e);
    }
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Registros excluídos."));
}

    }
}
