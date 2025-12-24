// ============================================================================
// ARQUIVO: src/API/Middleware/TenantMiddleware.cs
// AÇÃO: SUBSTITUIR COMPLETAMENTE O ARQUIVO EXISTENTE
// ============================================================================

using Microsoft.EntityFrameworkCore;
using RhSensoERP.Identity.Infrastructure.Persistence.Contexts;

namespace RhSensoERP.API.Middleware;

/// <summary>
/// Middleware que resolve o tenant atual baseado no hostname da requisição.
/// Armazena o TenantId no HttpContext.Items para uso posterior.
/// </summary>
public sealed class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IdentityDbContext dbContext)
    {
        try
        {
            // Obter hostname da requisição
            var hostname = context.Request.Host.Host;

            _logger.LogDebug("🔍 TenantMiddleware: Resolvendo tenant para hostname: {Hostname}", hostname);

            // Buscar tenant por domínio
            var tenant = await ResolverTenantPorDominioAsync(dbContext, hostname);

            if (tenant != null)
            {
                // Armazenar TenantId no contexto da requisição
                context.Items["TenantId"] = tenant.Id.ToString();
                context.Items["TenantDomain"] = tenant.Domain;

                _logger.LogInformation(
                    "✅ TenantMiddleware: Tenant resolvido - Id: {TenantId}, Domain: {Domain}",
                    tenant.Id,
                    tenant.Domain);
            }
            else
            {
                // Tenant não encontrado - pode ser ambiente local ou tenant padrão
                _logger.LogWarning(
                    "⚠️ TenantMiddleware: Tenant não encontrado para hostname: {Hostname}. Usando tenant padrão.",
                    hostname);

                // Estratégia: usar tenant padrão ou permitir acesso sem tenant
                // Você pode definir um TenantId padrão aqui se necessário
                // context.Items["TenantId"] = "default-tenant-id";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ TenantMiddleware: Erro ao resolver tenant");

            // Não bloquear a requisição - permitir continuar sem tenant
            // Em produção, você pode querer retornar um erro 500 ou 503
        }

        // Continuar o pipeline
        await _next(context);
    }

    /// <summary>
    /// Resolve o tenant por domínio consultando a tabela SaasTenants.
    /// </summary>
    private async Task<TenantInfo?> ResolverTenantPorDominioAsync(
        IdentityDbContext dbContext,
        string hostname)
    {
        try
        {
            // Verificar se a tabela SaasTenants existe
            // Nota: Você precisa criar a entidade SaasTenants se ainda não existir

            // Exemplo de query SQL direta (caso a entidade não exista ainda)
            var query = @"
                SELECT TOP 1 
                    Id, 
                    Domain, 
                    CompanyName 
                FROM dbo.SaasTenants 
                WHERE Domain = @p0 
                  AND IsActive = 1";

            var result = await dbContext.Database
                .SqlQueryRaw<TenantInfo>(query, hostname)
                .FirstOrDefaultAsync();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "⚠️ TenantMiddleware: Erro ao consultar SaasTenants. Tabela pode não existir ainda.");

            // Retornar null - tenant não encontrado
            return null;
        }
    }
}

/// <summary>
/// DTO para informações básicas do tenant.
/// </summary>
public sealed class TenantInfo
{
    public Guid Id { get; set; }
    public string Domain { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
}

/// <summary>
/// Extension method para registrar o TenantMiddleware.
/// </summary>
public static class TenantMiddlewareExtensions
{
    /// <summary>
    /// Adiciona o TenantMiddleware ao pipeline de requisições.
    /// Deve ser chamado ANTES da autenticação.
    /// </summary>
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TenantMiddleware>();
    }
}
