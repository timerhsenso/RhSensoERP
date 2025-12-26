// =============================================================================
// RHSENSOERP GENERATOR v3.1 - API CONTROLLER TEMPLATE
// =============================================================================
// Arquivo: src/Generators/Templates/ApiControllerTemplate.cs
// CORRIGIDO: Usa PagedRequest no construtor da Query (alinhado com QueriesTemplate)
// =============================================================================
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de API Controller.
/// </summary>
public static class ApiControllerTemplate
{
    /// <summary>
    /// Gera o API Controller completo.
    /// </summary>
    public static string GenerateController(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;
        var authAttribute = info.ApiRequiresAuth ? "\n[Authorize]" : "";
        var authUsing = info.ApiRequiresAuth ? "\nusing Microsoft.AspNetCore.Authorization;" : "";
        var batchEndpoint = info.SupportsBatchDelete ? GenerateBatchDeleteEndpoint(info) : "";

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;{{authUsing}}
using {{info.DtoNamespace}};
using {{info.CommandsNamespace}};
using {{info.QueriesNamespace}};
using RhSensoERP.Shared.Core.Common;
using RhSensoERP.Shared.Contracts.Common;

namespace {{info.ApiControllerNamespace}};

/// <summary>
/// Controller API para gerenciamento de {{info.DisplayName}}.
/// </summary>
[ApiController]
[Route("api/{{info.ApiRoute}}")]{{authAttribute}}
[ApiExplorerSettings(GroupName = "{{info.ApiGroup}}")]
public sealed class {{info.PluralName}}Controller : ControllerBase
{
    private readonly IMediator _mediator;

    public {{info.PluralName}}Controller(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Obtém um {{info.DisplayName}} pelo ID.
    /// </summary>
    /// <param name="id">ID do registro</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>{{info.DisplayName}} encontrado</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<{{info.EntityName}}Dto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<{{info.EntityName}}Dto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<{{info.EntityName}}Dto>>> GetById(
        [FromRoute] {{pkType}} id,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetBy{{info.EntityName}}IdQuery(id), ct);

        if (!result.IsSuccess)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Lista {{info.DisplayName}} com paginação.
    /// </summary>
    /// <param name="request">Parâmetros de paginação</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Lista paginada de {{info.DisplayName}}</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] PagedRequest request,
        CancellationToken ct)
    {
        var query = new Get{{info.PluralName}}PagedQuery(request);

        var result = await _mediator.Send(query, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Cria um novo {{info.DisplayName}}.
    /// </summary>
    /// <param name="body">Dados do registro</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>ID do registro criado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Result<{{pkType}}>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<{{pkType}}>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<{{pkType}}>>> Create(
        [FromBody] Create{{info.EntityName}}Request body,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new Create{{info.EntityName}}Command(body), ct);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Atualiza um {{info.DisplayName}} existente.
    /// </summary>
    /// <param name="id">ID do registro</param>
    /// <param name="body">Dados atualizados</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<bool>>> Update(
        [FromRoute] {{pkType}} id,
        [FromBody] Update{{info.EntityName}}Request body,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new Update{{info.EntityName}}Command(id, body), ct);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Remove um {{info.DisplayName}}.
    /// </summary>
    /// <param name="id">ID do registro</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<bool>>> Delete(
        [FromRoute] {{pkType}} id,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new Delete{{info.EntityName}}Command(id), ct);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
{{batchEndpoint}}
}
""";
    }

    /// <summary>
    /// Gera o endpoint de exclusão em lote.
    /// </summary>
    private static string GenerateBatchDeleteEndpoint(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;

        return $$"""


    /// <summary>
    /// Remove múltiplos {{info.DisplayName}} em lote.
    /// </summary>
    /// <param name="ids">Lista de IDs a serem removidos</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Resultado da operação em lote</returns>
    [HttpDelete("batch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteMultiple(
        [FromBody] List<{{pkType}}> ids,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new Delete{{info.PluralName}}Command(ids), ct);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
""";
    }
}
