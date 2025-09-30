using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Application.SEG.DTOs;
using RhSensoERP.Application.SEG.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RhSensoERP.Core.Security.Entities;


namespace RhSensoERP.API.Controllers.SEG;
{
    [ApiController]
    [Route("api/seg/sistemas")]
    [Authorize] // mantenha conforme seu projeto
    public sealed class SistemaController : ControllerBase
    {
        private readonly ISistemaService _service;

        public SistemaController(ISistemaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SistemaDto>>> GetAll(CancellationToken ct)
            => Ok(await _service.GetAllAsync(ct));

        [HttpGet("{cdSistema}")]
        public async Task<ActionResult<SistemaDto>> GetById(string cdSistema, CancellationToken ct)
        {
            var dto = await _service.GetByIdAsync(cdSistema, ct);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<SistemaDto>> Create([FromBody] SistemaUpsertDto dto, CancellationToken ct)
        {
            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { cdSistema = created.CdSistema }, created);
        }

        [HttpPut("{cdSistema}")]
        public async Task<ActionResult<SistemaDto>> Update(string cdSistema, [FromBody] SistemaUpsertDto dto, CancellationToken ct)
        {
            var updated = await _service.UpdateAsync(cdSistema, dto, ct);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{cdSistema}")]
        public async Task<IActionResult> Delete(string cdSistema, CancellationToken ct)
        {
            var ok = await _service.DeleteAsync(cdSistema, ct);
            return ok ? NoContent() : NotFound();
        }
    }
}
