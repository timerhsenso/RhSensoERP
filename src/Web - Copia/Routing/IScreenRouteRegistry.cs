// =============================================================================
// RHSENSOERP WEB - SCREEN ROUTE REGISTRY INTERFACE
// =============================================================================
// Arquivo: src/Web/Routing/IScreenRouteRegistry.cs
// Descrição: Interface para registry de rotas dinâmicas com GUID
// =============================================================================

using Microsoft.AspNetCore.Routing;

namespace RhSensoERP.Web.Routing;

/// <summary>
/// Registry para mapeamento ScreenKey <-> GUID <-> RouteValues.
/// O GUID é gerado por execução da aplicação (muda a cada restart).
/// A ScreenKey é estável e usada para persistir favoritos.
/// </summary>
public interface IScreenRouteRegistry
{
    /// <summary>
    /// Obtém ou cria um GUID para a ScreenKey informada.
    /// Também associa os RouteValues para resolução posterior.
    /// </summary>
    Guid GetOrCreateGuid(string screenKey, RouteValueDictionary routeValues);

    /// <summary>
    /// Resolve um GUID para RouteValues (usado pelo DynamicRouteTransformer).
    /// </summary>
    bool TryResolve(Guid guid, out RouteValueDictionary routeValues);

    /// <summary>
    /// Obtém o GUID associado a uma ScreenKey (se existir).
    /// </summary>
    bool TryGetGuidByScreenKey(string screenKey, out Guid guid);
}