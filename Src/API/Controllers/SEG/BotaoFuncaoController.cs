using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Application.SEG.DTOs;
using RhSensoERP.Application.SEG.Interfaces;
using RhSensoERP.Application.SEG.Validators;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace RhSensoERPAPI.Controllers.SEG
{
    [ApiController]
    [Route("api/seg/botoes-funcao")]
    public sealed class BotaoFuncaoController : ControllerBase
    {
        private readonly IBotaoFuncaoService _service;
        private readonly IValidator<BotaoFuncaoDto> _validator = new BotaoFuncaoDtoValidator();

        public BotaoFuncaoController(IBotaoFuncaoService service) => _service = service;

        // GET api/seg/botoes-funcao?cdSistema=SEG&cdFuncao=USUARIO&search=&page=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<PagedResult<BotaoFuncaoDto>>> GetAsync([FromQuery] BotaoFuncaoQuery query, CancellationToken ct)
            => Ok(await _service.GetAsync(query, ct));

        // GET api/seg/botoes-funcao/{cdSistema}/{cdFuncao}/{nmBotao}
        [HttpGet("{cdSistema}/{cdFuncao}/{nmBotao}")]
        public async Task<ActionResult<BotaoFuncaoDto>> GetByIdAsync(string cdSistema, string cdFuncao, string nmBotao, CancellationToken ct)
        {
            var dto = await _service.GetByIdAsync(cdSistema, cdFuncao, nmBotao, ct);
            return dto is null ? NotFound() : Ok(dto);
        }

        // POST api/seg/botoes-funcao
        [HttpPost]
        public async Task<ActionResult<BotaoFuncaoDto>> CreateAsync([FromBody] BotaoFuncaoDto dto, CancellationToken ct)
        {
            var val = await _validator.ValidateAsync(dto, ct);
            if (!val.IsValid) return BadRequest(val.Errors);

            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetByIdAsync),
                new { cdSistema = created.CdSistema, cdFuncao = created.CdFuncao, nmBotao = created.NmBotao },
                created);
        }

        // PUT api/seg/botoes-funcao/{cdSistema}/{cdFuncao}/{nmBotao}
        [HttpPut("{cdSistema}/{cdFuncao}/{nmBotao}")]
        public async Task<ActionResult<BotaoFuncaoDto>> UpdateAsync(
            string cdSistema, string cdFuncao, string nmBotao,
            [FromBody] BotaoFuncaoDto dto, CancellationToken ct)
        {
            // força consistência com a rota
            dto.CdSistema = cdSistema;
            dto.CdFuncao = cdFuncao;
            dto.NmBotao = nmBotao;

            var val = await _validator.ValidateAsync(dto, ct);
            if (!val.IsValid) return BadRequest(val.Errors);

            var updated = await _service.UpdateAsync(cdSistema, cdFuncao, nmBotao, dto, ct);
            return Ok(updated);
        }

        // DELETE api/seg/botoes-funcao/{cdSistema}/{cdFuncao}/{nmBotao}
        [HttpDelete("{cdSistema}/{cdFuncao}/{nmBotao}")]
        public async Task<IActionResult> DeleteAsync(string cdSistema, string cdFuncao, string nmBotao, CancellationToken ct)
            => (await _service.DeleteAsync(cdSistema, cdFuncao, nmBotao, ct)) ? NoContent() : NotFound();

        // DELETE api/seg/botoes-funcao  (exclusão em lote)
        [HttpDelete]
        public async Task<ActionResult<object>> DeleteManyAsync([FromBody] IEnumerable<BotaoFuncaoKeyDto> keys, CancellationToken ct)
        {
            if (keys is null) return BadRequest("Lista de chaves não enviada.");
            var count = await _service.DeleteManyAsync(keys, ct);
            return Ok(new { deleted = count });
        }
    }
}
