// Program.cs - RhSensoERP API
// --------------------------------------------------------------------------------------
// Arquivo principal de configuração da API ASP.NET Core 8
// Configurado para trabalhar com sistema legacy existente (tabelas tuse1, usrh1, etc.)
// Implementa Clean Architecture com autenticação JWT baseada em permissões granulares
// --------------------------------------------------------------------------------------

using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
// Configurações customizadas
using RhSensoERP.API.Configuration;
using RhSensoERP.API.Middlewares;
using RhSensoERP.Application.Common.Interfaces;
using RhSensoERP.Application.Security.Auth.Validators;
using RhSensoERP.Core.Abstractions.Interfaces;
using RhSensoERP.Infrastructure.Logging;
using RhSensoERP.Infrastructure.Persistence;
using RhSensoERP.Infrastructure.Persistence.Interceptors;
using RhSensoERP.Infrastructure.Repositories;
using RhSensoERP.Infrastructure.Services;
using Serilog;
using System.Threading.RateLimiting;

// Configuração inicial com Serilog
var builder = WebApplication.CreateBuilder(args)
    .AddSerilogLogging(); // Adiciona logging estruturado

// Atalho para configuração
var cfg = builder.Configuration;

// --------------------------------------------------------------------------------------
// CONFIGURAÇÃO DE SERVIÇOS (DEPENDENCY INJECTION)
// --------------------------------------------------------------------------------------

#region Controladores e API

/// <summary>
/// Configuração de controllers MVC para endpoints da API
/// </summary>
builder.Services.AddControllers();

/// <summary>
/// Configuração de controllers MVC para endpoints da API
/// </summary>
builder.Services.AddControllers();

/// <summary>
/// Validação automática com FluentValidation
/// Registra todos os validadores do assembly da Application
/// </summary>
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

/// <summary>
/// Habilita descoberta automática de endpoints para OpenAPI/Swagger
/// </summary>
builder.Services.AddEndpointsApiExplorer();

/// <summary>
/// Configuração do Swagger com autenticação JWT
/// </summary>
builder.Services.AddSwaggerDocs();

#endregion

#region CORS

/// <summary>
/// Política de CORS para permitir acesso de origens específicas
/// Configurações no appsettings.json em Cors:AllowedOrigins
/// </summary>
builder.Services.AddDefaultCors(cfg);

#endregion

#region Banco de Dados

/// <summary>
/// Acesso ao HttpContext para auditoria e obtenção do usuário atual
/// </summary>
builder.Services.AddHttpContextAccessor();

/// <summary>
/// Interceptor para auditoria automática de criação e atualização de registros
/// </summary>
builder.Services.AddScoped<AuditSaveChangesInterceptor>();

/// <summary>
/// Contexto do Entity Framework Core configurado para SQL Server
/// - NoTracking por padrão para melhor performance em consultas
/// - Connection string configurada via User Secrets em desenvolvimento
/// </summary>
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(cfg.GetConnectionString("Default"))
       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

#endregion

#region Autenticação e Autorização

/// <summary>
/// Configuração de autenticação JWT com suporte a:
/// - Chaves simétricas para desenvolvimento
/// - Chaves RSA para produção
/// - Políticas dinâmicas de autorização baseadas em permissões
/// </summary>
builder.Services.AddJwtAuth(cfg);

#endregion

#region Rate Limiting

/// <summary>
/// Configuração de Rate Limiting para proteger contra ataques de força bruta
/// </summary>
builder.Services.AddRateLimiter(options =>
{
    // Política global - 100 requests por minuto por IP
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 10
            }));

    // Política específica para login - 5 tentativas por 15 minutos por IP
    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(15),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0 // Sem fila para login - rejeita imediatamente
            }));

    // Resposta quando rate limit é atingido
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;

        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                ((int)retryAfter.TotalSeconds).ToString();
        }

        await context.HttpContext.Response.WriteAsync("""
            {
                "success": false,
                "message": "Muitas tentativas. Tente novamente mais tarde.",
                "data": null
            }
            """, token);
    };
});

#endregion

#region Repositórios e Unidade de Trabalho

/// <summary>
/// Repositório genérico para operações CRUD básicas
/// Implementa padrão Repository com EF Core
/// </summary>
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

/// <summary>
/// Unidade de trabalho para controle transacional
/// </summary>
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

/// <summary>
/// Serviço para obter dados do usuário atual logado
/// Extrai informações do JWT (claims, tenant, etc.)
/// </summary>
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

#endregion

#region Serviços de Autenticação Legacy

/// <summary>
/// Serviço de autenticação que trabalha com o sistema legacy
/// - Validação contra tabela tuse1 (usuários)
/// - Carregamento de permissões via usrh1, hbrh1 (grupos/funções)
/// - Funções de verificação: CheckHabilitacao, CheckBotao, CheckRestricao
/// </summary>
builder.Services.AddScoped<RhSensoERP.Application.Security.Auth.Services.ILegacyAuthService, RhSensoERP.Infrastructure.Services.LegacyAuthService>();

