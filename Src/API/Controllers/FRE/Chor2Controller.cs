using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Core.FRE.Entities;
using RhSensoERP.Core.Shared;
using RhSensoERP.Core.Abstractions.Paging;
using RhSensoERP.Infrastructure.Persistence;

namespace RhSensoERP.API.Controllers.FRE;

[ApiController]
[Route("api/v1/fre/[controller]")]
public class Chor2Controller : ControllerBase
{
    private readonly AppDbContext _db;
    public Chor2Controller(AppDbContext db) => _db = db;

    public sealed class Chor2ControllerKeyDto
    {
        public string CdCargHor { get; set; }
        public int DiaDaSemana { get; set; }
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<HorarioAdministrativoDia>>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sort = null)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var query = _db.Set<HorarioAdministrativoDia>().AsNoTracking();

        // Sorting (default by the first key property)
        var sortProp = sort ?? nameof(HorarioAdministrativoDia.CdCargHor);
        query = query.OrderBy(e => EF.Property<object>(e!, sortProp!));

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = new PagedResult<HorarioAdministrativoDia> {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };

        return Ok(ApiResponse.Ok(result));
    }
    
    [HttpGet("by-key")]
    public async Task<ActionResult<ApiResponse<HorarioAdministrativoDia>>> GetByKey([FromQuery] Chor2ControllerKeyDto key)
    {
        var entity = await _db.Set<HorarioAdministrativoDia>().AsNoTracking().FirstOrDefaultAsync(x => x.CdCargHor == key.CdCargHor && x.DiaDaSemana == key.DiaDaSemana);
        if (entity is null) return NotFound(ApiResponse.Fail<HorarioAdministrativoDia>("Registro não encontrado."));
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<HorarioAdministrativoDia>>> Create([FromBody] HorarioAdministrativoDia model)
    {
        // Checa duplicidade pela PK composta
        var exists = await _db.Set<HorarioAdministrativoDia>().AnyAsync(x => x.CdCargHor == model.CdCargHor && x.DiaDaSemana == model.DiaDaSemana);
        if (exists) return Conflict(ApiResponse.Fail<HorarioAdministrativoDia>("Registro já existe."));

        _db.Set<HorarioAdministrativoDia>().Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByKey), new { CdCargHor = model.CdCargHor, DiaDaSemana = model.DiaDaSemana }, ApiResponse.Ok(model));
    }
    
    [HttpPut]
    public async Task<ActionResult<ApiResponse<HorarioAdministrativoDia>>> Update([FromBody] HorarioAdministrativoDia model)
    {
        var entity = await _db.Set<HorarioAdministrativoDia>().FirstOrDefaultAsync(x => x.CdCargHor == model.CdCargHor && x.DiaDaSemana == model.DiaDaSemana);
        if (entity is null) return NotFound(ApiResponse.Fail<HorarioAdministrativoDia>("Registro não encontrado."));

        _db.Entry(entity).CurrentValues.SetValues(model);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<string>>> Delete([FromQuery] Chor2ControllerKeyDto key)
    {
        var entity = await _db.Set<HorarioAdministrativoDia>().FirstOrDefaultAsync(x => x.CdCargHor == key.CdCargHor && x.DiaDaSemana == key.DiaDaSemana);
        if (entity is null) return NotFound(ApiResponse.Fail<string>("Registro não encontrado."));

        _db.Remove(entity);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok("Excluído com sucesso."));
    }
}