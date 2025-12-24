using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RhSensoERP.Modules.AdministracaoPessoal.Infrastructure.Persistence.Contexts;

namespace RhSensoERP.Modules.AdministracaoPessoal;

/// <summary>
/// Dependency Injection do módulo AdministracaoPessoal.
/// </summary>
public static class AdministracaoPessoalDependencyInjection
{
    /// <summary>
    /// Registra os serviços do módulo AdministracaoPessoal.
    /// </summary>
    public static IServiceCollection AddAdministracaoPessoalModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var moduleAssembly = typeof(AdministracaoPessoalDependencyInjection).Assembly;

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AdministracaoPessoalDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(AdministracaoPessoalDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
                sqlOptions.CommandTimeout(60);
            });

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        services.AddAutoMapper(moduleAssembly);
        services.AddAdministracaoPessoalRepositories();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(moduleAssembly));

        return services;
    }

    private static IServiceCollection AddAdministracaoPessoalRepositories(this IServiceCollection services)
    {
        var assembly = typeof(AdministracaoPessoalDependencyInjection).Assembly;
        var types = assembly.GetTypes();

        var repoInterfaces = types
            .Where(t => t.IsInterface
                        && t.Name.StartsWith("I")
                        && t.Name.EndsWith("Repository")
                        && t.Namespace?.Contains("Interfaces.Repositories") == true);

        foreach (var interfaceType in repoInterfaces)
        {
            var implName = interfaceType.Name.Substring(1);

            var implType = types.FirstOrDefault(t =>
                t.IsClass && !t.IsAbstract && t.Name == implName && interfaceType.IsAssignableFrom(t));

            if (implType != null && !services.Any(sd => sd.ServiceType == interfaceType))
            {
                services.AddScoped(interfaceType, implType);
            }
        }

        return services;
    }
}
