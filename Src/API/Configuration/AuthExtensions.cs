// ============================================================================
// CORREÇÃO 1/9: AuthExtensions.cs - Remover Secrets Hardcoded
// ============================================================================
// Caminho: Src/API/Configuration/AuthExtensions.cs
// 
// INSTRUÇÕES:
// 1. Substitua o arquivo AuthExtensions.cs completo por este
// 2. Compile para verificar se não há erros
// 3. Configure User Secrets (veja comandos no final)
// 4. Me avise quando estiver OK para passar para a próxima correção
// ============================================================================

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

        // ✅ DETECTAR AMBIENTE (Production, Development, Staging)
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        var isProduction = environmentName.Equals("Production", StringComparison.OrdinalIgnoreCase);

        SecurityKey key;

        // ✅ PRODUÇÃO: Usar apenas RSA com chaves reais
        if (isProduction)
        {
            // ✅ VALIDAÇÃO RIGOROSA: Não aceitar chaves placeholder
            if (string.IsNullOrWhiteSpace(jwt.PublicKeyPem) ||
                jwt.PublicKeyPem.Contains("...paste") ||
                jwt.PublicKeyPem.Contains("TODO") ||
                jwt.PublicKeyPem.Contains("CHANGE") ||
                jwt.PublicKeyPem.Length < 100)
            {
                throw new InvalidOperationException(
                    "❌ ERRO CRÍTICO DE SEGURANÇA: Chaves RSA não configuradas para PRODUÇÃO.\n\n" +
                    "Configure via:\n" +
                    "1. Azure Key Vault (recomendado)\n" +
                    "2. Variáveis de Ambiente\n" +
                    "3. User Secrets (apenas desenvolvimento)\n\n" +
                    "NUNCA deixe chaves hardcoded em código!");
            }

            // ✅ Usar RSA em produção
            using var rsa = RSA.Create();
            rsa.ImportFromPem(jwt.PublicKeyPem);
            key = new RsaSecurityKey(rsa);

            Console.WriteLine("✅ JWT: Usando chaves RSA em PRODUÇÃO");
        }
        // ⚠️ DESENVOLVIMENTO: Permitir chave simétrica apenas em DEV
        else
        {
            // ✅ VALIDAÇÃO: Exigir que a chave seja configurada via User Secrets
            if (string.IsNullOrWhiteSpace(jwt.SecretKey))
            {
                throw new InvalidOperationException(
                    "❌ JWT SecretKey não configurado.\n\n" +
                    "Configure via User Secrets:\n" +
                    "dotnet user-secrets set \"Jwt:SecretKey\" \"SUA-CHAVE-AQUI-MINIMO-32-CARACTERES\"\n\n" +
                    "OU use RSA:\n" +
                    "dotnet user-secrets set \"Jwt:PublicKeyPem\" \"$(cat public_key.pem)\"\n" +
                    "dotnet user-secrets set \"Jwt:PrivateKeyPem\" \"$(cat private_key.pem)\"");
            }

            // ✅ VALIDAÇÃO: Tamanho mínimo de 32 caracteres (256 bits)
            if (jwt.SecretKey.Length < 32)
            {
                throw new InvalidOperationException(
                    $"❌ JWT SecretKey muito curta ({jwt.SecretKey.Length} caracteres).\n" +
                    "Mínimo requerido: 32 caracteres (256 bits) para segurança adequada.\n\n" +
                    "Gere uma chave forte:\n" +
                    "dotnet user-secrets set \"Jwt:SecretKey\" \"$(openssl rand -base64 32)\"");
            }

            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey));

            Console.WriteLine($"⚠️ JWT: Usando chave simétrica em DESENVOLVIMENTO (ambiente: {environmentName})");
            Console.WriteLine($"⚠️ JWT: Tamanho da chave: {jwt.SecretKey.Length} caracteres");
        }

        // ✅ VALIDAÇÃO: Issuer e Audience obrigatórios
        if (string.IsNullOrWhiteSpace(jwt.Issuer) || jwt.Issuer.Contains("TODO"))
        {
            throw new InvalidOperationException(
                "❌ JWT Issuer não configurado.\n" +
                "Configure: dotnet user-secrets set \"Jwt:Issuer\" \"https://api.rhsensoerp.com.br\"");
        }

        if (string.IsNullOrWhiteSpace(jwt.Audience) || jwt.Audience.Contains("TODO"))
        {
            throw new InvalidOperationException(
                "❌ JWT Audience não configurado.\n" +
                "Configure: dotnet user-secrets set \"Jwt:Audience\" \"https://app.rhsensoerp.com.br\"");
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

                    // ✅ SEGURANÇA: Remover tolerância de tempo (ClockSkew padrão = 5min)
                    ClockSkew = TimeSpan.Zero,

                    // Mapear "sub" como nome do usuário
                    NameClaimType = "sub",
                    RoleClaimType = "role"
                };

                // ✅ SEGURANÇA: Event handlers para auditoria
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<Program>>();

                        var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                        var path = context.HttpContext.Request.Path;

                        logger.LogWarning(
                            "SECURITY_EVENT: JWT_AUTH_FAILED | IP: {IP} | Path: {Path} | Error: {Error}",
                            ip, path, context.Exception.Message);

                        return Task.CompletedTask;
                    },

                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<Program>>();

                        var userName = context.Principal?.Identity?.Name ?? "unknown";

                        logger.LogDebug(
                            "JWT Token Validated: {User}",
                            userName);

                        return Task.CompletedTask;
                    },

                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<Program>>();

                        var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                        logger.LogWarning(
                            "SECURITY_EVENT: JWT_CHALLENGE | IP: {IP} | Path: {Path} | Error: {Error}",
                            ip, context.Request.Path, context.ErrorDescription ?? "No token provided");

                        return Task.CompletedTask;
                    }
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