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

    public class CargoController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CargoController(AppDbContext db) => _db = db;

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

/// <summary>Lista Cargo com paginação.</summary>
[HttpGet]
public async Task<ActionResult<ApiResponse<PagedResult<CargoListItemDto>>>> List(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? sort = null, string? cdCargo = null, string? descricao = null)
{
    var q = _db.Set<Cargo>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(cdCargo)) q = q.Where(x => x.CdCargo == cdCargo);
    if (!string.IsNullOrWhiteSpace(descricao)) q = q.Where(x => (x.DcCargo ?? "").Contains(descricao));
    var ordered = ApplySort(q, sort);
    var total = await ordered.CountAsync();
    var items = await ordered
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new CargoListItemDto { CdCargo = x.CdCargo, DcCargo = x.DcCargo, FlAtivo = x.FlAtivo })
        .ToListAsync();
    var resp = new PagedResult<CargoListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    return Ok(ApiResponse<PagedResult<CargoListItemDto>>.Ok(resp));
}

/// <summary>Exporta Cargo para CSV (projeção em DTO de listagem).</summary>
[HttpPost("export")]
public async Task<IActionResult> Export([FromQuery] string? sort = null, string? cdCargo = null, string? descricao = null)
{
    var q = _db.Set<Cargo>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(cdCargo)) q = q.Where(x => x.CdCargo == cdCargo);
    if (!string.IsNullOrWhiteSpace(descricao)) q = q.Where(x => (x.DcCargo ?? "").Contains(descricao));
    var ordered = ApplySort(q, sort);
    var rows = await ordered.Select(x => new CargoListItemDto { CdCargo = x.CdCargo, DcCargo = x.DcCargo, FlAtivo = x.FlAtivo }).ToListAsync();
    var bytes = ToCsv(rows);
    return File(bytes, "text/csv", "cargo1_export.csv");
}

/// <summary>Obtém Cargo por chave.</summary>
[HttpGet("{cdCargo}")]
public async Task<ActionResult<ApiResponse<Cargo>>> Get(string cdCargo)
{
    var entity = await _db.Set<Cargo>().AsNoTracking().FirstOrDefaultAsync(x => x.CdCargo == cdCargo);
    if (entity == null) return NotFound(ApiResponse<Cargo>.Fail("Registro não encontrado."));
    return Ok(ApiResponse<Cargo>.Ok(entity));
}

/// <summary>Cria um Cargo.</summary>
[HttpPost]
public async Task<ActionResult<ApiResponse<Cargo>>> Create([FromBody] Cargo model)
{
    _db.Set<Cargo>().Add(model);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<Cargo>.Ok(model, "Criado com sucesso."));
}

/// <summary>Atualiza um Cargo pela chave.</summary>
[HttpPut("{cdCargo}")]
public async Task<ActionResult<ApiResponse<object>>> Update(string cdCargo, [FromBody] Cargo model)
{
    var exists = await _db.Set<Cargo>().AnyAsync(x => x.CdCargo == cdCargo);
    if (!exists) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para atualização."));
    _db.Entry(model).State = EntityState.Modified;
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Atualizado com sucesso."));
}

/// <summary>Exclui um Cargo pela chave.</summary>
[HttpDelete("{cdCargo}")]
public async Task<ActionResult<ApiResponse<object>>> Delete(string cdCargo)
{
    var e = await _db.Set<Cargo>().FirstOrDefaultAsync(x => x.CdCargo == cdCargo);
    if (e == null) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para exclusão."));
    _db.Remove(e);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Excluído com sucesso."));
}

/// <summary>Exclui em lote por chaves.</summary>
[HttpPost("bulk-delete")]
public async Task<ActionResult<ApiResponse<object>>> BulkDelete([FromBody] IEnumerable<CargoKeyDto> keys)
{
    var set = _db.Set<Cargo>();
    foreach (var k in keys)
    {
        var e = await set.FirstOrDefaultAsync(x => x.CdCargo == k.CdCargo);
        if (e != null) _db.Remove(e);
    }
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Registros excluídos."));
}

    }
}
