// ============================================================================
// CORREÇÃO 2/9: JwtTokenService.cs - Implementar Refresh Tokens
// ============================================================================
// Caminho: Src/Infrastructure/Auth/JwtTokenService.cs
// 
// INSTRUÇÕES:
// 1. Substitua o arquivo JwtTokenService.cs completo por este
// 2. Crie a entidade RefreshToken (próximo arquivo)
// 3. Adicione ao DbContext (próximo arquivo)
// 4. Crie migration
// 5. Compile e teste
// ============================================================================

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RhSensoERP.Application.Auth;

namespace RhSensoERP.Infrastructure.Auth;

/// <summary>
/// Serviço para geração e validação de tokens JWT com suporte a Refresh Tokens
/// ✅ SEGURANÇA: Implementa tokens de curta duração + refresh tokens
/// </summary>
public class JwtTokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(
        IOptionsMonitor<JwtOptions> jwtOptions,
        ILogger<JwtTokenService> logger)
    {
        _jwtOptions = jwtOptions.CurrentValue;
        _logger = logger;
    }

    /// <summary>
    /// Cria um Access Token JWT de curta duração
    /// ✅ MELHORIA: Reduzido de 15min para valor configurável (padrão 5min)
    /// </summary>
    public string CreateAccessToken(string userId, Guid tenantId, IEnumerable<string> permissions)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = GetSigningKey();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ✅ ID único do token
            new Claim("tenant_id", tenantId.ToString()),
            new Claim("user_type", "onprem")
        };

        // Adicionar permissões como claims
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // ✅ SEGURANÇA: Log apenas o ID do token, nunca o token completo
        var jti = claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
        _logger.LogDebug(
            "Access Token criado | JTI: {TokenId} | User: {User} | ExpiresIn: {Minutes}min",
            jti, userId, _jwtOptions.AccessTokenMinutes);

        return tokenString;
    }

    /// <summary>
    /// Gera um Refresh Token criptograficamente seguro
    /// ✅ NOVO: Implementação de Refresh Tokens
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64]; // 512 bits
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        var refreshToken = Convert.ToBase64String(randomBytes);

        _logger.LogDebug("Refresh Token gerado | Length: {Length}", refreshToken.Length);

        return refreshToken;
    }

    /// <summary>
    /// Calcula o hash SHA256 do refresh token para armazenamento seguro
    /// ✅ SEGURANÇA: Nunca armazenar refresh tokens em texto puro
    /// </summary>
    public string HashRefreshToken(string refreshToken)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Valida se um token JWT é válido (sem verificar expiração)
    /// ✅ ÚTIL: Para verificar se refresh token está associado a token válido
    /// </summary>
    public bool ValidateToken(string token, out ClaimsPrincipal? principal)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = GetSigningKey();

            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // ✅ Não validar expiração (para refresh)
                ClockSkew = TimeSpan.Zero
            };

            principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Falha ao validar token: {Error}", ex.Message);
            principal = null;
            return false;
        }
    }

    /// <summary>
    /// Extrai o JTI (ID único) de um token JWT
    /// ✅ ÚTIL: Para logs e auditoria
    /// </summary>
    public string? GetTokenId(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Id;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Extrai o Subject (usuário) de um token JWT
    /// ✅ ÚTIL: Para identificar usuário em refresh
    /// </summary>
    public string? GetUserId(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Subject;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Extrai a data de expiração de um token JWT
    /// </summary>
    public DateTime? GetTokenExpiry(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Obtém a chave de assinatura baseada na configuração
    /// ✅ PRIVADO: Não expor chave
    /// </summary>
    private byte[] GetSigningKey()
    {
        // Em desenvolvimento: usar chave simétrica
        if (!string.IsNullOrEmpty(_jwtOptions.SecretKey))
        {
            return Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
        }

        // Em produção: deveria usar RSA (não implementado neste método)
        throw new InvalidOperationException(
            "Chave de assinatura não configurada. Configure Jwt:SecretKey via User Secrets.");
    }
}