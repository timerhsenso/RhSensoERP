// src/Identity/Application/Services/SecurityAuditService.cs

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RhSensoERP.Identity.Core.Entities;
using RhSensoERP.Identity.Infrastructure.Persistence.Contexts;

namespace RhSensoERP.Identity.Application.Services;

/// <summary>
/// Serviço para gerenciar auditoria de segurança.
/// ✅ FASE 5: Centraliza logging de eventos de segurança
/// </summary>
public interface ISecurityAuditService
{
    Task LogAsync(
        string eventType,
        string eventCategory,
        string severity,
        string description,
        HttpContext httpContext,
        bool isSuccess,
        Guid? idUserSecurity = null,
        string? username = null,
        string? additionalData = null,
        string? errorMessage = null,
        CancellationToken ct = default);

    Task<List<SecurityAuditLog>> GetRecentEventsAsync(
        int count = 100,
        string? eventType = null,
        string? severity = null,
        CancellationToken ct = default);

    Task<Dictionary<string, int>> GetEventStatisticsAsync(
        DateTime since,
        CancellationToken ct = default);
}

public class SecurityAuditService : ISecurityAuditService
{
    private readonly IdentityDbContext _db;
    private readonly ILogger<SecurityAuditService> _logger;

    public SecurityAuditService(
        IdentityDbContext db,
        ILogger<SecurityAuditService> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Registra um evento de segurança no banco de dados.
    /// </summary>
    public async Task LogAsync(
        string eventType,
        string eventCategory,
        string severity,
        string description,
        HttpContext httpContext,
        bool isSuccess,
        Guid? idUserSecurity = null,
        string? username = null,
        string? additionalData = null,
        string? errorMessage = null,
        CancellationToken ct = default)
    {
        try
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = httpContext.Request.Headers.UserAgent.ToString();
            var requestPath = httpContext.Request.Path.Value;
            var requestMethod = httpContext.Request.Method;

            var auditLog = new SecurityAuditLog(
                eventType: eventType,
                eventCategory: eventCategory,
                severity: severity,
                description: description,
                ipAddress: ipAddress,
                isSuccess: isSuccess,
                idUserSecurity: idUserSecurity,
                username: username,
                userAgent: userAgent,
                requestPath: requestPath,
                requestMethod: requestMethod,
                additionalData: additionalData,
                errorMessage: errorMessage
            );

            _db.Set<SecurityAuditLog>().Add(auditLog);
            await _db.SaveChangesAsync(ct);

            // Log estruturado para facilitar análise
            _logger.LogInformation(
                "🔒 Security Event | Type: {EventType} | Category: {Category} | " +
                "Severity: {Severity} | User: {Username} | IP: {IP} | Success: {Success}",
                eventType, eventCategory, severity, username ?? "Anonymous", ipAddress, isSuccess);
        }
        catch (Exception ex)
        {
            // Não falha a operação principal se auditoria falhar
            _logger.LogError(ex, "❌ Erro ao registrar evento de segurança");
        }
    }

    /// <summary>
    /// Retorna eventos recentes de segurança.
    /// </summary>
    public async Task<List<SecurityAuditLog>> GetRecentEventsAsync(
        int count = 100,
        string? eventType = null,
        string? severity = null,
        CancellationToken ct = default)
    {
        var query = _db.Set<SecurityAuditLog>()
            .AsNoTracking()
            .OrderByDescending(e => e.OccurredAt)
            .Take(count);

        if (!string.IsNullOrWhiteSpace(eventType))
        {
            query = (IOrderedQueryable<SecurityAuditLog>)query.Where(e => e.EventType == eventType);
        }

        if (!string.IsNullOrWhiteSpace(severity))
        {
            query = (IOrderedQueryable<SecurityAuditLog>)query.Where(e => e.Severity == severity);
        }

        return await query.ToListAsync(ct);
    }

    /// <summary>
    /// Retorna estatísticas de eventos de segurança.
    /// </summary>
    public async Task<Dictionary<string, int>> GetEventStatisticsAsync(
        DateTime since,
        CancellationToken ct = default)
    {
        var events = await _db.Set<SecurityAuditLog>()
            .AsNoTracking()
            .Where(e => e.OccurredAt >= since)
            .GroupBy(e => e.EventType)
            .Select(g => new { EventType = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return events.ToDictionary(e => e.EventType, e => e.Count);
    }
}
