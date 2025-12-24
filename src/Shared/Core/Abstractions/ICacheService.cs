// RhSensoERP.Shared.Application — ICacheService
// Finalidade: Contrato para cache distribuído com TTL.
// Uso: Implementar em Infrastructure (ex: Redis) e reutilizar em behaviors/handlers.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace RhSensoERP.Shared.Application.Abstractions;

/// <summary>
/// Abstração para operações básicas de cache distribuído.
/// </summary>
public interface ICacheService
{
    /// <summary>Obtém um item do cache ou null se não existir/expirado.</summary>
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);

    /// <summary>Define/atualiza um item com TTL.</summary>
    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default);

    /// <summary>Remove um item do cache.</summary>
    Task RemoveAsync(string key, CancellationToken ct = default);
}
