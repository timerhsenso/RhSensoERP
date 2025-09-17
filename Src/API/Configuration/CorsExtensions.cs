namespace RhSensoERP.API.Configuration;

/// <summary>
/// Extensões para configuração de CORS (Cross-Origin Resource Sharing)
/// </summary>
public static class CorsExtensions
{
    private const string PolicyName = "DefaultCors";

    /// <summary>
    /// Adiciona uma política CORS padrão baseada nas configurações
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <param name="cfg">Configuração da aplicação</param>
    /// <returns>Coleção de serviços para encadeamento</returns>
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
    /// Aplica a política CORS padrão no pipeline de middleware
    /// </summary>
    /// <param name="app">Builder da aplicação</param>
    /// <returns>Builder da aplicação para encadeamento</returns>
    public static IApplicationBuilder UseDefaultCors(this IApplicationBuilder app)
        => app.UseCors(PolicyName);
}