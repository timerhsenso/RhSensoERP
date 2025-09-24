using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Core.FRE.Entities;
using RhSensoERP.Core.Shared;
using RhSensoERP.Core.Abstractions.Paging;
using RhSensoERP.Infrastructure.Persistence;

namespace RhSensoERP.API.Controllers.FRE;

[ApiController]
[Route("api/v1/fre/[controller]")]
public class Freq1Controller : ControllerBase
{
    private readonly AppDbContext _db;
    public Freq1Controller(AppDbContext db) => _db = db;

    public sealed class Freq1ControllerKeyDto
    {
        public string NoMatric { get; set; }
        public int CdEmpresa { get; set; }
        public int CdFilial { get; set; }
        public DateOnly DtOcorr { get; set; }
        public string HhIniOcor { get; set; }
        public string TpOcorr { get; set; }
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<FrequenciaDetalhe>>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sort = null)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var query = _db.Set<FrequenciaDetalhe>().AsNoTracking();

        // Sorting (default by the first key property)
        var sortProp = sort ?? nameof(FrequenciaDetalhe.NoMatric);
        query = query.OrderBy(e => EF.Property<object>(e!, sortProp!));

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var result = new PagedResult<FrequenciaDetalhe> {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };

        return Ok(ApiResponse.Ok(result));
    }
    
    [HttpGet("by-key")]
    public async Task<ActionResult<ApiResponse<FrequenciaDetalhe>>> GetByKey([FromQuery] Freq1ControllerKeyDto key)
    {
        var entity = await _db.Set<FrequenciaDetalhe>().AsNoTracking().FirstOrDefaultAsync(x => x.NoMatric == key.NoMatric && x.CdEmpresa == key.CdEmpresa && x.CdFilial == key.CdFilial && x.DtOcorr == key.DtOcorr && x.HhIniOcor == key.HhIniOcor && x.TpOcorr == key.TpOcorr);
        if (entity is null) return NotFound(ApiResponse.Fail<FrequenciaDetalhe>("Registro não encontrado."));
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<FrequenciaDetalhe>>> Create([FromBody] FrequenciaDetalhe model)
    {
        // Checa duplicidade pela PK composta
        var exists = await _db.Set<FrequenciaDetalhe>().AnyAsync(x => x.NoMatric == model.NoMatric && x.CdEmpresa == model.CdEmpresa && x.CdFilial == model.CdFilial && x.DtOcorr == model.DtOcorr && x.HhIniOcor == model.HhIniOcor && x.TpOcorr == model.TpOcorr);
        if (exists) return Conflict(ApiResponse.Fail<FrequenciaDetalhe>("Registro já existe."));

        _db.Set<FrequenciaDetalhe>().Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByKey), new { NoMatric = model.NoMatric, CdEmpresa = model.CdEmpresa, CdFilial = model.CdFilial, DtOcorr = model.DtOcorr, HhIniOcor = model.HhIniOcor, TpOcorr = model.TpOcorr }, ApiResponse.Ok(model));
    }
    
    [HttpPut]
    public async Task<ActionResult<ApiResponse<FrequenciaDetalhe>>> Update([FromBody] FrequenciaDetalhe model)
    {
        var entity = await _db.Set<FrequenciaDetalhe>().FirstOrDefaultAsync(x => x.NoMatric == model.NoMatric && x.CdEmpresa == model.CdEmpresa && x.CdFilial == model.CdFilial && x.DtOcorr == model.DtOcorr && x.HhIniOcor == model.HhIniOcor && x.TpOcorr == model.TpOcorr);
        if (entity is null) return NotFound(ApiResponse.Fail<FrequenciaDetalhe>("Registro não encontrado."));

        _db.Entry(entity).CurrentValues.SetValues(model);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok(entity));
    }
    
    [HttpDelete]
    public async Task<ActionResult<ApiResponse<string>>> Delete([FromQuery] Freq1ControllerKeyDto key)
    {
        var entity = await _db.Set<FrequenciaDetalhe>().FirstOrDefaultAsync(x => x.NoMatric == key.NoMatric && x.CdEmpresa == key.CdEmpresa && x.CdFilial == key.CdFilial && x.DtOcorr == key.DtOcorr && x.HhIniOcor == key.HhIniOcor && x.TpOcorr == key.TpOcorr);
        if (entity is null) return NotFound(ApiResponse.Fail<string>("Registro não encontrado."));

        _db.Remove(entity);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse.Ok("Excluído com sucesso."));
    }
}