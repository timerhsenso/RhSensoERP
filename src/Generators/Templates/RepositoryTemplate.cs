// =============================================================================
// RHSENSOERP GENERATOR v3.6.2 - REPOSITORY TEMPLATE
// =============================================================================
// Arquivo: src/Generators/Templates/RepositoryTemplate.cs
// Versão: 3.6.2 - Corrigido using do DbContext
// =============================================================================
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de Repository (Interface + Implementação).
/// </summary>
public static class RepositoryTemplate
{
    /// <summary>
    /// Gera a Interface do Repository.
    /// </summary>
    public static string GenerateInterface(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;
        var entityNs = $"{info.BaseNamespace}.Core.Entities";

        return $$"""
{{info.FileHeader}}
using {{entityNs}};

namespace {{info.RepositoryInterfaceNamespace}};

using System;
using System.Collections.Generic;

/// <summary>
/// Interface do repositório de {{info.DisplayName}}.
/// </summary>
public interface I{{info.EntityName}}Repository
{
    /// <summary>
    /// Retorna um IQueryable para consultas customizadas.
    /// </summary>
    IQueryable<{{info.EntityName}}> Query();

    /// <summary>
    /// Busca por ID.
    /// </summary>
    Task<{{info.EntityName}}?> GetByIdAsync({{pkType}} id, CancellationToken ct = default);

    /// <summary>
    /// Busca todos os registros.
    /// </summary>
    Task<IEnumerable<{{info.EntityName}}>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Adiciona um novo registro.
    /// </summary>
    Task AddAsync({{info.EntityName}} entity, CancellationToken ct = default);

    /// <summary>
    /// Atualiza um registro existente.
    /// </summary>
    Task UpdateAsync({{info.EntityName}} entity, CancellationToken ct = default);

    /// <summary>
    /// Remove um registro.
    /// </summary>
    Task DeleteAsync({{info.EntityName}} entity, CancellationToken ct = default);

    /// <summary>
    /// Verifica se existe um registro com o ID especificado.
    /// </summary>
    Task<bool> ExistsAsync({{pkType}} id, CancellationToken ct = default);
}
""";
    }

    /// <summary>
    /// Gera a Implementação do Repository.
    /// </summary>
    public static string GenerateImplementation(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;
        var pkProp = info.PrimaryKeyProperty;
        var entityNs = $"{info.BaseNamespace}.Core.Entities";

        return $$"""
{{info.FileHeader}}
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using {{entityNs}};
using {{info.RepositoryInterfaceNamespace}};
using {{info.DbContextNamespace}};

namespace {{info.RepositoryImplNamespace}};

/// <summary>
/// Implementação do repositório de {{info.DisplayName}}.
/// </summary>
public sealed class {{info.EntityName}}Repository : I{{info.EntityName}}Repository
{
    private readonly {{info.DbContextName}} _context;
    private readonly ILogger<{{info.EntityName}}Repository> _logger;

    public {{info.EntityName}}Repository(
        {{info.DbContextName}} context,
        ILogger<{{info.EntityName}}Repository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public IQueryable<{{info.EntityName}}> Query()
    {
        
        return _context.Set<{{info.EntityName}}>();
    }

    /// <inheritdoc/>
    public async Task<{{info.EntityName}}?> GetByIdAsync({{pkType}} id, CancellationToken ct = default)
    {
        return await _context.Set<{{info.EntityName}}>()
            .FirstOrDefaultAsync(e => e.{{pkProp}} == id, ct);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<{{info.EntityName}}>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Set<{{info.EntityName}}>()
            .AsNoTracking()
            .ToListAsync(ct);
    }

    /// <inheritdoc/>
    public async Task AddAsync({{info.EntityName}} entity, CancellationToken ct = default)
    {
        await _context.Set<{{info.EntityName}}>().AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogDebug("{{info.DisplayName}} adicionado: {Id}", entity.{{pkProp}});
    }

    /// <inheritdoc/>
    public async Task UpdateAsync({{info.EntityName}} entity, CancellationToken ct = default)
    {
        // ✅ Marca explicitamente como modificado para garantir tracking correto
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync(ct);

        _logger.LogDebug("{{info.DisplayName}} atualizado: {Id}", entity.{{pkProp}});
    }

    /// <inheritdoc/>
    public async Task DeleteAsync({{info.EntityName}} entity, CancellationToken ct = default)
    {
        _context.Set<{{info.EntityName}}>().Remove(entity);
        await _context.SaveChangesAsync(ct);

        _logger.LogDebug("{{info.DisplayName}} removido: {Id}", entity.{{pkProp}});
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync({{pkType}} id, CancellationToken ct = default)
    {
        return await _context.Set<{{info.EntityName}}>()
            .AnyAsync(e => e.{{pkProp}} == id, ct);
    }
}
""";
    }
}