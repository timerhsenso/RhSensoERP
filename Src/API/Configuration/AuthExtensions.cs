using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using RhSensoERP.Application.Auth;
using RhSensoERP.Infrastructure.Auth;

namespace RhSensoERP.API.Configuration;

public static class AuthExtensions
{
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<JwtOptions>(cfg.GetSection("Jwt"));

        var jwt = cfg.GetSection("Jwt").Get<JwtOptions>() ?? new();
        
        // Para desenvolvimento, usar chave simétrica se as chaves PEM não estiverem configuradas
        SecurityKey key;
        if (string.IsNullOrEmpty(jwt.PublicKeyPem) || jwt.PublicKeyPem.Contains("...paste"))
        {
            // Usar chave simétrica para desenvolvimento
            var secretKey = jwt.SecretKey ?? "RhSensoERP-Super-Secret-Key-For-Development-Only-2024!";
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }
        else
        {
            // Usar chave RSA para produção
            using var rsa = RSA.Create();
            rsa.ImportFromPem(jwt.PublicKeyPem);
            key = new RsaSecurityKey(rsa);
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = key,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true
                };
            });

        services.AddAuthorization();

        services.AddSingleton<PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicyProvider>();
        services.AddSingleton<JwtTokenService>();
        return services;
    }
}
