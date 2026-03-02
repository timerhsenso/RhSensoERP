// =============================================================================
// RHSENSOERP WEB - GUID ROUTE TRANSFORMER
// =============================================================================
// Arquivo: src/Web/Routing/GuidRouteTransformer.cs
// Descrição: Transforma URLs /go/{guid} em rotas reais via registry
// =============================================================================

using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace RhSensoERP.Web.Routing;

/// <summary>
/// Transforma URLs dinâmicas /go/{guid} em rotas reais.
/// Usado com MapDynamicControllerRoute no Program.cs.
/// </summary>
public sealed class GuidRouteTransformer : DynamicRouteValueTransformer
{
    public override ValueTask<RouteValueDictionary?> TransformAsync(
        HttpContext httpContext,
        RouteValueDictionary values)
    {
        // 1) Extrai o GUID da URL
        if (!values.TryGetValue("guid", out var raw) || raw is null)
            return ValueTask.FromResult<RouteValueDictionary?>(null);

        if (!Guid.TryParse(raw.ToString(), out var guid))
            return ValueTask.FromResult<RouteValueDictionary?>(null);

        // 2) Resolve via registry
        var registry = httpContext.RequestServices.GetRequiredService<IScreenRouteRegistry>();

        return ValueTask.FromResult(
            registry.TryResolve(guid, out var route)
                ? route
                : null
        );
    }
}