// =============================================================================
// RHSENSOERP WEB - SCREEN LINK SERVICE INTERFACE
// =============================================================================
// Arquivo: src/Web/Security/IScreenLinkService.cs
// Descrição: Interface para geração de links mascarados (tokens criptografados)
// =============================================================================

namespace RhSensoERP.Web.Security;

/// <summary>
/// Service para gerar e resolver links mascarados.
/// Usa DataProtection para criptografar ScreenKeys em tokens.
/// Os links /s/{token} são permanentes e funcionam mesmo após restart.
/// </summary>
public interface IScreenLinkService
{
    /// <summary>
    /// Criptografa uma ScreenKey em um token Base64Url.
    /// </summary>
    string Protect(string screenKey);

    /// <summary>
    /// Descriptografa um token de volta para ScreenKey.
    /// </summary>
    /// <exception cref="CryptographicException">Se o token for inválido ou expirado.</exception>
    string Unprotect(string token);

    /// <summary>
    /// Gera uma URL mascarada completa: /s/{token}
    /// </summary>
    string BuildMaskedUrl(string screenKey);
}