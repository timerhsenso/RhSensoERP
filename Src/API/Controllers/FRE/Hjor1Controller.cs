using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Core.FRE.Entities;
using RhSensoERP.Core.Shared;
using RhSensoERP.Core.Abstractions.Paging;
using RhSensoERP.Infrastructure.Persistence;

namespace RhSensoERP.API.Controllers.FRE;

[ApiController]
[Route("api/v1/fre/[controller]")]
public class Hjor1Controller : ControllerBase
{
    private readonly AppDbContext _db;
    public Hjor1Controller(AppDbContext db) => _db = db;

    public sealed class Hjor1ControllerKeyDto
    {
        public string NoMatric { get; set; }
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public DateOnly DtMudanca { get; set; }
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<HorarioAdministrativo>>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sort = null)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var query = _db.Set<HorarioAdministrativo>().AsNoTracking();

        // Sorting (default by the first key property)
        var sortProp = sort ?? nameof(HorarioAdministrativo.NoMatric);
        query = query.OrderBy(e => EF.Property<object>(e!, sortProp!));

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = new PagedResult<HorarioAdministrativo> {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };

        return Ok(ApiResponse.Ok(result));
    }
    
    [HttpGet("by-key")]
    public async Task<ActionResult<ApiResponse<HorarioAdministrativo>>> GetByKey([FromQuery] Hjor1ControllerKeyDto key)
    {
        var entity = await _db.Set<HorarioAdministrativo>().AsNoTracking().FirstOrDefaultAsync(x => x.NoMatric == key.NoMatric && x.CdEmpresa == key.CdEmpresa && x.CdFilial == key.CdFilial && x.DtMudanca == key.DtMudanca);
        if (entity is null) return NotFound(ApiResponse.Fail<HorarioAdministrativo>("Registro não encontrado."));
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<HorarioAdministrativo>>> Create([FromBody] HorarioAdministrativo model)
    {
        // Checa duplicidade pela PK composta
        var exists = await _db.Set<HorarioAdministrativo>().AnyAsync(x => x.NoMatric == model.NoMatric && x.CdEmpresa == model.CdEmpresa && x.CdFilial == model.CdFilial && x.DtMudanca == model.DtMudanca);
        if (exists) return Conflict(ApiResponse.Fail<HorarioAdministrativo>("Registro já existe."));

        _db.Set<HorarioAdministrativo>().Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByKey), new { NoMatric = model.NoMatric, CdEmpresa = model.CdEmpresa, CdFilial = model.CdFilial, DtMudanca = model.DtMudanca }, ApiResponse.Ok(model));
    }
    
    [HttpPut]
    public async Task<ActionResult<ApiResponse<HorarioAdministrativo>>> Update([FromBody] HorarioAdministrativo model)
    {
        var entity = await _db.Set<HorarioAdministrativo>().FirstOrDefaultAsync(x => x.NoMatric == model.NoMatric && x.CdEmpresa == model.CdEmpresa && x.CdFilial == model.CdFilial && x.DtMudanca == model.DtMudanca);
        if (entity is null) return NotFound(ApiResponse.Fail<HorarioAdministrativo>("Registro não encontrado."));

        _db.Entry(entity).CurrentValues.SetValues(model);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<string>>> Delete([FromQuery] Hjor1ControllerKeyDto key)
    {
        var entity = await _db.Set<HorarioAdministrativo>().FirstOrDefaultAsync(x => x.NoMatric == key.NoMatric && x.CdEmpresa == key.CdEmpresa && x.CdFilial == key.CdFilial && x.DtMudanca == key.DtMudanca);
        if (entity is null) return NotFound(ApiResponse.Fail<string>("Registro não encontrado."));

        _db.Remove(entity);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok("Excluído com sucesso."));
    }
}