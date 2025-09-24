using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Infrastructure.Persistence;
using RhSensoERP.Core.FRE.Entities;
using RhSensoERP.Core.Abstractions.Paging;

namespace RhSensoERP.API.Controllers.FRE;

[ApiController]
[Route("api/v1/fre/[controller]")]
public class Comp2Controller : ControllerBase
{
    private readonly AppDbContext _db;
    public Comp2Controller(AppDbContext db) => _db = db;

    private static PagedResult<T> ToPaged<T>(IEnumerable<T> items, int total, int page, int pageSize)
        => new PagedResult<T>(items.ToList(), total, page, pageSize);

[HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] int? tpOcorr = null, [FromQuery] string? cdMotoc = null)
    {
        var query = _db.Set<Comp2>().AsNoTracking().AsQueryable();
        if (tpOcorr.HasValue) query = query.Where(x => x.TpOcorr == tpOcorr.Value);
        if (!string.IsNullOrWhiteSpace(cdMotoc)) query = query.Where(x => x.CdMotoc == cdMotoc);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(x => x.Inicio)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new Comp2ListItemDto { })
            .ToListAsync();
        return Ok(new { total, page, pageSize, items });
    }

[HttpGet("find")]
    public async Task<IActionResult> Find([FromQuery] int? tpOcorr = null, [FromQuery] string? cdMotoc = null)
    {
        var q = _db.Set<Comp2>().AsNoTracking().AsQueryable();
        if (tpOcorr.HasValue) q = q.Where(x => x.TpOcorr == tpOcorr.Value);
        if (!string.IsNullOrWhiteSpace(cdMotoc)) q = q.Where(x => x.CdMotoc == cdMotoc);
        var item = await q.FirstOrDefaultAsync();
        if (item == null) return NotFound();
        return Ok(item);
    }

}
