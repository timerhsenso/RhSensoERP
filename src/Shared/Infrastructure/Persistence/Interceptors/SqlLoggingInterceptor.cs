// ============================================================================
//  RhSensoERP.Shared.Infrastructure.Persistence.Interceptors.SqlLoggingInterceptor
//  Autor: Carlos Eduardo
//  Data: 15/11/2025
//  Descrição: Interceptor para logar todas as queries SQL executadas pelo EF Core
// ============================================================================

using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Text;

namespace RhSensoERP.Shared.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Interceptor customizado para capturar e logar todas as operações SQL.
/// Fornece métricas de performance, detalhes de parâmetros e alertas de queries lentas.
/// </summary>
public sealed class SqlLoggingInterceptor : DbCommandInterceptor
{
    private readonly ILogger<SqlLoggingInterceptor> _logger;
    private readonly SqlLoggingOptions _options;

    public SqlLoggingInterceptor(
        ILogger<SqlLoggingInterceptor> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _options = configuration
            .GetSection("SqlLogging")
            .Get<SqlLoggingOptions>() ?? new SqlLoggingOptions();
    }

    // ========================================
    // COMANDOS SÍNCRONOS
    // ========================================

    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        LogCommandExecution(command, eventData, "SELECT");
        return base.ReaderExecuted(command, eventData, result);
    }

    public override object? ScalarExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        object? result)
    {
        LogCommandExecution(command, eventData, "SCALAR");
        return base.ScalarExecuted(command, eventData, result);
    }

    public override int NonQueryExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result)
    {
        LogCommandExecution(command, eventData, "NON-QUERY", result);
        return base.NonQueryExecuted(command, eventData, result);
    }

    // ========================================
    // COMANDOS ASSÍNCRONOS
    // ========================================

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        LogCommandExecution(command, eventData, "SELECT (ASYNC)");
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<object?> ScalarExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        object? result,
        CancellationToken cancellationToken = default)
    {
        LogCommandExecution(command, eventData, "SCALAR (ASYNC)");
        return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<int> NonQueryExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        LogCommandExecution(command, eventData, "NON-QUERY (ASYNC)", result);
        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    // ========================================
    // TRATAMENTO DE ERROS
    // ========================================

    public override void CommandFailed(
        DbCommand command,
        CommandErrorEventData eventData)
    {
        LogCommandError(command, eventData);
        base.CommandFailed(command, eventData);
    }

    public override Task CommandFailedAsync(
        DbCommand command,
        CommandErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        LogCommandError(command, eventData);
        return base.CommandFailedAsync(command, eventData, cancellationToken);
    }

    // ========================================
    // MÉTODOS AUXILIARES
    // ========================================

    private void LogCommandExecution(
        DbCommand command,
        CommandExecutedEventData eventData,
        string commandType,
        int? rowsAffected = null)
    {
        if (!_options.Enabled)
            return;

        var durationMs = eventData.Duration.TotalMilliseconds;

        // Determinar nível de log baseado na duração
        var logLevel = DetermineLogLevel(durationMs);

        // Filtrar queries triviais se configurado
        if (!_options.LogTrivialQueries && durationMs < _options.TrivialQueryThresholdMs)
            return;

        // Construir mensagem de log
        var message = BuildLogMessage(command, commandType, durationMs, rowsAffected);

        // Logar
        _logger.Log(logLevel, message);

        // Alerta de performance para queries lentas
        if (durationMs >= _options.SlowQueryWarningThresholdMs)
        {
            _logger.LogWarning(
                "⚠️ SLOW QUERY DETECTED ({Duration}ms) - Threshold: {Threshold}ms{NewLine}{SQL}",
                durationMs,
                _options.SlowQueryWarningThresholdMs,
                Environment.NewLine,
                FormatSql(command.CommandText));
        }
    }

    private void LogCommandError(DbCommand command, CommandErrorEventData eventData)
    {
        if (!_options.Enabled)
            return;

        var durationMs = eventData.Duration.TotalMilliseconds;

        _logger.LogError(
            eventData.Exception,
            "❌ SQL ERROR ({Duration}ms){NewLine}" +
            "Command Type: {CommandType}{NewLine}" +
            "SQL:{NewLine}{SQL}{NewLine}" +
            "Parameters: {Parameters}{NewLine}" +
            "Error: {ErrorMessage}",
            durationMs,
            Environment.NewLine,
            command.CommandType,
            Environment.NewLine,
            FormatSql(command.CommandText),
            Environment.NewLine,
            GetParametersLog(command),
            Environment.NewLine,
            eventData.Exception.Message);
    }

    private string BuildLogMessage(
        DbCommand command,
        string commandType,
        double durationMs,
        int? rowsAffected)
    {
        var sb = new StringBuilder();

        // Header com emoji e tipo
        sb.Append(GetCommandEmoji(commandType));
        sb.Append($" SQL {commandType} ({durationMs:F2}ms)");

        if (rowsAffected.HasValue)
            sb.Append($" | Rows: {rowsAffected.Value}");

        // SQL formatado (se habilitado)
        if (_options.LogSqlText)
        {
            sb.AppendLine();
            sb.Append(_options.FormatSql
                ? FormatSql(command.CommandText)
                : command.CommandText);
        }

        // Parâmetros (se habilitado)
        if (_options.LogParameters && command.Parameters.Count > 0)
        {
            sb.AppendLine();
            sb.Append("Parameters: ");
            sb.Append(GetParametersLog(command));
        }

        return sb.ToString();
    }

    private LogLevel DetermineLogLevel(double durationMs)
    {
        return durationMs switch
        {
            >= 1000 => LogLevel.Warning,      // >= 1s
            >= 500 => LogLevel.Information,   // >= 500ms
            >= 100 => LogLevel.Debug,         // >= 100ms
            _ => LogLevel.Trace               // < 100ms
        };
    }

    private static string GetCommandEmoji(string commandType)
    {
        return commandType switch
        {
            var s when s.Contains("SELECT") => "🔍",
            var s when s.Contains("INSERT") => "➕",
            var s when s.Contains("UPDATE") => "✏️",
            var s when s.Contains("DELETE") => "🗑️",
            _ => "📊"
        };
    }

    private string GetParametersLog(DbCommand command)
    {
        if (command.Parameters.Count == 0)
            return "[]";

        var parameters = new List<string>();

        foreach (DbParameter param in command.Parameters)
        {
            var value = FormatParameterValue(param.Value);

            // Mascarar valores sensíveis se configurado
            if (_options.MaskSensitiveData && IsSensitiveParameter(param.ParameterName))
            {
                value = "***MASKED***";
            }

            parameters.Add($"{param.ParameterName}={value}");
        }

        return $"[{string.Join(", ", parameters)}]";
    }

    private static string FormatParameterValue(object? value)
    {
        return value switch
        {
            null => "NULL",
            DBNull => "NULL",
            string s => $"'{s}'",
            DateTime dt => $"'{dt:yyyy-MM-dd HH:mm:ss}'",
            _ => value.ToString() ?? "NULL"
        };
    }

    private static bool IsSensitiveParameter(string parameterName)
    {
        var sensitiveKeywords = new[]
        {
            "password", "senha", "pwd", "secret", "token",
            "hash", "salt", "apikey", "creditcard", "cvv"
        };

        return sensitiveKeywords.Any(keyword =>
            parameterName.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    private static string FormatSql(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            return sql;

        // Substituições básicas para melhor legibilidade
        return sql
            .Replace("SELECT", "\nSELECT")
            .Replace("FROM", "\nFROM")
            .Replace("WHERE", "\nWHERE")
            .Replace("INNER JOIN", "\nINNER JOIN")
            .Replace("LEFT JOIN", "\nLEFT JOIN")
            .Replace("ORDER BY", "\nORDER BY")
            .Replace("GROUP BY", "\nGROUP BY")
            .Replace("HAVING", "\nHAVING")
            .Replace("INSERT INTO", "\nINSERT INTO")
            .Replace("UPDATE", "\nUPDATE")
            .Replace("DELETE FROM", "\nDELETE FROM")
            .Replace("SET", "\nSET")
            .Replace("VALUES", "\nVALUES")
            .Trim();
    }
}

// ========================================
// CLASSE DE CONFIGURAÇÃO
// ========================================

/// <summary>
/// Opções de configuração para o SqlLoggingInterceptor.
/// </summary>
public sealed class SqlLoggingOptions
{
    /// <summary>Habilitar/desabilitar logging SQL.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Logar o texto SQL completo.</summary>
    public bool LogSqlText { get; set; } = true;

    /// <summary>Logar parâmetros SQL.</summary>
    public bool LogParameters { get; set; } = true;

    /// <summary>Formatar SQL para melhor legibilidade.</summary>
    public bool FormatSql { get; set; } = true;

    /// <summary>Mascarar dados sensíveis (senhas, tokens, etc.).</summary>
    public bool MaskSensitiveData { get; set; } = true;

    /// <summary>Logar queries triviais (muito rápidas).</summary>
    public bool LogTrivialQueries { get; set; } = false;

    /// <summary>Threshold para queries triviais (ms).</summary>
    public double TrivialQueryThresholdMs { get; set; } = 5;

    /// <summary>Threshold para alertas de queries lentas (ms).</summary>
    public double SlowQueryWarningThresholdMs { get; set; } = 500;
}