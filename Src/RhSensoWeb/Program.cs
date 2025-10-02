using RhSensoWeb.Extensions;
using Serilog;

// ========================================
// CONFIGURAR SERILOG ANTES DE CRIAR O HOST
// ========================================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "RhSensoWeb")
    .Enrich.WithMachineName()  // ✅ AGORA FUNCIONA
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("========================================");
    Log.Information("🚀 Iniciando RhSensoWeb...");
    Log.Information("🌐 Ambiente: {Environment}", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production");
    Log.Information("========================================");

    var builder = WebApplication.CreateBuilder(args);

    // ========================================
    // CONFIGURAR SERILOG NO HOST
    // ========================================
    builder.Host.UseSerilog();

    // ========================================
    // CONFIGURAÇÃO DE SERVIÇOS
    // ========================================

    // 1️⃣ Adicionar Controllers com Views
    builder.Services.AddControllersWithViews();

    // 2️⃣ Adicionar Session
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromHours(8);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.Name = "RhSensoWeb.Session";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

    // 3️⃣ Adicionar Autenticação ✅ CRÍTICO
    builder.Services.AddCustomAuthentication(builder.Configuration);

    // 4️⃣ Adicionar Serviços da Aplicação ✅ CRÍTICO
    builder.Services.AddApplicationServices(builder.Configuration);

    // 5️⃣ Adicionar CORS
    builder.Services.AddCustomCors();

    // 6️⃣ Adicionar Compressão
    builder.Services.AddCustomCompression();

    // 7️⃣ Adicionar Cache
    builder.Services.AddCustomCaching();

    // 8️⃣ Adicionar Health Checks
    builder.Services.AddCustomHealthChecks(builder.Configuration);

    // 9️⃣ Adicionar AntiForgery
    builder.Services.AddAntiforgery(options =>
    {
        options.HeaderName = "X-CSRF-TOKEN";
        options.Cookie.Name = "RhSensoWeb.AntiForgery";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

    var app = builder.Build();

    // ========================================
    // CONFIGURAÇÃO DO PIPELINE DE MIDDLEWARE
    // ========================================

    // 1️⃣ Logging estruturado
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} respondeu {StatusCode} em {Elapsed:0.0000}ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());
        };
    });

    // 2️⃣ Tratamento de erros
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
        app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    // 3️⃣ Segurança
    app.UseHttpsRedirection();

    // 4️⃣ Compressão
    app.UseResponseCompression();

    // 5️⃣ Arquivos estáticos
    app.UseStaticFiles();

    // 6️⃣ Roteamento
    app.UseRouting();

    // 7️⃣ CORS (se necessário)
    if (app.Environment.IsDevelopment())
    {
        app.UseCors("DevelopmentPolicy");
    }

    // 8️⃣ Sessão ✅ ANTES DE AUTENTICAÇÃO
    app.UseSession();

    // 9️⃣ Autenticação
    app.UseAuthentication();

    // 🔟 Autorização
    app.UseAuthorization();

    // 1️⃣1️⃣ Health Checks
    app.MapHealthChecks("/health");

    // 1️⃣2️⃣ Rotas das Áreas
    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    // 1️⃣3️⃣ Rota padrão
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // ========================================
    // INICIALIZAÇÃO
    // ========================================
    Log.Information("✅ Aplicação iniciada com sucesso!");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ ERRO FATAL ao iniciar a aplicação");
    throw;
}
finally
{
    Log.Information("🛑 Encerrando RhSensoWeb...");
    Log.CloseAndFlush();
}