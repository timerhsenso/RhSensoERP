using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Polly;
using Polly.Extensions.Http;
using RhSensoWeb.Configuration;
using RhSensoWeb.Services;
using RhSensoWeb.Services.Interfaces;
using Serilog;
using System.Net;

namespace RhSensoWeb.Extensions;

/// <summary>
/// Extensões para configuração de serviços
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configura os serviços da aplicação
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurações
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));
        services.Configure<AuthSettings>(configuration.GetSection("AuthSettings"));

        // HttpContextAccessor
        services.AddHttpContextAccessor();

        // Serviços da aplicação
        services.AddScoped<IAuthApiService, AuthApiService>();
        services.AddScoped<IUsuarioApiService, UsuarioApiService>();

        // HttpClient com Polly para resiliência
        services.AddHttpClient<IAuthApiService, AuthApiService>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

        services.AddHttpClient<IUsuarioApiService, UsuarioApiService>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy());

        return services;
    }

    /// <summary>
    /// Configura autenticação com cookies
    /// </summary>
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authSettings = configuration.GetSection("AuthSettings").Get<AuthSettings>() ?? new AuthSettings();

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.LogoutPath = "/Auth/Logout";
                    options.AccessDeniedPath = "/Auth/AccessDenied";
                    options.ExpireTimeSpan = authSettings.ExpireTimeSpan;
                    options.SlidingExpiration = true;
                    options.Cookie.Name = authSettings.CookieName;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.SameSite = SameSiteMode.Lax;

                    options.Events.OnValidatePrincipal = async context =>
                    {
                        // Validar se o token JWT ainda é válido
                        var accessToken = context.Principal?.FindFirst("access_token")?.Value;
                        if (string.IsNullOrEmpty(accessToken))
                        {
                            context.RejectPrincipal();
                            await context.HttpContext.SignOutAsync();
                            return;
                        }
                    };
                });

        return services;
    }

    /// <summary>
    /// Política de retry para HttpClient
    /// ✅ CORRIGIDO: Apenas erros transitórios (5xx, timeout, network errors)
    /// ❌ NÃO faz retry em 4xx (Bad Request, Unauthorized, Forbidden)
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError() // 5xx e network errors
            .OrResult(msg => msg.StatusCode == HttpStatusCode.RequestTimeout) // 408
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests) // 429 (rate limit)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    var statusCode = outcome.Result?.StatusCode.ToString() ?? "NetworkError";
                    var requestUri = context.GetValueOrDefault("RequestUri")?.ToString() ?? "Unknown";

                    Log.Warning(
                        "Retry {RetryCount}/3 após {Delay}ms | Status: {StatusCode} | URI: {RequestUri}",
                        retryCount,
                        timespan.TotalMilliseconds,
                        statusCode,
                        requestUri
                    );
                });
    }

    /// <summary>
    /// Política de circuit breaker para HttpClient
    /// ✅ CORRIGIDO: Apenas erros transitórios
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.RequestTimeout)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (exception, duration) =>
                {
                    var statusCode = exception.Result?.StatusCode.ToString() ?? "NetworkError";
                    Log.Warning(
                        "⚡ Circuit Breaker ABERTO por {Duration}s | Motivo: {StatusCode}",
                        duration.TotalSeconds,
                        statusCode
                    );
                },
                onReset: () =>
                {
                    Log.Information("✅ Circuit Breaker RESETADO - API voltou ao normal");
                },
                onHalfOpen: () =>
                {
                    Log.Information("⚠️ Circuit Breaker SEMI-ABERTO - Testando API...");
                });
    }

    /// <summary>
    /// Configura CORS
    /// </summary>
    public static IServiceCollection AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("DevelopmentPolicy", builder =>
            {
                builder.WithOrigins("https://localhost:7227", "http://localhost:7227")
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            });

            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        return services;
    }

    /// <summary>
    /// Configura compressão de resposta
    /// </summary>
    public static IServiceCollection AddCustomCompression(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
        });

        return services;
    }

    /// <summary>
    /// Configura cache em memória
    /// </summary>
    public static IServiceCollection AddCustomCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddResponseCaching();

        return services;
    }

    /// <summary>
    /// Configura health checks
    /// </summary>
    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
                .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

        return services;
    }
}