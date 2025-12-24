// ============================================================================
// ARQUIVO ATUALIZADO - FASE 2: src/Identity/Application/Services/IJwtService.cs
// ============================================================================
// ALTERAÇÕES:
// 1. Adicionado parâmetro UserPermissionsDto no GenerateAccessToken
// 2. Adicionado parâmetro deviceName no GenerateRefreshTokenAsync
// ============================================================================

using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Identity.Core.Entities;

namespace RhSensoERP.Identity.Application.Services;

/// <summary>
/// Serviço para geração e validação de tokens JWT.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Gera um token de acesso JWT.
    /// ✅ ATUALIZADO - FASE 2: Incluído parâmetro de permissões.
    /// </summary>
    /// <param name="usuario">Dados do usuário</param>
    /// <param name="userSecurity">Dados de segurança do usuário (opcional)</param>
    /// <param name="permissions">Permissões do usuário (opcional) - NOVO</param>
    /// <returns>Token JWT</returns>
    string GenerateAccessToken(
        Usuario usuario,
        UserSecurity? userSecurity = null,
        UserPermissionsDto? permissions = null);

    /// <summary>
    /// Gera um refresh token e o armazena no banco.
    /// ✅ CORRIGIDO: Adicionado parâmetro deviceName
    /// </summary>
    Task<string> GenerateRefreshTokenAsync(
        Guid userId,
        string ipAddress,
        string? deviceId = null,
        string? deviceName = null, // ✅ NOVO PARÂMETRO
        int? expirationDays = null,
        CancellationToken ct = default);

    /// <summary>
    /// Valida se um refresh token é válido.
    /// </summary>
    Task<bool> ValidateRefreshTokenAsync(
        string token,
        Guid userId,
        CancellationToken ct = default);

    /// <summary>
    /// Revoga um refresh token específico.
    /// </summary>
    Task RevokeRefreshTokenAsync(
        string token,
        string ipAddress,
        string? reason = null,
        CancellationToken ct = default);

    /// <summary>
    /// Revoga todos os refresh tokens de um usuário.
    /// </summary>
    Task RevokeAllUserTokensAsync(
        Guid userId,
        string ipAddress,
        string? reason = null,
        CancellationToken ct = default);
}
