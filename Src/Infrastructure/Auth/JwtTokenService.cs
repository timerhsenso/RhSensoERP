using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RhSensoERP.Application.Auth;

namespace RhSensoERP.Infrastructure.Auth;

public class JwtTokenService
{
    private readonly JwtOptions _opt;
    public JwtTokenService(IOptions<JwtOptions> opt) => _opt = opt.Value;

    public string CreateAccessToken(string userId, Guid tenantId, IEnumerable<string> permissions)
    {
        // Determinar o tipo de chave e credenciais
        SigningCredentials creds;
        if (string.IsNullOrEmpty(_opt.PrivateKeyPem) || _opt.PrivateKeyPem.Contains("...dev only"))
        {
            // Usar chave simétrica para desenvolvimento
            var secretKey = _opt.SecretKey ?? "RhSensoERP-Super-Secret-Key-For-Development-Only-2024!";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }
        else
        {
            // Usar chave RSA para produção
            using var rsa = RSA.Create();
            rsa.ImportFromPem(_opt.PrivateKeyPem);
            creds = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new("tenant", tenantId.ToString()),
        };
        claims.AddRange(permissions.Select(p => new Claim("perm", p)));

        var token = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_opt.AccessTokenMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
