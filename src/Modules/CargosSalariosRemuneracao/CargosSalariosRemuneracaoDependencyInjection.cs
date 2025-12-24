using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RhSensoERP.Modules.CargosSalariosRemuneracao.Infrastructure.Persistence.Contexts;

namespace RhSensoERP.Modules.CargosSalariosRemuneracao;

/// <summary>
/// Dependency Injection do módulo CargosSalariosRemuneracao.
/// </summary>
public static class CargosSalariosRemuneracaoDependencyInjection
{
    /// <summary>
    /// Registra os serviços do módulo CargosSalariosRemuneracao.
    /// </summary>
    public static IServiceCollection AddCargosSalariosRemuneracaoModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var moduleAssembly = typeof(CargosSalariosRemuneracaoDependencyInjection).Assembly;

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<CargosSalariosRemuneracaoDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(CargosSalariosRemuneracaoDbContext).Assembly.FullName);
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
        services.AddCargosSalariosRemuneracaoRepositories();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(moduleAssembly));

        return services;
    }

    private static IServiceCollection AddCargosSalariosRemuneracaoRepositories(this IServiceCollection services)
    {
        var assembly = typeof(CargosSalariosRemuneracaoDependencyInjection).Assembly;
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
