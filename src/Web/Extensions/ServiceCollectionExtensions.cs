// =============================================================================
// RHSENSOERP WEB - SERVICE COLLECTION EXTENSIONS
// =============================================================================
// Arquivo: src/Web/Extensions/ServiceCollectionExtensions.cs
// Descrição: Métodos de extensão para registro de serviços no DI Container
// Versão: 5.0 (Com registro automático de serviços CrudTool)
// =============================================================================

using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using RhSensoERP.Web.Configuration;
using RhSensoERP.Web.Services;
//using RhSensoERP.Web.Services.Bancos;
//using RhSensoERP.Web.Services.Sistemas;
using Serilog;
using ILogger = Serilog.ILogger;

namespace RhSensoERP.Web.Extensions;

/// <summary>
/// Métodos de extensão para configuração de serviços da aplicação Web.
/// </summary>
public static class ServiceCollectionExtensions
{
    private static readonly ILogger Logger = Log.ForContext(typeof(ServiceCollectionExtensions));

    /// <summary>
    /// Namespaces a serem excluídos do registro automático.
    /// </summary>
    private static readonly string[] ExcludedNamespaces =
    [
        ".Permissions",
        ".Base",
        ".Common"
    ];

    /// <summary>
    /// Serviços que devem ser registrados manualmente (não via auto-discovery).
    /// Inclui serviços que usam IHttpClientFactory (cliente nomeado) em vez de HttpClient tipado.
    /// </summary>
    private static readonly string[] ManualServices =
    [
        "IAuthApiService",      // Usa HttpClient nomeado (AuthApiClient)
        "IMetadataService",     // Configuração especial
        "ISistemaApiService",   // Usa IHttpClientFactory (ApiClient)
        "IBancoApiService"      // Usa IHttpClientFactory (ApiClient)
    ];

    /// <summary>
    /// Registra todos os serviços de API (HttpClients e implementações) com Polly.
    /// </summary>
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // =====================================================================
        // 1. CONFIGURAÇÃO E VALIDAÇÃO DE API SETTINGS
        // =====================================================================
        var apiSettings = ConfigureAndValidateApiSettings(services, configuration);

        // =====================================================================
        // 2. REGISTRO DO HTTPCONTEXTACCESSOR (singleton)
        // =====================================================================
        services.AddHttpContextAccessor();

        // =====================================================================
        // 3. HTTPCLIENT NOMEADOS (para serviços manuais)
        // =====================================================================
        ConfigureNamedHttpClients(services, apiSettings);

        // =====================================================================
        // 4. SERVIÇOS MANUAIS (Core - não seguem padrão CrudTool)
        // =====================================================================
        services.AddScoped<IAuthApiService, AuthApiService>();

        // =====================================================================
        // 5. SERVIÇOS CRUDTOOL (Registro Automático via Reflection)
        // =====================================================================
        services.AddCrudToolServicesAutomatically(apiSettings);

        // =====================================================================
        // 6. SERVIÇO DE METADADOS (UI Dinâmica)
        // =====================================================================
        services.AddHttpClient<IMetadataService, MetadataService>(client =>
        {
            ConfigureHttpClient(client, apiSettings, apiSettings.TimeoutSeconds);
        })
            .ConfigurePrimaryHttpMessageHandler(CreateHttpMessageHandler)
            .AddPolicyHandler((sp, _) => CreateRetryPolicy(apiSettings, sp))
            .AddPolicyHandler((sp, _) => CreateCircuitBreakerPolicy(apiSettings, sp));

        // =====================================================================
        // 7. LOG DE CONFIGURAÇÃO
        // =====================================================================
        Logger.Information(
            "API Services configurados | BaseUrl: {BaseUrl} | Timeout: {Timeout}s | Retry: {Retry}x | CircuitBreaker: {CB}/{CBD}s",
            apiSettings.BaseUrl,
            apiSettings.TimeoutSeconds,
            apiSettings.RetryCount,
            apiSettings.CircuitBreakerThreshold,
            apiSettings.CircuitBreakerDurationSeconds);

