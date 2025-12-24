// src/API/Controllers/SecurityMetricsController.cs

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Identity.Core.Entities;
using RhSensoERP.Identity.Infrastructure.Persistence.Contexts;

namespace RhSensoERP.API.Controllers
{
    /// <summary>
    /// Controller para métricas de segurança do sistema.
    /// ✅ FASE 2: Protegido com rate limiting e desabilitado em produção
    /// </summary>
    [ApiController]
    [Route("api/security/metrics")]
    [Authorize(Roles = "Admin")] // ✅ Apenas administradores
    [EnableRateLimiting("diagnostics")] // ✅ FASE 2: Rate limiting específico
#if !DEBUG
    [ApiExplorerSettings(IgnoreApi = true)] // ✅ FASE 2: Oculta do Swagger em Release
#endif
    public class SecurityMetricsController : ControllerBase
    {
        private readonly IdentityDbContext _db;
        private readonly ILogger<SecurityMetricsController> _logger;
        private readonly IWebHostEnvironment _environment;

        public SecurityMetricsController(
            IdentityDbContext db,
            ILogger<SecurityMetricsController> logger,
            IWebHostEnvironment environment)
        {
            _db = db;
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        /// Retorna métricas de segurança do sistema.
        /// ✅ FASE 2: Desabilitado em produção por segurança
        /// </summary>
        /// <param name="ct">Token de cancelamento</param>
        /// <returns>Métricas de segurança</returns>
        /// <response code="200">Métricas retornadas com sucesso</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="403">Acesso negado ou desabilitado em produção</response>
        /// <response code="429">Limite de requisições excedido</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> GetMetrics(CancellationToken ct)
        {
            // ✅ FASE 2: Desabilitar em produção por segurança
            if (_environment.IsProduction())
            {
                _logger.LogWarning(
                    "⚠️ Tentativa de acesso a métricas de segurança em produção bloqueada | User: {User}",
                    User.Identity?.Name ?? "Unknown");

                return StatusCode(403, new
                {
                    error = "FORBIDDEN",
                    message = "Métricas de segurança desabilitadas em produção. Use ferramentas de monitoramento dedicadas."
                });
            }

            try
            {
                _logger.LogInformation(
                    "📊 Consultando métricas de segurança | User: {User}",
                    User.Identity?.Name ?? "Unknown");

                var last24h = DateTime.UtcNow.AddHours(-24);
                var last7d = DateTime.UtcNow.AddDays(-7);

                var metrics = new
                {
                    // Tentativas de login
                    loginAttempts = new
                    {
                        last24h = await _db.Set<LoginAuditLog>()
                            .CountAsync(l => l.LoginAttemptAt >= last24h, ct),

                        failedLast24h = await _db.Set<LoginAuditLog>()
                            .CountAsync(l => l.LoginAttemptAt >= last24h && !l.IsSuccess, ct),

                        successRateLast24h = await CalculateSuccessRate(last24h, ct),

                        uniqueIPs = await _db.Set<LoginAuditLog>()
                            .Where(l => l.LoginAttemptAt >= last7d)
                            .Select(l => l.IpAddress)
                            .Distinct()
                            .CountAsync(ct)
                    },

                    // Contas bloqueadas
                    lockedAccounts = await _db.Set<UserSecurity>()
                        .CountAsync(u => u.LockoutEnd.HasValue && u.LockoutEnd > DateTime.UtcNow, ct),

                    // Senhas fracas (legado)
                    weakPasswords = await _db.Usuarios
                        .CountAsync(u => u.SenhaUser != null, ct),

                    // 2FA habilitado
                    twoFactorEnabled = await _db.Set<UserSecurity>()
                        .CountAsync(u => u.TwoFactorEnabled && !u.IsDeleted, ct),

                    // Refresh tokens ativos
                    activeRefreshTokens = await _db.Set<RefreshToken>()
                        .CountAsync(t => !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow, ct),

                    // ✅ FASE 2: Métricas adicionais
                    security = new
                    {
                        // Tokens expirados nas últimas 24h
                        expiredTokensLast24h = await _db.Set<RefreshToken>()
                            .CountAsync(t => t.ExpiresAt >= last24h && t.ExpiresAt < DateTime.UtcNow, ct),

                        // Tokens revogados nas últimas 24h
                        revokedTokensLast24h = await _db.Set<RefreshToken>()
                            .CountAsync(t => t.IsRevoked && t.RevokedAt >= last24h, ct),

                        // Usuários ativos (não deletados)
                        activeUsers = await _db.Set<UserSecurity>()
                            .CountAsync(u => !u.IsDeleted, ct),

                        // Usuários com email confirmado
                        emailConfirmedUsers = await _db.Set<UserSecurity>()
                            .CountAsync(u => u.EmailConfirmed && !u.IsDeleted, ct)
                    },

                    // Timestamp da consulta
                    queriedAt = DateTime.UtcNow
                };

                _logger.LogInformation("✅ Métricas de segurança retornadas com sucesso");

                return Ok(metrics);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("⏱️ Timeout ao consultar métricas de segurança");
                return StatusCode(504, new
                {
                    error = "TIMEOUT",
                    message = "A consulta foi cancelada ou excedeu o tempo limite."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao consultar métricas de segurança");
                return StatusCode(500, new
                {
                    error = "INTERNAL_ERROR",
                    message = "Erro ao consultar métricas de segurança."
                });
            }
        }

        /// <summary>
        /// Calcula a taxa de sucesso de login.
        /// </summary>
        private async Task<double> CalculateSuccessRate(DateTime since, CancellationToken ct)
        {
            var total = await _db.Set<LoginAuditLog>()
                .CountAsync(l => l.LoginAttemptAt >= since, ct);

            if (total == 0) return 100.0;

            var successful = await _db.Set<LoginAuditLog>()
                .CountAsync(l => l.LoginAttemptAt >= since && l.IsSuccess, ct);

            return Math.Round((double)successful / total * 100, 2);
        }
    }
}
