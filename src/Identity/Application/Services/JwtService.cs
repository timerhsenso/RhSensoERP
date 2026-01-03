// ============================================================================
// ARQUIVO ATUALIZADO - FASE 2: src/Identity/Application/Services/JwtService.cs
// ============================================================================
// ALTERAÇÕES:
// 1. Adicionado parâmetro UserPermissionsDto no GenerateAccessToken
// 2. Incluídos claims de grupos e permissões no token
// 3. ✅ CORRIGIDO: Claim "tenantId" em CamelCase + alias "IdSaas"
// ============================================================================

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RhSensoERP.Identity.Application.Configuration;
using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Identity.Core.Entities;
using RhSensoERP.Identity.Infrastructure.Persistence.Contexts;
using RhSensoERP.Shared.Core.Abstractions;

namespace RhSensoERP.Identity.Application.Services;

/// <summary>
/// Implementação do serviço JWT.
/// </summary>
public sealed class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IdentityDbContext _db;
    private readonly IDateTimeProvider _dateTimeProvider;

    public JwtService(
        IOptions<JwtSettings> jwtSettings,
        IdentityDbContext db,
        IDateTimeProvider dateTimeProvider)
    {
        _jwtSettings = jwtSettings.Value;
        _db = db;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <summary>
    /// Gera token de acesso JWT com claims do usuário e permissões.
    /// ✅ ATUALIZADO - FASE 2: Incluído parâmetro de permissões.
    /// </summary>
    public string GenerateAccessToken(
        Usuario usuario,
        UserSecurity? userSecurity = null,
        UserPermissionsDto? permissions = null)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),

            new("cdusuario", usuario.CdUsuario),
            new("dcusuario", usuario.DcUsuario),
            new(ClaimTypes.Name, usuario.DcUsuario),
            new(ClaimTypes.NameIdentifier, usuario.CdUsuario)
        };

        // Claims opcionais do usuário
        if (!string.IsNullOrWhiteSpace(usuario.Email_Usuario))
            claims.Add(new Claim(ClaimTypes.Email, usuario.Email_Usuario));

        if (!string.IsNullOrWhiteSpace(usuario.NoMatric))
            claims.Add(new Claim("nomatric", usuario.NoMatric));

        if (usuario.CdEmpresa.HasValue)
            claims.Add(new Claim("cdempresa", usuario.CdEmpresa.Value.ToString()));

        if (usuario.CdFilial.HasValue)
            claims.Add(new Claim("cdfilial", usuario.CdFilial.Value.ToString()));

        // ✅ CORRIGIDO: TenantId em CamelCase + alias
        if (usuario.TenantId.HasValue)
        {
            claims.Add(new Claim("tenantId", usuario.TenantId.Value.ToString()));
            claims.Add(new Claim("IdSaas", usuario.TenantId.Value.ToString()));
        }

        // Flags de segurança
        if (userSecurity != null)
        {
            claims.Add(new Claim("twofactor_enabled",
                userSecurity.TwoFactorEnabled.ToString().ToLower()));
            claims.Add(new Claim("must_change_password",
                userSecurity.MustChangePassword.ToString().ToLower()));
            claims.Add(new Claim("email_confirmed",
                userSecurity.EmailConfirmed.ToString().ToLower()));

            // SecurityStamp (para invalidação de tokens)
            if (!string.IsNullOrWhiteSpace(userSecurity.SecurityStamp))
                claims.Add(new Claim("security_stamp", userSecurity.SecurityStamp));
        }

        // ========================================================================
        // ✅ NOVO - FASE 2: CLAIMS DE PERMISSÕES
        // ========================================================================
        if (permissions != null)
        {
            // Adicionar grupos
            foreach (var grupo in permissions.Grupos)
            {
                claims.Add(new Claim("grupo", $"{grupo.CdSistema}:{grupo.CdGrUser}"));
            }

            // Adicionar permissões (formato: "funcao:acoes")
            foreach (var permissao in permissions.PermissionsForClaims)
            {
                claims.Add(new Claim("permissao", permissao));
            }

            // Adicionar contador de permissões (útil para debugging)
            claims.Add(new Claim("total_grupos", permissions.Grupos.Count.ToString()));
            claims.Add(new Claim("total_funcoes", permissions.Funcoes.Count.ToString()));
            claims.Add(new Claim("total_botoes", permissions.Botoes.Count.ToString()));
        }
        // ========================================================================

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey))
        {
            KeyId = "rhsenso-jwt-key"
        };

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = _dateTimeProvider.UtcNow
                .AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = creds,
            TokenType = "JWT"
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public async Task<string> GenerateRefreshTokenAsync(
        Guid userId,
        string ipAddress,
        string? deviceId = null,
        string? deviceName = null,
        int? expirationDays = null,
        CancellationToken ct = default)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var token = Convert.ToBase64String(randomBytes);

        var expiresAt = _dateTimeProvider.UtcNow
            .AddDays(expirationDays ?? _jwtSettings.RefreshTokenExpirationDays);

        var refreshToken = new RefreshToken(
            idUserSecurity: userId,
            tokenHash: token,
            expiresAt: expiresAt,
            createdByIp: ipAddress,
            deviceId: deviceId,
            deviceName: deviceName);

        _db.Set<RefreshToken>().Add(refreshToken);
        await _db.SaveChangesAsync(ct);

        return token;
    }

    public async Task<bool> ValidateRefreshTokenAsync(
        string token,
        Guid userId,
        CancellationToken ct = default)
    {
        var refreshToken = await _db.Set<RefreshToken>()
            .AsNoTracking()
            .FirstOrDefaultAsync(rt =>
                rt.TokenHash == token &&
                rt.IdUserSecurity == userId &&
                !rt.IsRevoked &&
                rt.ExpiresAt > _dateTimeProvider.UtcNow,
                ct);

        return refreshToken != null;
    }

    public async Task RevokeRefreshTokenAsync(
        string token,
        string ipAddress,
        string? reason = null,
        CancellationToken ct = default)
    {
        var refreshToken = await _db.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.TokenHash == token, ct);

        if (refreshToken == null)
            return;

        refreshToken.Revoke(ipAddress, reason ?? "Token revoked");

        await _db.SaveChangesAsync(ct);
    }

    public async Task RevokeAllUserTokensAsync(
        Guid userId,
        string ipAddress,
        string? reason = null,
        CancellationToken ct = default)
    {
        var tokens = await _db.Set<RefreshToken>()
            .Where(rt => rt.IdUserSecurity == userId && !rt.IsRevoked)
            .ToListAsync(ct);

        foreach (var token in tokens)
        {
            token.Revoke(ipAddress, reason ?? "All tokens revoked");
        }

        await _db.SaveChangesAsync(ct);
    }
}