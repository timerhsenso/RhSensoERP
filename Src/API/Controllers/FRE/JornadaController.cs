using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Core.FRE.Entities;
using RhSensoERP.Core.Shared;
using RhSensoERP.Core.Abstractions.Paging;
using RhSensoERP.Infrastructure.Persistence;

namespace RhSensoERP.API.Controllers.FRE;

[ApiController]
[Route("api/v1/fre/[controller]")]
public class JornadaController : ControllerBase
{
    private readonly AppDbContext _db;
    public JornadaController(AppDbContext db) => _db = db;

    public sealed class JornadaControllerKeyDto
    {
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public string TpJornada { get; set; }
        public short Ano { get; set; }
        public byte Mes { get; set; }
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<Jornada>>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sort = null)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var query = _db.Set<Jornada>().AsNoTracking();

        // Sorting (default by the first key property)
        var sortProp = sort ?? nameof(Jornada.CdEmpresa);
        query = query.OrderBy(e => EF.Property<object>(e!, sortProp!));

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = new PagedResult<Jornada> {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };

        return Ok(ApiResponse.Ok(result));
    }
    
    [HttpGet("by-key")]
    public async Task<ActionResult<ApiResponse<Jornada>>> GetByKey([FromQuery] JornadaControllerKeyDto key)
    {
        var entity = await _db.Set<Jornada>().AsNoTracking().FirstOrDefaultAsync(x => x.CdEmpresa == key.CdEmpresa && x.CdFilial == key.CdFilial && x.TpJornada == key.TpJornada && x.Ano == key.Ano && x.Mes == key.Mes);
        if (entity is null) return NotFound(ApiResponse.Fail<Jornada>("Registro não encontrado."));
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Jornada>>> Create([FromBody] Jornada model)
    {
        // Checa duplicidade pela PK composta
        var exists = await _db.Set<Jornada>().AnyAsync(x => x.CdEmpresa == model.CdEmpresa && x.CdFilial == model.CdFilial && x.TpJornada == model.TpJornada && x.Ano == model.Ano && x.Mes == model.Mes);
        if (exists) return Conflict(ApiResponse.Fail<Jornada>("Registro já existe."));

        _db.Set<Jornada>().Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByKey), new { CdEmpresa = model.CdEmpresa, CdFilial = model.CdFilial, TpJornada = model.TpJornada, Ano = model.Ano, Mes = model.Mes }, ApiResponse.Ok(model));
    }
    
    [HttpPut]
    public async Task<ActionResult<ApiResponse<Jornada>>> Update([FromBody] Jornada model)
    {
        var entity = await _db.Set<Jornada>().FirstOrDefaultAsync(x => x.CdEmpresa == model.CdEmpresa && x.CdFilial == model.CdFilial && x.TpJornada == model.TpJornada && x.Ano == model.Ano && x.Mes == model.Mes);
        if (entity is null) return NotFound(ApiResponse.Fail<Jornada>("Registro não encontrado."));

        _db.Entry(entity).CurrentValues.SetValues(model);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<string>>> Delete([FromQuery] JornadaControllerKeyDto key)
    {
        var entity = await _db.Set<Jornada>().FirstOrDefaultAsync(x => x.CdEmpresa == key.CdEmpresa && x.CdFilial == key.CdFilial && x.TpJornada == key.TpJornada && x.Ano == key.Ano && x.Mes == key.Mes);
        if (entity is null) return NotFound(ApiResponse.Fail<string>("Registro não encontrado."));

        _db.Remove(entity);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok("Excluído com sucesso."));
    }
}