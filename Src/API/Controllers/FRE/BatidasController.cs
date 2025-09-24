using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Infrastructure.Persistence;
using RhSensoERP.Core.FRE.Entities;
using RhSensoERP.Core.Abstractions.Paging;

namespace RhSensoERP.API.Controllers.FRE;

[ApiController]
[Route("api/v1/fre/[controller]")]
public class BatidasController : ControllerBase
{
    private readonly AppDbContext _db;
    public BatidasController(AppDbContext db) => _db = db;

    private static PagedResult<T> ToPaged<T>(IEnumerable<T> items, int total, int page, int pageSize)
        => new PagedResult<T>(items.ToList(), total, page, pageSize);

[HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] int? cdEmpresa = null, [FromQuery] int? cdFilial = null, [FromQuery] string? noMatric = null)
    {
        var query = _db.Set<Batidas>().AsNoTracking().AsQueryable();
        if (cdEmpresa.HasValue) query = query.Where(x => x.CdEmpresa == cdEmpresa.Value);
        if (cdFilial.HasValue) query = query.Where(x => x.CdFilial == cdFilial.Value);
        if (!string.IsNullOrWhiteSpace(noMatric)) query = query.Where(x => x.NoMatric == noMatric);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(x => x.CdEmpresa)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new BatidasListItemDto { })
            .ToListAsync();
        return Ok(new { total, page, pageSize, items });
    }

[HttpGet("find")]
    public async Task<IActionResult> Find([FromQuery] int? cdEmpresa = null, [FromQuery] int? cdFilial = null, [FromQuery] string? noMatric = null)
    {
        var q = _db.Set<Batidas>().AsNoTracking().AsQueryable();
        if (cdEmpresa.HasValue) q = q.Where(x => x.CdEmpresa == cdEmpresa.Value);
        if (cdFilial.HasValue) q = q.Where(x => x.CdFilial == cdFilial.Value);
        if (!string.IsNullOrWhiteSpace(noMatric)) q = q.Where(x => x.NoMatric == noMatric);
        var item = await q.FirstOrDefaultAsync();
        if (item == null) return NotFound();
        return Ok(item);
    }

}
