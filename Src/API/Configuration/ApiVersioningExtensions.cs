namespace RhSensoERP.API.Configuration;

/// <summary>
/// Extensões para versionamento de API
/// </summary>
public static class ApiVersioningExtensions
{
    /// <summary>
    /// Mapeia um endpoint para a versão 1 da API
    /// </summary>
    /// <param name="endpoints">Builder de rotas de endpoint</param>
    /// <param name="pattern">Padrão da rota</param>
    /// <param name="handler">Handler do endpoint</param>
    /// <returns>Builder de convenção de endpoint para encadeamento</returns>
    public static IEndpointConventionBuilder MapV1(this IEndpointRouteBuilder endpoints, string pattern, Delegate handler)
        => endpoints.Map(pattern, handler).WithGroupName("v1");
}