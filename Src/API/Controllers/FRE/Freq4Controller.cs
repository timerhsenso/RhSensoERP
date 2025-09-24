using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Core.FRE.Entities;
using RhSensoERP.Core.Shared;
using RhSensoERP.Core.Abstractions.Paging;
using RhSensoERP.Infrastructure.Persistence;

namespace RhSensoERP.API.Controllers.FRE;

[ApiController]
[Route("api/v1/fre/[controller]")]
public class Freq4Controller : ControllerBase
{
    private readonly AppDbContext _db;
    public Freq4Controller(AppDbContext db) => _db = db;

    public sealed class Freq4ControllerKeyDto
    {
        public string NoMatric { get; set; }
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public DateOnly Data { get; set; }
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<Frequencia3>>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sort = null)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var query = _db.Set<Frequencia3>().AsNoTracking();

        // Sorting (default by the first key property)
        var sortProp = sort ?? nameof(Frequencia3.NoMatric);
        query = query.OrderBy(e => EF.Property<object>(e!, sortProp!));

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = new PagedResult<Frequencia3> {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };

        return Ok(ApiResponse.Ok(result));
    }
    
    [HttpGet("by-key")]
    public async Task<ActionResult<ApiResponse<Frequencia3>>> GetByKey([FromQuery] Freq4ControllerKeyDto key)
    {
        var entity = await _db.Set<Frequencia3>().AsNoTracking().FirstOrDefaultAsync(x => x.NoMatric == key.NoMatric && x.CdEmpresa == key.CdEmpresa && x.CdFilial == key.CdFilial && x.Data == key.Data);
        if (entity is null) return NotFound(ApiResponse.Fail<Frequencia3>("Registro não encontrado."));
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Frequencia3>>> Create([FromBody] Frequencia3 model)
    {
        // Checa duplicidade pela PK composta
        var exists = await _db.Set<Frequencia3>().AnyAsync(x => x.NoMatric == model.NoMatric && x.CdEmpresa == model.CdEmpresa && x.CdFilial == model.CdFilial && x.Data == model.Data);
        if (exists) return Conflict(ApiResponse.Fail<Frequencia3>("Registro já existe."));

        _db.Set<Frequencia3>().Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByKey), new { NoMatric = model.NoMatric, CdEmpresa = model.CdEmpresa, CdFilial = model.CdFilial, Data = model.Data }, ApiResponse.Ok(model));
    }
    
    [HttpPut]
    public async Task<ActionResult<ApiResponse<Frequencia3>>> Update([FromBody] Frequencia3 model)
    {
        var entity = await _db.Set<Frequencia3>().FirstOrDefaultAsync(x => x.NoMatric == model.NoMatric && x.CdEmpresa == model.CdEmpresa && x.CdFilial == model.CdFilial && x.Data == model.Data);
        if (entity is null) return NotFound(ApiResponse.Fail<Frequencia3>("Registro não encontrado."));

        _db.Entry(entity).CurrentValues.SetValues(model);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<string>>> Delete([FromQuery] Freq4ControllerKeyDto key)
    {
        var entity = await _db.Set<Frequencia3>().FirstOrDefaultAsync(x => x.NoMatric == key.NoMatric && x.CdEmpresa == key.CdEmpresa && x.CdFilial == key.CdFilial && x.Data == key.Data);
        if (entity is null) return NotFound(ApiResponse.Fail<string>("Registro não encontrado."));

        _db.Remove(entity);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok("Excluído com sucesso."));
    }
}