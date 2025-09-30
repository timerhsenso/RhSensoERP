using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Application.Auth;
using RhSensoERP.Application.SEG.DTOs;
using RhSensoERP.Application.SEG.Interfaces;
using RhSensoERP.Core.Shared;

namespace RhSensoERP.API.Controllers.SEG;

/// <summary>
/// Controller para gerenciamento de sistemas
/// Implementa controle de permissões granular por ação (IAEC)
/// </summary>
[ApiController]
[Route("api/v1/seg/sistemas")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "SEG")]
public sealed class SistemaController : ControllerBase
{
    private readonly ISistemaService _service;
    private readonly ILogger<SistemaController> _logger;

    public SistemaController(ISistemaService service, ILogger<SistemaController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ========================================
    // CONSULTAR (C) - Listar sistemas
    // ========================================

    /// <summary>
    /// Lista todos os sistemas do sistema
    /// </summary>
    /// <remarks>
    /// Requer permissão: SEG.SISTEMAS.C (Consultar)
    /// </remarks>
    [HttpGet]
    [HasPermission("SEG.SEG_FM_TSISTEMA.C")]
    [ProducesResponseType(typeof(ApiResponse<List<SistemaDto>>), 200)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<List<SistemaDto>>>> ListarSistemas(
        [FromQuery] bool incluirInativos = false,
        CancellationToken ct = default)
    {
        var sistemas = await _service.GetAllAsync(ct);

        // Filtrar inativos se necessário
        var lista = incluirInativos
            ? sistemas.ToList()
            : sistemas.Where(s => s.Ativo).ToList();

        _logger.LogInformation("Listagem de sistemas acessada por {User}. Total: {Count}",
            User.Identity?.Name, lista.Count);

        return Ok(ApiResponse<List<SistemaDto>>.Ok(lista));
    }

    // ========================================
    // CONSULTAR (C) - Obter sistema por ID
    // ========================================

    /// <summary>
    /// Obtém detalhes de um sistema específico
    /// </summary>
    /// <param name="cdSistema">Código do sistema</param>
    /// <remarks>
    /// Requer permissão: SEG.SISTEMAS.C (Consultar)
    /// </remarks>
    [HttpGet("{cdSistema}")]
    [HasPermission("SEG.SEG_FM_TSISTEMA.C")]
    [ProducesResponseType(typeof(ApiResponse<SistemaDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<SistemaDto>>> ObterSistema(
        string cdSistema,
        CancellationToken ct = default)
    {
        var sistema = await _service.GetByIdAsync(cdSistema, ct);

        if (sistema == null)
        {
            _logger.LogWarning("Sistema {CdSistema} não encontrado", cdSistema);
            return NotFound(ApiResponse<SistemaDto>.Fail($"Sistema '{cdSistema}' não encontrado"));
        }

        return Ok(ApiResponse<SistemaDto>.Ok(sistema));
    }

    // ========================================
    // CONSULTAR (C) - Filtrar sistemas
    // ========================================

    /// <summary>
    /// Filtra sistemas por descrição
    /// </summary>
    /// <param name="filtro">Texto para filtrar pela descrição</param>
    /// <remarks>
    /// Requer permissão: SEG.SISTEMAS.C (Consultar)
    /// </remarks>
    [HttpGet("filtrar")]
    [HasPermission("SEG.SEG_FM_TSISTEMA.C")]
    [ProducesResponseType(typeof(ApiResponse<List<SistemaDto>>), 200)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<List<SistemaDto>>>> FiltrarSistemas(
        [FromQuery] string? filtro,
        [FromQuery] bool incluirInativos = false,
        CancellationToken ct = default)
    {
        var sistemas = await _service.GetAllAsync(ct);

        var query = sistemas.AsQueryable();

        // Filtrar por texto
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            query = query.Where(s =>
                s.CdSistema.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                s.DcSistema.Contains(filtro, StringComparison.OrdinalIgnoreCase));
        }

        // Filtrar inativos
        if (!incluirInativos)
        {
            query = query.Where(s => s.Ativo);
        }

        var resultado = query.ToList();

        _logger.LogInformation("Filtro de sistemas por '{Filtro}' retornou {Count} resultado(s)",
            filtro ?? "todos", resultado.Count);

        return Ok(ApiResponse<List<SistemaDto>>.Ok(resultado));
    }

    // ========================================
    // INCLUIR (I) - Criar novo sistema
    // ========================================

    /// <summary>
    /// Cria um novo sistema
    /// </summary>
    /// <remarks>
    /// Requer permissão: SEG.SISTEMAS.I (Incluir)
    /// </remarks>
    [HttpPost]
    [HasPermission("SEG.SEG_FM_TSISTEMA.I")]
    [ProducesResponseType(typeof(ApiResponse<SistemaDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<SistemaDto>>> CriarSistema(
        [FromBody] SistemaUpsertDto dto,
        CancellationToken ct = default)
    {
        try
        {
            var created = await _service.CreateAsync(dto, ct);

            _logger.LogInformation("Sistema {CdSistema} criado por {User}",
                created.CdSistema, User.Identity?.Name);

            return CreatedAtAction(
                nameof(ObterSistema),
                new { cdSistema = created.CdSistema },
                ApiResponse<SistemaDto>.Ok(created, "Sistema criado com sucesso"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro ao criar sistema: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    // ========================================
    // ALTERAR (A) - Atualizar sistema
    // ========================================

    /// <summary>
    /// Atualiza dados de um sistema existente
    /// </summary>
    /// <remarks>
    /// Requer permissão: SEG.SISTEMAS.A (Alterar)
    /// </remarks>
    [HttpPut("{cdSistema}")]
    [HasPermission("SEG.SEG_FM_TSISTEMA.A")]
    [ProducesResponseType(typeof(ApiResponse<SistemaDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<SistemaDto>>> AtualizarSistema(
        string cdSistema,
        [FromBody] SistemaUpsertDto dto,
        CancellationToken ct = default)
    {
        var updated = await _service.UpdateAsync(cdSistema, dto, ct);

        if (updated == null)
        {
            _logger.LogWarning("Sistema {CdSistema} não encontrado para atualização", cdSistema);
            return NotFound(ApiResponse<SistemaDto>.Fail($"Sistema '{cdSistema}' não encontrado"));
        }

        _logger.LogInformation("Sistema {CdSistema} atualizado por {User}",
            cdSistema, User.Identity?.Name);

        return Ok(ApiResponse<SistemaDto>.Ok(updated, "Sistema atualizado com sucesso"));
    }

    // ========================================
    // EXCLUIR (E) - Deletar sistema
    // ========================================

    /// <summary>
    /// Exclui um sistema
    /// </summary>
    /// <remarks>
    /// Requer permissão: SEG.SISTEMAS.E (Excluir)
    /// </remarks>
    [HttpDelete("{cdSistema}")]
    [HasPermission("SEG.SEG_FM_TSISTEMA.E")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<object>>> ExcluirSistema(
        string cdSistema,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _service.DeleteAsync(cdSistema, ct);

            if (!result)
            {
                _logger.LogWarning("Sistema {CdSistema} não encontrado para exclusão", cdSistema);
                return NotFound(ApiResponse<object>.Fail($"Sistema '{cdSistema}' não encontrado"));
            }

            _logger.LogWarning("Sistema {CdSistema} excluído por {User}",
                cdSistema, User.Identity?.Name);

            return Ok(ApiResponse<object>.Ok(null, "Sistema excluído com sucesso"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro ao excluir sistema: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    // ========================================
    // EXCLUIR (E) - Deletar vários sistemas
    // ========================================

    /// <summary>
    /// Exclui múltiplos sistemas de uma vez
    /// </summary>
    /// <remarks>
    /// Requer permissão: SEG.SISTEMAS.E (Excluir)
    /// </remarks>
    [HttpDelete("excluir-varios")]
    [HasPermission("SEG.SEG_FM_TSISTEMA.E")]
    [ProducesResponseType(typeof(ApiResponse<BulkDeleteResult>), 200)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<BulkDeleteResult>>> ExcluirVariosSistemas(
        [FromBody] List<string> codigos,
        CancellationToken ct = default)
    {
        var resultado = new BulkDeleteResult();

        foreach (var codigo in codigos)
        {
            try
            {
                var sucesso = await _service.DeleteAsync(codigo, ct);

                if (sucesso)
                {
                    resultado.Sucesso++;
                    resultado.CodigosExcluidos.Add(codigo);
                }
                else
                {
                    resultado.Falhas++;
                    resultado.ErrosPorCodigo[codigo] = "Sistema não encontrado";
                }
            }
            catch (InvalidOperationException ex)
            {
                resultado.Falhas++;
                resultado.ErrosPorCodigo[codigo] = ex.Message;
            }
        }

        _logger.LogInformation("Exclusão em lote: {Sucesso} sucesso(s), {Falhas} falha(s) por {User}",
            resultado.Sucesso, resultado.Falhas, User.Identity?.Name);

        return Ok(ApiResponse<BulkDeleteResult>.Ok(
            resultado,
            $"Processados: {resultado.Total}. Sucesso: {resultado.Sucesso}, Falhas: {resultado.Falhas}"));
    }

    // ========================================
    // AÇÃO ADICIONAL - Ativar/Desativar
    // ========================================

    /// <summary>
    /// Ativa ou desativa um sistema
    /// </summary>
    /// <remarks>
    /// Requer permissão: SEG.SISTEMAS.A (Alterar)
    /// </remarks>
    [HttpPatch("{cdSistema}/status")]
    [HasPermission("SEG.SEG_FM_TSISTEMA.A")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<object>>> AlterarStatus(
        string cdSistema,
        [FromBody] AlterarStatusRequest request,
        CancellationToken ct = default)
    {
        var sistema = await _service.GetByIdAsync(cdSistema, ct);

        if (sistema == null)
        {
            return NotFound(ApiResponse<object>.Fail($"Sistema '{cdSistema}' não encontrado"));
        }

        var dto = new SistemaUpsertDto
        {
            CdSistema = sistema.CdSistema,
            DcSistema = sistema.DcSistema,
            Ativo = request.Ativo
        };

        await _service.UpdateAsync(cdSistema, dto, ct);

        var acao = request.Ativo ? "ativado" : "desativado";
        _logger.LogInformation("Sistema {CdSistema} {Acao} por {User}",
            cdSistema, acao, User.Identity?.Name);

        return Ok(ApiResponse<object>.Ok(null, $"Sistema {acao} com sucesso"));
    }
}

// ========================================
// DTOs Auxiliares
// ========================================

public class AlterarStatusRequest
{
    public bool Ativo { get; set; }
}

public class BulkDeleteResult
{
    public int Sucesso { get; set; }
    public int Falhas { get; set; }
    public int Total => Sucesso + Falhas;
    public List<string> CodigosExcluidos { get; set; } = new();
    public Dictionary<string, string> ErrosPorCodigo { get; set; } = new();
}