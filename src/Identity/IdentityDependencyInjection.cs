using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RhSensoERP.Identity.Application.Services;
using RhSensoERP.Identity.Core.Interfaces.Repositories;
using RhSensoERP.Identity.Infrastructure.Persistence.Contexts;
using RhSensoERP.Identity.Infrastructure.Repositories;
using RhSensoERP.Identity.Infrastructure.Services;
using RhSensoERP.Shared.Application.Behaviors;
using RhSensoERP.Shared.Core.Abstractions;
using RhSensoERP.Shared.Infrastructure.Persistence.Interceptors;

namespace RhSensoERP.Identity;

/// <summary>
/// Dependency Injection do módulo Identity.
/// </summary>
public static class IdentityDependencyInjection
{
    /// <summary>
    /// Registra os serviços do módulo Identity.
    /// </summary>
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var moduleAssembly = typeof(IdentityDependencyInjection).Assembly;
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // =====================================================================
        // DBCONTEXT
        // =====================================================================
        services.AddDbContext<IdentityDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
                sqlOptions.CommandTimeout(60);
            });

            // ✅ INTERCEPTORES (ordem importa!)
            var auditInterceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
            var sqlLoggingInterceptor = sp.GetRequiredService<SqlLoggingInterceptor>();
            options.AddInterceptors(auditInterceptor, sqlLoggingInterceptor);

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // =====================================================================
        // UNIT OF WORK
        // =====================================================================
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<IdentityDbContext>());

        // =====================================================================
        // MEDIATR
        // =====================================================================
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(moduleAssembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        });

        // =====================================================================
        // FLUENT VALIDATION
        // =====================================================================
        services.AddValidatorsFromAssembly(moduleAssembly);

        // =====================================================================
        // AUTOMAPPER
        // =====================================================================
        services.AddAutoMapper(moduleAssembly);

        // =====================================================================
        // REPOSITORIES
        // =====================================================================
        // ✅ MANUAIS (registrados explicitamente)
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPermissaoRepository, PermissaoRepository>();

        // ✅ GERADOS (auto-discovery)
        services.AddIdentityRepositories();

        // =====================================================================
        // APPLICATION SERVICES
        // =====================================================================
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IPermissaoService, PermissaoService>();

        // ✅ CORRIGIDO: AuthService registrado (NÃO bkpAuthService!)
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();

        // =====================================================================
        // ACTIVE DIRECTORY
        // =====================================================================
        services.Configure<ActiveDirectorySettings>(
            configuration.GetSection(ActiveDirectorySettings.SectionName));
        services.AddScoped<IActiveDirectoryService, ActiveDirectoryService>();

        return services;
    }

    /// <summary>
    /// Registra automaticamente todos os repositórios GERADOS.
    /// </summary>
    private static IServiceCollection AddIdentityRepositories(this IServiceCollection services)
    {
        var assembly = typeof(IdentityDependencyInjection).Assembly;
        var types = assembly.GetTypes();

        var repoInterfaces = types
            .Where(t => t.IsInterface
                        && t.Name.StartsWith("I")
                        && t.Name.EndsWith("Repository")
                        && t.Namespace?.Contains("Interfaces.Repositories") == true);

        foreach (var interfaceType in repoInterfaces)
        {
            // ✅ PULA os repositórios manuais (já registrados)
            if (interfaceType.Name == "IUsuarioRepository" ||
                interfaceType.Name == "IPermissaoRepository")
            {
                continue;
            }

            var implName = interfaceType.Name.Substring(1); // Remove "I"

            var implType = types.FirstOrDefault(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.Name == implName &&
                interfaceType.IsAssignableFrom(t));

            if (implType != null && !services.Any(sd => sd.ServiceType == interfaceType))
            {
                services.AddScoped(interfaceType, implType);
            }
        }

        return services;
    }
}