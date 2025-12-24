// src/API/BackgroundServices/AuditCleanupBackgroundService.cs

using Microsoft.EntityFrameworkCore;
using RhSensoERP.Identity.Core.Entities;
using RhSensoERP.Identity.Infrastructure.Persistence.Contexts;

namespace RhSensoERP.API.BackgroundServices;

/// <summary>
/// Serviço background para limpeza automática de logs de auditoria antigos.
/// ✅ FASE 5: Job agendado para manter tabela de auditoria gerenciável
/// </summary>
public class AuditCleanupBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AuditCleanupBackgroundService> _logger;
    private readonly IConfiguration _configuration;

    public AuditCleanupBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<AuditCleanupBackgroundService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🧹 AuditCleanupBackgroundService iniciado");

        // Aguarda 1 minuto após startup para não interferir na inicialização
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteCleanupAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao executar limpeza de auditoria");
            }

            // Aguarda até o próximo ciclo
            var nextRun = CalculateNextRunTime();
            var delay = nextRun - DateTime.UtcNow;

            if (delay > TimeSpan.Zero)
            {
                _logger.LogInformation(
                    "⏰ Próxima limpeza de auditoria agendada para: {NextRun} (em {Hours}h {Minutes}m)",
                    nextRun.ToLocalTime(),
                    (int)delay.TotalHours,
                    delay.Minutes);

                await Task.Delay(delay, stoppingToken);
            }
        }

        _logger.LogInformation("🛑 AuditCleanupBackgroundService finalizado");
    }

    /// <summary>
    /// Executa a limpeza de logs antigos.
    /// </summary>
    private async Task ExecuteCleanupAsync(CancellationToken ct)
    {
        // Ler configurações
        var enabled = _configuration.GetValue<bool>("AuditCleanup:Enabled", true);
        if (!enabled)
        {
            _logger.LogInformation("⏸️ Limpeza de auditoria está desabilitada");
            return;
        }

        var retentionDays = _configuration.GetValue<int>("AuditCleanup:RetentionDays", 90);
        var batchSize = _configuration.GetValue<int>("AuditCleanup:BatchSize", 1000);

        _logger.LogInformation(
            "🧹 Iniciando limpeza de auditoria | Retenção: {RetentionDays} dias | BatchSize: {BatchSize}",
            retentionDays,
            batchSize);

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
        var totalDeleted = 0;

        // Deletar em lotes para não travar o banco
        while (true)
        {
            var deleted = await db.Database.ExecuteSqlRawAsync(
                @"DELETE TOP (@batchSize) FROM SEG_SecurityAuditLogs 
                  WHERE OccurredAt < @cutoffDate",
                new[]
                {
                    new Microsoft.Data.SqlClient.SqlParameter("@batchSize", batchSize),
                    new Microsoft.Data.SqlClient.SqlParameter("@cutoffDate", cutoffDate)
                },
                ct);

            totalDeleted += deleted;

            if (deleted < batchSize)
            {
                // Não há mais registros para deletar
                break;
            }

            // Pequeno delay entre lotes para não sobrecarregar o banco
            await Task.Delay(100, ct);
        }

        if (totalDeleted > 0)
        {
            _logger.LogInformation(
                "✅ Limpeza de auditoria concluída | Registros deletados: {TotalDeleted} | " +
                "Cutoff: {CutoffDate}",
                totalDeleted,
                cutoffDate);
        }
        else
        {
            _logger.LogInformation("✅ Limpeza de auditoria concluída | Nenhum registro para deletar");
        }
    }

    /// <summary>
    /// Calcula o próximo horário de execução baseado na frequência configurada.
    /// </summary>
    private DateTime CalculateNextRunTime()
    {
        var frequency = _configuration.GetValue<string>("AuditCleanup:Frequency", "Daily");
        var runAtHour = _configuration.GetValue<int>("AuditCleanup:RunAtHour", 3); // 3 AM padrão

        var now = DateTime.UtcNow;
        DateTime nextRun;

        switch (frequency.ToUpper())
        {
            case "HOURLY":
                // Executa a cada hora
                nextRun = now.AddHours(1);
                break;

            case "DAILY":
                // Executa diariamente no horário configurado
                nextRun = new DateTime(now.Year, now.Month, now.Day, runAtHour, 0, 0, DateTimeKind.Utc);
                if (nextRun <= now)
                {
                    nextRun = nextRun.AddDays(1);
                }
                break;

            case "WEEKLY":
                // Executa semanalmente no domingo no horário configurado
                var daysUntilSunday = ((int)DayOfWeek.Sunday - (int)now.DayOfWeek + 7) % 7;
                if (daysUntilSunday == 0 && now.Hour >= runAtHour)
                {
                    daysUntilSunday = 7; // Próximo domingo
                }
                nextRun = new DateTime(now.Year, now.Month, now.Day, runAtHour, 0, 0, DateTimeKind.Utc)
                    .AddDays(daysUntilSunday);
                break;

            case "MONTHLY":
                // Executa mensalmente no dia 1 no horário configurado
                nextRun = new DateTime(now.Year, now.Month, 1, runAtHour, 0, 0, DateTimeKind.Utc)
                    .AddMonths(1);
                if (nextRun <= now)
                {
                    nextRun = nextRun.AddMonths(1);
                }
                break;

            default:
                // Padrão: diário
                _logger.LogWarning(
                    "⚠️ Frequência inválida: {Frequency}. Usando 'Daily' como padrão.",
                    frequency);
                nextRun = new DateTime(now.Year, now.Month, now.Day, runAtHour, 0, 0, DateTimeKind.Utc);
                if (nextRun <= now)
                {
                    nextRun = nextRun.AddDays(1);
                }
                break;
        }

        return nextRun;
    }
}
