// =============================================================================
// RHSENSOERP - METADATA CONTROLLER
// =============================================================================
// Arquivo: src/RhSensoERP.API/Controllers/MetadataController.cs
// Descrição: API REST para consulta de metadados de entidades
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Shared.Application.Metadata;

namespace RhSensoERP.API.Controllers;

/// <summary>
/// Controller para consulta de metadados de entidades.
/// Usado pelo frontend dinâmico para renderizar UI.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MetadataController : ControllerBase
{
    private readonly IMetadataRegistry _registry;
    private readonly ILogger<MetadataController> _logger;

    public MetadataController(IMetadataRegistry registry, ILogger<MetadataController> logger)
    {
        _registry = registry;
        _logger = logger;
    }

    // =========================================================================
    // ENDPOINTS DE CONSULTA
    // =========================================================================

    /// <summary>
    /// Lista todos os módulos disponíveis.
    /// </summary>
    /// <returns>Lista de nomes de módulos</returns>
    [HttpGet("modules")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ModuleInfo>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<ModuleInfo>> GetModules()
    {
        var modules = _registry.GetModules()
            .Select(m => new ModuleInfo
            {
                Name = m,
                DisplayName = FormatModuleName(m),
                EntityCount = _registry.GetByModule(m).Count()
            })
            .ToList();

        _logger.LogDebug("Listando {Count} módulos", modules.Count);

        return Ok(modules);
    }

    /// <summary>
    /// Lista todas as entidades de um módulo.
    /// </summary>
    /// <param name="moduleName">Nome do módulo (ex: GestaoDePessoas)</param>
    /// <returns>Lista de entidades do módulo</returns>
    [HttpGet("modules/{moduleName}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<EntitySummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<EntitySummary>> GetModuleEntities(string moduleName)
    {
        var entities = _registry.GetByModule(moduleName).ToList();

        if (!entities.Any())
        {
            _logger.LogWarning("Módulo não encontrado ou sem entidades: {Module}", moduleName);
            return NotFound(new { message = $"Módulo '{moduleName}' não encontrado ou não possui entidades registradas." });
        }

        var summaries = entities.Select(e => new EntitySummary
        {
            EntityName = e.EntityName,
            DisplayName = e.DisplayName,
            PluralName = e.PluralName,
            Icon = e.UIConfig.Icon,
            BaseUrl = e.Endpoints.BaseUrl
        }).ToList();

        _logger.LogDebug("Listando {Count} entidades do módulo {Module}", summaries.Count, moduleName);

        return Ok(summaries);
    }

    /// <summary>
    /// Retorna metadados completos de uma entidade por módulo e nome.
    /// </summary>
    /// <param name="moduleName">Nome do módulo (ex: GestaoDePessoas)</param>
    /// <param name="entityName">Nome da entidade no plural (ex: bancos)</param>
    /// <returns>Metadados completos da entidade</returns>
    [HttpGet("modules/{moduleName}/{entityName}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EntityMetadata), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<EntityMetadata> GetByRoute(string moduleName, string entityName)
    {
        var metadata = _registry.GetByRoute(moduleName, entityName);

        if (metadata == null)
        {
            _logger.LogWarning("Entidade não encontrada: {Module}/{Entity}", moduleName, entityName);
            return NotFound(new { message = $"Entidade '{entityName}' não encontrada no módulo '{moduleName}'." });
        }

        _logger.LogDebug("Retornando metadados de {Module}/{Entity}", moduleName, entityName);

        return Ok(metadata);
    }

    /// <summary>
    /// Retorna metadados completos de uma entidade pelo nome.
    /// </summary>
    /// <param name="entityName">Nome da entidade (ex: Banco)</param>
    /// <returns>Metadados completos da entidade</returns>
    [HttpGet("{entityName}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EntityMetadata), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<EntityMetadata> GetByName(string entityName)
    {
        var metadata = _registry.GetByName(entityName);

        if (metadata == null)
        {
            _logger.LogWarning("Entidade não encontrada: {Entity}", entityName);
            return NotFound(new { message = $"Entidade '{entityName}' não encontrada." });
        }

        _logger.LogDebug("Retornando metadados de {Entity}", entityName);

        return Ok(metadata);
    }

    /// <summary>
    /// Lista todas as entidades registradas.
    /// </summary>
    /// <returns>Lista de todas as entidades</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<EntitySummary>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<EntitySummary>> GetAll()
    {
        var entities = _registry.GetAll()
            .Select(e => new EntitySummary
            {
                EntityName = e.EntityName,
                DisplayName = e.DisplayName,
                PluralName = e.PluralName,
                ModuleName = e.ModuleName,
                Icon = e.UIConfig.Icon,
                BaseUrl = e.Endpoints.BaseUrl
            })
            .ToList();

        _logger.LogDebug("Listando {Count} entidades", entities.Count);

        return Ok(entities);
    }

    /// <summary>
    /// Retorna estatísticas do registry de metadados.
    /// </summary>
    [HttpGet("stats")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(MetadataStats), StatusCodes.Status200OK)]
    public ActionResult<MetadataStats> GetStats()
    {
        var stats = new MetadataStats
        {
            TotalEntities = _registry.Count,
            TotalModules = _registry.GetModules().Count(),
            Modules = _registry.GetModules()
                .Select(m => new ModuleStats
                {
                    Name = m,
                    EntityCount = _registry.GetByModule(m).Count()
                })
                .ToList()
        };

        return Ok(stats);
    }

    // =========================================================================
    // HELPERS
    // =========================================================================

    /// <summary>
    /// Formata o nome do módulo para exibição.
    /// </summary>
    private static string FormatModuleName(string moduleName)
    {
        return moduleName switch
        {
            "GestaoDePessoas" => "Gestão de Pessoas",
            "ControleDePonto" => "Controle de Ponto",
            "SaudeOcupacional" => "Saúde Ocupacional",
            _ => moduleName
        };
    }
}

// =============================================================================
// DTOs DE RESPOSTA
// =============================================================================

/// <summary>
/// Informações resumidas de um módulo.
/// </summary>
public class ModuleInfo
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int EntityCount { get; set; }
}

/// <summary>
/// Informações resumidas de uma entidade.
/// </summary>
public class EntitySummary
{
    public string EntityName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PluralName { get; set; } = string.Empty;
    public string? ModuleName { get; set; }
    public string? Icon { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
}

/// <summary>
/// Estatísticas do registry de metadados.
/// </summary>
public class MetadataStats
{
    public int TotalEntities { get; set; }
    public int TotalModules { get; set; }
    public List<ModuleStats> Modules { get; set; } = new();
}

/// <summary>
/// Estatísticas de um módulo.
/// </summary>
public class ModuleStats
{
    public string Name { get; set; } = string.Empty;
    public int EntityCount { get; set; }
}