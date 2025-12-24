// ============================================================================
// RHSENSOERP API - PROGRAM.CS
// ============================================================================
// Arquivo: src/API/Program.cs
// Projeto: RhSensoERP - Sistema de Gestão de Recursos Humanos
// Versão: 1.0.3
// Última atualização: Dezembro 2025
//
// DESCRIÇÃO:
// Ponto de entrada da aplicação ASP.NET Core Web API.
// Configura toda a infraestrutura, middlewares, serviços e pipeline HTTP.
//
// MÓDULOS REGISTRADOS:
// - Identity (Autenticação/Autorização)
// - GestaoDePessoas (RHU - Colaboradores, Cargos, Departamentos)
// - ControleDePonto (Frequência, Marcações, Escalas)
// - Esocial (Eventos eSocial)
// - Avaliacoes (Avaliações de Desempenho)
// - SaudeOcupacional (PCMSO, ASO, Exames)
// - Treinamentos (Capacitações, Certificados)
// - AuditoriaCompliance (Logs, Conformidade)
// - ControleAcessoPortaria (Catracas, Visitantes)
// - GestaoDeTerceiros (Prestadores, Contratos)
//
// ARQUITETURA:
// - Modular: cada módulo é isolado com seu próprio DbContext
// - Clean Architecture: separação clara entre camadas
// - SourceGenerator: controllers CRUD gerados automaticamente
// - CQRS: Commands e Queries via MediatR
// ============================================================================

#region Usings

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RhSensoERP.API.BackgroundServices;
using RhSensoERP.API.Configuration;
using RhSensoERP.API.Extensions;
using RhSensoERP.API.Middleware;
using RhSensoERP.Identity;
using RhSensoERP.Identity.Application;
using RhSensoERP.Identity.Application.Configuration;
using RhSensoERP.Identity.Application.Services;
using RhSensoERP.Identity.Infrastructure;
// ===== Módulos de Negócio =====
using RhSensoERP.Modules.AdministracaoPessoal;
using RhSensoERP.Modules.CargosSalariosRemuneracao;
using RhSensoERP.Modules.ComplianceTrabalhistaJuridico;
using RhSensoERP.Modules.FolhaPagamentoEncargos;
using RhSensoERP.Modules.GestaoBeneficios;
using RhSensoERP.Modules.GestaoJornadaPonto;
using RhSensoERP.Modules.GestaoPortariaAcesso;
using RhSensoERP.Modules.GestaoTalentosDesempenho;
using RhSensoERP.Modules.GestaoTerceirosPrestadores;
using RhSensoERP.Modules.IntegracoesMensageria;
using RhSensoERP.Modules.PeopleAnalyticsBI;
using RhSensoERP.Modules.PortalColaborador;
using RhSensoERP.Modules.RecrutamentoSelecao;
using RhSensoERP.Modules.SaudeSegurancaTrabalho;
using RhSensoERP.Modules.TreinamentoDesenvolvimento;
using RhSensoERP.Modules.MultiTenant;
using RhSensoERP.Modules.ViagensDespesas;
using RhSensoERP.Shared.Core.Abstractions;
using RhSensoERP.Shared.Infrastructure;
using RhSensoERP.Shared.Infrastructure.Services;
using Serilog;
using System.Reflection;
using System.Text;


#endregion

// ============================================================================
// INICIALIZAÇÃO DO BUILDER
// ============================================================================
var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// 1. CONFIGURAÇÃO DO SERILOG (LOGGING ESTRUTURADO)
// ============================================================================
// Serilog oferece logging estruturado com múltiplos sinks (Console, File).
// Configuração lida do appsettings.json permite ajustar níveis por namespace.
// ============================================================================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

Log.Information("🚀 Iniciando aplicação RhSensoERP API v1.0.3");
Log.Information("⚙️ Ambiente: {Environment}", builder.Environment.EnvironmentName);

// ============================================================================
// 2. CARREGAMENTO DE CONFIGURAÇÕES TIPADAS (OPTIONS PATTERN)
// ============================================================================
// Options Pattern permite injetar configurações tipadas via IOptions<T>.
// Benefícios: type-safety, validação, intellisense, testabilidade.
// ============================================================================
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
builder.Services.Configure<SecurityPolicySettings>(builder.Configuration.GetSection("SecurityPolicy"));
builder.Services.Configure<RateLimitSettings>(builder.Configuration.GetSection("RateLimit"));

