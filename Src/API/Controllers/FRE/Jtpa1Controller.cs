using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Core.FRE.Entities;
using RhSensoERP.Core.Shared;
using RhSensoERP.Core.Abstractions.Paging;
using RhSensoERP.Infrastructure.Persistence;

namespace RhSensoERP.API.Controllers.FRE;

[ApiController]
[Route("api/v1/fre/[controller]")]
public class Jtpa1Controller : ControllerBase
{
    private readonly AppDbContext _db;
    public Jtpa1Controller(AppDbContext db) => _db = db;

    public sealed class Jtpa1ControllerKeyDto
    {
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public string TpJornada { get; set; }
        public short AaJornada { get; set; }
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<JornadaTipoAno>>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sort = null)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var query = _db.Set<JornadaTipoAno>().AsNoTracking();

        // Sorting (default by the first key property)
        var sortProp = sort ?? nameof(JornadaTipoAno.CdEmpresa);
        query = query.OrderBy(e => EF.Property<object>(e!, sortProp!));

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = new PagedResult<JornadaTipoAno> {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };

        return Ok(ApiResponse.Ok(result));
    }
    
    [HttpGet("by-key")]
    public async Task<ActionResult<ApiResponse<JornadaTipoAno>>> GetByKey([FromQuery] Jtpa1ControllerKeyDto key)
    {
        var entity = await _db.Set<JornadaTipoAno>().AsNoTracking().FirstOrDefaultAsync(x => x.CdEmpresa == key.CdEmpresa && x.CdFilial == key.CdFilial && x.TpJornada == key.TpJornada && x.AaJornada == key.AaJornada);
        if (entity is null) return NotFound(ApiResponse.Fail<JornadaTipoAno>("Registro não encontrado."));
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<JornadaTipoAno>>> Create([FromBody] JornadaTipoAno model)
    {
        // Checa duplicidade pela PK composta
        var exists = await _db.Set<JornadaTipoAno>().AnyAsync(x => x.CdEmpresa == model.CdEmpresa && x.CdFilial == model.CdFilial && x.TpJornada == model.TpJornada && x.AaJornada == model.AaJornada);
        if (exists) return Conflict(ApiResponse.Fail<JornadaTipoAno>("Registro já existe."));

        _db.Set<JornadaTipoAno>().Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByKey), new { CdEmpresa = model.CdEmpresa, CdFilial = model.CdFilial, TpJornada = model.TpJornada, AaJornada = model.AaJornada }, ApiResponse.Ok(model));
    }
    
    [HttpPut]
    public async Task<ActionResult<ApiResponse<JornadaTipoAno>>> Update([FromBody] JornadaTipoAno model)
    {
        var entity = await _db.Set<JornadaTipoAno>().FirstOrDefaultAsync(x => x.CdEmpresa == model.CdEmpresa && x.CdFilial == model.CdFilial && x.TpJornada == model.TpJornada && x.AaJornada == model.AaJornada);
        if (entity is null) return NotFound(ApiResponse.Fail<JornadaTipoAno>("Registro não encontrado."));

        _db.Entry(entity).CurrentValues.SetValues(model);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<string>>> Delete([FromQuery] Jtpa1ControllerKeyDto key)
    {
        var entity = await _db.Set<JornadaTipoAno>().FirstOrDefaultAsync(x => x.CdEmpresa == key.CdEmpresa && x.CdFilial == key.CdFilial && x.TpJornada == key.TpJornada && x.AaJornada == key.AaJornada);
        if (entity is null) return NotFound(ApiResponse.Fail<string>("Registro não encontrado."));

        _db.Remove(entity);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok("Excluído com sucesso."));
    }
}