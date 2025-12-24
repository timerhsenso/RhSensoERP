// =============================================================================
// RHSENSOERP WEB - PERMISSION SERVICES EXTENSIONS
// =============================================================================
using RhSensoERP.Web.Services.Permissions;

namespace RhSensoERP.Web.Extensions;

/// <summary>
/// Método de extensão para registrar os serviços de cache de permissão.
/// </summary>
public static class PermissionServicesExtensions
{
    /// <summary>
    /// Adiciona IMemoryCache e o serviço de cache de permissões (IUserPermissionsCacheService).
    /// </summary>
    public static IServiceCollection AddPermissionsCaching(this IServiceCollection services, int cacheSize = 1000)
    {
        // Adiciona o serviço de cache em memória do ASP.NET Core
        services.AddMemoryCache(options =>
        {
            // Define um limite para o tamanho do cache para evitar consumo excessivo de memória
            options.SizeLimit = cacheSize;
        });

        // Registra o serviço de cache de permissões como Scoped
        services.AddScoped<IUserPermissionsCacheService, UserPermissionsCacheService>();

        return services;
    }
}