var rateLimitConfig = builder.Configuration.GetSection("RateLimit");
if (!rateLimitConfig.Exists())
{
    Log.Warning("⚠️ Seção 'RateLimit' não encontrada no appsettings.json. Usando valores default.");
}
else
{
    Log.Information("✅ Configuração de Rate Limiting carregada");
}

// ============================================================================
// 3. INFRAESTRUTURA COMPARTILHADA
// ============================================================================
// Serviços base usados por todos os módulos: Audit, UnitOfWork, TenantContext.
// ============================================================================
builder.Services.AddSharedInfrastructure();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, TenantContext>();

// ============================================================================
// 4. MÓDULO IDENTITY (AUTENTICAÇÃO/AUTORIZAÇÃO)
// ============================================================================
// Gerencia usuários, grupos, permissões, tokens JWT e refresh tokens.
// ============================================================================
//builder.Services.AddIdentityInfrastructure(builder.Configuration);
//builder.Services.AddIdentityApplication();

builder.Services.AddIdentityModule(builder.Configuration);
Log.Information("✅ Módulo Identity registrado");

// ============================================================================
// 5. MÓDULOS DE NEGÓCIO
// ============================================================================
// Cada módulo registra seu próprio DbContext, Repositórios, AutoMapper e MediatR.
// Os controllers são gerados automaticamente via SourceGenerator.
// ============================================================================

// 5.1 Administração de Pessoal (Core HR / RHU)
// Colaboradores, Cargos, Departamentos, Centros de Custo, etc.
builder.Services.AddAdministracaoPessoalModule(builder.Configuration);
Log.Information("✅ Módulo AdministracaoPessoal registrado");

// 5.2 Folha de Pagamento e Encargos
// Cálculos, proventos, descontos, encargos legais.
builder.Services.AddFolhaPagamentoEncargosModule(builder.Configuration);
Log.Information("✅ Módulo FolhaPagamentoEncargos registrado");

// 5.3 Gestão de Jornada e Ponto
// Marcações, Escalas, Jornadas, Banco de Horas.
builder.Services.AddGestaoJornadaPontoModule(builder.Configuration);
Log.Information("✅ Módulo GestaoJornadaPonto registrado");

// 5.4 Gestão de Benefícios
// Vale transporte, refeição, planos, convênios.
builder.Services.AddGestaoBeneficiosModule(builder.Configuration);
Log.Information("✅ Módulo GestaoBeneficios registrado");

// 5.5 Saúde e Segurança do Trabalho (SST)
// PCMSO, ASO, Exames, Atestados, CAT.
builder.Services.AddSaudeSegurancaTrabalhoModule(builder.Configuration);
Log.Information("✅ Módulo SaudeSegurancaTrabalho registrado");

// 5.6 Gestão de Terceiros e Prestadores
// Prestadores de serviço, contratos, documentação obrigatória.
builder.Services.AddGestaoTerceirosPrestadoresModule(builder.Configuration);
Log.Information("✅ Módulo GestaoTerceirosPrestadores registrado");

// 5.7 Recrutamento e Seleção
// Vagas, candidatos, entrevistas, admissões.
builder.Services.AddRecrutamentoSelecaoModule(builder.Configuration);
Log.Information("✅ Módulo RecrutamentoSelecao registrado");

// 5.8 Gestão de Talentos e Desempenho
// Avaliações, competências, metas, feedbacks.
builder.Services.AddGestaoTalentosDesempenhoModule(builder.Configuration);
Log.Information("✅ Módulo GestaoTalentosDesempenho registrado");

// 5.9 Treinamento e Desenvolvimento
// Cursos, certificações, cronogramas, instrutores.
builder.Services.AddTreinamentoDesenvolvimentoModule(builder.Configuration);
Log.Information("✅ Módulo TreinamentoDesenvolvimento registrado");

// 5.10 Cargos, Salários e Remuneração
// Estrutura de cargos, faixas salariais, políticas.
builder.Services.AddCargosSalariosRemuneracaoModule(builder.Configuration);
Log.Information("✅ Módulo CargosSalariosRemuneracao registrado");

// 5.11 Compliance Trabalhista e Jurídico
// Conformidade legal, processos, auditorias.
builder.Services.AddComplianceTrabalhistaJuridicoModule(builder.Configuration);
Log.Information("✅ Módulo ComplianceTrabalhistaJuridico registrado");

// 5.12 Viagens e Despesas
// Reembolsos, adiantamentos, prestações de contas.
builder.Services.AddViagensDespesasModule(builder.Configuration);
Log.Information("✅ Módulo ViagensDespesas registrado");

