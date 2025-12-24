// ============================================================================
// src/API/Configuration/RateLimitingConfiguration.cs - REFATORADO
// ============================================================================
// Configuração de Rate Limiting para proteção contra abuse.
// 
// ✅ CORREÇÃO IMPLEMENTADA:
// ANTES: Todas as configurações estavam hardcoded neste arquivo
// DEPOIS: Lê configurações do appsettings.json via Options Pattern
// 
// MUDANÇAS:
// 1. Injeção de IOptions<RateLimitSettings> para ler configurações
// 2. Método auxiliar CreateLimiter para reduzir duplicação de código
// 3. Configuração dinâmica baseada no ambiente
// 4. Mantém valores default caso configuração não exista
// 
// BENEFÍCIOS:
// - Flexibilidade: ajustar limites sem recompilar
// - Ambiente-específico: limites diferentes em dev/staging/prod
// - Manutenibilidade: configuração centralizada em appsettings.json
// - Resposta rápida: ajustar limites durante ataques sem deploy
// ============================================================================

using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

namespace RhSensoERP.API.Configuration;

/// <summary>
/// Extensão para configuração de Rate Limiting da aplicação.
/// Protege contra DDoS, brute force e abuse de endpoints críticos.
/// </summary>
public static class RateLimitingConfiguration
{
    /// <summary>
    /// Adiciona e configura o Rate Limiting na aplicação.
    /// Lê configurações do appsettings.json via Options Pattern.
    /// </summary>
    /// <param name="services">Coleção de serviços do DI container</param>
    /// <returns>Coleção de serviços para encadeamento</returns>
    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // ====================================================================
            // CARREGAMENTO DE CONFIGURAÇÕES
            // ====================================================================
            // Obtém as configurações de rate limiting do appsettings.json
            // Se não existir, usa valores default seguros
            // ====================================================================
            var serviceProvider = services.BuildServiceProvider();
            var rateLimitSettings = serviceProvider.GetService<IOptions<RateLimitSettings>>()?.Value 
                                    ?? new RateLimitSettings();

            // ====================================================================
            // 1. RATE LIMIT GLOBAL POR IP
            // ====================================================================
            // Aplica-se a TODAS as requisições que não têm política específica.
            // Protege contra DDoS e abuse geral da API.
            //
            // ✅ AGORA CONFIGURÁVEL via appsettings.json:
            // {
            //   "RateLimit": {
            //     "Global": {
            //       "PermitLimit": 100,
            //       "WindowMinutes": 1,
            //       "WindowType": "Fixed"
            //     }
            //   }
            // }
            // ====================================================================
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var globalConfig = rateLimitSettings.Global;

