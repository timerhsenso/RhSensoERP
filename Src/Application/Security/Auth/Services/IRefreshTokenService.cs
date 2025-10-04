using RhSensoERP.Application.Security.Auth.DTOs;

namespace RhSensoERP.Application.Security.Auth.Services;

/// <summary>
/// Interface para gerenciamento de Refresh Tokens
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// Cria um novo refresh token para o usuário
    /// </summary>
    Task<string> CreateRefreshTokenAsync(
        string userId,
        string accessTokenJti,
        string ipAddress,
        string? userAgent,
        CancellationToken ct = default);

    /// <summary>
    /// Valida um refresh token e retorna os dados do usuário
    /// </summary>
    Task<RefreshTokenValidationResult?> ValidateRefreshTokenAsync(
        string refreshToken,
        CancellationToken ct = default);

    /// <summary>
    /// Rotaciona um refresh token (revoga o antigo e cria um novo)
    /// ✅ SEGURANÇA: Implementa refresh token rotation
    /// </summary>
    Task<string?> RotateRefreshTokenAsync(
        string oldRefreshToken,
        string ipAddress,
        string? userAgent,
        CancellationToken ct = default);

    /// <summary>
    /// Revoga um refresh token específico
    /// </summary>
    Task<bool> RevokeRefreshTokenAsync(
        string refreshToken,
        string revokedBy,
        string? replacedByToken = null,
        CancellationToken ct = default);

    /// <summary>
    /// Revoga todos os refresh tokens de um usuário
    /// </summary>
    Task<int> RevokeAllUserTokensAsync(
        string userId,
        CancellationToken ct = default);

    /// <summary>
    /// Remove refresh tokens expirados do banco de dados
    /// </summary>
    Task<int> CleanupExpiredTokensAsync(CancellationToken ct = default);
}