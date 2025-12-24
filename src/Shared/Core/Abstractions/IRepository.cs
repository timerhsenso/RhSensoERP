namespace RhSensoERP.Shared.Core.Abstractions;

/// <summary>
/// Interface base para repositórios.
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade.</typeparam>
public interface IRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Obtém uma entidade por ID.
    /// </summary>
    Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Obtém todas as entidades.
    /// </summary>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Adiciona uma nova entidade.
    /// </summary>
    Task AddAsync(TEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Atualiza uma entidade existente.
    /// </summary>
    void Update(TEntity entity);

    /// <summary>
    /// Remove uma entidade.
    /// </summary>
    void Remove(TEntity entity);

    /// <summary>
    /// Salva as alterações.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
