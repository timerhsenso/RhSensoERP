namespace RhSensoERP.API.Configuration;

/// <summary>
/// Extens�es para configura��o de CORS (Cross-Origin Resource Sharing)
/// </summary>
public static class CorsExtensions
{
    private const string PolicyName = "DefaultCors";

    /// <summary>
    /// Adiciona uma pol�tica CORS padr�o baseada nas configura��es
    /// </summary>
    /// <param name="services">Cole��o de servi�os</param>
    /// <param name="cfg">Configura��o da aplica��o</param>
    /// <returns>Cole��o de servi�os para encadeamento</returns>
    public static IServiceCollection AddDefaultCors(this IServiceCollection services, IConfiguration cfg)
    {
        var allowed = cfg.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        services.AddCors(o => o.AddPolicy(PolicyName, b => b
            .WithOrigins(allowed)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));
        return services;
    }

    /// <summary>
    /// Aplica a pol�tica CORS padr�o no pipeline de middleware
    /// </summary>
    /// <param name="app">Builder da aplica��o</param>
    /// <returns>Builder da aplica��o para encadeamento</returns>
    public static IApplicationBuilder UseDefaultCors(this IApplicationBuilder app)
        => app.UseCors(PolicyName);
}