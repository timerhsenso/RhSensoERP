namespace RhSensoERP.Application.Security.Auth.DTOs;

/// <summary>
/// DTO para requisição de refresh token
/// </summary>
public record RefreshTokenRequestDto
{
    public string RefreshToken { get; init; } = string.Empty;
}

/// <summary>
/// DTO para resposta de refresh token
/// </summary>
public record RefreshTokenResponseDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public int ExpiresIn { get; init; } // em segundos
}

/// <summary>
/// DTO para requisição de revogação de token
/// </summary>
public record RevokeTokenRequestDto
{
    public string RefreshToken { get; init; } = string.Empty;
}

/// <summary>
/// Dados validados de um refresh token
/// </summary>
public record RefreshTokenValidationResult
{
    public bool IsValid { get; init; }
    public string? UserId { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTime? ExpiresAt { get; init; }
}