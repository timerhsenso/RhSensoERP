using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Core.FRE.Entities;
using RhSensoERP.Core.Shared;
using RhSensoERP.Core.Abstractions.Paging;
using RhSensoERP.Infrastructure.Persistence;

namespace RhSensoERP.API.Controllers.FRE;

[ApiController]
[Route("api/v1/fre/[controller]")]
public class BatidasController : ControllerBase
{
    private readonly AppDbContext _db;
    public BatidasController(AppDbContext db) => _db = db;

    public sealed class BatidasControllerKeyDto
    {
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public string NoMatric { get; set; }
        public DateOnly Data { get; set; }
        public string Hora { get; set; }
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<Batidas>>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sort = null)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var query = _db.Set<Batidas>().AsNoTracking();

        // Sorting (default by the first key property)
        var sortProp = sort ?? nameof(Batidas.CdEmpresa);
        query = query.OrderBy(e => EF.Property<object>(e!, sortProp!));

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = new PagedResult<Batidas> {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };

        return Ok(ApiResponse.Ok(result));
    }
    
    [HttpGet("by-key")]
    public async Task<ActionResult<ApiResponse<Batidas>>> GetByKey([FromQuery] BatidasControllerKeyDto key)
    {
        var entity = await _db.Set<Batidas>().AsNoTracking().FirstOrDefaultAsync(x => x.CdEmpresa == key.CdEmpresa && x.CdFilial == key.CdFilial && x.NoMatric == key.NoMatric && x.Data == key.Data && x.Hora == key.Hora);
        if (entity is null) return NotFound(ApiResponse.Fail<Batidas>("Registro não encontrado."));
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Batidas>>> Create([FromBody] Batidas model)
    {
        // Checa duplicidade pela PK composta
        var exists = await _db.Set<Batidas>().AnyAsync(x => x.CdEmpresa == model.CdEmpresa && x.CdFilial == model.CdFilial && x.NoMatric == model.NoMatric && x.Data == model.Data && x.Hora == model.Hora);
        if (exists) return Conflict(ApiResponse.Fail<Batidas>("Registro já existe."));

        _db.Set<Batidas>().Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByKey), new { CdEmpresa = model.CdEmpresa, CdFilial = model.CdFilial, NoMatric = model.NoMatric, Data = model.Data, Hora = model.Hora }, ApiResponse.Ok(model));
    }
    
    [HttpPut]
    public async Task<ActionResult<ApiResponse<Batidas>>> Update([FromBody] Batidas model)
    {
        var entity = await _db.Set<Batidas>().FirstOrDefaultAsync(x => x.CdEmpresa == model.CdEmpresa && x.CdFilial == model.CdFilial && x.NoMatric == model.NoMatric && x.Data == model.Data && x.Hora == model.Hora);
        if (entity is null) return NotFound(ApiResponse.Fail<Batidas>("Registro não encontrado."));

        _db.Entry(entity).CurrentValues.SetValues(model);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<string>>> Delete([FromQuery] BatidasControllerKeyDto key)
    {
        var entity = await _db.Set<Batidas>().FirstOrDefaultAsync(x => x.CdEmpresa == key.CdEmpresa && x.CdFilial == key.CdFilial && x.NoMatric == key.NoMatric && x.Data == key.Data && x.Hora == key.Hora);
        if (entity is null) return NotFound(ApiResponse.Fail<string>("Registro não encontrado."));

        _db.Remove(entity);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok("Excluído com sucesso."));
    }
}