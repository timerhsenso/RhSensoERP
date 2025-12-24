namespace RhSensoERP.Identity.Application.Configuration;

/// <summary>
/// Configurações de política de segurança extraídas do appsettings.json.
/// </summary>
public sealed class SecurityPolicySettings
{
    public int PasswordMinLength { get; set; } = 8;
    public bool PasswordRequireDigit { get; set; } = true;
    public bool PasswordRequireUppercase { get; set; } = true;
    public bool PasswordRequireLowercase { get; set; } = true;
    public bool PasswordRequireNonAlphanumeric { get; set; } = true;
    public int MaxFailedAccessAttempts { get; set; } = 5;
    public int LockoutDurationMinutes { get; set; } = 30;
    public int ResetFailedCountAfterMinutes { get; set; } = 15;
}