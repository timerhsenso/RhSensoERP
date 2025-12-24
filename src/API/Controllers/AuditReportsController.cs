// src/API/Controllers/AuditReportsController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Identity.Application.Services;
using RhSensoERP.Identity.Core.Entities;
using RhSensoERP.Identity.Infrastructure.Persistence.Contexts;

namespace RhSensoERP.API.Controllers;

/// <summary>
/// Controller para relatórios de auditoria de segurança.
/// ✅ FASE 5: Relatórios completos de eventos de segurança
/// </summary>
[ApiController]
[Route("api/security/audit")]
[Authorize(Roles = "Admin")]
[EnableRateLimiting("diagnostics")]
#if !DEBUG
[ApiExplorerSettings(IgnoreApi = true)]
#endif
public class AuditReportsController : ControllerBase
{
    private readonly ISecurityAuditService _auditService;
    private readonly IdentityDbContext _db;
    private readonly ILogger<AuditReportsController> _logger;
    private readonly IWebHostEnvironment _environment;

    public AuditReportsController(
        ISecurityAuditService auditService,
        IdentityDbContext db,
        ILogger<AuditReportsController> logger,
        IWebHostEnvironment environment)
    {
        _auditService = auditService;
        _db = db;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Retorna eventos recentes de segurança.
    /// </summary>
    /// <param name="count">Número de eventos a retornar (padrão: 100, máx: 1000)</param>
    /// <param name="eventType">Filtrar por tipo de evento</param>
    /// <param name="severity">Filtrar por severidade</param>
    /// <param name="ct">Token de cancelamento</param>
    [HttpGet("events")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetRecentEvents(
        [FromQuery] int count = 100,
        [FromQuery] string? eventType = null,
        [FromQuery] string? severity = null,
        CancellationToken ct = default)
    {
        // ✅ FASE 5: Desabilitar em produção
        if (_environment.IsProduction())
        {
            return StatusCode(403, new
            {
                error = "FORBIDDEN",
                message = "Relatórios de auditoria desabilitados em produção. Use ferramentas de monitoramento dedicadas."
            });
        }

        // Limitar count máximo
        count = Math.Min(count, 1000);

        var events = await _auditService.GetRecentEventsAsync(count, eventType, severity, ct);

        return Ok(new
        {
            total = events.Count,
            filters = new
            {
                count,
                eventType,
                severity
            },
            events = events.Select(e => new
            {
                id = e.Id,
                eventType = e.EventType,
                category = e.EventCategory,
                severity = e.Severity,
                description = e.Description,
                username = e.Username ?? "Anonymous",
                ipAddress = e.IpAddress,
                requestPath = e.RequestPath,
                requestMethod = e.RequestMethod,
                isSuccess = e.IsSuccess,
                errorMessage = e.ErrorMessage,
                occurredAt = e.OccurredAt
            })
        });
    }

    /// <summary>
    /// Retorna estatísticas de eventos de segurança.
    /// </summary>
    /// <param name="hours">Período em horas (padrão: 24)</param>
    /// <param name="ct">Token de cancelamento</param>
    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetStatistics(
        [FromQuery] int hours = 24,
        CancellationToken ct = default)
    {
        // ✅ FASE 5: Desabilitar em produção
        if (_environment.IsProduction())
        {
            return StatusCode(403, new
            {
                error = "FORBIDDEN",
                message = "Relatórios de auditoria desabilitados em produção. Use ferramentas de monitoramento dedicadas."
            });
        }

        var since = DateTime.UtcNow.AddHours(-hours);

        // Estatísticas por tipo de evento
        var eventStats = await _auditService.GetEventStatisticsAsync(since, ct);

        // Estatísticas por severidade
        var severityStats = await _db.Set<SecurityAuditLog>()
            .Where(e => e.OccurredAt >= since)
            .GroupBy(e => e.Severity)
            .Select(g => new { Severity = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        // Top IPs com mais eventos
        var topIPs = await _db.Set<SecurityAuditLog>()
            .Where(e => e.OccurredAt >= since)
            .GroupBy(e => e.IpAddress)
            .Select(g => new { IP = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync(ct);

        // Eventos falhados
        var failedEvents = await _db.Set<SecurityAuditLog>()
            .Where(e => e.OccurredAt >= since && !e.IsSuccess)
            .GroupBy(e => e.EventType)
            .Select(g => new { EventType = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return Ok(new
        {
            period = new
            {
                hours,
                since,
                until = DateTime.UtcNow
            },
            eventsByType = eventStats,
            eventsBySeverity = severityStats.ToDictionary(s => s.Severity, s => s.Count),
            topIPs = topIPs.Select(ip => new { ip.IP, ip.Count }),
            failedEvents = failedEvents.ToDictionary(f => f.EventType, f => f.Count),
            totalEvents = eventStats.Values.Sum()
        });
    }

    /// <summary>
    /// Retorna relatório de tentativas de login.
    /// </summary>
    /// <param name="hours">Período em horas (padrão: 24)</param>
    /// <param name="ct">Token de cancelamento</param>
    [HttpGet("login-report")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetLoginReport(
        [FromQuery] int hours = 24,
        CancellationToken ct = default)
    {
        // ✅ FASE 5: Desabilitar em produção
        if (_environment.IsProduction())
        {
            return StatusCode(403, new
            {
                error = "FORBIDDEN",
                message = "Relatórios de auditoria desabilitados em produção. Use ferramentas de monitoramento dedicadas."
            });
        }

        var since = DateTime.UtcNow.AddHours(-hours);

        // Tentativas de login (tabela LoginAuditLog)
        var loginAttempts = await _db.Set<LoginAuditLog>()
            .Where(l => l.LoginAttemptAt >= since)
            .ToListAsync(ct);

        var totalAttempts = loginAttempts.Count;
        var successfulAttempts = loginAttempts.Count(l => l.IsSuccess);
        var failedAttempts = loginAttempts.Count(l => !l.IsSuccess);

        // Falhas por razão
        var failuresByReason = loginAttempts
            .Where(l => !l.IsSuccess && !string.IsNullOrWhiteSpace(l.FailureReason))
            .GroupBy(l => l.FailureReason)
            .Select(g => new { Reason = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToList();

        // Top usuários com mais tentativas
        var topUsers = loginAttempts
            .GroupBy(l => l.IdUserSecurity)
            .Select(g => new
            {
                IdUserSecurity = g.Key,
                TotalAttempts = g.Count(),
                SuccessfulAttempts = g.Count(l => l.IsSuccess),
                FailedAttempts = g.Count(l => !l.IsSuccess)
            })
            .OrderByDescending(x => x.TotalAttempts)
            .Take(10)
            .ToList();

        // IPs suspeitos (muitas falhas)
        var suspiciousIPs = loginAttempts
            .Where(l => !l.IsSuccess)
            .GroupBy(l => l.IpAddress)
            .Select(g => new { IP = g.Key, FailedAttempts = g.Count() })
            .Where(x => x.FailedAttempts >= 5)
            .OrderByDescending(x => x.FailedAttempts)
            .ToList();

        return Ok(new
        {
            period = new
            {
                hours,
                since,
                until = DateTime.UtcNow
            },
            summary = new
            {
                totalAttempts,
                successfulAttempts,
                failedAttempts,
                successRate = totalAttempts > 0
                    ? Math.Round((double)successfulAttempts / totalAttempts * 100, 2)
                    : 0
            },
            failuresByReason = failuresByReason.ToDictionary(f => f.Reason!, f => f.Count),
            topUsers,
            suspiciousIPs
        });
    }

    /// <summary>
    /// Retorna relatório de atividades suspeitas.
    /// </summary>
    /// <param name="hours">Período em horas (padrão: 24)</param>
    /// <param name="ct">Token de cancelamento</param>
    [HttpGet("suspicious-activities")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetSuspiciousActivities(
        [FromQuery] int hours = 24,
        CancellationToken ct = default)
    {
        // ✅ FASE 5: Desabilitar em produção
        if (_environment.IsProduction())
        {
            return StatusCode(403, new
            {
                error = "FORBIDDEN",
                message = "Relatórios de auditoria desabilitados em produção. Use ferramentas de monitoramento dedicadas."
            });
        }

        var since = DateTime.UtcNow.AddHours(-hours);

        // Eventos suspeitos
        var suspiciousEvents = await _db.Set<SecurityAuditLog>()
            .Where(e => e.OccurredAt >= since &&
                       (e.EventType == SecurityEventType.SuspiciousActivity ||
                        e.EventType == SecurityEventType.UnauthorizedAccess ||
                        e.EventType == SecurityEventType.RateLimitExceeded ||
                        e.Severity == SecuritySeverity.Critical))
            .OrderByDescending(e => e.OccurredAt)
            .Take(100)
            .ToListAsync(ct);

        // Agrupar por IP
        var suspiciousByIP = suspiciousEvents
            .GroupBy(e => e.IpAddress)
            .Select(g => new
            {
                IP = g.Key,
                Count = g.Count(),
                Events = g.Select(e => new
                {
                    e.EventType,
                    e.Description,
                    e.OccurredAt
                }).ToList()
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return Ok(new
        {
            period = new
            {
                hours,
                since,
                until = DateTime.UtcNow
            },
            totalSuspiciousEvents = suspiciousEvents.Count,
            suspiciousByIP,
            recentEvents = suspiciousEvents.Take(20).Select(e => new
            {
                e.EventType,
                e.Severity,
                e.Description,
                e.Username,
                e.IpAddress,
                e.RequestPath,
                e.OccurredAt
            })
        });
    }
}
