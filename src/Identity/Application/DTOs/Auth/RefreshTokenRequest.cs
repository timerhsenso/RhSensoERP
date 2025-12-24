namespace RhSensoERP.Identity.Application.DTOs.Auth;

/// <summary>
/// Request para renovação de token via refresh token.
/// </summary>
public sealed class RefreshTokenRequest
{
    /// <summary>Access Token expirado ou próximo de expirar.</summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>Refresh Token válido.</summary>
    public string RefreshToken { get; init; } = string.Empty;
}