// =============================================================================
// RHSENSOERP.WEB - METADATA SERVICE
// =============================================================================
// Arquivo: src/RhSensoERP.Web/Services/MetadataService.cs
// Descrição: Serviço para consumir a API de metadados
// =============================================================================

using System.Net.Http.Json;
using System.Text.Json;
using RhSensoERP.Shared.Application.Metadata;

namespace RhSensoERP.Web.Services;

/// <summary>
/// Serviço para consumir a API de metadados.
/// </summary>
public interface IMetadataService
{
    /// <summary>
    /// Obtém metadados de uma entidade pelo nome.
    /// </summary>
    Task<EntityMetadata?> GetByNameAsync(string entityName);

    /// <summary>
    /// Obtém metadados de uma entidade por módulo e nome.
    /// </summary>
    Task<EntityMetadata?> GetByRouteAsync(string moduleName, string entityName);

    /// <summary>
    /// Lista todos os módulos disponíveis.
    /// </summary>
    Task<List<ModuleInfo>> GetModulesAsync();

    /// <summary>
    /// Lista entidades de um módulo.
    /// </summary>
    Task<List<EntitySummary>> GetModuleEntitiesAsync(string moduleName);

    /// <summary>
    /// Lista todas as entidades registradas.
    /// </summary>
    Task<List<EntitySummary>> GetAllEntitiesAsync();
}

/// <summary>
/// Implementação do serviço de metadados.
/// </summary>
public class MetadataService : IMetadataService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MetadataService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    // Cache em memória (opcional, para performance)
    private readonly Dictionary<string, EntityMetadata> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    public MetadataService(HttpClient httpClient, ILogger<MetadataService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <inheritdoc />
    public async Task<EntityMetadata?> GetByNameAsync(string entityName)
    {
        // Verifica cache
        if (_cache.TryGetValue(entityName, out var cached))
        {
            _logger.LogDebug("Metadados de {Entity} obtidos do cache", entityName);
            return cached;
        }

        try
        {
            await _cacheLock.WaitAsync();

            // Double-check após obter lock
            if (_cache.TryGetValue(entityName, out cached))
                return cached;

            var response = await _httpClient.GetAsync($"api/metadata/{entityName}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Falha ao obter metadados de {Entity}: {Status}", entityName, response.StatusCode);
                return null;
            }

            var metadata = await response.Content.ReadFromJsonAsync<EntityMetadata>(_jsonOptions);

            if (metadata != null)
            {
                _cache[entityName] = metadata;
                _logger.LogDebug("Metadados de {Entity} carregados e cacheados", entityName);
            }

            return metadata;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter metadados de {Entity}", entityName);
            return null;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task<EntityMetadata?> GetByRouteAsync(string moduleName, string entityName)
    {
        var cacheKey = $"{moduleName}/{entityName}";

        if (_cache.TryGetValue(cacheKey, out var cached))
            return cached;

        try
        {
            await _cacheLock.WaitAsync();

            if (_cache.TryGetValue(cacheKey, out cached))
                return cached;

            var response = await _httpClient.GetAsync($"api/metadata/modules/{moduleName}/{entityName}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Falha ao obter metadados de {Module}/{Entity}: {Status}",
                    moduleName, entityName, response.StatusCode);
                return null;
            }

            var metadata = await response.Content.ReadFromJsonAsync<EntityMetadata>(_jsonOptions);

            if (metadata != null)
            {
                _cache[cacheKey] = metadata;
                // Também cacheia pelo nome da entidade
                _cache[metadata.EntityName] = metadata;
            }

            return metadata;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter metadados de {Module}/{Entity}", moduleName, entityName);
            return null;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    /// <inheritdoc />
    public async Task<List<ModuleInfo>> GetModulesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/metadata/modules");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Falha ao obter lista de módulos: {Status}", response.StatusCode);
                return new List<ModuleInfo>();
            }

            return await response.Content.ReadFromJsonAsync<List<ModuleInfo>>(_jsonOptions)
                   ?? new List<ModuleInfo>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter lista de módulos");
            return new List<ModuleInfo>();
        }
    }

    /// <inheritdoc />
    public async Task<List<EntitySummary>> GetModuleEntitiesAsync(string moduleName)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/metadata/modules/{moduleName}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Falha ao obter entidades do módulo {Module}: {Status}",
                    moduleName, response.StatusCode);
                return new List<EntitySummary>();
            }

            return await response.Content.ReadFromJsonAsync<List<EntitySummary>>(_jsonOptions)
                   ?? new List<EntitySummary>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter entidades do módulo {Module}", moduleName);
            return new List<EntitySummary>();
        }
    }

    /// <inheritdoc />
    public async Task<List<EntitySummary>> GetAllEntitiesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/metadata");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Falha ao obter lista de entidades: {Status}", response.StatusCode);
                return new List<EntitySummary>();
            }

            return await response.Content.ReadFromJsonAsync<List<EntitySummary>>(_jsonOptions)
                   ?? new List<EntitySummary>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter lista de entidades");
            return new List<EntitySummary>();
        }
    }
}

// =============================================================================
// DTOs (cópia dos DTOs da API para não ter dependência circular)
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