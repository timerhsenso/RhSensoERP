// src/API/Extensions/ServiceCollectionExtensions.cs

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RhSensoERP.Identity.Application.Configuration;

namespace RhSensoERP.API.Extensions;

/// <summary>
/// Extensões para configuração de serviços da API.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configura autenticação JWT.
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
            ?? throw new InvalidOperationException("JwtSettings não configurado no appsettings.json");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false; // ✅ Alterar para true em produção

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

                ClockSkew = TimeSpan.Zero,

                // ✅ FIX: Permitir tokens sem kid ou com kid específico
                TryAllIssuerSigningKeys = true,
                RequireSignedTokens = true,

                // ✅ Validar tempo de vida do token
                RequireExpirationTime = true,

                // ✅ Nome das claims
                NameClaimType = "dcusuario",
                RoleClaimType = "role"
            };

            // ✅ Eventos personalizados
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        context.Response.Headers.Append("Token-Expired", "true");
                        context.Response.Headers.Append(
                            "Access-Control-Expose-Headers",
                            "Token-Expired");
                    }
                    return Task.CompletedTask;
                },

                OnChallenge = context =>
                {
                    // Log de falha de autenticação
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<Program>>();

                    logger.LogWarning(
                        "Falha na autenticação JWT: {Error} - {ErrorDescription}",
                        context.Error,
                        context.ErrorDescription);

                    return Task.CompletedTask;
                },

                OnTokenValidated = context =>
                {
                    // Token validado com sucesso
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<Program>>();

                    var cdUsuario = context.Principal?.FindFirst("cdusuario")?.Value;

                    logger.LogDebug(
                        "Token JWT validado para usuário: {CdUsuario}",
                        cdUsuario);

                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    /// <summary>
    /// Configura autorização com políticas customizadas.
    /// </summary>
    public static IServiceCollection AddCustomAuthorization(
        this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Política padrão: usuário autenticado
            options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Política: Email confirmado
            options.AddPolicy("EmailConfirmado", policy =>
                policy.RequireClaim("email_confirmed", "true"));

            // Política: 2FA habilitado
            options.AddPolicy("TwoFactorObrigatorio", policy =>
                policy.RequireClaim("twofactor_enabled", "true"));

            // Política: Usuário administrador (exemplo)
            options.AddPolicy("Admin", policy =>
                policy.RequireClaim("tpusuario", "A"));
        });

        return services;
    }
}