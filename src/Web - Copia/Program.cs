// =============================================================================
// RHSENSOERP WEB - PROGRAM.CS
// =============================================================================
// Arquivo: Program.cs
// Descrição: Ponto de entrada da aplicação Web ASP.NET Core 8
// Versão: 4.2 (Menu Dinâmico + GUID Routes + Favoritos)
// =============================================================================

using Microsoft.AspNetCore.Authentication.Cookies;
using RhSensoERP.Web.Extensions;
using RhSensoERP.Web.Filters;
using RhSensoERP.Web.Routing;                    // [NOVO] - GuidRouteTransformer, IScreenRouteRegistry
using RhSensoERP.Web.Security;                   // [NOVO] - IScreenLinkService
using RhSensoERP.Web.Services.Favorites;         // [NOVO] - IFavoritesService
using RhSensoERP.Web.Services.Menu;              // Menu dinâmico
using Serilog;

namespace RhSensoERP.Web;

/// <summary>
/// Classe principal da aplicação Web RhSensoERP.
/// </summary>
public static class Program
{
    /// <summary>
    /// Ponto de entrada da aplicação.
    /// </summary>
    public static async Task Main(string[] args)
    {
        // =====================================================================
        // CONFIGURAÇÃO INICIAL DO SERILOG (Bootstrap)
        // =====================================================================
        // Logger mínimo para capturar erros durante a inicialização
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Iniciando RhSensoERP.Web...");

            var builder = WebApplication.CreateBuilder(args);

            // =================================================================
            // CONFIGURAÇÃO DO SERILOG (Lê do appsettings.json)
            // =================================================================
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", "RhSensoERP.Web")
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);
            });

            // =================================================================
            // REGISTRO DE SERVIÇOS
            // =================================================================
            ConfigureServices(builder.Services, builder.Configuration);

            // =================================================================
            // BUILD DA APLICAÇÃO
            // =================================================================
            var app = builder.Build();

            // =================================================================
            // CONFIGURAÇÃO DO PIPELINE HTTP
            // =================================================================
            ConfigurePipeline(app);

            // =================================================================
            // EXECUÇÃO
            // =================================================================
            Log.Information("Ambiente: {Environment}", app.Environment.EnvironmentName);
            Log.Information("URLs: {Urls}", string.Join(", ", app.Urls.DefaultIfEmpty("Não configuradas")));

            await app.RunAsync().ConfigureAwait(false);
        }
        catch (HostAbortedException)
        {
            // Ignorar - ocorre durante migrations do EF Core
            Log.Information("Host abortado (provavelmente durante EF migrations)");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Erro fatal ao iniciar a aplicação");
            throw;
        }
        finally
        {
            Log.Information("Encerrando RhSensoERP.Web...");
            await Log.CloseAndFlushAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Configura todos os serviços da aplicação.
    /// </summary>
    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // =====================================================================
        // MVC + FILTROS GLOBAIS
        // =====================================================================
        services.AddControllersWithViews(options =>
        {
            options.Filters.Add<GlobalExceptionFilter>();
            options.Filters.Add<ValidateModelStateFilter>();
        });

        // =====================================================================
        // SERVIÇOS DE API (HttpClients + Polly)
        // =====================================================================
        services.AddApiServices(configuration);

        // =====================================================================
        // CACHE DE PERMISSÕES (usar método existente do projeto)
        // =====================================================================
        services.AddPermissionsCaching(configuration.GetValue("PermissionsCache:MaxSize", 1000));

        // =====================================================================
        // MENU DINÂMICO + ROTAS GUID + FAVORITOS
        // ---------------------------------------------------------------------
        // Infraestrutura completa para:
        //   * Menu dinâmico com descoberta automática de controllers [MenuItem]
        //   * URLs mascaradas com GUID (/go/{guid}) - muda a cada restart
        //   * Links permanentes criptografados (/s/{token}) - DataProtection
        //   * Sistema de favoritos persistido no banco (WebFavorite)
        // =====================================================================

        // HttpContextAccessor - necessário para obter usuário logado
        services.AddHttpContextAccessor();

        // MemoryCache - usado por permissões e menu
        services.AddMemoryCache();

        // DataProtection - necessário para ScreenLinkService (tokens /s/{token})
        services.AddDataProtection();

        // Menu Discovery - descobre controllers [MenuItem] e monta menu
        services.AddScoped<IMenuDiscoveryService, MenuDiscoveryService>();

        // Screen Route Registry - mapeia ScreenKey <-> GUID (em memória, por execução)
        services.AddSingleton<IScreenRouteRegistry, ScreenRouteRegistry>();

        // GUID Route Transformer - resolve /go/{guid} para rotas reais
        services.AddScoped<GuidRouteTransformer>();

        // Screen Link Service - gera/resolve tokens criptografados (/s/{token})
        services.AddScoped<IScreenLinkService, ScreenLinkService>();

        // Favorites Service - CRUD de favoritos no banco (tabela WebFavorite)
        services.AddScoped<IFavoritesService, FavoritesService>();

        // =====================================================================
        // AUTENTICAÇÃO (Cookie)
        // =====================================================================
        ConfigureAuthentication(services, configuration);

        // =====================================================================
        // SESSÃO
        // =====================================================================
        ConfigureSession(services, configuration);
    }

    /// <summary>
    /// Configura autenticação por cookie.
    /// </summary>
    private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var authSection = configuration.GetSection("Authentication");

        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = authSection["LoginPath"] ?? "/Account/Login";
                options.LogoutPath = authSection["LogoutPath"] ?? "/Account/Logout";
                options.AccessDeniedPath = authSection["AccessDeniedPath"] ?? "/Account/AccessDenied";

                options.ExpireTimeSpan = TimeSpan.FromMinutes(
                    authSection.GetValue("ExpireTimeSpan", 480));

                options.SlidingExpiration = authSection.GetValue("SlidingExpiration", true);

                options.Cookie.Name = authSection["CookieName"] ?? "RhSensoERP.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;

                options.Events.OnRedirectToLogin = context =>
                {
                    // Para requisições AJAX, retorna 401 em vez de redirect
                    if (context.Request.Headers.XRequestedWith == "XMLHttpRequest")
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    }

                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToAccessDenied = context =>
                {
                    // Para requisições AJAX, retorna 403 em vez de redirect
                    if (context.Request.Headers.XRequestedWith == "XMLHttpRequest")
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    }

                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            });

        services.AddAuthorization();
    }

    /// <summary>
    /// Configura sessão HTTP.
    /// </summary>
    private static void ConfigureSession(IServiceCollection services, IConfiguration configuration)
    {
        var sessionSection = configuration.GetSection("Session");

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(
                sessionSection.GetValue("IdleTimeoutMinutes", 30));

            options.Cookie.Name = sessionSection["CookieName"] ?? "RhSensoERP.Session";
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
    }

    /// <summary>
    /// Configura o pipeline de requisições HTTP.
    /// </summary>
    private static void ConfigurePipeline(WebApplication app)
    {
        // =====================================================================
        // TRATAMENTO DE ERROS
        // =====================================================================
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        // =====================================================================
        // MIDDLEWARES DE INFRAESTRUTURA
        // =====================================================================
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        // =====================================================================
        // SERILOG REQUEST LOGGING
        // =====================================================================
        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());

                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value ?? "N/A");
                }
            };
        });

        // =====================================================================
        // SESSÃO, AUTENTICAÇÃO E AUTORIZAÇÃO
        // =====================================================================
        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();

        // =====================================================================
        // ROTAS
        // =====================================================================

        // [NOVO] Rota dinâmica /go/{guid} - resolve GUID para Area/Controller/Action
        // O GUID é gerado por execução da aplicação e mapeado via IScreenRouteRegistry
        app.MapDynamicControllerRoute<GuidRouteTransformer>("go/{guid:guid}");

        // Rota para Areas (ex: /SEG/Usuario/Index)
        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        // Rota padrão
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    }
}