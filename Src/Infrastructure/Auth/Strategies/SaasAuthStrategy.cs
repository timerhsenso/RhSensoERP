using Microsoft.IdentityModel.Tokens;
using RhSensoERP.Core.Security.Entities;
using RhSensoERP.Core.Security.SaaS;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RhSensoERP.Infrastructure.Security;

public class SaasAuthStrategy
{
    private readonly string _jwtSecret;
    private readonly string _issuer;
    private readonly string _audience;

    public SaasAuthStrategy(string jwtSecret, string issuer, string audience)
    {
        _jwtSecret = jwtSecret ?? throw new ArgumentNullException(nameof(jwtSecret));
        _issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
        _audience = audience ?? throw new ArgumentNullException(nameof(audience));
    }

    /// <summary>
    /// Cria hash da senha com salt único
    /// </summary>
    public (string passwordHash, string salt) CreatePasswordHash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Senha não pode ser vazia", nameof(password));

        var salt = GenerateSalt();
        var hash = HashPassword(password, salt);

        return (hash, salt);
    }

    /// <summary>
    /// Valida se a senha fornecida corresponde ao hash armazenado
    /// </summary>
    public bool IsPasswordValid(string password, string storedHash, string salt)
    {
        if (string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(storedHash) ||
            string.IsNullOrWhiteSpace(salt))
            return false;

        var computedHash = HashPassword(password, salt);
        return storedHash.Equals(computedHash, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gera token JWT para o usuário autenticado
    /// </summary>
    public string GenerateToken(SaasUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim("tenant_id", user.TenantId.ToString()),
            new Claim("user_type", "saas")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Gera refresh token único
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// Valida se o token JWT é válido
    /// </summary>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gera salt único para hash de senha
    /// </summary>
    private static string GenerateSalt()
    {
        var saltBytes = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    /// <summary>
    /// Cria hash da senha usando PBKDF2
    /// </summary>
    private static string HashPassword(string password, string salt)
    {
        const int iterations = 10000;
        const int hashLength = 32;

        var saltBytes = Convert.FromBase64String(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256);
        var hashBytes = pbkdf2.GetBytes(hashLength);

        return Convert.ToBase64String(hashBytes);
    }
}