namespace RhSensoERP.Identity.Application.DTOs.Auth;

/// <summary>
/// Request para logout (revogação de refresh token).
/// </summary>
public sealed class LogoutRequest
{
    /// <summary>Refresh Token a ser revogado (opcional, revoga todos se não informado).</summary>
    public string? RefreshToken { get; init; }

    /// <summary>Revogar todos os tokens do usuário?</summary>
    public bool RevokeAllTokens { get; init; }
}