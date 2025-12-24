namespace RhSensoERP.Identity.Application.Configuration;

/// <summary>
/// Configurações de JWT extraídas do appsettings.json.
/// </summary>
public sealed class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 15;
    public int RefreshTokenExpirationDays { get; set; } = 7;
    public int ClockSkewMinutes { get; set; } = 5;
}