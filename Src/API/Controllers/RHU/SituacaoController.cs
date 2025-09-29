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

    public class SituacaoController : ControllerBase
    {
        private readonly AppDbContext _db;
        public SituacaoController(AppDbContext db) => _db = db;

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

/// <summary>Lista Situacao com paginação.</summary>
[HttpGet]
public async Task<ActionResult<ApiResponse<PagedResult<SituacaoListItemDto>>>> List(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? sort = null, string? cdSituacao = null, string? descricao = null)
{
    var q = _db.Set<Situacao>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(cdSituacao)) q = q.Where(x => x.CdSituacao == cdSituacao);
    if (!string.IsNullOrWhiteSpace(descricao)) q = q.Where(x => (x.DcSituacao ?? "").Contains(descricao));
    var ordered = ApplySort(q, sort);
    var total = await ordered.CountAsync();
    var items = await ordered
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new SituacaoListItemDto { CdSituacao = x.CdSituacao, DcSituacao = x.DcSituacao })
        .ToListAsync();
    var resp = new PagedResult<SituacaoListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    return Ok(ApiResponse<PagedResult<SituacaoListItemDto>>.Ok(resp));
}

/// <summary>Exporta Situacao para CSV (projeção em DTO de listagem).</summary>
[HttpPost("export")]
public async Task<IActionResult> Export([FromQuery] string? sort = null, string? cdSituacao = null, string? descricao = null)
{
    var q = _db.Set<Situacao>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(cdSituacao)) q = q.Where(x => x.CdSituacao == cdSituacao);
    if (!string.IsNullOrWhiteSpace(descricao)) q = q.Where(x => (x.DcSituacao ?? "").Contains(descricao));
    var ordered = ApplySort(q, sort);
    var rows = await ordered.Select(x => new SituacaoListItemDto { CdSituacao = x.CdSituacao, DcSituacao = x.DcSituacao }).ToListAsync();
    var bytes = ToCsv(rows);
    return File(bytes, "text/csv", "tsitu1_export.csv");
}

/// <summary>Obtém Situacao por chave.</summary>
[HttpGet("{cdSituacao}")]
public async Task<ActionResult<ApiResponse<Situacao>>> Get(string cdSituacao)
{
    var entity = await _db.Set<Situacao>().AsNoTracking().FirstOrDefaultAsync(x => x.CdSituacao == cdSituacao);
    if (entity == null) return NotFound(ApiResponse<Situacao>.Fail("Registro não encontrado."));
    return Ok(ApiResponse<Situacao>.Ok(entity));
}

/// <summary>Cria um Situacao.</summary>
[HttpPost]
public async Task<ActionResult<ApiResponse<Situacao>>> Create([FromBody] Situacao model)
{
    _db.Set<Situacao>().Add(model);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<Situacao>.Ok(model, "Criado com sucesso."));
}

/// <summary>Atualiza um Situacao pela chave.</summary>
[HttpPut("{cdSituacao}")]
public async Task<ActionResult<ApiResponse<object>>> Update(string cdSituacao, [FromBody] Situacao model)
{
    var exists = await _db.Set<Situacao>().AnyAsync(x => x.CdSituacao == cdSituacao);
    if (!exists) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para atualização."));
    _db.Entry(model).State = EntityState.Modified;
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Atualizado com sucesso."));
}

/// <summary>Exclui um Situacao pela chave.</summary>
[HttpDelete("{cdSituacao}")]
public async Task<ActionResult<ApiResponse<object>>> Delete(string cdSituacao)
{
    var e = await _db.Set<Situacao>().FirstOrDefaultAsync(x => x.CdSituacao == cdSituacao);
    if (e == null) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para exclusão."));
    _db.Remove(e);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Excluído com sucesso."));
}

/// <summary>Exclui em lote por chaves.</summary>
[HttpPost("bulk-delete")]
public async Task<ActionResult<ApiResponse<object>>> BulkDelete([FromBody] IEnumerable<SituacaoKeyDto> keys)
{
    var set = _db.Set<Situacao>();
    foreach (var k in keys)
    {
        var e = await set.FirstOrDefaultAsync(x => x.CdSituacao == k.CdSituacao);
        if (e != null) _db.Remove(e);
    }
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Registros excluídos."));
}

    }
}
