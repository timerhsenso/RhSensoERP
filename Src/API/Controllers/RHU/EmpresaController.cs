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
    public class EmpresaController : ControllerBase
    {
        private readonly AppDbContext _db;
        public EmpresaController(AppDbContext db) => _db = db;

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

/// <summary>Lista Empresa com paginação.</summary>
[HttpGet]
public async Task<ActionResult<ApiResponse<PagedResult<EmpresaListItemDto>>>> List(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? sort = null, int? cdEmpresa = null, string? nome = null)
{
    var q = _db.Set<Empresa>().AsNoTracking().AsQueryable();
    if (cdEmpresa.HasValue) q = q.Where(x => x.CdEmpresa == cdEmpresa.Value);
    if (!string.IsNullOrWhiteSpace(nome)) q = q.Where(x => (x.NmEmpresa ?? "").Contains(nome) || (x.NmFantasia ?? "").Contains(nome));
    var ordered = ApplySort(q, sort);
    var total = await ordered.CountAsync();
    var items = await ordered
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new EmpresaListItemDto { CdEmpresa = x.CdEmpresa, NmEmpresa = x.NmEmpresa, NmFantasia = x.NmFantasia })
        .ToListAsync();
    var resp = new PagedResult<EmpresaListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    return Ok(ApiResponse<PagedResult<EmpresaListItemDto>>.Ok(resp));
}

/// <summary>Exporta Empresa para CSV (projeção em DTO de listagem).</summary>
[HttpPost("export")]
public async Task<IActionResult> Export([FromQuery] string? sort = null, int? cdEmpresa = null, string? nome = null)
{
    var q = _db.Set<Empresa>().AsNoTracking().AsQueryable();
    if (cdEmpresa.HasValue) q = q.Where(x => x.CdEmpresa == cdEmpresa.Value);
    if (!string.IsNullOrWhiteSpace(nome)) q = q.Where(x => (x.NmEmpresa ?? "").Contains(nome) || (x.NmFantasia ?? "").Contains(nome));
    var ordered = ApplySort(q, sort);
    var rows = await ordered.Select(x => new EmpresaListItemDto { CdEmpresa = x.CdEmpresa, NmEmpresa = x.NmEmpresa, NmFantasia = x.NmFantasia }).ToListAsync();
    var bytes = ToCsv(rows);
    return File(bytes, "text/csv", "temp1_export.csv");
}

/// <summary>Obtém Empresa por chave.</summary>
[HttpGet("{cdEmpresa:int}")]
public async Task<ActionResult<ApiResponse<Empresa>>> Get(int cdEmpresa)
{
    var entity = await _db.Set<Empresa>().AsNoTracking().FirstOrDefaultAsync(x => x.CdEmpresa == cdEmpresa);
    if (entity == null) return NotFound(ApiResponse<Empresa>.Fail("Registro não encontrado."));
    return Ok(ApiResponse<Empresa>.Ok(entity));
}

/// <summary>Cria um Empresa.</summary>
[HttpPost]
public async Task<ActionResult<ApiResponse<Empresa>>> Create([FromBody] Empresa model)
{
    _db.Set<Empresa>().Add(model);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<Empresa>.Ok(model, "Criado com sucesso."));
}

/// <summary>Atualiza um Empresa pela chave.</summary>
[HttpPut("{cdEmpresa:int}")]
public async Task<ActionResult<ApiResponse<object>>> Update(int cdEmpresa, [FromBody] Empresa model)
{
    var exists = await _db.Set<Empresa>().AnyAsync(x => x.CdEmpresa == cdEmpresa);
    if (!exists) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para atualização."));
    _db.Entry(model).State = EntityState.Modified;
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Atualizado com sucesso."));
}

/// <summary>Exclui um Empresa pela chave.</summary>
[HttpDelete("{cdEmpresa:int}")]
public async Task<ActionResult<ApiResponse<object>>> Delete(int cdEmpresa)
{
    var e = await _db.Set<Empresa>().FirstOrDefaultAsync(x => x.CdEmpresa == cdEmpresa);
    if (e == null) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para exclusão."));
    _db.Remove(e);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Excluído com sucesso."));
}

/// <summary>Exclui em lote por chaves.</summary>
[HttpPost("bulk-delete")]
public async Task<ActionResult<ApiResponse<object>>> BulkDelete([FromBody] IEnumerable<EmpresaKeyDto> keys)
{
    var set = _db.Set<Empresa>();
    foreach (var k in keys)
    {
        var e = await set.FirstOrDefaultAsync(x => x.CdEmpresa == k.CdEmpresa);
        if (e != null) _db.Remove(e);
    }
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Registros excluídos."));
}

    }
}
