using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using RhSensoERP.Application.Auth;
using RhSensoERP.Application.Security.Auth.Services;
using RhSensoERP.Core.Security.Auth;
using RhSensoERP.Infrastructure.Auth;
using RhSensoERP.Infrastructure.Auth.Strategies;

namespace RhSensoERP.API.Configuration;

public static class AuthExtensions
{
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<AuthOptions>(cfg.GetSection("Auth"));
        services.Configure<JwtOptions>(cfg.GetSection("Jwt"));

        var jwt = cfg.GetSection("Jwt").Get<JwtOptions>() ?? new();

        SecurityKey key;
        if (string.IsNullOrEmpty(jwt.PublicKeyPem) || jwt.PublicKeyPem.Contains("...paste"))
        {
            var secretKey = jwt.SecretKey ?? "RhSensoERP-Super-Secret-Key-For-Development-Only-2024!";
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }
        else
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(jwt.PublicKeyPem);
            key = new RsaSecurityKey(rsa);
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = key,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,

                    // Mapear "sub" como nome do usuário
                    NameClaimType = "sub",
                    RoleClaimType = "role"
                };
            });

        services.AddAuthorization();
        services.AddSingleton<PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicyProvider>();
        services.AddSingleton<JwtTokenService>();

        services.AddScoped<IAuthStrategy, OnPremAuthStrategy>();
        services.AddScoped<IAuthStrategy, SaasAuthStrategy>();
        services.AddScoped<IAuthStrategy, WindowsAuthStrategy>();
        services.AddScoped<IAuthStrategyFactory, AuthStrategyFactory>();
        services.AddScoped<ILegacyAuthService, RhSensoERP.Infrastructure.Services.LegacyAuthService>();
        services.AddScoped<IAuthenticationService, RhSensoERP.Infrastructure.Services.AuthenticationService>();

        return services;
    }
}