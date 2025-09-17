namespace RhSensoERP.API.Configuration;

/// <summary>
/// Extens�es para versionamento de API
/// </summary>
public static class ApiVersioningExtensions
{
    /// <summary>
    /// Mapeia um endpoint para a vers�o 1 da API
    /// </summary>
    /// <param name="endpoints">Builder de rotas de endpoint</param>
    /// <param name="pattern">Padr�o da rota</param>
    /// <param name="handler">Handler do endpoint</param>
    /// <returns>Builder de conven��o de endpoint para encadeamento</returns>
    public static IEndpointConventionBuilder MapV1(this IEndpointRouteBuilder endpoints, string pattern, Delegate handler)
        => endpoints.Map(pattern, handler).WithGroupName("v1");
}