#endregion

#region Middlewares

/// <summary>
/// Middleware global para tratamento de exceções
/// - Captura ValidationException (FluentValidation)
/// - Padroniza respostas de erro em formato JSON
/// - Logs estruturados de erros
/// </summary>
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

/// <summary>
/// Middleware de headers de segurança
/// - Adiciona headers de proteção contra XSS, clickjacking, etc.
/// - Remove headers que revelam informações do servidor
/// </summary>
builder.Services.AddTransient<SecurityHeadersMiddleware>();

#endregion

#region Health Checks

/// <summary>
/// Monitoramento de saúde da aplicação
/// - Self check: verifica se a API está respondendo
/// - Database check: testa conectividade com SQL Server
/// </summary>
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API operacional"))
    .AddCheck<DatabaseHealthCheck>("database");

#endregion

// --------------------------------------------------------------------------------------
// CONSTRUÇÃO DA APLICAÇÃO
// --------------------------------------------------------------------------------------

var app = builder.Build();

// --------------------------------------------------------------------------------------
// CONFIGURAÇÃO DO PIPELINE HTTP (ORDEM É CRÍTICA!)
// --------------------------------------------------------------------------------------

#region Middlewares de Tratamento (Primeira Camada)

/// <summary>
/// Middleware de exceções deve ser o primeiro para capturar todos os erros
/// </summary>
app.UseMiddleware<ExceptionHandlingMiddleware>();

/// <summary>
/// Headers de segurança devem ser aplicados cedo no pipeline
/// - Protege contra XSS, clickjacking, content sniffing
/// - Remove headers que revelam informações do servidor
/// </summary>
app.UseMiddleware<SecurityHeadersMiddleware>();

/// <summary>
/// Logs estruturados de todas as requisições HTTP
/// </summary>
app.UseSerilogRequestLogging();

#endregion

#region CORS

/// <summary>
/// Aplica política de CORS - deve vir antes dos endpoints
/// </summary>
app.UseDefaultCors();

#endregion

#region Autenticação e Autorização

/// <summary>
/// Pipeline de autenticação e autorização
/// Ordem é importante: Authentication antes de Authorization
/// </summary>
app.UseAuthentication();
app.UseAuthorization();

#endregion

#region Rate Limiting

/// <summary>
/// Rate limiting deve vir após autenticação para permitir identificação por usuário
/// - Política global: 100 req/min por IP
/// - Política login: 5 tentativas por 15min por IP
/// </summary>
app.UseRateLimiter();

#endregion

#region Documentação

/// <summary>
/// Swagger UI disponível apenas em ambiente de desenvolvimento
/// Acesso via: https://localhost:57148/swagger
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RhSensoERP API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "RhSensoERP API - Documentação";
    });
}

#endregion

#region Mapeamento de Endpoints

/// <summary>
/// Health check endpoint público
/// GET /health - retorna status da API e conectividade do banco
/// </summary>
app.MapHealthChecks("/health").AllowAnonymous();

/// <summary>
/// Mapeamento automático de todos os controllers
/// Inclui controllers de Auth (/api/v1/auth/*) e Security (/api/v1/security/*)
/// </summary>
app.MapControllers();

#endregion

// --------------------------------------------------------------------------------------
// INICIALIZAÇÃO DA APLICAÇÃO
// --------------------------------------------------------------------------------------

/// <summary>
/// Inicia a aplicação de forma bloqueante
/// API estará disponível em:
/// - HTTPS: https://localhost:57148
/// - HTTP: http://localhost:57149
/// </summary>
app.Run();

// ======================================================================================
// HEALTH CHECK CUSTOMIZADO PARA BANCO DE DADOS
// ======================================================================================

/// <summary>
/// Health check que verifica conectividade com o banco de dados
/// Executa um SELECT 1 simples para validar:
/// - Conectividade de rede
/// - Credenciais válidas
/// - Banco de dados online e acessível
/// </summary>
file sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly AppDbContext _db;

    public DatabaseHealthCheck(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Executa verificação de saúde do banco de dados
    /// </summary>
    /// <param name="context">Contexto da verificação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado indicando se o banco está saudável</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Executa query simples para testar conectividade
            // Se funcionar, o banco está operacional
            await _db.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
            return HealthCheckResult.Healthy("Banco de dados acessível e operacional");
        }
        catch (Exception ex)
        {
            // Qualquer erro indica problema de conectividade/credenciais/disponibilidade
            return HealthCheckResult.Unhealthy("Falha na conectividade com banco de dados", ex);
        }
    }
}