        return services;
    }

    /// <summary>
    /// Registra automaticamente todos os serviços gerados pelo CrudTool.
    /// Procura por interfaces I{Nome}ApiService e suas implementações {Nome}ApiService.
    /// </summary>
    private static void AddCrudToolServicesAutomatically(
        this IServiceCollection services,
        ApiSettings apiSettings)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;
        var registeredCount = 0;

        // Encontra todas as interfaces que seguem o padrão I{Nome}ApiService
        var interfaceTypes = assembly.GetTypes()
            .Where(t => t.IsInterface
                        && t.Name.EndsWith("ApiService")
                        && t.Name.StartsWith("I")
                        && t.Namespace != null
                        && t.Namespace.Contains(".Services.")
                        && !IsExcludedNamespace(t.Namespace)
                        && !ManualServices.Contains(t.Name))
            .ToList();

        foreach (var interfaceType in interfaceTypes)
        {
            // Encontra a implementação correspondente
            var implementationName = interfaceType.Name[1..]; // Remove o "I" do início

            var implementationType = assembly.GetTypes()
                .FirstOrDefault(t => t.Name == implementationName
                                     && t.IsClass
                                     && !t.IsAbstract
                                     && interfaceType.IsAssignableFrom(t));

            if (implementationType == null)
            {
                Logger.Warning(
                    "⚠️ Interface {Interface} encontrada mas implementação {Implementation} não existe",
                    interfaceType.Name, implementationName);
                continue;
            }

            // Registra o serviço com HttpClient tipado
            RegisterHttpClientService(services, interfaceType, implementationType, apiSettings);
            registeredCount++;

            Logger.Debug(
                "✅ CrudTool Service: {Interface} -> {Implementation}",
                interfaceType.Name, implementationType.Name);
        }

        if (registeredCount > 0)
        {
            Logger.Information(
                "🔧 {Count} serviço(s) CrudTool registrado(s) automaticamente",
                registeredCount);
        }
    }

    /// <summary>
    /// Registra um serviço HTTP tipado com todas as políticas de resiliência.
    /// </summary>
    private static void RegisterHttpClientService(
        IServiceCollection services,
        Type interfaceType,
        Type implementationType,
        ApiSettings apiSettings)
    {
        // Usa reflection para chamar AddHttpClient<TInterface, TImplementation>
        var method = typeof(HttpClientFactoryServiceCollectionExtensions)
            .GetMethods()
            .First(m => m.Name == "AddHttpClient"
                        && m.IsGenericMethod
                        && m.GetGenericArguments().Length == 2
                        && m.GetParameters().Length == 2
                        && m.GetParameters()[1].ParameterType == typeof(Action<HttpClient>));

        var genericMethod = method.MakeGenericMethod(interfaceType, implementationType);

        Action<HttpClient> configureClient = client =>
        {
            ConfigureHttpClient(client, apiSettings, apiSettings.TimeoutSeconds);
        };

        var builder = genericMethod.Invoke(null, [services, configureClient]);

        // Adiciona handler e políticas via reflection
        if (builder is IHttpClientBuilder httpClientBuilder)
        {
            httpClientBuilder
                .ConfigurePrimaryHttpMessageHandler(CreateHttpMessageHandler)
                .AddPolicyHandler((sp, _) => CreateRetryPolicy(apiSettings, sp))
                .AddPolicyHandler((sp, _) => CreateCircuitBreakerPolicy(apiSettings, sp));
        }
    }

    /// <summary>
    /// Verifica se o namespace deve ser excluído do registro automático.
    /// </summary>
    private static bool IsExcludedNamespace(string ns)
    {
        return ExcludedNamespaces.Any(excluded => ns.Contains(excluded));
    }

    /// <summary>
    /// Configura os HttpClients nomeados (para serviços que não usam typed client).
    /// </summary>
    private static void ConfigureNamedHttpClients(
        IServiceCollection services,
        ApiSettings apiSettings)
    {
        // ApiClient - Cliente genérico
        services.AddHttpClient(HttpClientNames.ApiClient, client =>
        {
            ConfigureHttpClient(client, apiSettings, apiSettings.TimeoutSeconds);
        })
            .ConfigurePrimaryHttpMessageHandler(CreateHttpMessageHandler)
            .AddPolicyHandler((sp, _) => CreateRetryPolicy(apiSettings, sp))
            .AddPolicyHandler((sp, _) => CreateCircuitBreakerPolicy(apiSettings, sp));

        // AuthApiClient - Cliente de autenticação (timeout diferente)
        services.AddHttpClient(HttpClientNames.AuthApiClient, client =>
        {
            ConfigureHttpClient(client, apiSettings, apiSettings.AuthTimeoutSeconds);
        })
            .ConfigurePrimaryHttpMessageHandler(CreateHttpMessageHandler)
            .AddPolicyHandler((sp, _) => CreateRetryPolicy(apiSettings, sp, maxRetries: 2))
            .AddPolicyHandler((sp, _) => CreateCircuitBreakerPolicy(apiSettings, sp));
    }

    #region Configuração e Validação

    /// <summary>
    /// Configura e valida as ApiSettings.
    /// </summary>
    private static ApiSettings ConfigureAndValidateApiSettings(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<ApiSettings>()
            .Bind(configuration.GetSection("ApiSettings"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>();

        if (apiSettings is null || string.IsNullOrWhiteSpace(apiSettings.BaseUrl))
        {
            throw new InvalidOperationException(
                "ApiSettings não configurado. Verifique a seção 'ApiSettings' no appsettings.json");
        }

        return apiSettings;
    }

    #endregion

    #region HttpClient Configuration

    /// <summary>
    /// Configura um HttpClient com as configurações padrão.
    /// </summary>
    private static void ConfigureHttpClient(HttpClient client, ApiSettings settings, int timeoutSeconds)
    {
        client.BaseAddress = new Uri(settings.BaseUrl.TrimEnd('/') + "/");
        client.Timeout = TimeSpan.FromSeconds(timeoutSeconds + 5); // Margem para Polly
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("User-Agent", settings.UserAgent);
    }

    /// <summary>
    /// Cria o handler HTTP com connection pooling otimizado.
    /// </summary>
    private static HttpMessageHandler CreateHttpMessageHandler()
    {
        return new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
            MaxConnectionsPerServer = 100,
            EnableMultipleHttp2Connections = true
        };
    }

    #endregion

    #region Polly Policies

    /// <summary>
    /// Cria política de retry com exponential backoff e jitter.
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(
        ApiSettings settings,
        IServiceProvider sp,
        int? maxRetries = null)
    {
        var retries = maxRetries ?? settings.RetryCount;
        var logger = sp.GetService<ILoggerFactory>()?.CreateLogger("Polly.Retry");

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retries,
                retryAttempt =>
                {
                    var baseDelay = TimeSpan.FromSeconds(
                        Math.Pow(2, retryAttempt) * settings.RetryBaseDelaySeconds);
                    var jitter = TimeSpan.FromMilliseconds(Random.Shared.Next(0, 1000));
                    return baseDelay + jitter;
                },
                onRetry: (outcome, timespan, retryAttempt, _) =>
                {
                    if (settings.EnableDetailedLogging)
                    {
                        logger?.LogWarning(
                            "🔄 Retry {Attempt}/{Max} após {Delay}ms | Status: {Status} | Erro: {Error}",
                            retryAttempt,
                            retries,
                            timespan.TotalMilliseconds,
                            outcome.Result?.StatusCode,
                            outcome.Exception?.Message ?? "N/A");
                    }
                });
    }

    /// <summary>
    /// Cria política de circuit breaker.
    /// </summary>
    private static IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy(
        ApiSettings settings,
        IServiceProvider sp)
    {
        var logger = sp.GetService<ILoggerFactory>()?.CreateLogger("Polly.CircuitBreaker");

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: settings.CircuitBreakerThreshold,
                durationOfBreak: TimeSpan.FromSeconds(settings.CircuitBreakerDurationSeconds),
                onBreak: (outcome, breakDelay) =>
                {
                    logger?.LogError(
                        "🔴 Circuit ABERTO por {Duration}s | Razão: {Reason}",
                        breakDelay.TotalSeconds,
                        outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                },
                onReset: () =>
                {
                    logger?.LogInformation("🟢 Circuit FECHADO - Conexões restabelecidas");
                },
                onHalfOpen: () =>
                {
                    logger?.LogInformation("🟡 Circuit HALF-OPEN - Testando conexão...");
                });
    }

    #endregion
}

/// <summary>
/// Constantes para nomes de HttpClients.
/// </summary>
public static class HttpClientNames
{
    public const string ApiClient = "ApiClient";
    public const string AuthApiClient = "AuthApiClient";
}