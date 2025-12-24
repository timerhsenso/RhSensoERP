using Microsoft.Extensions.DependencyInjection;
using RhSensoERP.Shared.Core.Abstractions;
using RhSensoERP.Shared.Infrastructure.Persistence.Interceptors;
using RhSensoERP.Shared.Infrastructure.Services;

namespace RhSensoERP.Shared.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
    {
        // ==================== ABSTRACTIONS ====================
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // ==================== EF CORE INTERCEPTORS ====================
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<SqlLoggingInterceptor>(); // ✅ NOVO

        return services;
    }
}