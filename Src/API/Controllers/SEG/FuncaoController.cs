using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Application.Auth;
using RhSensoERP.Application.SEG.DTOs;
using RhSensoERP.Application.SEG.Interfaces;
using RhSensoERP.Core.Shared;

namespace RhSensoERP.API.Controllers.SEG;

/// <summary>
/// Controller para gerenciamento de funções do sistema
/// Implementa controle de permissões granular por ação (IAEC)
/// </summary>
[ApiController]
[Route("api/v1/seg/funcoes")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "SEG")]
public class FuncaoController : ControllerBase
{
    private readonly IFuncaoService _funcaoService;
    private readonly ILogger<FuncaoController> _logger;

    public FuncaoController(
        IFuncaoService funcaoService,
        ILogger<FuncaoController> logger)
    {
        _funcaoService = funcaoService ?? throw new ArgumentNullException(nameof(funcaoService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ========================================
    // CONSULTAR (C) - Listar funções
    // ========================================

    /// <summary>
    /// Lista todas as funções do sistema
    /// </summary>
    /// <remarks>
    /// Requer permissão: SEG.FUNCOES.C (Consultar)
    /// </remarks>
    [HttpGet]
    [HasPermission("SEG.SEG_FUNCOES.C")]
    [ProducesResponseType(typeof(ApiResponse<List<FuncaoDTO>>), 200)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<List<FuncaoDTO>>>> ListarFuncoes()
    {
        var funcoes = await _funcaoService.GetAllAsync();
        var lista = funcoes.ToList();

        _logger.LogInformation("Listagem de funções acessada por {User}. Total: {Count}",
            User.Identity?.Name, lista.Count);

        return Ok(ApiResponse<List<FuncaoDTO>>.Ok(lista));
    }

    // ========================================
    // CONSULTAR (C) - Obter função por chave composta
    // ========================================

    /// <summary>
    /// Obtém detalhes de uma função específica
    /// </summary>
    /// <param name="cdFuncao">Código da função</param>
    /// <param name="cdSistema">Código do sistema</param>
    /// <remarks>
    /// Requer permissão: SEG.FUNCOES.C (Consultar)
    /// </remarks>
    [HttpGet("{cdFuncao}/{cdSistema}")]
    [HasPermission("SEG.SEG_FUNCOES.C")]
    [ProducesResponseType(typeof(ApiResponse<FuncaoDTO>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<FuncaoDTO>>> ObterFuncao(
        string cdFuncao,
        string cdSistema)
    {
        var funcao = await _funcaoService.GetByCompositeKeyAsync(cdFuncao, cdSistema);

        if (funcao == null)
        {
            _logger.LogWarning("Função {CdFuncao} do sistema {CdSistema} não encontrada",
                cdFuncao, cdSistema);
            return NotFound(ApiResponse<FuncaoDTO>.Fail(
                $"Função '{cdFuncao}' no sistema '{cdSistema}' não encontrada"));
        }

        return Ok(ApiResponse<FuncaoDTO>.Ok(funcao));
    }

    // ========================================
    // CONSULTAR (C) - Funções por sistema
    // ========================================

    /// <summary>
    /// Lista funções de um sistema específico
    /// </summary>
    /// <param name="cdSistema">Código do sistema</param>
    /// <remarks>
    /// Requer permissão: SEG.FUNCOES.C (Consultar)
    /// </remarks>
    [HttpGet("sistema/{cdSistema}")]
    [HasPermission("SEG.SEG_FUNCOES.C")]
    [ProducesResponseType(typeof(ApiResponse<List<FuncaoDTO>>), 200)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<List<FuncaoDTO>>>> ListarPorSistema(string cdSistema)
    {
        var funcoes = await _funcaoService.GetBySistemaAsync(cdSistema);
        var lista = funcoes.ToList();

        _logger.LogInformation("Listagem de funções do sistema {CdSistema} retornou {Count} resultado(s)",
            cdSistema, lista.Count);

        return Ok(ApiResponse<List<FuncaoDTO>>.Ok(lista));
    }

    // ========================================
    // CONSULTAR (C) - Filtrar funções
    // ========================================

    /// <summary>
    /// Filtra funções por descrição ou módulo
    /// </summary>
    /// <param name="filtro">Texto para filtrar</param>
    /// <param name="cdSistema">Filtrar por sistema específico (opcional)</param>
    /// <remarks>
    /// Requer permissão: SEG.FUNCOES.C (Consultar)
    /// </remarks>
    [HttpGet("filtrar")]
    [HasPermission("SEG.SEG_FUNCOES.C")]
    [ProducesResponseType(typeof(ApiResponse<List<FuncaoDTO>>), 200)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<List<FuncaoDTO>>>> FiltrarFuncoes(
        [FromQuery] string? filtro,
        [FromQuery] string? cdSistema)
    {
        var funcoes = await _funcaoService.GetAllAsync();
        var query = funcoes.AsQueryable();

        // Filtrar por sistema
        if (!string.IsNullOrWhiteSpace(cdSistema))
        {
            query = query.Where(f => f.CdSistema.Equals(cdSistema, StringComparison.OrdinalIgnoreCase));
        }

        // Filtrar por texto
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            query = query.Where(f =>
                f.CdFuncao.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                (f.DcFuncao != null && f.DcFuncao.Contains(filtro, StringComparison.OrdinalIgnoreCase)) ||
                (f.DcModulo != null && f.DcModulo.Contains(filtro, StringComparison.OrdinalIgnoreCase)));
        }

        var resultado = query.ToList();

        _logger.LogInformation("Filtro de funções retornou {Count} resultado(s)", resultado.Count);

        return Ok(ApiResponse<List<FuncaoDTO>>.Ok(resultado));
    }

    // ========================================
    // INCLUIR (I) - Criar nova função
    // ========================================

    /// <summary>
    /// Cria uma nova função
    /// </summary>
    /// <remarks>
    /// Requer permissão: SEG.FUNCOES.I (Incluir)
    /// </remarks>
    [HttpPost]
    [HasPermission("SEG.SEG_FUNCOES.I")]
    [ProducesResponseType(typeof(ApiResponse<FuncaoDTO>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<FuncaoDTO>>> CriarFuncao(
        [FromBody] CreateFuncaoDTO funcaoDto)
    {
        try
        {
            var created = await _funcaoService.CreateAsync(funcaoDto);

            _logger.LogInformation("Função {CdFuncao} criada no sistema {CdSistema} por {User}",
                created.CdFuncao, created.CdSistema, User.Identity?.Name);

            return CreatedAtAction(
                nameof(ObterFuncao),
                new { cdFuncao = created.CdFuncao, cdSistema = created.CdSistema },
                ApiResponse<FuncaoDTO>.Ok(created, "Função criada com sucesso"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro ao criar função: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    // ========================================
    // ALTERAR (A) - Atualizar função
    // ========================================

    /// <summary>
    /// Atualiza dados de uma função existente
    /// </summary>
    /// <remarks>
    /// Requer permissão: SEG.FUNCOES.A (Alterar)
    /// </remarks>
    [HttpPut("{cdFuncao}/{cdSistema}")]
    [HasPermission("SEG.SEG_FUNCOES.A")]
    [ProducesResponseType(typeof(ApiResponse<FuncaoDTO>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<FuncaoDTO>>> AtualizarFuncao(
        string cdFuncao,
        string cdSistema,
        [FromBody] UpdateFuncaoDTO funcaoDto)
    {
        try
        {
            var updated = await _funcaoService.UpdateAsync(cdFuncao, cdSistema, funcaoDto);

            _logger.LogInformation("Função {CdFuncao} do sistema {CdSistema} atualizada por {User}",
                cdFuncao, cdSistema, User.Identity?.Name);

            return Ok(ApiResponse<FuncaoDTO>.Ok(updated, "Função atualizada com sucesso"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Função {CdFuncao} do sistema {CdSistema} não encontrada",
                cdFuncao, cdSistema);
            return NotFound(ApiResponse<FuncaoDTO>.Fail(ex.Message));
        }
    }

    // ========================================
    // EXCLUIR (E) - Deletar função
    // ========================================

    /// <summary>
    /// Exclui uma função
    /// </summary>
    /// <remarks>
    /// Requer permissão: SEG.FUNCOES.E (Excluir)
    /// </remarks>
    [HttpDelete("{cdFuncao}/{cdSistema}")]
    [HasPermission("SEG.SEG_FUNCOES.E")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<object>>> ExcluirFuncao(
        string cdFuncao,
        string cdSistema)
    {
        try
        {
            var result = await _funcaoService.DeleteAsync(cdFuncao, cdSistema);

            if (!result)
            {
                _logger.LogWarning("Função {CdFuncao} do sistema {CdSistema} não encontrada para exclusão",
                    cdFuncao, cdSistema);
                return NotFound(ApiResponse<object>.Fail("Função não encontrada"));
            }

            _logger.LogWarning("Função {CdFuncao} do sistema {CdSistema} excluída por {User}",
                cdFuncao, cdSistema, User.Identity?.Name);

            return Ok(ApiResponse<object>.Ok(null, "Função excluída com sucesso"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erro ao excluir função: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    // ========================================
    // EXCLUIR (E) - Deletar várias funções
    // ========================================

    /// <summary>
    /// Exclui múltiplas funções de uma vez
    /// </summary>
    /// <remarks>
    /// Requer permissão: SEG.FUNCOES.E (Excluir)
    /// </remarks>
    [HttpDelete("excluir-varios")]
    [HasPermission("SEG.SEG_FUNCOES.E")]
    [ProducesResponseType(typeof(ApiResponse<BulkDeleteFuncaoResult>), 200)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<ApiResponse<BulkDeleteFuncaoResult>>> ExcluirVariasFuncoes(
        [FromBody] List<FuncaoKey> keys)
    {
        var resultado = new BulkDeleteFuncaoResult();

        foreach (var key in keys)
        {
            try
            {
                var sucesso = await _funcaoService.DeleteAsync(key.CdFuncao, key.CdSistema);

                if (sucesso)
                {
                    resultado.Sucesso++;
                    resultado.FuncoesExcluidas.Add($"{key.CdSistema}.{key.CdFuncao}");
                }
                else
                {
                    resultado.Falhas++;
                    resultado.ErrosPorFuncao[$"{key.CdSistema}.{key.CdFuncao}"] = "Função não encontrada";
                }
            }
            catch (InvalidOperationException ex)
            {
                resultado.Falhas++;
                resultado.ErrosPorFuncao[$"{key.CdSistema}.{key.CdFuncao}"] = ex.Message;
            }
        }

        _logger.LogInformation("Exclusão em lote de funções: {Sucesso} sucesso(s), {Falhas} falha(s) por {User}",
            resultado.Sucesso, resultado.Falhas, User.Identity?.Name);

        return Ok(ApiResponse<BulkDeleteFuncaoResult>.Ok(
            resultado,
            $"Processados: {resultado.Total}. Sucesso: {resultado.Sucesso}, Falhas: {resultado.Falhas}"));
    }
}

// ========================================
// DTOs Auxiliares
// ========================================

public class FuncaoKey
{
    public string CdFuncao { get; set; } = string.Empty;
    public string CdSistema { get; set; } = string.Empty;
}

public class BulkDeleteFuncaoResult
{
    public int Sucesso { get; set; }
    public int Falhas { get; set; }
    public int Total => Sucesso + Falhas;
    public List<string> FuncoesExcluidas { get; set; } = new();
    public Dictionary<string, string> ErrosPorFuncao { get; set; } = new();
}