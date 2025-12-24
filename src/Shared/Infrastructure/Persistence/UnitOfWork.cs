// =============================================================================
// RHSENSOERP - SHARED INFRASTRUCTURE
// =============================================================================
// Arquivo: src/Shared/RhSensoERP.Shared.Infrastructure/Persistence/UnitOfWork.cs
// Descrição: Implementação do Unit of Work para gerenciamento de transações
// =============================================================================

using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using RhSensoERP.Shared.Core.Abstractions;

namespace RhSensoERP.Shared.Infrastructure.Persistence;

/// <summary>
/// Implementação do Unit of Work para gerenciamento de transações.
/// </summary>
/// <typeparam name="TContext">Tipo do DbContext</typeparam>
public class UnitOfWork<TContext> : IUnitOfWorkWithTransaction
    where TContext : DbContext
{
    private readonly TContext _context;
    private readonly ILogger<UnitOfWork<TContext>> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<string, object> _repositories;
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    /// <summary>
    /// Construtor.
    /// </summary>
    public UnitOfWork(
        TContext context,
        ILogger<UnitOfWork<TContext>> logger,
        IServiceProvider serviceProvider)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _repositories = new ConcurrentDictionary<string, object>();
    }

    /// <inheritdoc />
    public bool HasActiveTransaction => _currentTransaction != null;

    /// <summary>
    /// Obtém a transação atual (se houver).
    /// </summary>
    public IDbContextTransaction? CurrentTransaction => _currentTransaction;

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            _logger.LogDebug("Salvando alterações no banco de dados");
            var result = await _context.SaveChangesAsync(ct);
            _logger.LogDebug("Alterações salvas: {Count} registros afetados", result);
            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Erro de concorrência ao salvar alterações");
            throw;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro ao salvar alterações no banco de dados");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction != null)
        {
            _logger.LogWarning("Tentativa de iniciar transação quando já existe uma ativa");
            return;
        }

        _logger.LogDebug("Iniciando transação");
        _currentTransaction = await _context.Database.BeginTransactionAsync(ct);
    }

    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_currentTransaction == null)
        {
            _logger.LogWarning("Tentativa de commit sem transação ativa");
            return;
        }

        try
        {
            await SaveChangesAsync(ct);
            await _currentTransaction.CommitAsync(ct);
            _logger.LogDebug("Transação commitada com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao commitar transação");
            await RollbackAsync(ct);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    /// <inheritdoc />
    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_currentTransaction == null)
        {
            _logger.LogWarning("Tentativa de rollback sem transação ativa");
            return;
        }

        try
        {
            await _currentTransaction.RollbackAsync(ct);
            _logger.LogDebug("Transação revertida");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao reverter transação");
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    /// <inheritdoc />
    public IRepositoryWithKey<TEntity, TKey> GetRepository<TEntity, TKey>()
        where TEntity : class
        where TKey : notnull
    {
        var key = $"{typeof(TEntity).FullName}_{typeof(TKey).FullName}";

        return (IRepositoryWithKey<TEntity, TKey>)_repositories.GetOrAdd(key, _ =>
        {
            _logger.LogDebug("Criando repositório para {Entity}", typeof(TEntity).Name);

            // Tentar obter do DI primeiro
            var repository = _serviceProvider.GetService(typeof(IRepositoryWithKey<TEntity, TKey>));
            if (repository != null)
            {
                return repository;
            }

            // Criar repositório genérico
            var loggerType = typeof(ILogger<>).MakeGenericType(
                typeof(GenericRepository<,,>).MakeGenericType(typeof(TEntity), typeof(TKey), typeof(TContext)));

            var loggerInstance = _serviceProvider.GetService(loggerType)
                ?? Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

            var repositoryType = typeof(GenericRepository<,,>).MakeGenericType(
                typeof(TEntity), typeof(TKey), typeof(TContext));

            return Activator.CreateInstance(repositoryType, _context, loggerInstance)
                ?? throw new InvalidOperationException($"Não foi possível criar repositório para {typeof(TEntity).Name}");
        });
    }

    /// <inheritdoc />
    public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken ct = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await BeginTransactionAsync(ct);

            try
            {
                await operation();
                await CommitAsync(ct);
            }
            catch
            {
                await RollbackAsync(ct);
                throw;
            }
        });
    }

    /// <inheritdoc />
    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation, CancellationToken ct = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await BeginTransactionAsync(ct);

            try
            {
                var result = await operation();
                await CommitAsync(ct);
                return result;
            }
            catch
            {
                await RollbackAsync(ct);
                throw;
            }
        });
    }

    /// <summary>
    /// Limpa o change tracker.
    /// </summary>
    public void ClearChangeTracker()
    {
        _logger.LogDebug("Limpando change tracker");
        _context.ChangeTracker.Clear();
    }

    /// <summary>
    /// Obtém o DbContext subjacente.
    /// </summary>
    public TContext Context => _context;

    private async Task DisposeTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _currentTransaction?.Dispose();
            _repositories.Clear();
            _disposed = true;
        }
    }
}
