// =============================================================================
// RHSENSOERP - SHARED CORE
// =============================================================================
// Arquivo: src/Shared/RhSensoERP.Shared.Core/Abstractions/IRepositoryExtended.cs
// Descrição: Interfaces de repositório estendidas com chave tipada
// NOTA: Complementa IRepository<TEntity> existente, NÃO substitui
// =============================================================================

using System.Linq.Expressions;

namespace RhSensoERP.Shared.Core.Abstractions;

/// <summary>
/// Interface de repositório somente leitura com chave tipada.
/// Estende o conceito do IRepository existente adicionando suporte a chave genérica.
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
public interface IReadRepositoryWithKey<TEntity, TKey>
    where TEntity : class
    where TKey : notnull
{
    /// <summary>
    /// Obtém uma entidade pelo ID.
    /// </summary>
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default);

    /// <summary>
    /// Obtém uma entidade pelo ID com includes.
    /// </summary>
    Task<TEntity?> GetByIdWithIncludesAsync(TKey id, params Expression<Func<TEntity, object>>[] includes);

    /// <summary>
    /// Obtém todas as entidades.
    /// </summary>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Busca entidades por predicado.
    /// </summary>
    Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default);

    /// <summary>
    /// Obtém a primeira entidade que satisfaz o predicado.
    /// </summary>
    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default);

    /// <summary>
    /// Verifica se existe alguma entidade que satisfaz o predicado.
    /// </summary>
    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default);

    /// <summary>
    /// Conta entidades que satisfazem o predicado.
    /// </summary>
    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default);

    /// <summary>
    /// Expõe IQueryable para queries customizadas.
    /// </summary>
    IQueryable<TEntity> Query();
}

/// <summary>
/// Interface de repositório somente escrita com chave tipada.
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
public interface IWriteRepositoryWithKey<TEntity, TKey>
    where TEntity : class
    where TKey : notnull
{
    /// <summary>
    /// Adiciona uma nova entidade.
    /// </summary>
    Task AddAsync(TEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Adiciona várias entidades.
    /// </summary>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    /// <summary>
    /// Atualiza uma entidade.
    /// </summary>
    Task UpdateAsync(TEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Atualiza várias entidades.
    /// </summary>
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    /// <summary>
    /// Remove uma entidade.
    /// </summary>
    Task DeleteAsync(TEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Remove uma entidade pelo ID.
    /// </summary>
    Task<bool> DeleteByIdAsync(TKey id, CancellationToken ct = default);

    /// <summary>
    /// Remove várias entidades.
    /// </summary>
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    /// <summary>
    /// Remove entidades por predicado.
    /// </summary>
    Task<int> DeleteWhereAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default);
}

/// <summary>
/// Interface completa de repositório (leitura + escrita) com chave tipada.
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
public interface IRepositoryWithKey<TEntity, TKey>
    : IReadRepositoryWithKey<TEntity, TKey>,
      IWriteRepositoryWithKey<TEntity, TKey>
    where TEntity : class
    where TKey : notnull
{
}
