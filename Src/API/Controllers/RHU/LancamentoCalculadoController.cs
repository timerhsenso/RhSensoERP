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
    public class LancamentoCalculadoController : ControllerBase
    {
        private readonly AppDbContext _db;
        public LancamentoCalculadoController(AppDbContext db) => _db = db;

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

/// <summary>Lista LancamentoCalculado com paginação.</summary>
[HttpGet]
public async Task<ActionResult<ApiResponse<PagedResult<LancamentoCalculadoListItemDto>>>> List(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] string? sort = null, string? noMatric = null, int? cdEmpresa = null, int? cdFilial = null, string? noProcesso = null, string? cdConta = null)
{
    var q = _db.Set<LancamentoCalculado>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(noMatric)) q = q.Where(x => x.NoMatric == noMatric);
    if (cdEmpresa.HasValue) q = q.Where(x => x.CdEmpresa == cdEmpresa.Value);
    if (cdFilial.HasValue) q = q.Where(x => x.CdFilial == cdFilial.Value);
    if (!string.IsNullOrWhiteSpace(noProcesso)) q = q.Where(x => x.NoProcesso == noProcesso);
    if (!string.IsNullOrWhiteSpace(cdConta)) q = q.Where(x => x.CdConta == cdConta);
    var ordered = ApplySort(q, sort);
    var total = await ordered.CountAsync();
    var items = await ordered
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new LancamentoCalculadoListItemDto { NoMatric = x.NoMatric, CdEmpresa = x.CdEmpresa, CdFilial = x.CdFilial, NoProcesso = x.NoProcesso, CdConta = x.CdConta, VlConta = x.VlConta, QtConta = x.QtConta })
        .ToListAsync();
    var resp = new PagedResult<LancamentoCalculadoListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    return Ok(ApiResponse<PagedResult<LancamentoCalculadoListItemDto>>.Ok(resp));
}

/// <summary>Exporta LancamentoCalculado para CSV (projeção em DTO de listagem).</summary>
[HttpPost("export")]
public async Task<IActionResult> Export([FromQuery] string? sort = null, string? noMatric = null, int? cdEmpresa = null, int? cdFilial = null, string? noProcesso = null, string? cdConta = null)
{
    var q = _db.Set<LancamentoCalculado>().AsNoTracking().AsQueryable();
    if (!string.IsNullOrWhiteSpace(noMatric)) q = q.Where(x => x.NoMatric == noMatric);
    if (cdEmpresa.HasValue) q = q.Where(x => x.CdEmpresa == cdEmpresa.Value);
    if (cdFilial.HasValue) q = q.Where(x => x.CdFilial == cdFilial.Value);
    if (!string.IsNullOrWhiteSpace(noProcesso)) q = q.Where(x => x.NoProcesso == noProcesso);
    if (!string.IsNullOrWhiteSpace(cdConta)) q = q.Where(x => x.CdConta == cdConta);
    var ordered = ApplySort(q, sort);
    var rows = await ordered.Select(x => new LancamentoCalculadoListItemDto { NoMatric = x.NoMatric, CdEmpresa = x.CdEmpresa, CdFilial = x.CdFilial, NoProcesso = x.NoProcesso, CdConta = x.CdConta, VlConta = x.VlConta, QtConta = x.QtConta }).ToListAsync();
    var bytes = ToCsv(rows);
    return File(bytes, "text/csv", "calc1_export.csv");
}

/// <summary>Obtém LancamentoCalculado por chave.</summary>
[HttpGet("{noMatric}/{cdEmpresa:int}/{cdFilial:int}/{noProcesso}/{cdConta}")]
public async Task<ActionResult<ApiResponse<LancamentoCalculado>>> Get(string noMatric, int cdEmpresa, int cdFilial, string noProcesso, string cdConta)
{
    var entity = await _db.Set<LancamentoCalculado>().AsNoTracking().FirstOrDefaultAsync(x => x.NoMatric == noMatric && x.CdEmpresa == cdEmpresa && x.CdFilial == cdFilial && x.NoProcesso == noProcesso && x.CdConta == cdConta);
    if (entity == null) return NotFound(ApiResponse<LancamentoCalculado>.Fail("Registro não encontrado."));
    return Ok(ApiResponse<LancamentoCalculado>.Ok(entity));
}

/// <summary>Cria um LancamentoCalculado.</summary>
[HttpPost]
public async Task<ActionResult<ApiResponse<LancamentoCalculado>>> Create([FromBody] LancamentoCalculado model)
{
    _db.Set<LancamentoCalculado>().Add(model);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<LancamentoCalculado>.Ok(model, "Criado com sucesso."));
}

/// <summary>Atualiza um LancamentoCalculado pela chave.</summary>
[HttpPut("{noMatric}/{cdEmpresa:int}/{cdFilial:int}/{noProcesso}/{cdConta}")]
public async Task<ActionResult<ApiResponse<object>>> Update(string noMatric, int cdEmpresa, int cdFilial, string noProcesso, string cdConta, [FromBody] LancamentoCalculado model)
{
    var exists = await _db.Set<LancamentoCalculado>().AnyAsync(x => x.NoMatric == noMatric && x.CdEmpresa == cdEmpresa && x.CdFilial == cdFilial && x.NoProcesso == noProcesso && x.CdConta == cdConta);
    if (!exists) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para atualização."));
    _db.Entry(model).State = EntityState.Modified;
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Atualizado com sucesso."));
}

/// <summary>Exclui um LancamentoCalculado pela chave.</summary>
[HttpDelete("{noMatric}/{cdEmpresa:int}/{cdFilial:int}/{noProcesso}/{cdConta}")]
public async Task<ActionResult<ApiResponse<object>>> Delete(string noMatric, int cdEmpresa, int cdFilial, string noProcesso, string cdConta)
{
    var e = await _db.Set<LancamentoCalculado>().FirstOrDefaultAsync(x => x.NoMatric == noMatric && x.CdEmpresa == cdEmpresa && x.CdFilial == cdFilial && x.NoProcesso == noProcesso && x.CdConta == cdConta);
    if (e == null) return NotFound(ApiResponse<object>.Fail("Registro não encontrado para exclusão."));
    _db.Remove(e);
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Excluído com sucesso."));
}

/// <summary>Exclui em lote por chaves.</summary>
[HttpPost("bulk-delete")]
public async Task<ActionResult<ApiResponse<object>>> BulkDelete([FromBody] IEnumerable<LancamentoCalculadoKeyDto> keys)
{
    var set = _db.Set<LancamentoCalculado>();
    foreach (var k in keys)
    {
        var e = await set.FirstOrDefaultAsync(x => x.NoMatric == k.NoMatric && x.CdEmpresa == k.CdEmpresa && x.CdFilial == k.CdFilial && x.NoProcesso == k.NoProcesso && x.CdConta == k.CdConta);
        if (e != null) _db.Remove(e);
    }
    await _db.SaveChangesAsync();
    return Ok(ApiResponse<object>.Ok(new { }, "Registros excluídos."));
}

    }
}
