// src/API/Controllers/Identity/UsuariosController.cs

using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Identity.Application.Services;

namespace RhSensoERP.API.Controllers.Identity;

/// <summary>
/// Controller para gerenciamento de usuários.
/// </summary>
[ApiController]
[Route("api/identity/usuarios")]
[ApiExplorerSettings(GroupName = "Identity")]
public sealed class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuariosController(IUsuarioService service) => _service = service;

    /// <summary>
    /// Obtém um usuário pelo código (cdusuario).
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados do usuário ou NotFound</returns>
    [HttpGet("{cdUsuario}")]
    public async Task<IActionResult> GetByCodigo(
        [FromRoute] string cdUsuario,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cdUsuario))
            return BadRequest(new { error = "cdUsuario obrigatório." });

        var result = await _service.GetAsync(cdUsuario, ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Pesquisa usuários por termo (cdusuario, nome ou e-mail).
    /// </summary>
    /// <param name="term">Termo de busca (opcional)</param>
    /// <param name="take">Quantidade de registros (padrão 20, máximo 100)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Lista de usuários encontrados</returns>
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? term,
        [FromQuery] int take = 20,
        CancellationToken ct = default)
    {
        // Validação
        take = Math.Clamp(take, 1, 100);

        var result = await _service.SearchAsync(term, take, ct);
        return Ok(result);
    }
}