// 5.13 Portal do Colaborador
// Autoatendimento, solicitações, consultas.
builder.Services.AddPortalColaboradorModule(builder.Configuration);
Log.Information("✅ Módulo PortalColaborador registrado");

// 5.14 People Analytics e BI
// Indicadores, dashboards, análises estratégicas.
builder.Services.AddPeopleAnalyticsBIModule(builder.Configuration);
Log.Information("✅ Módulo PeopleAnalyticsBI registrado");

// 5.15 Integrações e Mensageria
// APIs, filas, eventos, dispositivos externos.
builder.Services.AddIntegracoesMensageriaModule(builder.Configuration);
Log.Information("✅ Módulo IntegracoesMensageria registrado");

// 5.16 Gestão de Portaria e Acesso Físico
// Catracas, visitantes, veículos, crachás.
builder.Services.AddGestaoPortariaAcessoModule(builder.Configuration);
Log.Information("✅ Módulo GestaoPortariaAcesso registrado");

// 5.17 MultiTenant
builder.Services.AddMultiTenantModule(builder.Configuration);
Log.Information("✅ Módulo MultiTenant registrado");


// ============================================================================
// 6. METADATA REGISTRY (UI DINÂMICA)
// ============================================================================
// Registra metadados de entidades para frontend dinâmico.
// Endpoint: GET /api/metadata/{entity}
// ============================================================================
builder.Services.AddEntityMetadata();

// ============================================================================
// 7. CONFIGURAÇÃO DE CONTROLLERS E API EXPLORER
// ============================================================================
// AddControllers com ModuleGroupConvention para agrupar por módulo no Swagger.
// AddApplicationPart para descobrir controllers de outros assemblies.
// ============================================================================
var mvcBuilder = builder.Services.AddControllers(options =>
{
    // Aplica convenção de agrupamento por módulo (namespace → GroupName)
    options.Conventions.Add(new ModuleGroupConvention());
});

// ============================================================================
// 7.1 REGISTRO DE ASSEMBLIES COM CONTROLLERS
// ============================================================================
// O ASP.NET Core não descobre automaticamente controllers de outros assemblies.
// Cada módulo que contém controllers gerados precisa ser registrado aqui.
// ============================================================================

// Assemblies dos módulos (contêm controllers gerados pelo SourceGenerator)
var moduleAssemblies = new[]
{
// Módulo AdministracaoPessoal
typeof(RhSensoERP.Modules.AdministracaoPessoal.AdministracaoPessoalDependencyInjection).Assembly,

// Módulo FolhaPagamentoEncargos
typeof(RhSensoERP.Modules.FolhaPagamentoEncargos.FolhaPagamentoEncargosDependencyInjection).Assembly,

// Módulo GestaoJornadaPonto
typeof(RhSensoERP.Modules.GestaoJornadaPonto.GestaoJornadaPontoDependencyInjection).Assembly,

// Módulo GestaoBeneficios
typeof(RhSensoERP.Modules.GestaoBeneficios.GestaoBeneficiosDependencyInjection).Assembly,

// Módulo SaudeSegurancaTrabalho
typeof(RhSensoERP.Modules.SaudeSegurancaTrabalho.SaudeSegurancaTrabalhoDependencyInjection).Assembly,

// Módulo GestaoTerceirosPrestadores
typeof(RhSensoERP.Modules.GestaoTerceirosPrestadores.GestaoTerceirosPrestadoresDependencyInjection).Assembly,

// Módulo RecrutamentoSelecao
typeof(RhSensoERP.Modules.RecrutamentoSelecao.RecrutamentoSelecaoDependencyInjection).Assembly,

// Módulo GestaoTalentosDesempenho
typeof(RhSensoERP.Modules.GestaoTalentosDesempenho.GestaoTalentosDesempenhoDependencyInjection).Assembly,

// Módulo TreinamentoDesenvolvimento
typeof(RhSensoERP.Modules.TreinamentoDesenvolvimento.TreinamentoDesenvolvimentoDependencyInjection).Assembly,

// Módulo CargosSalariosRemuneracao
typeof(RhSensoERP.Modules.CargosSalariosRemuneracao.CargosSalariosRemuneracaoDependencyInjection).Assembly,

// Módulo ComplianceTrabalhistaJuridico
typeof(RhSensoERP.Modules.ComplianceTrabalhistaJuridico.ComplianceTrabalhistaJuridicoDependencyInjection).Assembly,

// Módulo ViagensDespesas
typeof(RhSensoERP.Modules.ViagensDespesas.ViagensDespesasDependencyInjection).Assembly,

// Módulo PortalColaborador
typeof(RhSensoERP.Modules.PortalColaborador.PortalColaboradorDependencyInjection).Assembly,

// Módulo PeopleAnalyticsBI
typeof(RhSensoERP.Modules.PeopleAnalyticsBI.PeopleAnalyticsBIDependencyInjection).Assembly,

// Módulo IntegracoesMensageria
typeof(RhSensoERP.Modules.IntegracoesMensageria.IntegracoesMensageriaDependencyInjection).Assembly,

// Módulo GestaoPortariaAcesso
typeof(RhSensoERP.Modules.GestaoPortariaAcesso.GestaoPortariaAcessoDependencyInjection).Assembly,

};

