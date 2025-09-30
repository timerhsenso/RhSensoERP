using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Application.SEG.DTOs;
using RhSensoERP.Application.SEG.Interfaces;
using RhSensoERP.Core.Shared;

namespace RhSensoERP.API.Controllers.SEG;

/// <summary>
/// Controller para gerenciamento de sistemas
/// </summary>
[ApiController]
[Route("api/seg/sistemas")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "SEG")]
public sealed class SistemaController : ControllerBase
{
    private readonly ISistemaService _service;
    private readonly ILogger<SistemaController> _logger;

    public SistemaController(ISistemaService service, ILogger<SistemaController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os sistemas
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SistemaDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SistemaDto>>>> GetAll(CancellationToken ct)
    {
        var sistemas = await _service.GetAllAsync(ct);
        return Ok(ApiResponse<IEnumerable<SistemaDto>>.Ok(sistemas));
    }

    /// <summary>
    /// Busca sistema por código
    /// </summary>
    [HttpGet("{cdSistema}")]
    [ProducesResponseType(typeof(ApiResponse<SistemaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SistemaDto>>> GetById(string cdSistema, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(cdSistema, ct);

        if (dto == null)
        {
            _logger.LogWarning("Sistema {CdSistema} não encontrado", cdSistema);
            return NotFound(ApiResponse<SistemaDto>.Fail($"Sistema '{cdSistema}' não encontrado"));
        }

        return Ok(ApiResponse<SistemaDto>.Ok(dto));
    }

    /// <summary>
    /// Cria novo sistema
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SistemaDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<SistemaDto>>> Create([FromBody] SistemaUpsertDto dto, CancellationToken ct)
    {
        try
        {
            var created = await _service.CreateAsync(dto, ct);

            _logger.LogInformation("Sistema {CdSistema} criado com sucesso", created.CdSistema);

            return CreatedAtAction(
                nameof(GetById),
                new { cdSistema = created.CdSistema },
                ApiResponse<SistemaDto>.Ok(created, "Sistema criado com sucesso"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro ao criar sistema: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Atualiza sistema existente
    /// </summary>
    [HttpPut("{cdSistema}")]
    [ProducesResponseType(typeof(ApiResponse<SistemaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SistemaDto>>> Update(
        string cdSistema,
        [FromBody] SistemaUpsertDto dto,
        CancellationToken ct)
    {
        var updated = await _service.UpdateAsync(cdSistema, dto, ct);

        if (updated == null)
        {
            _logger.LogWarning("Sistema {CdSistema} não encontrado para atualização", cdSistema);
            return NotFound(ApiResponse<SistemaDto>.Fail($"Sistema '{cdSistema}' não encontrado"));
        }

        _logger.LogInformation("Sistema {CdSistema} atualizado com sucesso", cdSistema);
        return Ok(ApiResponse<SistemaDto>.Ok(updated, "Sistema atualizado com sucesso"));
    }

    /// <summary>
    /// Exclui sistema
    /// </summary>
    [HttpDelete("{cdSistema}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete(string cdSistema, CancellationToken ct)
    {
        try
        {
            var result = await _service.DeleteAsync(cdSistema, ct);

            if (!result)
            {
                _logger.LogWarning("Sistema {CdSistema} não encontrado para exclusão", cdSistema);
                return NotFound(ApiResponse<object>.Fail($"Sistema '{cdSistema}' não encontrado"));
            }

            _logger.LogInformation("Sistema {CdSistema} excluído com sucesso", cdSistema);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro ao excluir sistema: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}