                return CreateLimiter(
                    ipAddress,
                    globalConfig.PermitLimit,
                    globalConfig.WindowMinutes,
                    globalConfig.WindowType,
                    segmentsPerWindow: 1 // Global sempre usa janela simples
                );
            });

            // ====================================================================
            // 2. POLÍTICAS ESPECÍFICAS POR ENDPOINT
            // ====================================================================
            // Carrega políticas do appsettings.json e cria limitadores para cada uma.
            // Políticas padrão: "login", "refresh", "diagnostics"
            // ====================================================================

            // ====================================================================
            // POLÍTICA "login" - Proteção contra Brute Force
            // ====================================================================
            // Aplicada ao endpoint POST /api/identity/auth/login
            //
            // ✅ AGORA CONFIGURÁVEL via appsettings.json:
            // {
            //   "RateLimit": {
            //     "Policies": {
            //       "login": {
            //         "PermitLimit": 5,
            //         "WindowMinutes": 5,
            //         "WindowType": "Sliding",
            //         "SegmentsPerWindow": 5
            //       }
            //     }
            //   }
            // }
            //
            // Valores default (se não configurado):
            // - 5 tentativas a cada 5 minutos
            // - Janela deslizante com 5 segmentos
            // ====================================================================
            if (rateLimitSettings.Policies.TryGetValue("login", out var loginPolicy))
            {
                options.AddPolicy("login", context =>
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return CreateLimiter(
                        ipAddress,
                        loginPolicy.PermitLimit,
                        loginPolicy.WindowMinutes,
                        loginPolicy.WindowType,
                        loginPolicy.SegmentsPerWindow
                    );
                });
            }
            else
            {
                // Fallback: valores default seguros caso configuração não exista
                options.AddPolicy("login", context =>
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return CreateLimiter(ipAddress, 5, 5, "Sliding", 5);
                });
            }

            // ====================================================================
            // POLÍTICA "refresh" - Renovação de Tokens
            // ====================================================================
            // Aplicada ao endpoint POST /api/identity/auth/refresh-token
            //
            // ✅ AGORA CONFIGURÁVEL via appsettings.json
            //
            // Valores default (se não configurado):
            // - 20 requisições por minuto
            // - Janela fixa
            //
            // Justificativa: Refresh tokens são renovados automaticamente por
            // aplicações frontend, então precisa ser mais permissivo que login.
            // ====================================================================
            if (rateLimitSettings.Policies.TryGetValue("refresh", out var refreshPolicy))
            {
                options.AddPolicy("refresh", context =>
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return CreateLimiter(
                        ipAddress,
                        refreshPolicy.PermitLimit,
                        refreshPolicy.WindowMinutes,
                        refreshPolicy.WindowType,
                        refreshPolicy.SegmentsPerWindow
                    );
                });
            }
            else
            {
                // Fallback: valores default
                options.AddPolicy("refresh", context =>
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return CreateLimiter(ipAddress, 20, 1, "Fixed", 1);
                });
            }

            // ====================================================================
            // POLÍTICA "diagnostics" - Endpoints de Diagnóstico
            // ====================================================================
            // Aplicada aos endpoints:
            // - GET /api/diagnostics/*
            // - GET /api/security/metrics
            //
            // ✅ AGORA CONFIGURÁVEL via appsettings.json
            //
            // Valores default (se não configurado):
            // - 10 requisições a cada 5 minutos
            // - Janela fixa
            //
            // Justificativa: Endpoints de diagnóstico são usados raramente e
            // apenas por administradores. Limite baixo previne abuse e
            // scanning automatizado.
            // ====================================================================
            if (rateLimitSettings.Policies.TryGetValue("diagnostics", out var diagnosticsPolicy))
            {
                options.AddPolicy("diagnostics", context =>
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return CreateLimiter(
                        ipAddress,
                        diagnosticsPolicy.PermitLimit,
                        diagnosticsPolicy.WindowMinutes,
                        diagnosticsPolicy.WindowType,
                        diagnosticsPolicy.SegmentsPerWindow
                    );
                });
            }
            else
            {
                // Fallback: valores default
                options.AddPolicy("diagnostics", context =>
                {
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return CreateLimiter(ipAddress, 10, 5, "Fixed", 1);
                });
            }

            // ====================================================================
            // HANDLER DE REJEIÇÃO (429 TOO MANY REQUESTS)
            // ====================================================================
            // Executado quando uma requisição é rejeitada por rate limiting.
            // Retorna resposta JSON padronizada com informações úteis.
            // ====================================================================
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                // Tenta obter o tempo de retry dos metadados
                var retryAfter = context.Lease.GetAllMetadata()
                    .FirstOrDefault(m => m.Key == "RETRY_AFTER").Value;

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    error = "RATE_LIMIT_EXCEEDED",
                    message = "Muitas tentativas. Aguarde alguns minutos antes de tentar novamente.",
                    retryAfter = retryAfter
                }, token);
            };
        });

        return services;
    }

    // ========================================================================
    // MÉTODO AUXILIAR: CRIAÇÃO DE LIMITADORES
    // ========================================================================
    // Centraliza a lógica de criação de limitadores para evitar duplicação.
    // Suporta tanto janelas fixas quanto deslizantes.
    // ========================================================================

    /// <summary>
    /// Cria um limitador de taxa baseado nos parâmetros fornecidos.
    /// Suporta janelas fixas (Fixed) e deslizantes (Sliding).
    /// </summary>
    /// <param name="partitionKey">Chave de partição (geralmente IP do cliente)</param>
    /// <param name="permitLimit">Número máximo de requisições permitidas</param>
    /// <param name="windowMinutes">Duração da janela em minutos</param>
    /// <param name="windowType">Tipo de janela: "Fixed" ou "Sliding"</param>
    /// <param name="segmentsPerWindow">Número de segmentos (apenas para Sliding)</param>
    /// <returns>Partição de rate limiter configurada</returns>
    private static RateLimitPartition<string> CreateLimiter(
        string partitionKey,
        int permitLimit,
        int windowMinutes,
        string windowType,
        int segmentsPerWindow)
    {
        var window = TimeSpan.FromMinutes(windowMinutes);

        // Decide entre janela fixa ou deslizante baseado na configuração
        if (windowType.Equals("Sliding", StringComparison.OrdinalIgnoreCase))
        {
            // Janela deslizante: mais precisa, considera histórico de requisições
            return RateLimitPartition.GetSlidingWindowLimiter(partitionKey, _ =>
                new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit,
                    Window = window,
                    SegmentsPerWindow = segmentsPerWindow,
                    QueueLimit = 0 // Sem fila: rejeita imediatamente se exceder
                });
        }
        else
        {
            // Janela fixa: mais performático, reseta completamente a cada período
            return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ =>
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit,
                    Window = window,
                    QueueLimit = 0 // Sem fila: rejeita imediatamente se exceder
                });
        }
    }
}
