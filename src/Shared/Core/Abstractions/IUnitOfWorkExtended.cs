// =============================================================================
// RHSENSOERP - SHARED CORE
// =============================================================================
// Arquivo: src/Shared/RhSensoERP.Shared.Core/Abstractions/IUnitOfWorkExtended.cs
// Descrição: Interface estendida de Unit of Work
// NOTA: Complementa IUnitOfWork existente, NÃO substitui
// =============================================================================

namespace RhSensoERP.Shared.Core.Abstractions;

/// <summary>
/// Interface estendida de Unit of Work com funcionalidades adicionais.
/// Use esta interface quando precisar de transações e repositórios tipados.
/// </summary>
public interface IUnitOfWorkExtended : IUnitOfWork
{
    /// <summary>
    /// Inicia uma transação.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken ct = default);

    /// <summary>
    /// Confirma a transação atual.
    /// </summary>
    Task CommitAsync(CancellationToken ct = default);

    /// <summary>
    /// Reverte a transação atual.
    /// </summary>
    Task RollbackAsync(CancellationToken ct = default);

    /// <summary>
    /// Obtém um repositório genérico com chave tipada para a entidade.
    /// </summary>
    IRepositoryWithKey<TEntity, TKey> GetRepository<TEntity, TKey>()
        where TEntity : class
        where TKey : notnull;

    /// <summary>
    /// Executa uma operação dentro de uma transação.
    /// </summary>
    Task ExecuteInTransactionAsync(
        Func<Task> operation,
        CancellationToken ct = default);

    /// <summary>
    /// Executa uma operação dentro de uma transação e retorna o resultado.
    /// </summary>
    Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> operation,
        CancellationToken ct = default);

    /// <summary>
    /// Indica se há uma transação ativa.
    /// </summary>
    bool HasActiveTransaction { get; }
}
