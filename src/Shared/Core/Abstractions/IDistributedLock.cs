// RhSensoERP.Shared.Application — IDistributedLock
// Finalidade: Prevenir concorrência em cenários distribuídos (jobs, integrações).
// Uso: Implementar via Redis/SQL/Azure e envolver operações críticas.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace RhSensoERP.Shared.Application.Abstractions;

/// <summary>
/// Abstração para obtenção de locks distribuídos com expiração (TTL).
/// </summary>
public interface IDistributedLock
{
    /// <summary>
    /// Tenta adquirir um lock para o recurso informado dentro do TTL.
    /// Retorna um <see cref="IAsyncDisposable"/> para liberar o lock, ou null se não obtiver.
    /// </summary>
    Task<IAsyncDisposable?> TryAcquireAsync(string resource, TimeSpan ttl, CancellationToken ct = default);
}
