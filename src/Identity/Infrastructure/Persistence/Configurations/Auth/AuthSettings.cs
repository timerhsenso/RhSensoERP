namespace RhSensoERP.Identity.Application.Configuration;

/// <summary>
/// Configurações de estratégias de autenticação.
/// </summary>
public sealed class AuthSettings
{
    public string DefaultStrategy { get; set; } = "Legado";
    public bool AllowMultipleStrategies { get; set; } = true;

    // ✅ CRÍTICO: Inicializar o Dictionary para evitar null
    public Dictionary<string, StrategyConfig> Strategies { get; set; } = new();
}

public sealed class StrategyConfig
{
    public bool Enabled { get; set; }
    public bool UseBCrypt { get; set; }
    public bool SyncWithUserSecurity { get; set; }
    public bool RequireEmailConfirmation { get; set; }
    public bool Require2FA { get; set; }
    public string? Domain { get; set; }
    public string? LdapPath { get; set; }
}