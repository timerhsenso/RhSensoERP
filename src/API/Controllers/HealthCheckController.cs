// src/API/Controllers/HealthCheckController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Identity.Infrastructure.Persistence.Contexts;
using System.Diagnostics;
using System.Reflection;

namespace RhSensoERP.API.Controllers;

/// <summary>
/// Controller para health checks detalhados do sistema.
/// ✅ FASE 4: Health checks completos com status de dependências
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : ControllerBase
{
    private readonly IdentityDbContext _db;
    private readonly ILogger<HealthCheckController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public HealthCheckController(
        IdentityDbContext db,
        ILogger<HealthCheckController> logger,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _db = db;
        _logger = logger;
        _configuration = configuration;
        _environment = environment;
    }

    /// <summary>
    /// Health check básico (público, sem autenticação).
    /// Usado por load balancers e monitoramento externo.
    /// </summary>
    /// <returns>Status básico da aplicação</returns>
    /// <response code="200">Aplicação está saudável</response>
    /// <response code="503">Aplicação está com problemas</response>
    [HttpGet("live")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult Liveness()
    {
        // Health check minimalista: apenas verifica se a aplicação está rodando
        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            environment = _environment.EnvironmentName
        });
    }

    /// <summary>
    /// Health check completo (requer autenticação de admin).
    /// Verifica status de todas as dependências.
    /// </summary>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Status detalhado da aplicação e dependências</returns>
    /// <response code="200">Todos os componentes estão saudáveis</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="503">Um ou mais componentes estão com problemas</response>
    [HttpGet("ready")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Readiness(CancellationToken ct)
    {
        var checks = new List<HealthCheckResult>();
        var overallHealthy = true;

        // ====================================================================
        // 1. DATABASE CONNECTIVITY
        // ====================================================================
        var dbCheck = await CheckDatabaseAsync(ct);
        checks.Add(dbCheck);
        if (!dbCheck.Healthy) overallHealthy = false;

        // ====================================================================
        // 2. CONFIGURATION
        // ====================================================================
        var configCheck = CheckConfiguration();
        checks.Add(configCheck);
        if (!configCheck.Healthy) overallHealthy = false;

        // ====================================================================
        // 3. MEMORY USAGE
        // ====================================================================
        var memoryCheck = CheckMemoryUsage();
        checks.Add(memoryCheck);
        if (!memoryCheck.Healthy) overallHealthy = false;

        // ====================================================================
        // 4. DISK SPACE (se aplicável)
        // ====================================================================
        var diskCheck = CheckDiskSpace();
        checks.Add(diskCheck);
        if (!diskCheck.Healthy) overallHealthy = false;

        // ====================================================================
        // RESPONSE
        // ====================================================================
        var response = new
        {
            status = overallHealthy ? "Healthy" : "Unhealthy",
            timestamp = DateTime.UtcNow,
            environment = _environment.EnvironmentName,
            version = GetApplicationVersion(),
            uptime = GetUptime(),
            checks = checks.Select(c => new
            {
                name = c.Name,
                status = c.Healthy ? "Healthy" : "Unhealthy",
                description = c.Description,
                responseTime = c.ResponseTime,
                details = c.Details
            })
        };

        var statusCode = overallHealthy ? 200 : 503;
        return StatusCode(statusCode, response);
    }

    // ========================================
    // MÉTODOS DE HEALTH CHECK
    // ========================================

    /// <summary>
    /// Verifica conectividade com o banco de dados.
    /// </summary>
    private async Task<HealthCheckResult> CheckDatabaseAsync(CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var canConnect = await _db.Database.CanConnectAsync(ct);
            sw.Stop();

            if (!canConnect)
            {
                return new HealthCheckResult
                {
                    Name = "Database",
                    Healthy = false,
                    Description = "Não foi possível conectar ao banco de dados",
                    ResponseTime = sw.ElapsedMilliseconds
                };
            }

            // Tenta executar uma query simples
            var count = await _db.Usuarios.CountAsync(ct);
            sw.Stop();

            return new HealthCheckResult
            {
                Name = "Database",
                Healthy = true,
                Description = "Banco de dados está acessível e respondendo",
                ResponseTime = sw.ElapsedMilliseconds,
                Details = new Dictionary<string, object>
                {
                    { "provider", _db.Database.ProviderName ?? "Unknown" },
                    { "database", _db.Database.GetDbConnection().Database },
                    { "userCount", count }
                }
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Erro ao verificar saúde do banco de dados");

            return new HealthCheckResult
            {
                Name = "Database",
                Healthy = false,
                Description = $"Erro ao conectar: {ex.Message}",
                ResponseTime = sw.ElapsedMilliseconds
            };
        }
    }

    /// <summary>
    /// Verifica se configurações críticas estão presentes.
    /// </summary>
    private HealthCheckResult CheckConfiguration()
    {
        var sw = Stopwatch.StartNew();
        var issues = new List<string>();

        // Verifica JWT SecretKey
        var jwtSecret = _configuration["JwtSettings:SecretKey"];
        if (string.IsNullOrWhiteSpace(jwtSecret))
        {
            issues.Add("JwtSettings:SecretKey não configurada");
        }

        // Verifica Connection String
        var connString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connString))
        {
            issues.Add("ConnectionString DefaultConnection não configurada");
        }

        sw.Stop();

        return new HealthCheckResult
        {
            Name = "Configuration",
            Healthy = issues.Count == 0,
            Description = issues.Count == 0
                ? "Todas as configurações críticas estão presentes"
                : $"{issues.Count} configuração(ões) ausente(s)",
            ResponseTime = sw.ElapsedMilliseconds,
            Details = issues.Count > 0
                ? new Dictionary<string, object> { { "issues", issues } }
                : null
        };
    }

    /// <summary>
    /// Verifica uso de memória da aplicação.
    /// </summary>
    private HealthCheckResult CheckMemoryUsage()
    {
        var sw = Stopwatch.StartNew();

        var process = Process.GetCurrentProcess();
        var memoryMB = process.WorkingSet64 / 1024 / 1024;
        var threshold = 1024; // 1GB threshold

        sw.Stop();

        return new HealthCheckResult
        {
            Name = "Memory",
            Healthy = memoryMB < threshold,
            Description = memoryMB < threshold
                ? "Uso de memória está normal"
                : "Uso de memória está alto",
            ResponseTime = sw.ElapsedMilliseconds,
            Details = new Dictionary<string, object>
            {
                { "workingSetMB", memoryMB },
                { "thresholdMB", threshold }
            }
        };
    }

    /// <summary>
    /// Verifica espaço em disco disponível.
    /// </summary>
    private HealthCheckResult CheckDiskSpace()
    {
        var sw = Stopwatch.StartNew();

        try
        {
            var drive = DriveInfo.GetDrives()
                .FirstOrDefault(d => d.IsReady && d.DriveType == DriveType.Fixed);

            if (drive == null)
            {
                sw.Stop();
                return new HealthCheckResult
                {
                    Name = "Disk",
                    Healthy = true,
                    Description = "Não foi possível verificar espaço em disco",
                    ResponseTime = sw.ElapsedMilliseconds
                };
            }

            var freeSpaceGB = drive.AvailableFreeSpace / 1024 / 1024 / 1024;
            var threshold = 5; // 5GB threshold

            sw.Stop();

            return new HealthCheckResult
            {
                Name = "Disk",
                Healthy = freeSpaceGB > threshold,
                Description = freeSpaceGB > threshold
                    ? "Espaço em disco está adequado"
                    : "Espaço em disco está baixo",
                ResponseTime = sw.ElapsedMilliseconds,
                Details = new Dictionary<string, object>
                {
                    { "freeSpaceGB", freeSpaceGB },
                    { "thresholdGB", threshold },
                    { "driveName", drive.Name }
                }
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogWarning(ex, "Erro ao verificar espaço em disco");

            return new HealthCheckResult
            {
                Name = "Disk",
                Healthy = true, // Não falha o health check por isso
                Description = "Não foi possível verificar espaço em disco",
                ResponseTime = sw.ElapsedMilliseconds
            };
        }
    }

    // ========================================
    // MÉTODOS AUXILIARES
    // ========================================

    /// <summary>
    /// Retorna a versão da aplicação.
    /// </summary>
    private string GetApplicationVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        return version?.ToString() ?? "Unknown";
    }

    /// <summary>
    /// Retorna o tempo de uptime da aplicação.
    /// </summary>
    private string GetUptime()
    {
        var process = Process.GetCurrentProcess();
        var uptime = DateTime.Now - process.StartTime;
        return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m";
    }
}

/// <summary>
/// Resultado de um health check individual.
/// </summary>
public class HealthCheckResult
{
    public string Name { get; set; } = string.Empty;
    public bool Healthy { get; set; }
    public string Description { get; set; } = string.Empty;
    public long ResponseTime { get; set; }
    public Dictionary<string, object>? Details { get; set; }
}
