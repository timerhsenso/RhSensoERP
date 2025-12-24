// src/Identity/Domain/Entities/SecurityAuditLog.cs

namespace RhSensoERP.Identity.Core.Entities;

/// <summary>
/// Log de auditoria de eventos de segurança do sistema.
/// ✅ FASE 5: Auditoria completa de eventos de segurança
/// </summary>
public class SecurityAuditLog
{
    public long Id { get; private set; }

    // Identificação do evento
    public string EventType { get; private set; } = string.Empty;
    public string EventCategory { get; private set; } = string.Empty;
    public string Severity { get; private set; } = string.Empty;

    // Timestamp
    public DateTime OccurredAt { get; private set; }

    // Usuário (se aplicável)
    public Guid? IdUserSecurity { get; private set; }
    public string? Username { get; private set; }

    // Contexto da requisição
    public string IpAddress { get; private set; } = string.Empty;
    public string? UserAgent { get; private set; }
    public string? RequestPath { get; private set; }
    public string? RequestMethod { get; private set; }

    // Detalhes do evento
    public string Description { get; private set; } = string.Empty;
    public string? AdditionalData { get; private set; }

    // Resultado
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }

    // Navegação
    public virtual UserSecurity? UserSecurity { get; private set; }

    private SecurityAuditLog() { } // EF Core

    public SecurityAuditLog(
        string eventType,
        string eventCategory,
        string severity,
        string description,
        string ipAddress,
        bool isSuccess,
        Guid? idUserSecurity = null,
        string? username = null,
        string? userAgent = null,
        string? requestPath = null,
        string? requestMethod = null,
        string? additionalData = null,
        string? errorMessage = null)
    {
        EventType = eventType;
        EventCategory = eventCategory;
        Severity = severity;
        Description = description;
        IpAddress = ipAddress;
        IsSuccess = isSuccess;
        IdUserSecurity = idUserSecurity;
        Username = username;
        UserAgent = userAgent;
        RequestPath = requestPath;
        RequestMethod = requestMethod;
        AdditionalData = additionalData;
        ErrorMessage = errorMessage;
        OccurredAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Tipos de eventos de segurança.
/// </summary>
public static class SecurityEventType
{
    public const string Login = "LOGIN";
    public const string Logout = "LOGOUT";
    public const string LoginFailed = "LOGIN_FAILED";
    public const string PasswordChanged = "PASSWORD_CHANGED";
    public const string PasswordResetRequested = "PASSWORD_RESET_REQUESTED";
    public const string PasswordResetCompleted = "PASSWORD_RESET_COMPLETED";
    public const string TwoFactorEnabled = "TWO_FACTOR_ENABLED";
    public const string TwoFactorDisabled = "TWO_FACTOR_DISABLED";
    public const string AccountLocked = "ACCOUNT_LOCKED";
    public const string AccountUnlocked = "ACCOUNT_UNLOCKED";
    public const string TokenRefreshed = "TOKEN_REFRESHED";
    public const string TokenRevoked = "TOKEN_REVOKED";
    public const string UnauthorizedAccess = "UNAUTHORIZED_ACCESS";
    public const string SuspiciousActivity = "SUSPICIOUS_ACTIVITY";
    public const string RateLimitExceeded = "RATE_LIMIT_EXCEEDED";
    public const string ConfigurationChanged = "CONFIGURATION_CHANGED";
}

/// <summary>
/// Categorias de eventos de segurança.
/// </summary>
public static class SecurityEventCategory
{
    public const string Authentication = "AUTHENTICATION";
    public const string Authorization = "AUTHORIZATION";
    public const string AccountManagement = "ACCOUNT_MANAGEMENT";
    public const string TokenManagement = "TOKEN_MANAGEMENT";
    public const string Security = "SECURITY";
    public const string Configuration = "CONFIGURATION";
}

/// <summary>
/// Níveis de severidade de eventos de segurança.
/// </summary>
public static class SecuritySeverity
{
    public const string Info = "INFO";
    public const string Warning = "WARNING";
    public const string Error = "ERROR";
    public const string Critical = "CRITICAL";
}
