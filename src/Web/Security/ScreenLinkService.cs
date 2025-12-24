// =============================================================================
// RHSENSOERP WEB - SCREEN LINK SERVICE
// =============================================================================
// Arquivo: src/Web/Security/ScreenLinkService.cs
// Descrição: Implementação de links mascarados usando ASP.NET Data Protection
// =============================================================================

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;

namespace RhSensoERP.Web.Security;

/// <summary>
/// Service para gerar links mascarados permanentes.
/// Usa ASP.NET Data Protection para criptografar ScreenKeys.
/// 
/// IMPORTANTE: As chaves de proteção são persistidas por padrão em:
/// - Windows: %LOCALAPPDATA%\ASP.NET\DataProtection-Keys
/// - Linux/Mac: ~/.aspnet/DataProtection-Keys
/// 
/// Para produção com múltiplas instâncias, configure storage compartilhado:
/// - Azure Blob Storage
/// - Redis
/// - SQL Server
/// </summary>
public sealed class ScreenLinkService : IScreenLinkService
{
    private const string Purpose = "RhSensoERP.ScreenLink.v1";
    private readonly IDataProtector _protector;
    private readonly ILogger<ScreenLinkService> _logger;

    public ScreenLinkService(IDataProtectionProvider provider, ILogger<ScreenLinkService> logger)
    {
        _protector = provider.CreateProtector(Purpose);
        _logger = logger;
    }

    public string Protect(string screenKey)
    {
        var protectedText = _protector.Protect(screenKey);
        var bytes = System.Text.Encoding.UTF8.GetBytes(protectedText);
        var token = WebEncoders.Base64UrlEncode(bytes);

        _logger.LogDebug("[SCREEN_LINK] Protect | ScreenKey: {ScreenKey} | TokenLength: {Length}", screenKey, token.Length);

        return token;
    }

    public string Unprotect(string token)
    {
        var bytes = WebEncoders.Base64UrlDecode(token);
        var protectedText = System.Text.Encoding.UTF8.GetString(bytes);
        var screenKey = _protector.Unprotect(protectedText);

        _logger.LogDebug("[SCREEN_LINK] Unprotect | Token: {Token}... | ScreenKey: {ScreenKey}",
            token[..Math.Min(20, token.Length)], screenKey);

        return screenKey;
    }

    public string BuildMaskedUrl(string screenKey)
        => $"/s/{Protect(screenKey)}";
}