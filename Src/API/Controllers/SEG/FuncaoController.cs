// RhSensoERP.API/Controllers/FuncaoController.cs
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Application.DTOs.Funcao;
using RhSensoERP.Application.Interfaces.Services;

namespace RhSensoERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FuncaoController : ControllerBase
{
    private readonly IFuncaoService _funcaoService;

    public FuncaoController(IFuncaoService funcaoService)
    {
        _funcaoService = funcaoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FuncaoDTO>>> GetAll()
    {
        var funcoes = await _funcaoService.GetAllAsync();
        return Ok(funcoes);
    }

    [HttpGet("{cdFuncao}/{cdSistema}")]
    public async Task<ActionResult<FuncaoDTO>> GetByCompositeKey(string cdFuncao, string cdSistema)
    {
        var funcao = await _funcaoService.GetByCompositeKeyAsync(cdFuncao, cdSistema);
        if (funcao == null) return NotFound();
        return Ok(funcao);
    }

    [HttpGet("sistema/{cdSistema}")]
    public async Task<ActionResult<IEnumerable<FuncaoDTO>>> GetBySistema(string cdSistema)
    {
        var funcoes = await _funcaoService.GetBySistemaAsync(cdSistema);
        return Ok(funcoes);
    }

    [HttpPost]
    public async Task<ActionResult<FuncaoDTO>> Create(CreateFuncaoDTO funcaoDto)
    {
        try
        {
            var createdFuncao = await _funcaoService.CreateAsync(funcaoDto);
            return CreatedAtAction(nameof(GetByCompositeKey),
                new { cdFuncao = createdFuncao.CdFuncao, cdSistema = createdFuncao.CdSistema },
                createdFuncao);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{cdFuncao}/{cdSistema}")]
    public async Task<ActionResult<FuncaoDTO>> Update(string cdFuncao, string cdSistema, UpdateFuncaoDTO funcaoDto)
    {
        try
        {
            var updatedFuncao = await _funcaoService.UpdateAsync(cdFuncao, cdSistema, funcaoDto);
            return Ok(updatedFuncao);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{cdFuncao}/{cdSistema}")]
    public async Task<ActionResult> Delete(string cdFuncao, string cdSistema)
    {
        var result = await _funcaoService.DeleteAsync(cdFuncao, cdSistema);
        if (!result) return NotFound();
        return NoContent();
    }
}