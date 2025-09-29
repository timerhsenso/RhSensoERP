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
    [ApiExplorerSettings(GroupName = "RHU")]

    public class VerbaController : ControllerBase
    {
        private readonly AppDbContext _db;
        public VerbaController(AppDbContext db) => _db = db;

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

/// <summary>Lista Verba com paginação.</summary>
[HttpGet]
public async Task<ActionResult<ApiResponse<PagedResult<VerbaListItemDto>>>> List(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? sort = null, string? cdConta = null, string? descricao = null)
{
    var q = _db.Set<Verba>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(cdConta)) q = q.Where(x => x.CdConta == cdConta);
    if (!string.IsNullOrWhiteSpace(descricao)) q = q.Where(x => (x.DcConta ?? "").Contains(descricao));
    var ordered = ApplySort(q, sort);
    var total = await ordered.CountAsync();
    var items = await ordered
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new VerbaListItemDto { CdConta = x.CdConta, DcConta = x.DcConta, SgConta = x.SgConta })
        .ToListAsync();
    var resp = new PagedResult<VerbaListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    return Ok(ApiResponse<PagedResult<VerbaListItemDto>>.Ok(resp));
}

/// <summary>Exporta Verba para CSV (projeção em DTO de listagem).</summary>
[HttpPost("export")]
public async Task<IActionResult> Export([FromQuery] string? sort = null, string? cdConta = null, string? descricao = null)
{
    var q = _db.Set<Verba>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(cdConta)) q = q.Where(x => x.CdConta == cdConta);
    if (!string.IsNullOrWhiteSpace(descricao)) q = q.Where(x => (x.DcConta ?? "").Contains(descricao));
    var ordered = ApplySort(q, sort);
    var rows = await ordered.Select(x => new VerbaListItemDto { CdConta = x.CdConta, DcConta = x.DcConta, SgConta = x.SgConta }).ToListAsync();
    var bytes = ToCsv(rows);
    return File(bytes, "text/csv", "tcon2_export.csv");
}

/// <summary>Obtém Verba por chave.</summary>
[HttpGet("{cdConta}")]
public async Task<ActionResult<ApiResponse<Verba>>> Get(string cdConta)
{
    var entity = await _db.Set<Verba>().AsNoTracking().FirstOrDefaultAsync(x => x.CdConta == cdConta);
    if (entity == null) return NotFound(ApiResponse<Verba>.Fail("Registro não encontrado."));
    return Ok(ApiResponse<Verba>.Ok(entity));
}

/// <summary>Cria um Verba.</summary>
[HttpPost]
public async Task<ActionResult<ApiResponse<Verba>>> Create([FromBody] Verba model)
{
    _db.Set<Verba>().Add(model);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<Verba>.Ok(model, "Criado com sucesso."));
}

/// <summary>Atualiza um Verba pela chave.</summary>
[HttpPut("{cdConta}")]
public async Task<ActionResult<ApiResponse<object>>> Update(string cdConta, [FromBody] Verba model)
{
    var exists = await _db.Set<Verba>().AnyAsync(x => x.CdConta == cdConta);
    if (!exists) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para atualização."));
    _db.Entry(model).State = EntityState.Modified;
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Atualizado com sucesso."));
}

/// <summary>Exclui um Verba pela chave.</summary>
[HttpDelete("{cdConta}")]
public async Task<ActionResult<ApiResponse<object>>> Delete(string cdConta)
{
    var e = await _db.Set<Verba>().FirstOrDefaultAsync(x => x.CdConta == cdConta);
    if (e == null) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para exclusão."));
    _db.Remove(e);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Excluído com sucesso."));
}

/// <summary>Exclui em lote por chaves.</summary>
[HttpPost("bulk-delete")]
public async Task<ActionResult<ApiResponse<object>>> BulkDelete([FromBody] IEnumerable<VerbaKeyDto> keys)
{
    var set = _db.Set<Verba>();
    foreach (var k in keys)
    {
        var e = await set.FirstOrDefaultAsync(x => x.CdConta == k.CdConta);
        if (e != null) _db.Remove(e);
    }
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Registros excluídos."));
}

    }
}
