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

/// <summary>
/// Extensões para configuração de autenticação JWT e estratégias multi-modo
/// </summary>
public static class AuthExtensions
{
    /// <summary>
    /// Configura autenticação JWT com suporte a chaves RSA (produção) e simétricas (desenvolvimento)
    /// Adiciona suporte a estratégias de autenticação (OnPrem, SaaS, Windows)
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <param name="cfg">Configuração da aplicação</param>
    /// <returns>Coleção de serviços para encadeamento</returns>
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration cfg)
    {
        // Configurações de autenticação
        services.Configure<AuthOptions>(cfg.GetSection("Auth"));
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

        // Serviços de autorização existentes
        services.AddSingleton<PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, DynamicAuthorizationPolicyProvider>();
        services.AddSingleton<JwtTokenService>();

        // ========================================
        // NOVO: Estratégias de Autenticação
        // ========================================

        // Registrar todas as estratégias de autenticação
        services.AddScoped<IAuthStrategy, OnPremAuthStrategy>();
        services.AddScoped<IAuthStrategy, SaasAuthStrategy>();
        services.AddScoped<IAuthStrategy, WindowsAuthStrategy>();

        // Registrar o factory que resolve as estratégias
        services.AddScoped<IAuthStrategyFactory, AuthStrategyFactory>();

        // Manter o serviço legacy existente (usado pela estratégia OnPrem)
        services.AddScoped<ILegacyAuthService, RhSensoERP.Infrastructure.Services.LegacyAuthService>();

        // Registrar o serviço de autenticação unificado
        services.AddScoped<IAuthenticationService, RhSensoERP.Infrastructure.Services.AuthenticationService>();

        return services;
    }
}