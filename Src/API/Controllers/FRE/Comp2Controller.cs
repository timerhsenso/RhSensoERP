using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Core.FRE.Entities;
using RhSensoERP.Core.Shared;
using RhSensoERP.Core.Abstractions.Paging;
using RhSensoERP.Infrastructure.Persistence;

namespace RhSensoERP.API.Controllers.FRE;

[ApiController]
[Route("api/v1/fre/[controller]")]
public class Comp2Controller : ControllerBase
{
    private readonly AppDbContext _db;
    public Comp2Controller(AppDbContext db) => _db = db;

    public sealed class Comp2ControllerKeyDto
    {
        public long IdComp { get; set; }
        public DateOnly Inicio { get; set; }
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<CompensacaoJanela>>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sort = null)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var query = _db.Set<CompensacaoJanela>().AsNoTracking();

        // Sorting (default by the first key property)
        var sortProp = sort ?? nameof(CompensacaoJanela.IdComp);
        query = query.OrderBy(e => EF.Property<object>(e!, sortProp!));

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = new PagedResult<CompensacaoJanela> {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };

        return Ok(ApiResponse.Ok(result));
    }
    
    [HttpGet("by-key")]
    public async Task<ActionResult<ApiResponse<CompensacaoJanela>>> GetByKey([FromQuery] Comp2ControllerKeyDto key)
    {
        var entity = await _db.Set<CompensacaoJanela>().AsNoTracking().FirstOrDefaultAsync(x => x.IdComp == key.IdComp && x.Inicio == key.Inicio);
        if (entity is null) return NotFound(ApiResponse.Fail<CompensacaoJanela>("Registro não encontrado."));
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CompensacaoJanela>>> Create([FromBody] CompensacaoJanela model)
    {
        // Checa duplicidade pela PK composta
        var exists = await _db.Set<CompensacaoJanela>().AnyAsync(x => x.IdComp == model.IdComp && x.Inicio == model.Inicio);
        if (exists) return Conflict(ApiResponse.Fail<CompensacaoJanela>("Registro já existe."));

        _db.Set<CompensacaoJanela>().Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByKey), new { IdComp = model.IdComp, Inicio = model.Inicio }, ApiResponse.Ok(model));
    }
    
    [HttpPut]
    public async Task<ActionResult<ApiResponse<CompensacaoJanela>>> Update([FromBody] CompensacaoJanela model)
    {
        var entity = await _db.Set<CompensacaoJanela>().FirstOrDefaultAsync(x => x.IdComp == model.IdComp && x.Inicio == model.Inicio);
        if (entity is null) return NotFound(ApiResponse.Fail<CompensacaoJanela>("Registro não encontrado."));

        _db.Entry(entity).CurrentValues.SetValues(model);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<string>>> Delete([FromQuery] Comp2ControllerKeyDto key)
    {
        var entity = await _db.Set<CompensacaoJanela>().FirstOrDefaultAsync(x => x.IdComp == key.IdComp && x.Inicio == key.Inicio);
        if (entity is null) return NotFound(ApiResponse.Fail<string>("Registro não encontrado."));

        _db.Remove(entity);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok("Excluído com sucesso."));
    }
}