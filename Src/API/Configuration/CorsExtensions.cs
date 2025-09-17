namespace RhSensoERP.API.Configuration;

public static class CorsExtensions
{
    private const string PolicyName = "DefaultCors";

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

    public static IApplicationBuilder UseDefaultCors(this IApplicationBuilder app)
        => app.UseCors(PolicyName);
}
