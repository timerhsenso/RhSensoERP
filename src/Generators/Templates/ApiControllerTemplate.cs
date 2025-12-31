// =============================================================================
// RHSENSOERP GENERATOR v4.3 - API CONTROLLER TEMPLATE
// =============================================================================
// Arquivo: src/Generators/Templates/ApiControllerTemplate.cs
// Versão: 4.3 - DELETE com FK handling + BatchDeleteResult + ToggleAtivo
// 
// ✅ NOVO v4.3:
// - Endpoint PATCH /{id}/toggle-ativo gerado automaticamente quando entidade tem campo Ativo
// - Detecta campos: Ativo, IsAtivo, Active, IsActive
// - Gera endpoint completo no Controller
// 
// ✅ CORREÇÕES v4.2:
// - DELETE retorna HTTP 409 Conflict para Foreign Key violations
// - DELETE BATCH retorna HTTP 200 OK com BatchDeleteResult detalhado
// - HTTP 404 para registro não encontrado
// - HTTP 403 para acesso cross-tenant
// =============================================================================
using RhSensoERP.Generators.Models;
using System.Linq;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de API Controller.
/// v4.3: Adiciona suporte automático para Toggle Ativo.
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

        // ✅ NOVO v4.3: Detecta campo Ativo para gerar endpoint ToggleAtivo
        var hasAtivoField = info.Properties.Any(p =>
            p.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsAtivo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("Active", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsActive", StringComparison.OrdinalIgnoreCase));
        var toggleAtivoEndpoint = hasAtivoField ? GenerateToggleAtivoEndpoint(info) : "";

        // ✅ NOVO v4.0: CurrentUser para multi-tenancy e unique validation
        var hasUniqueProps = info.Properties.Any(p => p.IsUnique);
        var needsCurrentUser = !info.IsLegacyTable || hasUniqueProps;

        var currentUserUsing = needsCurrentUser ? "\nusing RhSensoERP.Shared.Core.Abstractions;" : "";
        var currentUserField = needsCurrentUser ? "\n    private readonly ICurrentUser _currentUser;" : "";
        var currentUserParam = needsCurrentUser ? ",\n        ICurrentUser currentUser" : "";
        var currentUserAssign = needsCurrentUser ? "\n        _currentUser = currentUser;" : "";

        // ✅ Gera instanciação dos commands
        var createCommandCode = GenerateCreateCommandInstantiation(info);
        var updateCommandCode = GenerateUpdateCommandInstantiation(info);

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;{{authUsing}}{{currentUserUsing}}
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
    private readonly IMediator _mediator;{{currentUserField}}

    public {{info.PluralName}}Controller(IMediator mediator{{currentUserParam}})
    {
        _mediator = mediator;{{currentUserAssign}}
    }

    /// <summary>
    /// Obtém um {{info.DisplayName}} pelo ID.
    /// </summary>
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
    [HttpPost]
    [ProducesResponseType(typeof(Result<{{pkType}}>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<{{pkType}}>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<{{pkType}}>>> Create(
        [FromBody] Create{{info.EntityName}}Request body,
        CancellationToken ct)
    {
{{createCommandCode}}

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Atualiza um {{info.DisplayName}} existente.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<bool>>> Update(
        [FromRoute] {{pkType}} id,
        [FromBody] Update{{info.EntityName}}Request body,
        CancellationToken ct)
    {
{{updateCommandCode}}

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Exclui um {{info.DisplayName}} por ID.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Result<bool>>> Delete(
        [FromRoute] {{pkType}} id,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new Delete{{info.EntityName}}Command(id), ct);

        if (!result.IsSuccess)
        {
            // ✅ CONFLICT (409) para Foreign Key Violation
            if (result.Error.Code.Contains("ForeignKeyViolation"))
            {
                return Conflict(result);
            }

            // ✅ NOT FOUND (404) para registro não encontrado
            if (result.Error.Code.Contains("NotFound"))
            {
                return NotFound(result);
            }

            // ✅ FORBIDDEN (403) para acesso negado (cross-tenant)
            if (result.Error.Code.Contains("Forbidden"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }

            // ✅ BAD REQUEST (400) para outros erros
            return BadRequest(result);
        }

        return Ok(result);
    }
{{batchEndpoint}}{{toggleAtivoEndpoint}}
}
""";
    }

    /// <summary>
    /// ✅ v4.2: Gera endpoint de exclusão em lote com BatchDeleteResult.
    /// </summary>
    private static string GenerateBatchDeleteEndpoint(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;

        return $$"""


    /// <summary>
    /// Exclui múltiplos {{info.DisplayName}} em lote.
    /// </summary>
    [HttpDelete("batch")]
    [ProducesResponseType(typeof(Result<BatchDeleteResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<BatchDeleteResult>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<BatchDeleteResult>>> DeleteMultiple(
        [FromBody] List<{{pkType}}> ids,
        CancellationToken ct)
    {
        if (ids == null || ids.Count == 0)
        {
            return BadRequest(Result<BatchDeleteResult>.Failure(
                Error.Validation("{{info.EntityName}}.InvalidIds", "Nenhum ID foi fornecido para exclusão")));
        }

        var result = await _mediator.Send(new Delete{{info.PluralName}}Command(ids), ct);

        // ✅ SEMPRE RETORNA 200 OK com detalhes do processamento
        // Mesmo se alguns não foram deletados, retorna sucesso parcial
        if (result.IsSuccess)
        {
            return Ok(result);
        }

        // ✅ Se NENHUM foi deletado, retorna 400 Bad Request
        return BadRequest(result);
    }
""";
    }

    /// <summary>
    /// ✅ v4.3: Gera endpoint PATCH /{id}/toggle-ativo.
    /// Permite alternar o status Ativo/Desativo de forma rápida.
    /// </summary>
    private static string GenerateToggleAtivoEndpoint(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;

        // Encontra o nome exato do campo Ativo
        var ativoField = info.Properties.FirstOrDefault(p =>
            p.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsAtivo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("Active", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsActive", StringComparison.OrdinalIgnoreCase));

        var fieldName = ativoField?.Name ?? "Ativo";

        return $$"""


    /// <summary>
    /// Alterna o status Ativo/Desativo de um {{info.DisplayName}}.
    /// </summary>
    [HttpPatch("{id}/toggle-ativo")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<bool>>> ToggleAtivo(
        [FromRoute] {{pkType}} id,
        [FromBody] ToggleAtivoRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new Toggle{{info.EntityName}}AtivoCommand(id, request.{{fieldName}}), ct);

        if (!result.IsSuccess)
        {
            // ✅ NOT FOUND (404) para registro não encontrado
            if (result.Error.Code.Contains("NotFound"))
            {
                return NotFound(result);
            }

            // ✅ FORBIDDEN (403) para acesso negado (cross-tenant)
            if (result.Error.Code.Contains("Forbidden"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }

            // ✅ BAD REQUEST (400) para outros erros
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Request para alternar status Ativo.
    /// </summary>
    public record ToggleAtivoRequest(bool {{fieldName}});
""";
    }

    // =========================================================================
    // ✅ CORRIGIDO v4.0: Métodos para instanciar Commands com TenantId
    // Usa concatenação de strings ao invés de raw literals para evitar problemas com chaves
    // =========================================================================

    private static string GenerateCreateCommandInstantiation(EntityInfo info)
    {
        var hasUniqueProps = info.Properties.Any(p => p.IsUnique);
        var needsTenantId = !info.IsLegacyTable && hasUniqueProps;

        if (needsTenantId)
        {
            return
                "        // ✅ Cria command com TenantId do usuário logado (necessário para validação de unicidade)\n" +
                $"        var command = new Create{info.EntityName}Command(body)\n" +
                "        {\n" +
                "            TenantId = _currentUser.TenantId\n" +
                "        };\n" +
                "        \n" +
                "        var result = await _mediator.Send(command, ct);";
        }
        else
        {
            return $"        var result = await _mediator.Send(new Create{info.EntityName}Command(body), ct);";
        }
    }

    private static string GenerateUpdateCommandInstantiation(EntityInfo info)
    {
        var hasUniqueProps = info.Properties.Any(p => p.IsUnique);
        var needsTenantId = !info.IsLegacyTable && hasUniqueProps;

        if (needsTenantId)
        {
            return
                "        // ✅ Cria command com TenantId do usuário logado (necessário para validação de unicidade)\n" +
                $"        var command = new Update{info.EntityName}Command(id, body)\n" +
                "        {\n" +
                "            TenantId = _currentUser.TenantId\n" +
                "        };\n" +
                "        \n" +
                "        var result = await _mediator.Send(command, ct);";
        }
        else
        {
            return $"        var result = await _mediator.Send(new Update{info.EntityName}Command(id, body), ct);";
        }
    }
}