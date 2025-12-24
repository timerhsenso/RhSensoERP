using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Identity.Application.Services;

/// <summary>
/// Serviço de autenticação principal.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Autentica um usuário com credenciais.
    /// </summary>
    Task<Result<AuthResponse>> LoginAsync(
        LoginRequest request,
        string ipAddress,
        string? userAgent = null,
        CancellationToken ct = default);

    /// <summary>
    /// Renova tokens usando refresh token.
    /// </summary>
    Task<Result<AuthResponse>> RefreshTokenAsync(
        RefreshTokenRequest request,
        string ipAddress,
        CancellationToken ct = default);

    /// <summary>
    /// Logout do usuário (revoga refresh tokens).
    /// </summary>
    Task<Result<bool>> LogoutAsync(
        string userId,
        LogoutRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Valida senha do usuário (suporta múltiplas estratégias).
    /// </summary>
    Task<bool> ValidatePasswordAsync(
        string cdUsuario,
        string senha,
        string strategy,
        CancellationToken ct = default);
}