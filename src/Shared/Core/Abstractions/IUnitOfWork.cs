// =============================================================================
// RHSENSOERP - SHARED CORE
// =============================================================================
// Arquivo: src/Shared/RhSensoERP.Shared.Core/Abstractions/IUnitOfWork.cs
// Descrição: Interface simples de Unit of Work (compatível com DbContext)
// =============================================================================

namespace RhSensoERP.Shared.Core.Abstractions;

/// <summary>
/// Interface simples de Unit of Work.
/// Pode ser implementada diretamente pelo DbContext.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Salva todas as alterações pendentes.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

/// <summary>
/// Interface estendida de Unit of Work com suporte a transações.
/// Use a classe UnitOfWork&lt;TContext&gt; para implementação completa.
/// </summary>
public interface IUnitOfWorkWithTransaction : IUnitOfWork
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
