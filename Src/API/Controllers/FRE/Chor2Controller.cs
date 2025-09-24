using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Infrastructure.Persistence;
using RhSensoERP.Core.FRE.Entities;
using RhSensoERP.Core.Abstractions.Paging;

namespace RhSensoERP.API.Controllers.FRE;

[ApiController]
[Route("api/v1/fre/[controller]")]
public class Chor2Controller : ControllerBase
{
    private readonly AppDbContext _db;
    public Chor2Controller(AppDbContext db) => _db = db;

    private static PagedResult<T> ToPaged<T>(IEnumerable<T> items, int total, int page, int pageSize)
        => new PagedResult<T>(items.ToList(), total, page, pageSize);

[HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? cdCargHor = null, [FromQuery] int? diaDaSemana = null)
    {
        var query = _db.Set<Chor2>().AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(cdCargHor)) query = query.Where(x => x.CdCargHor == cdCargHor);
        if (diaDaSemana.HasValue) query = query.Where(x => x.DiaDaSemana == diaDaSemana.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(x => x.CdCargHor)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new Chor2ListItemDto { })
            .ToListAsync();
        return Ok(new { total, page, pageSize, items });
    }

[HttpGet("find")]
    public async Task<IActionResult> Find([FromQuery] string? cdCargHor = null, [FromQuery] int? diaDaSemana = null)
    {
        var q = _db.Set<Chor2>().AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(cdCargHor)) q = q.Where(x => x.CdCargHor == cdCargHor);
        if (diaDaSemana.HasValue) q = q.Where(x => x.DiaDaSemana == diaDaSemana.Value);
        var item = await q.FirstOrDefaultAsync();
        if (item == null) return NotFound();
        return Ok(item);
    }

}