foreach (var assembly in moduleAssemblies)
{
    mvcBuilder.AddApplicationPart(assembly);
    Log.Debug("📦 Assembly registrado: {Assembly}", assembly.GetName().Name);
}

// ============================================================================
// 7.2 DESCOBERTA AUTOMÁTICA DE CONTROLLERS (FALLBACK)
// ============================================================================
// Garante que qualquer assembly RhSensoERP com controllers seja descoberto.
// Útil para controllers que não estejam nos assemblies listados acima.
// ============================================================================
var rhSensoAssemblies = AppDomain.CurrentDomain.GetAssemblies()
    .Where(a => !a.IsDynamic &&
                !string.IsNullOrEmpty(a.Location) &&
                (a.GetName().Name?.StartsWith("RhSensoERP") ?? false))
    .ToList();

foreach (var assembly in rhSensoAssemblies)
{
    try
    {
        var hasControllers = assembly.GetTypes()
            .Any(t => t.IsClass &&
                     !t.IsAbstract &&
                     t.Name.EndsWith("Controller") &&
                     t.GetCustomAttributes(typeof(Microsoft.AspNetCore.Mvc.ApiControllerAttribute), true).Any());

        if (hasControllers && !moduleAssemblies.Contains(assembly))
        {
            mvcBuilder.AddApplicationPart(assembly);
            Log.Information("📦 Assembly adicional com controllers: {Assembly}", assembly.GetName().Name);
        }
    }
    catch (ReflectionTypeLoadException)
    {
        // Ignora assemblies que não podem ser carregados completamente
    }
}

mvcBuilder.AddControllersAsServices();
builder.Services.AddEndpointsApiExplorer();

Log.Information("✅ Controllers registrados - {Count} assemblies de módulos + descoberta automática", moduleAssemblies.Length);

// ============================================================================
// 8. CONFIGURAÇÃO DE CORS
// ============================================================================
// CORS necessário para requisições cross-origin (frontend em outro domínio).
// ============================================================================
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

var allOrigins = new List<string>(corsOrigins)
{
    "https://localhost:7193",    
    "http://localhost:5174"
};

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.WithOrigins(allOrigins.ToArray())
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    // 🆕 Política para Manifest (desenvolvimento) - libera TUDO
    options.AddPolicy("ManifestDev", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

});

// ============================================================================
// 9. CONFIGURAÇÃO DE AUTENTICAÇÃO JWT
// ============================================================================
// JWT é o padrão para autenticação stateless em APIs REST.
// ============================================================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
{
    throw new InvalidOperationException(
        "CRITICAL SECURITY ERROR: JwtSettings:SecretKey não configurada!\n\n" +
        "Para configurar:\n" +
        "  - Desenvolvimento: dotnet user-secrets set \"JwtSettings:SecretKey\" \"SUA_CHAVE_AQUI\"\n" +
        "  - Produção: Defina variável de ambiente JwtSettings__SecretKey\n\n" +
        "Gerar chave segura: openssl rand -base64 64");
}

// Validações de segurança em produção
if (builder.Environment.IsProduction())
{
    if (jwtSettings.SecretKey.Length < 64)
    {
        throw new InvalidOperationException(
            $"CRITICAL: Em produção, JwtSettings:SecretKey deve ter no mínimo 64 caracteres! " +
            $"Chave atual tem apenas {jwtSettings.SecretKey.Length} caracteres.");
    }

    var forbiddenTerms = new[] { "Development", "Example", "Test", "Demo", "Sample", "Desenvolvimento" };
    if (forbiddenTerms.Any(term => jwtSettings.SecretKey.Contains(term, StringComparison.OrdinalIgnoreCase)))
    {
        throw new InvalidOperationException(
            "CRITICAL: JwtSettings:SecretKey em produção não pode conter termos genéricos!");
    }

    var connString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (connString?.Contains("Password=123") == true ||
        connString?.Contains("Password=admin") == true)
    {
        throw new InvalidOperationException(
            "CRITICAL: Connection string em produção não pode usar senhas default!");
    }

    Log.Information("✅ Validações de segurança de produção OK");
}

