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

    public class FilialController : ControllerBase
    {
        private readonly AppDbContext _db;
        public FilialController(AppDbContext db) => _db = db;

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

/// <summary>Lista Filial com paginação.</summary>
[HttpGet]
public async Task<ActionResult<ApiResponse<PagedResult<FilialListItemDto>>>> List(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? sort = null, int? cdEmpresa = null, int? cdFilial = null, string? nome = null)
{
    var q = _db.Set<Filial>().AsNoTracking().AsQueryable();
    if (cdEmpresa.HasValue) q = q.Where(x => x.CdEmpresa == cdEmpresa.Value);
    if (cdFilial.HasValue) q = q.Where(x => x.CdFilial == cdFilial.Value);
    if (!string.IsNullOrWhiteSpace(nome)) q = q.Where(x => (x.NmFantasia ?? "").Contains(nome) || (x.DcEstab ?? "").Contains(nome));
    var ordered = ApplySort(q, sort);
    var total = await ordered.CountAsync();
    var items = await ordered
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new FilialListItemDto { CdEmpresa = x.CdEmpresa, CdFilial = x.CdFilial, NmFantasia = x.NmFantasia, DcEstab = x.DcEstab })
        .ToListAsync();
    var resp = new PagedResult<FilialListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    return Ok(ApiResponse<PagedResult<FilialListItemDto>>.Ok(resp));
}

/// <summary>Exporta Filial para CSV (projeção em DTO de listagem).</summary>
[HttpPost("export")]
public async Task<IActionResult> Export([FromQuery] string? sort = null, int? cdEmpresa = null, int? cdFilial = null, string? nome = null)
{
    var q = _db.Set<Filial>().AsNoTracking().AsQueryable();
    if (cdEmpresa.HasValue) q = q.Where(x => x.CdEmpresa == cdEmpresa.Value);
    if (cdFilial.HasValue) q = q.Where(x => x.CdFilial == cdFilial.Value);
    if (!string.IsNullOrWhiteSpace(nome)) q = q.Where(x => (x.NmFantasia ?? "").Contains(nome) || (x.DcEstab ?? "").Contains(nome));
    var ordered = ApplySort(q, sort);
    var rows = await ordered.Select(x => new FilialListItemDto { CdEmpresa = x.CdEmpresa, CdFilial = x.CdFilial, NmFantasia = x.NmFantasia, DcEstab = x.DcEstab }).ToListAsync();
    var bytes = ToCsv(rows);
    return File(bytes, "text/csv", "test1_export.csv");
}

/// <summary>Obtém Filial por chave.</summary>
[HttpGet("{cdEmpresa:int}/{cdFilial:int}")]
public async Task<ActionResult<ApiResponse<Filial>>> Get(int cdEmpresa, int cdFilial)
{
    var entity = await _db.Set<Filial>().AsNoTracking().FirstOrDefaultAsync(x => x.CdEmpresa == cdEmpresa && x.CdFilial == cdFilial);
    if (entity == null) return NotFound(ApiResponse<Filial>.Fail("Registro não encontrado."));
    return Ok(ApiResponse<Filial>.Ok(entity));
}

/// <summary>Cria um Filial.</summary>
[HttpPost]
public async Task<ActionResult<ApiResponse<Filial>>> Create([FromBody] Filial model)
{
    _db.Set<Filial>().Add(model);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<Filial>.Ok(model, "Criado com sucesso."));
}

/// <summary>Atualiza um Filial pela chave.</summary>
[HttpPut("{cdEmpresa:int}/{cdFilial:int}")]
public async Task<ActionResult<ApiResponse<object>>> Update(int cdEmpresa, int cdFilial, [FromBody] Filial model)
{
    var exists = await _db.Set<Filial>().AnyAsync(x => x.CdEmpresa == cdEmpresa && x.CdFilial == cdFilial);
    if (!exists) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para atualização."));
    _db.Entry(model).State = EntityState.Modified;
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Atualizado com sucesso."));
}

/// <summary>Exclui um Filial pela chave.</summary>
[HttpDelete("{cdEmpresa:int}/{cdFilial:int}")]
public async Task<ActionResult<ApiResponse<object>>> Delete(int cdEmpresa, int cdFilial)
{
    var e = await _db.Set<Filial>().FirstOrDefaultAsync(x => x.CdEmpresa == cdEmpresa && x.CdFilial == cdFilial);
    if (e == null) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para exclusão."));
    _db.Remove(e);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Excluído com sucesso."));
}

/// <summary>Exclui em lote por chaves.</summary>
[HttpPost("bulk-delete")]
public async Task<ActionResult<ApiResponse<object>>> BulkDelete([FromBody] IEnumerable<FilialKeyDto> keys)
{
    var set = _db.Set<Filial>();
    foreach (var k in keys)
    {
        var e = await set.FirstOrDefaultAsync(x => x.CdEmpresa == k.CdEmpresa && x.CdFilial == k.CdFilial);
        if (e != null) _db.Remove(e);
    }
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Registros excluídos."));
}

    }
}
