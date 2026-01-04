// =============================================================================
// RHSENSOERP GENERATOR v4.4 - API CONTROLLER TEMPLATE
// =============================================================================
// Arquivo: src/Generators/Templates/ApiControllerTemplate.cs
// Versão: 4.4 - CORRIGIDO - Lookup retorna formato Select2 nativo
// 
// ✅ CORREÇÃO v4.4:
// - Endpoint GET /lookup retorna formato Select2 sem encapsular em Result<>
// - Formato: { results: [{id, text}], pagination: {more} }
// - Parâmetro 'term' em vez de 'search' para compatibilidade Select2
// 
// ✅ RECURSOS v4.3:
// - DELETE com FK handling + BatchDeleteResult + ToggleAtivo
// - Endpoint PATCH /{id}/toggle-ativo gerado automaticamente quando entidade tem campo Ativo
// - Detecta campos: Ativo, IsAtivo, Active, IsActive
// =============================================================================
using RhSensoERP.Generators.Models;
using System.Linq;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de API Controller.
/// v4.4: Corrige endpoint de lookup para retornar formato Select2 nativo.
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

        // ✅ v4.4: Endpoint de Lookup com formato Select2 correto
        var lookupEndpoint = info.GenerateLookup ? GenerateLookupEndpoint(info) : "";

        // ✅ v4.3: Detecta campo Ativo para gerar endpoint ToggleAtivo
        var hasAtivoField = info.Properties.Any(p =>
            p.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsAtivo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("Active", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsActive", StringComparison.OrdinalIgnoreCase));
        var toggleAtivoEndpoint = hasAtivoField ? GenerateToggleAtivoEndpoint(info) : "";

        // ✅ v4.0: CurrentUser para multi-tenancy e unique validation
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
{{lookupEndpoint}}

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
    /// Atualiza um {{info.EntityName}} existente.
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
    /// ✅ v4.6 NOVO: Gera endpoint de Lookup com NOMES REAIS dos campos.
    /// NÃO gera campo "text" - retorna campos com seus nomes originais em camelCase.
    /// Formato: { results: [{id, razaoSocial, nomeFantasia, ...}], pagination: {more} }
    /// </summary>
    private static string GenerateLookupEndpoint(EntityInfo info)
    {
        // ✅ Procura propriedade com [LookupKey]
        var lookupKeyProp = info.Properties.FirstOrDefault(p => p.IsLookupKey);

        // ✅ TODOS os campos com [LookupText] viram colunas (não concatena em "text")
        var allLookupProps = info.Properties
            .Where(p => p.IsLookupText)
            .ToList();

        // ✅ Fallback: se não tem [LookupKey], usa [Key]
        if (lookupKeyProp == null)
        {
            lookupKeyProp = info.Properties.FirstOrDefault(p => p.IsPrimaryKey);
        }

        // ✅ Fallback: se não tem [LookupText], auto-detecta campo principal
        if (!allLookupProps.Any())
        {
            var displayField = info.Properties.FirstOrDefault(p =>
                p.Name.Equals("RazaoSocial", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Equals("Nome", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Equals("Descricao", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Equals("NomeFantasia", StringComparison.OrdinalIgnoreCase) ||
                p.Name.Equals("Titulo", StringComparison.OrdinalIgnoreCase));

            if (displayField != null)
            {
                allLookupProps.Add(displayField);
            }
        }

        // ✅ Gera propriedades dinâmicas com nomes REAIS
        var dynamicProps = GenerateDynamicProperties(allLookupProps);

        return $$"""


    /// <summary>
    /// Busca {{info.DisplayName}} para componentes de seleção (Lookup/Select2).
    /// Retorna campos com nomes reais: { results: [{id, campo1, campo2, ...}], pagination: {more} }
    /// </summary>
    [HttpGet("lookup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLookup(
        [FromQuery] string? term,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var request = new PagedRequest 
        { 
            Search = term, 
            Page = page, 
            PageSize = pageSize 
        };

        var result = await _mediator.Send(new Get{{info.PluralName}}PagedQuery(request), ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error.Message });
        }

        // ✅ RETORNA CAMPOS COM NOMES REAIS (SEM "text")
        var response = new
        {
            results = result.Value.Items.Select(x => new 
            { 
                id = x.{{lookupKeyProp.Name}}{{dynamicProps}}
            }),
            pagination = new
            {
                more = result.Value.HasNextPage
            }
        };

        return Ok(response);
    }
""";
    }

    /// <summary>
    /// ✅ v4.5 NOVO: Gera expressão para o campo "text" do Select2.
    /// Concatena múltiplos campos com separadores customizados.
    /// </summary>
    private static string GenerateTextExpression(List<PropertyInfo> textProps, string fallbackIdField)
    {
        if (!textProps.Any())
        {
            return $"x.{fallbackIdField}.ToString()";
        }

        if (textProps.Count == 1)
        {
            var prop = textProps[0];
            var format = string.IsNullOrEmpty(prop.LookupTextFormat)
                ? $"x.{prop.Name}"
                : $"string.Format(\"{prop.LookupTextFormat}\", x.{prop.Name})";

            return $"{format} ?? x.{fallbackIdField}.ToString()";
        }

        // ✅ Múltiplos campos: concatena com proteção contra nulls
        var parts = new List<string>();

        for (int i = 0; i < textProps.Count; i++)
        {
            var prop = textProps[i];
            var isFirst = i == 0;

            var value = string.IsNullOrEmpty(prop.LookupTextFormat)
                ? $"x.{prop.Name}"
                : $"string.Format(\"{prop.LookupTextFormat}\", x.{prop.Name})";

            if (isFirst)
            {
                // Primeiro campo: usa direto com fallback para ""
                parts.Add($"({value} ?? \"\")");
            }
            else
            {
                // Campos seguintes: só adiciona se não for null/empty
                var separator = prop.LookupTextSeparator;
                parts.Add($" + (!string.IsNullOrWhiteSpace({value}) ? \"{separator}\" + {value} : \"\")");
            }
        }

        return string.Join("", parts);
    }

    /// <summary>
    /// ✅ v4.5 NOVO: Gera propriedades adicionais (colunas separadas no JSON).
    /// </summary>
    private static string GenerateAdditionalColumns(List<PropertyInfo> columnProps)
    {
        if (!columnProps.Any())
        {
            return "";
        }

        var lines = new List<string>();

        foreach (var prop in columnProps)
        {
            var columnName = prop.LookupColumnName ??
                (char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1));

            var value = string.IsNullOrEmpty(prop.LookupTextFormat)
                ? $"x.{prop.Name}"
                : $"string.Format(\"{prop.LookupTextFormat}\", x.{prop.Name})";

            lines.Add($",\n                {columnName} = {value}");
        }

        return string.Join("", lines);
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
    // ✅ v4.0: Métodos para instanciar Commands com TenantId
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

    /// <summary>
    /// ✅ v4.6 NOVO: Gera propriedades dinâmicas com nomes REAIS dos campos.
    /// Cada campo marcado com [LookupText] vira uma propriedade no JSON.
    /// </summary>
    private static string GenerateDynamicProperties(List<PropertyInfo> lookupProps)
    {
        if (!lookupProps.Any())
        {
            return "";
        }

        var lines = new List<string>();

        foreach (var prop in lookupProps)
        {
            // ✅ Converte para camelCase (RazaoSocial → razaoSocial)
            var propertyName = char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1);

            // ✅ Aplica formatação se tiver
            var value = string.IsNullOrEmpty(prop.LookupTextFormat)
                ? $"x.{prop.Name}"
                : $"string.Format(\"{prop.LookupTextFormat}\", x.{prop.Name})";

            lines.Add($",\n                {propertyName} = {value}");
        }

        return string.Join("", lines);
    }
}