// =============================================================================
// RHSENSOERP WEB - SCREEN ROUTE REGISTRY
// =============================================================================
// Arquivo: src/Web/Routing/ScreenRouteRegistry.cs
// Descrição: Registry em memória para rotas dinâmicas com GUID
// =============================================================================

using Microsoft.AspNetCore.Routing;
using System.Collections.Concurrent;

namespace RhSensoERP.Web.Routing;

/// <summary>
/// Registry em memória para a execução atual.
/// O GUID muda a cada restart da aplicação.
/// Isso significa que URLs com GUID são válidas apenas enquanto a app estiver rodando.
/// Para links permanentes, use ScreenLinkService (tokens criptografados).
/// </summary>
public sealed class ScreenRouteRegistry : IScreenRouteRegistry
{
    private readonly ConcurrentDictionary<string, Guid> _screenKeyToGuid = new();
    private readonly ConcurrentDictionary<Guid, RouteValueDictionary> _guidToRoute = new();

    public Guid GetOrCreateGuid(string screenKey, RouteValueDictionary routeValues)
    {
        // Gera um GUID aleatório por execução
        var guid = _screenKeyToGuid.GetOrAdd(screenKey, _ => Guid.NewGuid());

        // Garante que o GUID resolve para a rota atual
        _guidToRoute[guid] = routeValues;

        return guid;
    }

    public bool TryResolve(Guid guid, out RouteValueDictionary routeValues)
        => _guidToRoute.TryGetValue(guid, out routeValues!);

    public bool TryGetGuidByScreenKey(string screenKey, out Guid guid)
        => _screenKeyToGuid.TryGetValue(screenKey, out guid);
}