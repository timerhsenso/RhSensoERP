// src/API/Controllers/SEG/FuncaoController.cs
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Application.SEG.DTOs;
using RhSensoERP.Application.SEG.Interfaces;

namespace RhSensoERP.API.Controllers.SEG;

[ApiController]
[Route("api/seg/[controller]")]
public class FuncaoController : ControllerBase
{
    private readonly IFuncaoService _funcaoService;
    private readonly ILogger<FuncaoController> _logger;

    public FuncaoController(
        IFuncaoService funcaoService,
        ILogger<FuncaoController> logger)
    {
        _funcaoService = funcaoService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todas as funções
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FuncaoDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FuncaoDTO>>> GetAll()
    {
        var funcoes = await _funcaoService.GetAllAsync();
        return Ok(funcoes);
    }

    /// <summary>
    /// Busca função por chave composta (cdFuncao + cdSistema)
    /// </summary>
    [HttpGet("{cdFuncao}/{cdSistema}")]
    [ProducesResponseType(typeof(FuncaoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FuncaoDTO>> GetByCompositeKey(string cdFuncao, string cdSistema)
    {
        var funcao = await _funcaoService.GetByCompositeKeyAsync(cdFuncao, cdSistema);

        if (funcao == null)
        {
            _logger.LogWarning("Função {CdFuncao} do sistema {CdSistema} não encontrada", cdFuncao, cdSistema);
            return NotFound(new { message = $"Função '{cdFuncao}' no sistema '{cdSistema}' não encontrada" });
        }

        return Ok(funcao);
    }

    /// <summary>
    /// Lista funções de um sistema específico
    /// </summary>
    [HttpGet("sistema/{cdSistema}")]
    [ProducesResponseType(typeof(IEnumerable<FuncaoDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FuncaoDTO>>> GetBySistema(string cdSistema)
    {
        var funcoes = await _funcaoService.GetBySistemaAsync(cdSistema);
        return Ok(funcoes);
    }

    /// <summary>
    /// Cria nova função
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(FuncaoDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FuncaoDTO>> Create([FromBody] CreateFuncaoDTO funcaoDto)
    {
        try
        {
            var createdFuncao = await _funcaoService.CreateAsync(funcaoDto);

            _logger.LogInformation("Função {CdFuncao} criada no sistema {CdSistema}",
                createdFuncao.CdFuncao, createdFuncao.CdSistema);

            return CreatedAtAction(
                nameof(GetByCompositeKey),
                new { cdFuncao = createdFuncao.CdFuncao, cdSistema = createdFuncao.CdSistema },
                createdFuncao);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro ao criar função: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza função existente
    /// </summary>
    [HttpPut("{cdFuncao}/{cdSistema}")]
    [ProducesResponseType(typeof(FuncaoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FuncaoDTO>> Update(
        string cdFuncao,
        string cdSistema,
        [FromBody] UpdateFuncaoDTO funcaoDto)
    {
        try
        {
            var updatedFuncao = await _funcaoService.UpdateAsync(cdFuncao, cdSistema, funcaoDto);

            _logger.LogInformation("Função {CdFuncao} do sistema {CdSistema} atualizada", cdFuncao, cdSistema);

            return Ok(updatedFuncao);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Função {CdFuncao} do sistema {CdSistema} não encontrada", cdFuncao, cdSistema);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Exclui função
    /// </summary>
    [HttpDelete("{cdFuncao}/{cdSistema}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete(string cdFuncao, string cdSistema)
    {
        try
        {
            var result = await _funcaoService.DeleteAsync(cdFuncao, cdSistema);

            if (!result)
            {
                _logger.LogWarning("Função {CdFuncao} do sistema {CdSistema} não encontrada para exclusão",
                    cdFuncao, cdSistema);
                return NotFound(new { message = "Função não encontrada" });
            }

            _logger.LogInformation("Função {CdFuncao} do sistema {CdSistema} excluída", cdFuncao, cdSistema);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro ao excluir função: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }
}