using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Core.FRE.Entities;
using RhSensoERP.Core.Shared;
using RhSensoERP.Core.Abstractions.Paging;
using RhSensoERP.Infrastructure.Persistence;

namespace RhSensoERP.API.Controllers.FRE;

[ApiController]
[Route("api/v1/fre/[controller]")]
public class Mfre1Controller : ControllerBase
{
    private readonly AppDbContext _db;
    public Mfre1Controller(AppDbContext db) => _db = db;

    public sealed class Mfre1ControllerKeyDto
    {
        public string TpOcorr { get; set; }
        public string CdMotoc { get; set; }
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<Mfre1>>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sort = null)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var query = _db.Set<Mfre1>().AsNoTracking();

        // Sorting (default by the first key property)
        var sortProp = sort ?? nameof(Mfre1.TpOcorr);
        query = query.OrderBy(e => EF.Property<object>(e!, sortProp!));

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = new PagedResult<Mfre1> {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };

        return Ok(ApiResponse.Ok(result));
    }
    
    [HttpGet("by-key")]
    public async Task<ActionResult<ApiResponse<Mfre1>>> GetByKey([FromQuery] Mfre1ControllerKeyDto key)
    {
        var entity = await _db.Set<Mfre1>().AsNoTracking().FirstOrDefaultAsync(x => x.TpOcorr == key.TpOcorr && x.CdMotoc == key.CdMotoc);
        if (entity is null) return NotFound(ApiResponse.Fail<Mfre1>("Registro não encontrado."));
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Mfre1>>> Create([FromBody] Mfre1 model)
    {
        // Checa duplicidade pela PK composta
        var exists = await _db.Set<Mfre1>().AnyAsync(x => x.TpOcorr == model.TpOcorr && x.CdMotoc == model.CdMotoc);
        if (exists) return Conflict(ApiResponse.Fail<Mfre1>("Registro já existe."));

        _db.Set<Mfre1>().Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByKey), new { TpOcorr = model.TpOcorr, CdMotoc = model.CdMotoc }, ApiResponse.Ok(model));
    }
    
    [HttpPut]
    public async Task<ActionResult<ApiResponse<Mfre1>>> Update([FromBody] Mfre1 model)
    {
        var entity = await _db.Set<Mfre1>().FirstOrDefaultAsync(x => x.TpOcorr == model.TpOcorr && x.CdMotoc == model.CdMotoc);
        if (entity is null) return NotFound(ApiResponse.Fail<Mfre1>("Registro não encontrado."));

        _db.Entry(entity).CurrentValues.SetValues(model);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<string>>> Delete([FromQuery] Mfre1ControllerKeyDto key)
    {
        var entity = await _db.Set<Mfre1>().FirstOrDefaultAsync(x => x.TpOcorr == key.TpOcorr && x.CdMotoc == key.CdMotoc);
        if (entity is null) return NotFound(ApiResponse.Fail<string>("Registro não encontrado."));

        _db.Remove(entity);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok("Excluído com sucesso."));
    }
}