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
    public class CalendarioMunicipalController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CalendarioMunicipalController(AppDbContext db) => _db = db;

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

/// <summary>Lista CalendarioMunicipal com paginação.</summary>
[HttpGet]
public async Task<ActionResult<ApiResponse<PagedResult<CalendarioMunicipalListItemDto>>>> List(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? sort = null, string? cdMunicip = null, DateTime? de = null, DateTime? ate = null)
{
    var q = _db.Set<CalendarioMunicipal>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(cdMunicip)) q = q.Where(x => x.CdMunicip == cdMunicip);
    if (de.HasValue) q = q.Where(x => x.DtCalend >= de.Value);
    if (ate.HasValue) q = q.Where(x => x.DtCalend <= ate.Value);
    var ordered = ApplySort(q, sort);
    var total = await ordered.CountAsync();
    var items = await ordered
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new CalendarioMunicipalListItemDto { CdMunicip = x.CdMunicip, DtCalend = x.DtCalend, CdFeriado = x.CdFeriado })
        .ToListAsync();
    var resp = new PagedResult<CalendarioMunicipalListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    return Ok(ApiResponse<PagedResult<CalendarioMunicipalListItemDto>>.Ok(resp));
}

/// <summary>Exporta CalendarioMunicipal para CSV (projeção em DTO de listagem).</summary>
[HttpPost("export")]
public async Task<IActionResult> Export([FromQuery] string? sort = null, string? cdMunicip = null, DateTime? de = null, DateTime? ate = null)
{
    var q = _db.Set<CalendarioMunicipal>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(cdMunicip)) q = q.Where(x => x.CdMunicip == cdMunicip);
    if (de.HasValue) q = q.Where(x => x.DtCalend >= de.Value);
    if (ate.HasValue) q = q.Where(x => x.DtCalend <= ate.Value);
    var ordered = ApplySort(q, sort);
    var rows = await ordered.Select(x => new CalendarioMunicipalListItemDto { CdMunicip = x.CdMunicip, DtCalend = x.DtCalend, CdFeriado = x.CdFeriado }).ToListAsync();
    var bytes = ToCsv(rows);
    return File(bytes, "text/csv", "calnd1_export.csv");
}

/// <summary>Obtém CalendarioMunicipal por chave.</summary>
[HttpGet("{cdMunicip}/{dtCalend:datetime}")]
public async Task<ActionResult<ApiResponse<CalendarioMunicipal>>> Get(string cdMunicip, DateTime dtCalend)
{
    var entity = await _db.Set<CalendarioMunicipal>().AsNoTracking().FirstOrDefaultAsync(x => x.CdMunicip == cdMunicip && x.DtCalend == dtCalend);
    if (entity == null) return NotFound(ApiResponse<CalendarioMunicipal>.Fail("Registro não encontrado."));
    return Ok(ApiResponse<CalendarioMunicipal>.Ok(entity));
}

/// <summary>Cria um CalendarioMunicipal.</summary>
[HttpPost]
public async Task<ActionResult<ApiResponse<CalendarioMunicipal>>> Create([FromBody] CalendarioMunicipal model)
{
    _db.Set<CalendarioMunicipal>().Add(model);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<CalendarioMunicipal>.Ok(model, "Criado com sucesso."));
}

/// <summary>Atualiza um CalendarioMunicipal pela chave.</summary>
[HttpPut("{cdMunicip}/{dtCalend:datetime}")]
public async Task<ActionResult<ApiResponse<object>>> Update(string cdMunicip, DateTime dtCalend, [FromBody] CalendarioMunicipal model)
{
    var exists = await _db.Set<CalendarioMunicipal>().AnyAsync(x => x.CdMunicip == cdMunicip && x.DtCalend == dtCalend);
    if (!exists) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para atualização."));
    _db.Entry(model).State = EntityState.Modified;
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Atualizado com sucesso."));
}

/// <summary>Exclui um CalendarioMunicipal pela chave.</summary>
[HttpDelete("{cdMunicip}/{dtCalend:datetime}")]
public async Task<ActionResult<ApiResponse<object>>> Delete(string cdMunicip, DateTime dtCalend)
{
    var e = await _db.Set<CalendarioMunicipal>().FirstOrDefaultAsync(x => x.CdMunicip == cdMunicip && x.DtCalend == dtCalend);
    if (e == null) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para exclusão."));
    _db.Remove(e);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Excluído com sucesso."));
}

/// <summary>Exclui em lote por chaves.</summary>
[HttpPost("bulk-delete")]
public async Task<ActionResult<ApiResponse<object>>> BulkDelete([FromBody] IEnumerable<CalendarioMunicipalKeyDto> keys)
{
    var set = _db.Set<CalendarioMunicipal>();
    foreach (var k in keys)
    {
        var e = await set.FirstOrDefaultAsync(x => x.CdMunicip == k.CdMunicip && x.DtCalend == k.DtCalend);
        if (e != null) _db.Remove(e);
    }
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Registros excluídos."));
}

    }
}
