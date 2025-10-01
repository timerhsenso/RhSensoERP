using RhSensoWeb.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// CONFIGURAÇÃO DE SERVIÇOS
// ========================================

// Adicionar todos os serviços da aplicação
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// ========================================
// CONFIGURAÇÃO DO PIPELINE
// ========================================

// Logging
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

try
{
    Log.Information("Iniciando RhSensoWeb...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro fatal ao iniciar a aplicação");
}
finally
{
    Log.CloseAndFlush();
}
