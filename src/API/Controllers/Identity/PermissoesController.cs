// =============================================================================
// ARQUIVO ATUALIZADO - src/API/Controllers/Identity/PermissoesController.cs
// =============================================================================
//
// CORREÇÕES IMPLEMENTADAS:
// 1. Endpoint correto: /api/identity/permissoes (não /permissions)
// 2. Novo endpoint POST /validar para validação específica de ações
// 3. Documentação Swagger completa com exemplos
// =============================================================================

using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Identity.Application.DTOs.Permissoes;
using RhSensoERP.Identity.Application.Services;

namespace RhSensoERP.API.Controllers.Identity;

/// <summary>
/// Controller para gerenciamento de permissões de usuários.
/// </summary>
[ApiController]
[Route("api/identity/permissoes")]
[ApiExplorerSettings(GroupName = "Identity")]
[Produces("application/json")]
public sealed class PermissoesController : ControllerBase
{
    private readonly IPermissaoService _service;
    private readonly ILogger<PermissoesController> _logger;

    public PermissoesController(
        IPermissaoService service,
        ILogger<PermissoesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Retorna todas as permissões (funções e botões) de um usuário,
    /// opcionalmente filtrando por sistema.
    /// </summary>
    /// <param name="cdUsuario">Código do usuário legado (tuse1.cdusuario)</param>
    /// <param name="cdSistema">Código do sistema (opcional). Exemplos: RHU, FIN, EST</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>DTO com permissões do usuário</returns>
    /// <response code="200">Permissões carregadas com sucesso</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{cdUsuario}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPermissoes(
        [FromRoute] string cdUsuario,
        [FromQuery] string? cdSistema,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cdUsuario))
        {
            _logger.LogWarning("Tentativa de buscar permissões sem cdUsuario");
            return BadRequest(new { error = "cdUsuario obrigatório." });
        }

        try
        {
            var result = await _service.CarregarPermissoesAsync(cdUsuario, cdSistema, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar permissões para usuário {CdUsuario}", cdUsuario);
            return StatusCode(500, new { error = "Erro ao buscar permissões do usuário." });
        }
    }

    /// <summary>
    /// Valida se um usuário tem permissão específica para uma ação em uma função.
    /// Este endpoint é usado para validar ações específicas como Incluir, Alterar, Excluir, Consultar.
    /// </summary>
    /// <param name="request">Dados da validação (cdUsuario, cdSistema, cdFuncao, acao)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Resultado detalhado da validação</returns>
    /// <response code="200">Validação realizada com sucesso</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    /// <remarks>
    /// Exemplo de request:
    /// 
    ///     POST /api/identity/permissoes/validar
    ///     {
    ///       "cdUsuario": "VERUSA",
    ///       "cdSistema": "RHU",
    ///       "cdFuncao": "SEG_FM_TSISTEMA",
    ///       "acao": "I"
    ///     }
    /// 
    /// Ações válidas:
    /// - I = Incluir
    /// - A = Alterar
    /// - E = Excluir
    /// - C = Consultar
    /// 
    /// Exemplo de response (com permissão):
    /// 
    ///     {
    ///       "temPermissao": true,
    ///       "cdUsuario": "VERUSA",
    ///       "cdSistema": "RHU",
    ///       "cdFuncao": "SEG_FM_TSISTEMA",
    ///       "acao": "I",
    ///       "descricaoAcao": "Incluir",
    ///       "acoesDisponiveis": "IAEC"
    ///     }
    /// 
    /// Exemplo de response (sem permissão):
    /// 
    ///     {
    ///       "temPermissao": false,
    ///       "cdUsuario": "VERUSA",
    ///       "cdSistema": "RHU",
    ///       "cdFuncao": "SEG_FM_TSISTEMA",
    ///       "acao": "E",
    ///       "descricaoAcao": "Excluir",
    ///       "motivo": "Usuário não possui permissão de 'Excluir' para esta função. Ações disponíveis: Incluir, Alterar, Consultar",
    ///       "acoesDisponiveis": "IAC"
    ///     }
    /// </remarks>
    [HttpPost("validar")]
    [ProducesResponseType(typeof(ValidarPermissaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ValidarPermissao(
        [FromBody] ValidarPermissaoRequest request,
        CancellationToken ct)
    {
        if (request == null)
        {
            _logger.LogWarning("Request de validação de permissão nulo");
            return BadRequest(new { error = "Request inválido." });
        }

        if (string.IsNullOrWhiteSpace(request.CdUsuario))
        {
            return BadRequest(new { error = "cdUsuario é obrigatório." });
        }

        if (string.IsNullOrWhiteSpace(request.CdSistema))
        {
            return BadRequest(new { error = "cdSistema é obrigatório." });
        }

        if (string.IsNullOrWhiteSpace(request.CdFuncao))
        {
            return BadRequest(new { error = "cdFuncao é obrigatório." });
        }

        // Valida se a ação é válida (I, A, E, C)
        var acoesValidas = new[] { 'I', 'A', 'E', 'C' };
        if (!acoesValidas.Contains(char.ToUpper(request.Acao)))
        {
            return BadRequest(new
            {
                error = "Ação inválida. Valores aceitos: I (Incluir), A (Alterar), E (Excluir), C (Consultar)"
            });
        }

        try
        {
            // Normaliza a ação para maiúscula
            request.Acao = char.ToUpper(request.Acao);

            var result = await _service.ValidarPermissaoAsync(request, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao validar permissão para usuário {CdUsuario}, função {CdFuncao}",
                request.CdUsuario,
                request.CdFuncao);

            return StatusCode(500, new { error = "Erro ao validar permissão." });
        }
    }

    /// <summary>
    /// Retorna lista de funções permitidas para um usuário.
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="cdSistema">Código do sistema (opcional)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Lista de códigos de funções</returns>
    /// <response code="200">Lista de funções retornada com sucesso</response>
    /// <response code="400">Parâmetros inválidos</response>
    [HttpGet("{cdUsuario}/funcoes")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFuncoesPermitidas(
        [FromRoute] string cdUsuario,
        [FromQuery] string? cdSistema,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cdUsuario))
        {
            return BadRequest(new { error = "cdUsuario obrigatório." });
        }

        try
        {
            var funcoes = await _service.ObterFuncoesPermitidasAsync(cdUsuario, cdSistema, ct);
            return Ok(funcoes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar funções para usuário {CdUsuario}", cdUsuario);
            return StatusCode(500, new { error = "Erro ao buscar funções." });
        }
    }

    /// <summary>
    /// Retorna lista de botões permitidos para um usuário em uma função específica.
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="cdFuncao">Código da função</param>
    /// <param name="cdSistema">Código do sistema (opcional)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Lista de códigos de botões</returns>
    /// <response code="200">Lista de botões retornada com sucesso</response>
    /// <response code="400">Parâmetros inválidos</response>
    [HttpGet("{cdUsuario}/funcoes/{cdFuncao}/botoes")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBotoesPermitidos(
        [FromRoute] string cdUsuario,
        [FromRoute] string cdFuncao,
        [FromQuery] string? cdSistema,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cdUsuario))
        {
            return BadRequest(new { error = "cdUsuario obrigatório." });
        }

        if (string.IsNullOrWhiteSpace(cdFuncao))
        {
            return BadRequest(new { error = "cdFuncao obrigatório." });
        }

        try
        {
            var botoes = await _service.ObterBotoesPermitidosAsync(cdUsuario, cdFuncao, cdSistema, ct);
            return Ok(botoes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao buscar botões para usuário {CdUsuario}, função {CdFuncao}",
                cdUsuario,
                cdFuncao);

            return StatusCode(500, new { error = "Erro ao buscar botões." });
        }
    }

    /// <summary>
    /// Habilita ou desabilita uma permissão específica (ação) para o grupo do usuário.
    /// </summary>
    /// <param name="request">Dados do toggle</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    /// <response code="200">Permissão alterada com sucesso</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="500">Erro interno</response>
    /// <remarks>
    /// Exemplo de request:
    /// 
    ///     POST /api/identity/permissoes/toggle
    ///     {
    ///       "cdUsuario": "VERUSA",
    ///       "cdSistema": "RHU",
    ///       "cdFuncao": "FM_COMPINSS",
    ///       "acao": "I",
    ///       "enabled": false
    ///     }
    /// 
    /// **ATENÇÃO:** Esta operação altera a permissão no GRUPO do usuário,
    /// afetando todos os usuários deste grupo.
    /// </remarks>
    [HttpPost("toggle")]
    [ProducesResponseType(typeof(TogglePermissaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TogglePermissao(
        [FromBody] TogglePermissaoRequest request,
        CancellationToken ct)
    {
        if (request == null)
        {
            return BadRequest(new { error = "Request inválido." });
        }

        // Validações
        if (string.IsNullOrWhiteSpace(request.CdUsuario))
            return BadRequest(new { error = "cdUsuario é obrigatório." });

        if (string.IsNullOrWhiteSpace(request.CdSistema))
            return BadRequest(new { error = "cdSistema é obrigatório." });

        if (string.IsNullOrWhiteSpace(request.CdFuncao))
            return BadRequest(new { error = "cdFuncao é obrigatório." });

        var acoesValidas = new[] { 'I', 'A', 'E', 'C' };
        if (!acoesValidas.Contains(char.ToUpper(request.Acao)))
        {
            return BadRequest(new
            {
                error = "Ação inválida. Valores aceitos: I (Incluir), A (Alterar), E (Excluir), C (Consultar)"
            });
        }

        try
        {
            var result = await _service.TogglePermissaoAsync(request, ct);

            if (!result.Success)
            {
                return BadRequest(new { error = result.Message });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao toggle permissão");
            return StatusCode(500, new { error = "Erro ao atualizar permissão." });
        }
    }

}
