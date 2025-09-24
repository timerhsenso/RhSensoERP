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
    public class MunicipioController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MunicipioController(AppDbContext db) => _db = db;

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

/// <summary>Lista Municipio com paginação.</summary>
[HttpGet]
public async Task<ActionResult<ApiResponse<PagedResult<MunicipioListItemDto>>>> List(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? sort = null, string? cdMunicip = null, string? uf = null, string? nome = null)
{
    var q = _db.Set<Municipio>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(cdMunicip)) q = q.Where(x => x.CdMunicip == cdMunicip);
    if (!string.IsNullOrWhiteSpace(uf)) q = q.Where(x => (x.SgEstado ?? "") == uf);
    if (!string.IsNullOrWhiteSpace(nome)) q = q.Where(x => (x.NmMunicip ?? "").Contains(nome));
    var ordered = ApplySort(q, sort);
    var total = await ordered.CountAsync();
    var items = await ordered
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new MunicipioListItemDto { CdMunicip = x.CdMunicip, NmMunicip = x.NmMunicip, SgEstado = x.SgEstado, CodIbge = x.CodIbge })
        .ToListAsync();
    var resp = new PagedResult<MunicipioListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    return Ok(ApiResponse<PagedResult<MunicipioListItemDto>>.Ok(resp));
}

/// <summary>Exporta Municipio para CSV (projeção em DTO de listagem).</summary>
[HttpPost("export")]
public async Task<IActionResult> Export([FromQuery] string? sort = null, string? cdMunicip = null, string? uf = null, string? nome = null)
{
    var q = _db.Set<Municipio>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(cdMunicip)) q = q.Where(x => x.CdMunicip == cdMunicip);
    if (!string.IsNullOrWhiteSpace(uf)) q = q.Where(x => (x.SgEstado ?? "") == uf);
    if (!string.IsNullOrWhiteSpace(nome)) q = q.Where(x => (x.NmMunicip ?? "").Contains(nome));
    var ordered = ApplySort(q, sort);
    var rows = await ordered.Select(x => new MunicipioListItemDto { CdMunicip = x.CdMunicip, NmMunicip = x.NmMunicip, SgEstado = x.SgEstado, CodIbge = x.CodIbge }).ToListAsync();
    var bytes = ToCsv(rows);
    return File(bytes, "text/csv", "muni1_export.csv");
}

/// <summary>Obtém Municipio por chave.</summary>
[HttpGet("{cdMunicip}")]
public async Task<ActionResult<ApiResponse<Municipio>>> Get(string cdMunicip)
{
    var entity = await _db.Set<Municipio>().AsNoTracking().FirstOrDefaultAsync(x => x.CdMunicip == cdMunicip);
    if (entity == null) return NotFound(ApiResponse<Municipio>.Fail("Registro não encontrado."));
    return Ok(ApiResponse<Municipio>.Ok(entity));
}

/// <summary>Cria um Municipio.</summary>
[HttpPost]
public async Task<ActionResult<ApiResponse<Municipio>>> Create([FromBody] Municipio model)
{
    _db.Set<Municipio>().Add(model);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<Municipio>.Ok(model, "Criado com sucesso."));
}

/// <summary>Atualiza um Municipio pela chave.</summary>
[HttpPut("{cdMunicip}")]
public async Task<ActionResult<ApiResponse<object>>> Update(string cdMunicip, [FromBody] Municipio model)
{
    var exists = await _db.Set<Municipio>().AnyAsync(x => x.CdMunicip == cdMunicip);
    if (!exists) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para atualização."));
    _db.Entry(model).State = EntityState.Modified;
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Atualizado com sucesso."));
}

/// <summary>Exclui um Municipio pela chave.</summary>
[HttpDelete("{cdMunicip}")]
public async Task<ActionResult<ApiResponse<object>>> Delete(string cdMunicip)
{
    var e = await _db.Set<Municipio>().FirstOrDefaultAsync(x => x.CdMunicip == cdMunicip);
    if (e == null) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para exclusão."));
    _db.Remove(e);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Excluído com sucesso."));
}

/// <summary>Exclui em lote por chaves.</summary>
[HttpPost("bulk-delete")]
public async Task<ActionResult<ApiResponse<object>>> BulkDelete([FromBody] IEnumerable<MunicipioKeyDto> keys)
{
    var set = _db.Set<Municipio>();
    foreach (var k in keys)
    {
        var e = await set.FirstOrDefaultAsync(x => x.CdMunicip == k.CdMunicip);
        if (e != null) _db.Remove(e);
    }
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Registros excluídos."));
}

    }
}
