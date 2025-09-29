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
    public class MotivoAfastamentoController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MotivoAfastamentoController(AppDbContext db) => _db = db;

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

/// <summary>Lista MotivoAfastamento com paginação.</summary>
[HttpGet]
public async Task<ActionResult<ApiResponse<PagedResult<MotivoAfastamentoListItemDto>>>> List(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? sort = null, string? cdMotAfas = null, string? cdSituacao = null)
{
    var q = _db.Set<MotivoAfastamento>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(cdMotAfas)) q = q.Where(x => x.CdMotAfas == cdMotAfas);
    if (!string.IsNullOrWhiteSpace(cdSituacao)) q = q.Where(x => x.CdSituacao == cdSituacao);
    var ordered = ApplySort(q, sort);
    var total = await ordered.CountAsync();
    var items = await ordered
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new MotivoAfastamentoListItemDto { CdMotAfas = x.CdMotAfas, CdSituacao = x.CdSituacao, DcMotAfas = x.DcMotAfas })
        .ToListAsync();
    var resp = new PagedResult<MotivoAfastamentoListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    return Ok(ApiResponse<PagedResult<MotivoAfastamentoListItemDto>>.Ok(resp));
}

/// <summary>Exporta MotivoAfastamento para CSV (projeção em DTO de listagem).</summary>
[HttpPost("export")]
public async Task<IActionResult> Export([FromQuery] string? sort = null, string? cdMotAfas = null, string? cdSituacao = null)
{
    var q = _db.Set<MotivoAfastamento>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(cdMotAfas)) q = q.Where(x => x.CdMotAfas == cdMotAfas);
    if (!string.IsNullOrWhiteSpace(cdSituacao)) q = q.Where(x => x.CdSituacao == cdSituacao);
    var ordered = ApplySort(q, sort);
    var rows = await ordered.Select(x => new MotivoAfastamentoListItemDto { CdMotAfas = x.CdMotAfas, CdSituacao = x.CdSituacao, DcMotAfas = x.DcMotAfas }).ToListAsync();
    var bytes = ToCsv(rows);
    return File(bytes, "text/csv", "moaf1_export.csv");
}

/// <summary>Obtém MotivoAfastamento por chave.</summary>
[HttpGet("{cdMotAfas}/{cdSituacao}")]
public async Task<ActionResult<ApiResponse<MotivoAfastamento>>> Get(string cdMotAfas, string cdSituacao)
{
    var entity = await _db.Set<MotivoAfastamento>().AsNoTracking().FirstOrDefaultAsync(x => x.CdMotAfas == cdMotAfas && x.CdSituacao == cdSituacao);
    if (entity == null) return NotFound(ApiResponse<MotivoAfastamento>.Fail("Registro não encontrado."));
    return Ok(ApiResponse<MotivoAfastamento>.Ok(entity));
}

/// <summary>Cria um MotivoAfastamento.</summary>
[HttpPost]
public async Task<ActionResult<ApiResponse<MotivoAfastamento>>> Create([FromBody] MotivoAfastamento model)
{
    _db.Set<MotivoAfastamento>().Add(model);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<MotivoAfastamento>.Ok(model, "Criado com sucesso."));
}

/// <summary>Atualiza um MotivoAfastamento pela chave.</summary>
[HttpPut("{cdMotAfas}/{cdSituacao}")]
public async Task<ActionResult<ApiResponse<object>>> Update(string cdMotAfas, string cdSituacao, [FromBody] MotivoAfastamento model)
{
    var exists = await _db.Set<MotivoAfastamento>().AnyAsync(x => x.CdMotAfas == cdMotAfas && x.CdSituacao == cdSituacao);
    if (!exists) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para atualização."));
    _db.Entry(model).State = EntityState.Modified;
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Atualizado com sucesso."));
}

/// <summary>Exclui um MotivoAfastamento pela chave.</summary>
[HttpDelete("{cdMotAfas}/{cdSituacao}")]
public async Task<ActionResult<ApiResponse<object>>> Delete(string cdMotAfas, string cdSituacao)
{
    var e = await _db.Set<MotivoAfastamento>().FirstOrDefaultAsync(x => x.CdMotAfas == cdMotAfas && x.CdSituacao == cdSituacao);
    if (e == null) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para exclusão."));
    _db.Remove(e);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Excluído com sucesso."));
}

/// <summary>Exclui em lote por chaves.</summary>
[HttpPost("bulk-delete")]
public async Task<ActionResult<ApiResponse<object>>> BulkDelete([FromBody] IEnumerable<MotivoAfastamentoKeyDto> keys)
{
    var set = _db.Set<MotivoAfastamento>();
    foreach (var k in keys)
    {
        var e = await set.FirstOrDefaultAsync(x => x.CdMotAfas == k.CdMotAfas && x.CdSituacao == k.CdSituacao);
        if (e != null) _db.Remove(e);
    }
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Registros excluídos."));
}

    }
}
