// =============================================================================
// RHSENSOERP - SHARED INFRASTRUCTURE
// =============================================================================
// Arquivo: src/Shared/RhSensoERP.Shared.Infrastructure/Persistence/GenericRepository.cs
// Descrição: Implementação genérica do repositório com EF Core
// =============================================================================

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RhSensoERP.Shared.Core.Abstractions;
using RhSensoERP.Shared.Application.Specifications;

namespace RhSensoERP.Shared.Infrastructure.Persistence;

/// <summary>
/// Implementação genérica do repositório com Entity Framework Core.
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
/// <typeparam name="TContext">Tipo do DbContext</typeparam>
public class GenericRepository<TEntity, TKey, TContext> : IRepositoryWithKey<TEntity, TKey>
    where TEntity : class
    where TKey : notnull
    where TContext : DbContext
{
    protected readonly TContext Context;
    protected readonly DbSet<TEntity> DbSet;
    protected readonly ILogger Logger;

    /// <summary>
    /// Construtor.
    /// </summary>
    public GenericRepository(TContext context, ILogger<GenericRepository<TEntity, TKey, TContext>> logger)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = context.Set<TEntity>();
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region IReadRepositoryWithKey

    /// <inheritdoc />
    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default)
    {
        Logger.LogDebug("Buscando {Entity} por ID: {Id}", typeof(TEntity).Name, id);
        return await DbSet.FindAsync(new object[] { id }, ct);
    }

    /// <inheritdoc />
    public virtual async Task<TEntity?> GetByIdWithIncludesAsync(TKey id, params Expression<Func<TEntity, object>>[] includes)
    {
        Logger.LogDebug("Buscando {Entity} por ID com includes: {Id}", typeof(TEntity).Name, id);

        IQueryable<TEntity> query = DbSet;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        // Para FindAsync com includes, precisamos usar uma abordagem diferente
        var keyProperty = Context.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()?.Properties.FirstOrDefault();
        if (keyProperty == null)
        {
            Logger.LogWarning("Não foi possível encontrar a chave primária de {Entity}", typeof(TEntity).Name);
            return null;
        }

        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var property = Expression.Property(parameter, keyProperty.Name);
        var constant = Expression.Constant(id);
        var equals = Expression.Equal(property, constant);
        var lambda = Expression.Lambda<Func<TEntity, bool>>(equals, parameter);

        return await query.FirstOrDefaultAsync(lambda);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default)
    {
        Logger.LogDebug("Listando todos os {Entity}", typeof(TEntity).Name);
        return await DbSet.AsNoTracking().ToListAsync(ct);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default)
    {
        Logger.LogDebug("Buscando {Entity} por predicado", typeof(TEntity).Name);
        return await DbSet.AsNoTracking().Where(predicate).ToListAsync(ct);
    }

    /// <inheritdoc />
    public virtual async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default)
    {
        Logger.LogDebug("Buscando primeiro {Entity} por predicado", typeof(TEntity).Name);
        return await DbSet.AsNoTracking().FirstOrDefaultAsync(predicate, ct);
    }

    /// <inheritdoc />
    public virtual async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        Logger.LogDebug("Verificando existência de {Entity}", typeof(TEntity).Name);
        return predicate == null
            ? await DbSet.AnyAsync(ct)
            : await DbSet.AnyAsync(predicate, ct);
    }

    /// <inheritdoc />
    public virtual async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        Logger.LogDebug("Contando {Entity}", typeof(TEntity).Name);
        return predicate == null
            ? await DbSet.CountAsync(ct)
            : await DbSet.CountAsync(predicate, ct);
    }

    /// <inheritdoc />
    public virtual IQueryable<TEntity> Query()
    {
        return DbSet.AsNoTracking();
    }

    #endregion

    #region IWriteRepositoryWithKey

    /// <inheritdoc />
    public virtual async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        Logger.LogDebug("Adicionando {Entity}", typeof(TEntity).Name);
        await DbSet.AddAsync(entity, ct);
    }

    /// <inheritdoc />
    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        var list = entities.ToList();
        Logger.LogDebug("Adicionando {Count} {Entity}", list.Count, typeof(TEntity).Name);
        await DbSet.AddRangeAsync(list, ct);
    }

    /// <inheritdoc />
    public virtual Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        Logger.LogDebug("Atualizando {Entity}", typeof(TEntity).Name);
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        var list = entities.ToList();
        Logger.LogDebug("Atualizando {Count} {Entity}", list.Count, typeof(TEntity).Name);
        DbSet.UpdateRange(list);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task DeleteAsync(TEntity entity, CancellationToken ct = default)
    {
        Logger.LogDebug("Removendo {Entity}", typeof(TEntity).Name);
        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task<bool> DeleteByIdAsync(TKey id, CancellationToken ct = default)
    {
        Logger.LogDebug("Removendo {Entity} por ID: {Id}", typeof(TEntity).Name, id);
        var entity = await GetByIdAsync(id, ct);
        if (entity == null)
        {
            Logger.LogWarning("{Entity} não encontrado para exclusão: {Id}", typeof(TEntity).Name, id);
            return false;
        }

        DbSet.Remove(entity);
        return true;
    }

    /// <inheritdoc />
    public virtual Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        var list = entities.ToList();
        Logger.LogDebug("Removendo {Count} {Entity}", list.Count, typeof(TEntity).Name);
        DbSet.RemoveRange(list);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task<int> DeleteWhereAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default)
    {
        Logger.LogDebug("Removendo {Entity} por predicado", typeof(TEntity).Name);

        // EF Core 7+ tem ExecuteDeleteAsync
        try
        {
            return await DbSet.Where(predicate).ExecuteDeleteAsync(ct);
        }
        catch (InvalidOperationException)
        {
            // Fallback para versões anteriores
            var entities = await DbSet.Where(predicate).ToListAsync(ct);
            DbSet.RemoveRange(entities);
            return entities.Count;
        }
    }

    #endregion

    #region Specification Support

    /// <summary>
    /// Busca entidades usando uma Specification.
    /// </summary>
    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default)
    {
        Logger.LogDebug("Buscando {Entity} por specification", typeof(TEntity).Name);
        return await ApplySpecification(specification).ToListAsync(ct);
    }

    /// <summary>
    /// Conta entidades usando uma Specification.
    /// </summary>
    public virtual async Task<int> CountAsync(
        ISpecification<TEntity> specification,
        CancellationToken ct = default)
    {
        Logger.LogDebug("Contando {Entity} por specification", typeof(TEntity).Name);

        var query = DbSet.AsQueryable();

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        if (specification.IgnoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query.CountAsync(ct);
    }

    /// <summary>
    /// Aplica uma Specification ao IQueryable.
    /// </summary>
    protected IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification)
    {
        IQueryable<TEntity> query = DbSet;

        // Apply tracking
        if (specification.AsNoTracking)
        {
            query = query.AsNoTracking();
        }

        // Apply criteria
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // Apply includes
        foreach (var include in specification.Includes)
        {
            query = query.Include(include);
        }

        foreach (var includeString in specification.IncludeStrings)
        {
            query = query.Include(includeString);
        }

        // Apply ordering
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // Apply ThenBy
        if (specification.ThenByExpressions.Any())
        {
            var orderedQuery = query as IOrderedQueryable<TEntity>;
            if (orderedQuery != null)
            {
                foreach (var (keySelector, descending) in specification.ThenByExpressions)
                {
                    orderedQuery = descending
                        ? orderedQuery.ThenByDescending(keySelector)
                        : orderedQuery.ThenBy(keySelector);
                }
                query = orderedQuery;
            }
        }

        // Apply paging
        if (specification.IsPagingEnabled)
        {
            if (specification.Skip.HasValue)
            {
                query = query.Skip(specification.Skip.Value);
            }
            if (specification.Take.HasValue)
            {
                query = query.Take(specification.Take.Value);
            }
        }

        // Apply split query
        if (specification.AsSplitQuery)
        {
            query = query.AsSplitQuery();
        }

        // Apply ignore query filters
        if (specification.IgnoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        return query;
    }

    #endregion
}