Log.Information("✅ JwtSettings validado");

var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = builder.Environment.IsProduction();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.FromMinutes(jwtSettings.ClockSkewMinutes)
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Append("Token-Expired", "true");
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    error = "UNAUTHORIZED",
                    message = context.ErrorDescription ?? "Não autorizado. Token inválido ou expirado."
                });

                return context.Response.WriteAsync(result);
            }
        };
    });

builder.Services.AddAuthorization();

// ============================================================================
// 10. SERVIÇOS DE SEGURANÇA E AUDITORIA
// ============================================================================
builder.Services.AddScoped<ISecurityAuditService, SecurityAuditService>();
builder.Services.AddHostedService<AuditCleanupBackgroundService>();

// ============================================================================
// 11. CONFIGURAÇÃO DO SWAGGER
// ============================================================================
// Swagger configurado via SwaggerConfiguration.cs com todos os módulos.
// ============================================================================
if (builder.Configuration.GetValue<bool>("Features:EnableSwagger"))
{
    builder.Services.AddSwaggerDocs();
    Log.Information("📘 Swagger habilitado com documentação por módulos");
}

// ============================================================================
// 12. RATE LIMITING
// ============================================================================
builder.Services.AddRateLimiting();

// ============================================================================
// 13. BUILD DA APLICAÇÃO
// ============================================================================
var app = builder.Build();

// ============================================================================
// 14. CONFIGURAÇÃO DO PIPELINE DE MIDDLEWARES
// ============================================================================
// A ordem dos middlewares é crítica para o funcionamento correto.
// ============================================================================

// Exception Handling
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Swagger UI
if (builder.Configuration.GetValue<bool>("Features:EnableSwagger"))
{
    app.UseSwaggerDocs();
}

// HTTPS Redirection
app.UseHttpsRedirection();

// Serilog Request Logging
app.UseSerilogRequestLogging();

// CORS (deve vir ANTES de Authentication)
app.UseCors("DefaultCorsPolicy");

// Rate Limiting
app.UseRateLimiter();

// Tenant Resolution (Multi-Tenancy)
app.UseTenantResolution();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Security Headers
app.UseMiddleware<SecurityHeadersMiddleware>();

// Endpoints
app.MapControllers();

// Health Check
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.3",
    environment = app.Environment.EnvironmentName,
    modules = new[]
    {
        "Identity", "GestaoDePessoas", "ControleDePonto", "Esocial",
        "Avaliacoes", "SaudeOcupacional", "Treinamentos",
        "AuditoriaCompliance", "ControleAcessoPortaria", "GestaoDeTerceiros",
        "GestaoDeEPI", "Integracoes"
    }
})).AllowAnonymous();

// ============================================================================
// 15. INICIALIZAÇÃO E EXECUÇÃO
// ============================================================================
try
{
    Log.Information("═══════════════════════════════════════════════════════════════");
    Log.Information("✅ RhSensoERP API v1.0.3 iniciada com sucesso");
    Log.Information("═══════════════════════════════════════════════════════════════");
    Log.Information("📊 SQL Logging: {Status}",
        builder.Configuration.GetValue<bool>("SqlLogging:Enabled") ? "HABILITADO" : "DESABILITADO");
    Log.Information("⏱️ Rate Limiting: {Status}",
        rateLimitConfig.Exists() ? "CONFIGURADO" : "DEFAULT");
    Log.Information("🌐 CORS Origins: {Count} configurados", allOrigins.Count);
    Log.Information("🔒 HTTPS: {Status}",
        app.Environment.IsProduction() ? "OBRIGATÓRIO" : "Opcional");
    Log.Information("📘 Swagger: {Status}",
        builder.Configuration.GetValue<bool>("Features:EnableSwagger") ? "HABILITADO" : "DESABILITADO");
    Log.Information("📦 Módulos: 12 registrados (Identity + 11 de negócio)");
    Log.Information("═══════════════════════════════════════════════════════════════");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Aplicação encerrada inesperadamente");
}
finally
{
    Log.Information("🛑 Encerrando aplicação RhSensoERP API");
    Log.CloseAndFlush();
}
