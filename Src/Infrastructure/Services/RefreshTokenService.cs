using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RhSensoERP.Application.Auth;
using RhSensoERP.Application.Security.Auth.DTOs;
using RhSensoERP.Application.Security.Auth.Services;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Infrastructure.Auth;
using RhSensoERP.Infrastructure.Persistence;

namespace RhSensoERP.Infrastructure.Services;

/// <summary>
/// Serviço para gerenciamento de Refresh Tokens
/// ✅ SEGURANÇA: Implementa token rotation e revogação
/// </summary>
public class RefreshTokenService : IRefreshTokenService
{
    private readonly AppDbContext _context;
    private readonly JwtTokenService _jwtTokenService;
    private readonly ILogger<RefreshTokenService> _logger;
    private readonly int _refreshTokenDays;

    public RefreshTokenService(
        AppDbContext context,
        JwtTokenService jwtTokenService,
        ILogger<RefreshTokenService> logger)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
        _refreshTokenDays = 7; // Padrão: 7 dias
    }

    public async Task<string> CreateRefreshTokenAsync(
        string userId,
        string accessTokenJti,
        string ipAddress,
        string? userAgent,
        CancellationToken ct = default)
    {
        var refreshTokenString = _jwtTokenService.GenerateRefreshToken();
        var tokenHash = _jwtTokenService.HashRefreshToken(refreshTokenString);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            CdUsuario = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenDays),
            CreatedAt = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            AccessTokenJti = accessTokenJti
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Refresh Token criado | User: {User} | TokenId: {TokenId} | ExpiresAt: {ExpiresAt}",
            userId, refreshToken.Id, refreshToken.ExpiresAt);

        return refreshTokenString;
    }

    public async Task<RefreshTokenValidationResult?> ValidateRefreshTokenAsync(
        string refreshToken,
        CancellationToken ct = default)
    {
        try
        {
            var tokenHash = _jwtTokenService.HashRefreshToken(refreshToken);

            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

            if (storedToken == null)
            {
                return new RefreshTokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Token não encontrado"
                };
            }

            if (storedToken.IsRevoked)
            {
                _logger.LogWarning(
                    "SECURITY_EVENT: REVOKED_TOKEN_REUSE | User: {User} | TokenId: {TokenId}",
                    storedToken.CdUsuario, storedToken.Id);

                return new RefreshTokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Token foi revogado"
                };
            }

            if (storedToken.IsExpired)
            {
                return new RefreshTokenValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Token expirado"
                };
            }

            return new RefreshTokenValidationResult
            {
                IsValid = true,
                UserId = storedToken.CdUsuario,
                ExpiresAt = storedToken.ExpiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar refresh token");
            return new RefreshTokenValidationResult
            {
                IsValid = false,
                ErrorMessage = "Erro ao validar token"
            };
        }
    }

    public async Task<string?> RotateRefreshTokenAsync(
        string oldRefreshToken,
        string ipAddress,
        string? userAgent,
        CancellationToken ct = default)
    {
        var tokenHash = _jwtTokenService.HashRefreshToken(oldRefreshToken);

        var oldToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

        if (oldToken == null || !oldToken.IsActive)
        {
            return null;
        }

        // Gerar novo refresh token
        var newRefreshTokenString = _jwtTokenService.GenerateRefreshToken();
        var newTokenHash = _jwtTokenService.HashRefreshToken(newRefreshTokenString);

        // Revogar token antigo
        oldToken.RevokedAt = DateTime.UtcNow;
        oldToken.RevokedBy = oldToken.CdUsuario;
        oldToken.ReplacedByToken = newTokenHash;

        // Criar novo token
        var newToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            CdUsuario = oldToken.CdUsuario,
            TokenHash = newTokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenDays),
            CreatedAt = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            AccessTokenJti = oldToken.AccessTokenJti
        };

        _context.RefreshTokens.Add(newToken);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Refresh Token rotacionado | User: {User} | OldTokenId: {OldId} | NewTokenId: {NewId}",
            oldToken.CdUsuario, oldToken.Id, newToken.Id);

        return newRefreshTokenString;
    }

    public async Task<bool> RevokeRefreshTokenAsync(
        string refreshToken,
        string revokedBy,
        string? replacedByToken = null,
        CancellationToken ct = default)
    {
        var tokenHash = _jwtTokenService.HashRefreshToken(refreshToken);

        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

        if (token == null || token.IsRevoked)
        {
            return false;
        }

        token.RevokedAt = DateTime.UtcNow;
        token.RevokedBy = revokedBy;
        token.ReplacedByToken = replacedByToken;

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Refresh Token revogado | User: {User} | TokenId: {TokenId} | RevokedBy: {RevokedBy}",
            token.CdUsuario, token.Id, revokedBy);

        return true;
    }

    public async Task<int> RevokeAllUserTokensAsync(
        string userId,
        CancellationToken ct = default)
    {
        var activeTokens = await _context.RefreshTokens
            .Where(rt => rt.CdUsuario == userId && rt.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedBy = userId;
        }

        await _context.SaveChangesAsync(ct);

        _logger.LogWarning(
            "Todos os tokens revogados | User: {User} | Count: {Count}",
            userId, activeTokens.Count);

        return activeTokens.Count;
    }

    public async Task<int> CleanupExpiredTokensAsync(CancellationToken ct = default)
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(ct);

        _context.RefreshTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Tokens expirados removidos | Count: {Count}",
            expiredTokens.Count);

        return expiredTokens.Count;
    }
}