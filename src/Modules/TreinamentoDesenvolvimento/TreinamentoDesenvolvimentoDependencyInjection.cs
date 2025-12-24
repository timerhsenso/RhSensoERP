using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RhSensoERP.Modules.TreinamentoDesenvolvimento.Infrastructure.Persistence.Contexts;

namespace RhSensoERP.Modules.TreinamentoDesenvolvimento;

/// <summary>
/// Dependency Injection do módulo TreinamentoDesenvolvimento.
/// </summary>
public static class TreinamentoDesenvolvimentoDependencyInjection
{
    /// <summary>
    /// Registra os serviços do módulo TreinamentoDesenvolvimento.
    /// </summary>
    public static IServiceCollection AddTreinamentoDesenvolvimentoModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var moduleAssembly = typeof(TreinamentoDesenvolvimentoDependencyInjection).Assembly;

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<TreinamentoDesenvolvimentoDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(TreinamentoDesenvolvimentoDbContext).Assembly.FullName);
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
        services.AddTreinamentoDesenvolvimentoRepositories();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(moduleAssembly));

        return services;
    }

    private static IServiceCollection AddTreinamentoDesenvolvimentoRepositories(this IServiceCollection services)
    {
        var assembly = typeof(TreinamentoDesenvolvimentoDependencyInjection).Assembly;
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
