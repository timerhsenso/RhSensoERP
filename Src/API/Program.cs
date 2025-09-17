// Program.cs
// --------------------------------------------------------------------------------------
// Este arquivo configura e inicializa a sua API ASP.NET Core.
// Cada bloco abaixo está comentado para explicar exatamente o que faz.
// --------------------------------------------------------------------------------------

using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Serilog;

// Health checks (básico e custom)
using Microsoft.Extensions.Diagnostics.HealthChecks;

// Suas namespaces (extensões/infra/serviços/etc.)
using RhSensoERP.API.Configuration;                  // Métodos de extensão: AddSerilogLogging, AddSwaggerDocs, AddDefaultCors, UseDefaultCors
using RhSensoERP.API.Middlewares;                    // ExceptionHandlingMiddleware
using RhSensoERP.Application.Auth;                   // AddJwtAuth
using RhSensoERP.Application.Common.Extensions;      // AutoMapperExtensions
using RhSensoERP.Application.Common.Interfaces;      // ICurrentUserService
using RhSensoERP.Application.Security.Users.Services;// IUserService, UserService
using RhSensoERP.Application.Security.Users.Validators; // UserCreateValidator (FluentValidation)
using RhSensoERP.Core.Abstractions.Interfaces;       // IRepository<>, IUnitOfWork
using RhSensoERP.Infrastructure.Logging;             // Configurações Serilog (se houver)
using RhSensoERP.Infrastructure.Persistence;         // AppDbContext
using RhSensoERP.Infrastructure.Persistence.Interceptors; // AuditSaveChangesInterceptor
using RhSensoERP.Infrastructure.Repositories;        // EfRepository<>, UnitOfWork
using RhSensoERP.Infrastructure.Services;            // CurrentUserService

var builder = WebApplication.CreateBuilder(args)
    // Liga Serilog no host para logs estruturados (console/arquivo, etc.)
    .AddSerilogLogging();

// Atalho local para appsettings/environment
var cfg = builder.Configuration;

// --------------------------------------------------------------------------------------
// 1) Serviços (Dependency Injection)
// --------------------------------------------------------------------------------------

// MVC Controllers (descoberta de controllers e model-binding)
builder.Services.AddControllers();

// Habilita descoberta de endpoints (para Swagger/OpenAPI e Minimal APIs)
builder.Services.AddEndpointsApiExplorer();

// Documentação Swagger (extensão sua)
builder.Services.AddSwaggerDocs();

// ----------------------------------
// CORS (extensão sua)
// ----------------------------------
// Adiciona uma política padrão de CORS (origens, métodos e headers permitidos)
// A configuração real fica dentro do método AddDefaultCors (provavelmente lê do appsettings).
builder.Services.AddDefaultCors(cfg);

// ----------------------------------
// DbContext + Acessórios
// ----------------------------------
builder.Services.AddHttpContextAccessor(); // Permite acessar HttpContext em serviços (ex.: pegar usuário logado)

// Interceptor de auditoria (grava CreatedBy/UpdatedAt, etc.)
builder.Services.AddScoped<AuditSaveChangesInterceptor>();

// Registra o EF Core apontando para SQL Server
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(cfg.GetConnectionString("Default"))
       // Por padrão, evita rastreamento (melhora performance em leituras). Em comandos de escrita,
       // você pode sobrescrever para Tracking quando necessário.
       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

// ----------------------------------
// Autenticação/JWT (extensão sua)
// ----------------------------------
builder.Services.AddJwtAuth(cfg);

// ----------------------------------
// DI – Core / Infra
// ----------------------------------
// Repositório genérico (CRUD base) via EF
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

// Unidade de trabalho (commit/transação)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Serviço que expõe o "usuário atual" (claims, Id, etc.)
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// ----------------------------------
// MediatR + Pipeline
// ----------------------------------
// Registra handlers/requests/notifications do MediatR na assembly alvo
builder.Services.AddMediatR(cfg =>
{
    // Descobre handlers a partir de um tipo conhecido (qualquer tipo da sua camada Application serve)
    cfg.RegisterServicesFromAssemblyContaining<RhSensoERP.Application.Security.Users.Queries.GetUserByIdQuery>();

    // Adiciona comportamento de validação (FluentValidation) no pipeline dos requests do MediatR
    cfg.AddOpenBehavior(typeof(RhSensoERP.Application.Common.Behaviors.ValidationBehavior<,>));
});

// ----------------------------------
// FluentValidation
// ----------------------------------
// Scaneia validators na assembly onde está o UserCreateValidator
builder.Services.AddValidatorsFromAssemblyContaining<UserCreateValidator>();

// ----------------------------------
// AutoMapper
// ----------------------------------
// Configura AutoMapper com todos os perfis de mapeamento
builder.Services.AddAutoMapperProfiles();

// ----------------------------------
// Serviços (Application)
// ----------------------------------
builder.Services.AddScoped<IUserService, UserService>();

// ----------------------------------
// Middlewares customizados
// ----------------------------------
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

// ----------------------------------
// Health Checks
// ----------------------------------
// 1) Health check "self" (sempre OK se a pipeline subir).
// 2) Health check de banco usando o próprio AppDbContext (sem pacotes de terceiros).
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API up"))
    .AddCheck<DatabaseHealthCheck>("database");

// --------------------------------------------------------------------------------------
// 2) Build do app e Pipeline HTTP
// --------------------------------------------------------------------------------------
var app = builder.Build();

// Middleware global de tratamento de exceção (captura erros, retorna 500 com JSON padronizado)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Logs de requisição Serilog (método de extensão do Serilog.AspNetCore)
app.UseSerilogRequestLogging();

// Aplica política CORS padrão (deve vir antes dos endpoints)
app.UseDefaultCors();

// Autenticação e Autorização
app.UseAuthentication();
app.UseAuthorization();

// Swagger apenas em Development (evita exibir em produção, a menos que deseje)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ----------------------------------
// Mapas de endpoints
// ----------------------------------

// Endpoint de Health Check:
// - /health -> retorna 200 (Healthy) se API e DB estiverem OK.
// - .AllowAnonymous() garante acesso público mesmo com autenticação global.
app.MapHealthChecks("/health").AllowAnonymous();

// Controllers (rotas de API tradicionais)
app.MapControllers();

// Sobe a aplicação (bloqueante)
app.Run();


// ======================================================================================
// HEALTH CHECK CUSTOM (DB) — Implementação simples SEM pacotes externos
// Tenta executar um "SELECT 1" no banco. Se funcionar, considera "Healthy".
// ======================================================================================
file sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly AppDbContext _db;

    public DatabaseHealthCheck(AppDbContext db)
    {
        _db = db;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Usa o provedor EF Core para executar um comando simples.
            // É leve e suficiente para verificar conectividade/credenciais/DB online.
            await _db.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
            return HealthCheckResult.Healthy("Database reachable");
        }
        catch (Exception ex)
        {
            // Qualquer erro de conexão/timeouts/credenciais cai aqui.
            return HealthCheckResult.Unhealthy("Database unreachable", ex);
        }
    }
}
