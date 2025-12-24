// RhSensoERP.Identity/Domain/Entities/LoginAuditLog.cs

namespace RhSensoERP.Identity.Core.Entities;

/// <summary>
/// Log de auditoria de tentativas de login (sucesso e falha).
/// </summary>
public class LoginAuditLog
{
    public long Id { get; private set; }
    public Guid IdUserSecurity { get; private set; }
    public Guid? IdSaaS { get; private set; }

    public DateTime LoginAttemptAt { get; private set; }
    public bool IsSuccess { get; private set; }
    public string? FailureReason { get; private set; }

    public string IpAddress { get; private set; } = string.Empty;
    public string? UserAgent { get; private set; }
    public string? DeviceType { get; private set; }
    public string? Location { get; private set; }

    public bool TwoFactorUsed { get; private set; }
    public string? SessionId { get; private set; }

    // Navegação
    public virtual UserSecurity? UserSecurity { get; private set; }

    private LoginAuditLog() { } // EF Core

    public LoginAuditLog(
        Guid idUserSecurity,
        Guid? idSaaS,
        bool isSuccess,
        string ipAddress,
        string? userAgent = null,
        string? failureReason = null,
        bool twoFactorUsed = false,
        string? sessionId = null)
    {
        IdUserSecurity = idUserSecurity;
        IdSaaS = idSaaS;
        IsSuccess = isSuccess;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        FailureReason = failureReason;
        TwoFactorUsed = twoFactorUsed;
        SessionId = sessionId;
        LoginAttemptAt = DateTime.UtcNow;
    }

    public void SetDeviceInfo(string? deviceType, string? location)
    {
        DeviceType = deviceType;
        Location = location;
    }
}