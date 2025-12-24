// =============================================================================
// RHSENSOERP - METADATA REGISTRY
// =============================================================================
// Arquivo: src/Shared/Application/Metadata/MetadataRegistry.cs
// Descrição: Implementação do registry central de metadados
// =============================================================================

using System.Collections.Concurrent;

namespace RhSensoERP.Shared.Application.Metadata;

/// <summary>
/// Registry central de metadados de todas as entidades.
/// Singleton que armazena e fornece acesso aos metadados em runtime.
/// </summary>
public class MetadataRegistry : IMetadataRegistry
{
    private readonly ConcurrentDictionary<string, EntityMetadata> _byName = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, EntityMetadata> _byRoute = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public EntityMetadata? GetByName(string entityName)
    {
        if (string.IsNullOrWhiteSpace(entityName))
            return null;

        return _byName.TryGetValue(entityName, out var metadata) ? metadata : null;
    }

    /// <inheritdoc/>
    public EntityMetadata? GetByRoute(string module, string entity)
    {
        if (string.IsNullOrWhiteSpace(module) || string.IsNullOrWhiteSpace(entity))
            return null;

        var key = BuildRouteKey(module, entity);
        return _byRoute.TryGetValue(key, out var metadata) ? metadata : null;
    }

    /// <inheritdoc/>
    public IEnumerable<EntityMetadata> GetByModule(string moduleName)
    {
        if (string.IsNullOrWhiteSpace(moduleName))
            return Enumerable.Empty<EntityMetadata>();

        return _byName.Values
            .Where(m => m.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
            .OrderBy(m => m.DisplayName);
    }

    /// <inheritdoc/>
    public IEnumerable<EntityMetadata> GetAll()
    {
        return _byName.Values.OrderBy(m => m.ModuleName).ThenBy(m => m.DisplayName);
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetModules()
    {
        return _byName.Values
            .Select(m => m.ModuleName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(m => m);
    }

    /// <inheritdoc/>
    public void Register(EntityMetadata metadata)
    {
        if (metadata == null)
            throw new ArgumentNullException(nameof(metadata));

        if (string.IsNullOrWhiteSpace(metadata.EntityName))
            throw new ArgumentException("EntityName é obrigatório", nameof(metadata));

        // Registra por nome
        _byName[metadata.EntityName] = metadata;

        // Registra por rota (module/plural)
        if (!string.IsNullOrWhiteSpace(metadata.ModuleName) && !string.IsNullOrWhiteSpace(metadata.PluralName))
        {
            var routeKey = BuildRouteKey(metadata.ModuleName, metadata.PluralName);
            _byRoute[routeKey] = metadata;
        }
    }

    /// <inheritdoc/>
    public void RegisterRange(IEnumerable<EntityMetadata> metadataList)
    {
        if (metadataList == null)
            throw new ArgumentNullException(nameof(metadataList));

        foreach (var metadata in metadataList)
        {
            Register(metadata);
        }
    }

    /// <inheritdoc/>
    public bool Exists(string entityName)
    {
        if (string.IsNullOrWhiteSpace(entityName))
            return false;

        return _byName.ContainsKey(entityName);
    }

    /// <inheritdoc/>
    public bool Remove(string entityName)
    {
        if (string.IsNullOrWhiteSpace(entityName))
            return false;

        if (_byName.TryRemove(entityName, out var metadata))
        {
            // Remove também da rota
            if (!string.IsNullOrWhiteSpace(metadata.ModuleName) && !string.IsNullOrWhiteSpace(metadata.PluralName))
            {
                var routeKey = BuildRouteKey(metadata.ModuleName, metadata.PluralName);
                _byRoute.TryRemove(routeKey, out _);
            }
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _byName.Clear();
        _byRoute.Clear();
    }

    /// <inheritdoc/>
    public int Count => _byName.Count;

    /// <summary>
    /// Constrói a chave de rota.
    /// </summary>
    private static string BuildRouteKey(string module, string entity)
    {
        return $"{module.ToLowerInvariant()}/{entity.ToLowerInvariant()}";
    }
}