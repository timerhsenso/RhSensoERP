using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.Logging;
using RhSensoERP.Shared.Core.Abstractions;

namespace RhSensoERP.Shared.Application.Behaviors;

/// <summary>
/// Pipeline behavior para logar automaticamente todas as requisições do MediatR.
/// </summary>
/// <typeparam name="TRequest">Tipo da requisição.</typeparam>
/// <typeparam name="TResponse">Tipo da resposta.</typeparam>
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const int PerformanceThresholdMs = 1000;
    private const int MaxStringLength = 100;

    private static readonly string[] _sensitiveKeywords =
    {
        "password", "senha", "pwd", "secret", "token",
        "creditcard", "cartao", "cvv", "cvc",
        "ssn", "cpf", "rg", "cnh"
    };

    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUser currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    // Corrigido SA1117: Parâmetros em linhas separadas
    [SuppressMessage(
        "Usage",
        "CA2016:Forward the 'CancellationToken' parameter to methods",
        Justification = "RequestHandlerDelegate does not accept CancellationToken parameter")]
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid().ToString();

        LogStart(requestId, requestName);
        LogRequestData(requestId, requestName, request);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            // O delegate 'next' não aceita CancellationToken como parâmetro
            var response = await next().ConfigureAwait(false);
            stopwatch.Stop();

            LogSuccess(requestId, requestName, stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex) when (LogError(requestId, requestName, stopwatch.ElapsedMilliseconds, ex))
        {
            // Este when sempre retorna false, então a exceção sempre é relançada
            throw;
        }
    }

    #region Static Methods

    private static Dictionary<string, object?> ExtractRequestData(TRequest request)
    {
        var requestData = new Dictionary<string, object?>();

        if (EqualityComparer<TRequest>.Default.Equals(request, default))
        {
            return requestData;
        }

        try
        {
            var properties = request!.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead);

            foreach (var prop in properties)
            {
                var propName = prop.Name;
                object? value = null;

                try
                {
                    value = prop.GetValue(request);
                }
                catch (TargetInvocationException)
                {
                    value = "[Error reading property]";
                }
                catch (NotImplementedException)
                {
                    value = "[Not implemented]";
                }

                requestData[propName] = IsSensitiveData(propName)
                    ? "***REDACTED***"
                    : FormatPropertyValue(value, prop.PropertyType);
            }
        }
        catch (ArgumentException)
        {
            // Erro ao obter propriedades do tipo
        }
        catch (InvalidOperationException)
        {
            // Erro ao processar o tipo
        }

        return requestData;
    }

    private static object? FormatPropertyValue(object? value, Type propertyType)
    {
        if (value == null)
        {
            return null;
        }

        if (value is string strValue && strValue.Length > MaxStringLength)
        {
            return string.Concat(strValue.AsSpan(0, MaxStringLength), "...[TRUNCATED]");
        }

        return IsSimpleType(propertyType) ? value : value.GetType().Name;
    }

    private static bool IsSensitiveData(string propertyName)
    {
        return _sensitiveKeywords.Any(keyword =>
            propertyName.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive ||
               type.IsEnum ||
               type == typeof(string) ||
               type == typeof(decimal) ||
               type == typeof(DateTime) ||
               type == typeof(DateTimeOffset) ||
               type == typeof(TimeSpan) ||
               type == typeof(Guid) ||
               (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                IsSimpleType(type.GetGenericArguments()[0]));
    }

    #endregion

    #region Instance Methods

    private void LogStart(string requestId, string requestName)
    {
        //var userId = _currentUser?.UserId ?? "Anonymous";
        var userId = _currentUser?.UserId?.ToString() ?? "Anonymous";

        var userName = _currentUser?.UserName ?? "Anonymous";

        _logger.LogInformation(
            "[START] [{TraceId}] Handling {RequestName} by {UserId}({UserName}) at {DateTime}",
            requestId,
            requestName,
            userId,
            userName,
            _dateTimeProvider.UtcNow);
    }

    private void LogRequestData(string requestId, string requestName, TRequest request)
    {
        if (!_logger.IsEnabled(LogLevel.Debug))
        {
            return;
        }

        var requestData = ExtractRequestData(request);
        _logger.LogDebug(
            "[PROPS] [{TraceId}] {RequestName} {@RequestData}",
            requestId,
            requestName,
            requestData);
    }

    private void LogSuccess(string requestId, string requestName, long elapsedMs)
    {
        _logger.LogInformation(
            "[END] [{TraceId}] Handled {RequestName} in {ElapsedMilliseconds}ms",
            requestId,
            requestName,
            elapsedMs);

        if (elapsedMs > PerformanceThresholdMs)
        {
            _logger.LogWarning(
                "[PERFORMANCE] [{TraceId}] {RequestName} took {ElapsedMilliseconds}ms - Expected less than {Threshold}ms",
                requestId,
                requestName,
                elapsedMs,
                PerformanceThresholdMs);
        }
    }

    private bool LogError(string requestId, string requestName, long elapsedMs, Exception ex)
    {
        _logger.LogError(
            ex,
            "[ERROR] [{TraceId}] {RequestName} failed after {ElapsedMilliseconds}ms",
            requestId,
            requestName,
            elapsedMs);

        // Retorna false para que a exceção seja sempre relançada
        return false;
    }

    #endregion
} // Corrigido SA1508: Removida linha em branco antes da chave de fechamento
