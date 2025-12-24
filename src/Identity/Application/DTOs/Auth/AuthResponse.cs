namespace RhSensoERP.Identity.Application.DTOs.Auth;

/// <summary>
/// Response de autenticação com tokens e dados do usuário.
/// </summary>
public sealed record AuthResponse
{
    /// <summary>Access Token JWT.</summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>Refresh Token (UUID criptografado).</summary>
    public string RefreshToken { get; init; } = string.Empty;

    /// <summary>Tipo do token (sempre "Bearer").</summary>
    public string TokenType { get; init; } = "Bearer";

    /// <summary>Tempo de expiração do access token em segundos.</summary>
    public int ExpiresIn { get; init; }

    /// <summary>Data/hora de expiração do access token (UTC).</summary>
    public DateTime ExpiresAt { get; init; }

    /// <summary>Dados básicos do usuário autenticado.</summary>
    public UserInfoDto User { get; init; } = default!;
}