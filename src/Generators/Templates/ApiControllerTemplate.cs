// =============================================================================
// RHSENSOERP GENERATOR v4.7 - API CONTROLLER TEMPLATE
// =============================================================================
// Arquivo: src/Generators/Templates/ApiControllerTemplate.cs
// Versão: 4.7 - ADICIONADO: Suporte a chave primária composta
//
// ✅ NOVIDADES v4.7:
// 1. Rotas com múltiplos segmentos para PK composta:
//    GET    api/seguranca/funcao/{cdSistema}/{cdFuncao}
//    PUT    api/seguranca/funcao/{cdSistema}/{cdFuncao}
//    DELETE api/seguranca/funcao/{cdSistema}/{cdFuncao}
// 2. Parâmetros [FromRoute] individuais para cada parte da PK
// 3. BatchDelete desabilitado para PK composta (sem sentido com List<string>)
// 4. ToggleAtivo com suporte a PK composta
//
// ✅ RECURSOS v4.6:
// - Endpoint GET /metadata retorna EntityMetadata para UI dinâmica
// - SEGURANÇA: Endpoint só funciona em ambiente Development
//
// ✅ RECURSOS v4.4:
// - Endpoint GET /lookup retorna formato Select2
// =============================================================================
using RhSensoERP.Generators.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de API Controller.
/// v4.7: Suporte completo a chave primária composta.
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

        // ✅ v4.7: BatchDelete NÃO é gerado para PK composta
        var batchEndpoint = (info.SupportsBatchDelete && !info.HasCompositeKey)
            ? GenerateBatchDeleteEndpoint(info)
            : "";

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

        // ✅ v4.7: Gera endpoints com suporte a PK composta
        var getByIdEndpoint = GenerateGetByIdEndpoint(info);
        var createEndpoint = GenerateCreateEndpoint(info);
        var updateEndpoint = GenerateUpdateEndpoint(info);
        var deleteEndpoint = GenerateDeleteEndpoint(info);

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;{{authUsing}}{{currentUserUsing}}
using {{info.DtoNamespace}};
using {{info.CommandsNamespace}};
using {{info.QueriesNamespace}};
using RhSensoERP.Modules.{{info.ModuleName}}.Application.Metadata;
using RhSensoERP.Shared.Core.Common;
using RhSensoERP.Shared.Contracts.Common;
using RhSensoERP.Shared.Application.Metadata;

namespace {{info.ApiControllerNamespace}};

/// <summary>
/// Controller API para gerenciamento de {{info.DisplayName}}.
/// </summary>
[ApiController]
[Route("api/{{info.ApiRoute}}")]{{authAttribute}}
[ApiExplorerSettings(GroupName = "{{info.ApiGroup}}")]
public sealed class {{info.ControllerName}}Controller : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IWebHostEnvironment _env;{{currentUserField}}

    public {{info.ControllerName}}Controller(
        IMediator mediator,
        IWebHostEnvironment env{{currentUserParam}})
    {
        _mediator = mediator;
        _env = env;{{currentUserAssign}}
    }

    /// <summary>
    /// 🔒 Retorna os metadados da entidade para UI dinâmica.
    /// SEGURANÇA: Endpoint disponível APENAS em ambiente Development.
    /// </summary>
    [HttpGet("metadata")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EntityMetadata), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetMetadata()
    {
        // 🔒 SEGURANÇA: Bloqueia em produção
        if (!_env.IsDevelopment())
        {
            return NotFound();
        }

        var metadata = {{info.EntityName}}MetadataProvider.GetMetadata();
        return Ok(metadata);
    }

{{getByIdEndpoint}}

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

{{createEndpoint}}

{{updateEndpoint}}

{{deleteEndpoint}}
{{batchEndpoint}}{{toggleAtivoEndpoint}}
}
""";
    }

    // =========================================================================
    // ✅ v4.7: ENDPOINTS COM SUPORTE A PK COMPOSTA
    // =========================================================================

    /// <summary>
    /// Gera endpoint GET /{id} ou GET /{pk1}/{pk2}/{pk3}
    /// </summary>
    private static string GenerateGetByIdEndpoint(EntityInfo info)
    {
        if (!info.HasCompositeKey)
        {
            // PK simples - mantém comportamento original
            return $$"""
    /// <summary>
    /// Obtém um {{info.DisplayName}} pelo ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<{{info.EntityName}}Dto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<{{info.EntityName}}Dto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<{{info.EntityName}}Dto>>> GetById(
        [FromRoute] {{info.PrimaryKeyType}} id,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetBy{{info.EntityName}}IdQuery(id), ct);

        if (!result.IsSuccess)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
