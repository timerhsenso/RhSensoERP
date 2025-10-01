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
    .CreateLogger();

try
{
    Log.Information("Iniciando RhSensoWeb...");

    var builder = WebApplication.CreateBuilder(args);

    // ========================================
    // CONFIGURAR SERILOG NO HOST
    // ========================================
    builder.Host.UseSerilog();

    // ========================================
    // CONFIGURAÇÃO DE SERVIÇOS
    // ========================================

    // Adicionar Controllers com Views
    builder.Services.AddControllersWithViews();

    // Adicionar Session
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromHours(8);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    // Adicionar todos os serviços da aplicação
    builder.Services.AddApplicationServices(builder.Configuration);
    builder.Services.AddCustomAuthentication(builder.Configuration);
    builder.Services.AddCustomCors();
    builder.Services.AddCustomCompression();
    builder.Services.AddCustomCaching();
    builder.Services.AddCustomHealthChecks(builder.Configuration);

    var app = builder.Build();

    // ========================================
    // CONFIGURAÇÃO DO PIPELINE
    // ========================================

    // Logging com Serilog
    app.UseSerilogRequestLogging();

    // Tratamento de erros
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    // Segurança
    app.UseHttpsRedirection();

    // Compressão
    app.UseResponseCompression();

    // Arquivos estáticos
    app.UseStaticFiles();

    // Roteamento
    app.UseRouting();

    // Sessão (antes de autenticação)
    app.UseSession();

    // Autenticação e autorização
    app.UseAuthentication();
    app.UseAuthorization();

    // Health checks
    app.MapHealthChecks("/health");

    // Rotas das áreas
    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    // Rota padrão
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // ========================================
    // INICIALIZAÇÃO
    // ========================================
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro fatal ao iniciar a aplicação");
    throw;
}
finally
{
    Log.CloseAndFlush();
}