""";
        }

        // PK composta
        var routeTemplate = GenerateCompositeRouteTemplate(info);
        var methodParams = GenerateCompositeMethodParams(info);
        var queryArgs = GenerateCompositeConstructorArgs(info);

        return $$"""
    /// <summary>
    /// Obtém um {{info.DisplayName}} pela chave composta.
    /// </summary>
    [HttpGet("{{routeTemplate}}")]
    [ProducesResponseType(typeof(Result<{{info.EntityName}}Dto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<{{info.EntityName}}Dto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<{{info.EntityName}}Dto>>> GetById(
{{methodParams}},
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetBy{{info.EntityName}}IdQuery({{queryArgs}}), ct);

        if (!result.IsSuccess)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
""";
    }

    /// <summary>
    /// Gera endpoint POST (Create).
    /// </summary>
    private static string GenerateCreateEndpoint(EntityInfo info)
    {
        var returnType = info.HasCompositeKey ? "string" : info.PrimaryKeyType;
        var createCommandCode = GenerateCreateCommandInstantiation(info);

        return $$"""
    /// <summary>
    /// Cria um novo {{info.DisplayName}}.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Result<{{returnType}}>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<{{returnType}}>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<{{returnType}}>>> Create(
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
""";
    }

    /// <summary>
    /// Gera endpoint PUT /{id} ou PUT /{pk1}/{pk2}/{pk3}
    /// </summary>
    private static string GenerateUpdateEndpoint(EntityInfo info)
    {
        if (!info.HasCompositeKey)
        {
            var updateCommandCode = GenerateUpdateCommandInstantiation(info);

            return $$"""
    /// <summary>
    /// Atualiza um {{info.EntityName}} existente.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<bool>>> Update(
        [FromRoute] {{info.PrimaryKeyType}} id,
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
""";
        }

        // PK composta
        var routeTemplate = GenerateCompositeRouteTemplate(info);
        var methodParams = GenerateCompositeMethodParams(info);
        var commandArgs = GenerateCompositeConstructorArgs(info) + ", body";
        var updateCommandCodeComposite = GenerateUpdateCommandInstantiationComposite(info);

        return $$"""
    /// <summary>
    /// Atualiza um {{info.EntityName}} existente pela chave composta.
    /// </summary>
    [HttpPut("{{routeTemplate}}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<bool>>> Update(
{{methodParams}},
        [FromBody] Update{{info.EntityName}}Request body,
        CancellationToken ct)
    {
{{updateCommandCodeComposite}}

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
""";
    }

    /// <summary>
    /// Gera endpoint DELETE /{id} ou DELETE /{pk1}/{pk2}/{pk3}
    /// </summary>
    private static string GenerateDeleteEndpoint(EntityInfo info)
    {
        if (!info.HasCompositeKey)
        {
            return $$"""
    /// <summary>
    /// Exclui um {{info.DisplayName}} por ID.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Result<bool>>> Delete(
        [FromRoute] {{info.PrimaryKeyType}} id,
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
""";
        }

        // PK composta
        var routeTemplate = GenerateCompositeRouteTemplate(info);
        var methodParams = GenerateCompositeMethodParams(info);
        var commandArgs = GenerateCompositeConstructorArgs(info);

        return $$"""
    /// <summary>
    /// Exclui um {{info.DisplayName}} pela chave composta.
    /// </summary>
    [HttpDelete("{{routeTemplate}}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Result<bool>>> Delete(
{{methodParams}},
        CancellationToken ct)
    {
        var result = await _mediator.Send(new Delete{{info.EntityName}}Command({{commandArgs}}), ct);

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
""";
    }

    // =========================================================================
    // ✅ v4.7: HELPERS PARA PK COMPOSTA
    // =========================================================================

    /// <summary>
    /// Gera template de rota para PK composta.
    /// Ex: "{cdSistema}/{cdFuncao}/{nmBotao}"
    /// </summary>
    private static string GenerateCompositeRouteTemplate(EntityInfo info)
    {
        var segments = info.CompositeKeyProperties
            .Select(p => "{" + ToCamelCase(p) + "}");
        return string.Join("/", segments);
    }

    /// <summary>
    /// Gera parâmetros de método [FromRoute] para PK composta.
    /// Ex:
    ///     [FromRoute] string cdSistema,
    ///     [FromRoute] string cdFuncao,
    ///     [FromRoute] string nmBotao
    /// </summary>
    private static string GenerateCompositeMethodParams(EntityInfo info)
    {
        var lines = new List<string>();
        for (int i = 0; i < info.CompositeKeyProperties.Count; i++)
        {
            var type = info.CompositeKeyTypes[i];
            var name = ToCamelCase(info.CompositeKeyProperties[i]);
            var separator = (i < info.CompositeKeyProperties.Count - 1) ? "," : "";
            lines.Add($"        [FromRoute] {type} {name}{separator}");
        }
        return string.Join("\n", lines);
    }

    /// <summary>
    /// Gera argumentos para construtor de Command/Query com PK composta.
    /// Ex: "cdSistema, cdFuncao, nmBotao"
    /// </summary>
    private static string GenerateCompositeConstructorArgs(EntityInfo info)
    {
        return string.Join(", ", info.CompositeKeyProperties.Select(p => ToCamelCase(p)));
    }

    // =========================================================================
    // ✅ v4.2: BATCH DELETE (apenas PK simples)
    // =========================================================================

    /// <summary>
    /// Gera endpoint de exclusão em lote com BatchDeleteResult.
    /// Apenas para PK simples (PK composta não suportada).
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

    // =========================================================================
    // ✅ v4.6: LOOKUP ENDPOINT
    // =========================================================================

    /// <summary>
    /// Gera endpoint de Lookup com NOMES REAIS dos campos.
    /// </summary>
    private static string GenerateLookupEndpoint(EntityInfo info)
    {
        // ✅ Procura propriedade com [LookupKey]
        var lookupKeyProp = info.Properties.FirstOrDefault(p => p.IsLookupKey);

        // ✅ TODOS os campos com [LookupText] viram colunas
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

    // =========================================================================
    // ✅ v4.3/v4.7: TOGGLE ATIVO (com suporte a PK composta)
    // =========================================================================

    /// <summary>
    /// Gera endpoint PATCH para alternar status Ativo.
    /// v4.7: Suporte a PK composta na rota.
    /// </summary>
    private static string GenerateToggleAtivoEndpoint(EntityInfo info)
    {
        // Encontra o nome exato do campo Ativo
        var ativoField = info.Properties.FirstOrDefault(p =>
            p.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsAtivo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("Active", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsActive", StringComparison.OrdinalIgnoreCase));

        var fieldName = ativoField?.Name ?? "Ativo";

        if (!info.HasCompositeKey)
        {
            // PK simples
            return $$"""


    /// <summary>
    /// Alterna o status Ativo/Desativo de um {{info.DisplayName}}.
    /// </summary>
    [HttpPatch("{id}/toggle-ativo")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<bool>>> ToggleAtivo(
        [FromRoute] {{info.PrimaryKeyType}} id,
        [FromBody] ToggleAtivoRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new Toggle{{info.EntityName}}AtivoCommand(id, request.{{fieldName}}), ct);

        if (!result.IsSuccess)
        {
            if (result.Error.Code.Contains("NotFound"))
            {
                return NotFound(result);
            }

            if (result.Error.Code.Contains("Forbidden"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }

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

        // PK composta
        var routeTemplate = GenerateCompositeRouteTemplate(info) + "/toggle-ativo";
        var methodParams = GenerateCompositeMethodParams(info);
        var commandArgs = GenerateCompositeConstructorArgs(info) + $", request.{fieldName}";

        return $$"""


    /// <summary>
    /// Alterna o status Ativo/Desativo de um {{info.DisplayName}}.
    /// </summary>
    [HttpPatch("{{routeTemplate}}")]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<bool>>> ToggleAtivo(
{{methodParams}},
        [FromBody] ToggleAtivoRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new Toggle{{info.EntityName}}AtivoCommand({{commandArgs}}), ct);

        if (!result.IsSuccess)
        {
            if (result.Error.Code.Contains("NotFound"))
            {
                return NotFound(result);
            }

            if (result.Error.Code.Contains("Forbidden"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }

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

    /// <summary>
    /// Gera instanciação do UpdateCommand para PK simples.
    /// </summary>
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
    /// ✅ v4.7: Gera instanciação do UpdateCommand para PK composta.
    /// Ex: new UpdateFuncaoCommand(cdSistema, cdFuncao, body)
    /// </summary>
    private static string GenerateUpdateCommandInstantiationComposite(EntityInfo info)
    {
        var args = GenerateCompositeConstructorArgs(info) + ", body";

        var hasUniqueProps = info.Properties.Any(p => p.IsUnique);
        var needsTenantId = !info.IsLegacyTable && hasUniqueProps;

        if (needsTenantId)
        {
            return
                "        // ✅ Cria command com TenantId do usuário logado (necessário para validação de unicidade)\n" +
                $"        var command = new Update{info.EntityName}Command({args})\n" +
                "        {\n" +
                "            TenantId = _currentUser.TenantId\n" +
                "        };\n" +
                "        \n" +
                "        var result = await _mediator.Send(command, ct);";
        }
        else
        {
            return $"        var result = await _mediator.Send(new Update{info.EntityName}Command({args}), ct);";
        }
    }

    // =========================================================================
    // ✅ v4.6: Propriedades dinâmicas para Lookup
    // =========================================================================

    /// <summary>
    /// Gera propriedades dinâmicas com nomes REAIS dos campos.
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

    // =========================================================================
    // UTILITÁRIOS
    // =========================================================================

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return char.ToLowerInvariant(value[0]) + value.Substring(1);
    }
}