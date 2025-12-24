// ============================================================================
// src/API/Configuration/RateLimitSettings.cs
// ============================================================================
// Classes de configuração para Rate Limiting extraídas do appsettings.json.
// 
// ✅ CORREÇÃO IMPLEMENTADA:
// Anteriormente, todas as configurações de rate limiting estavam hardcoded
// em RateLimitingConfiguration.cs. Agora usamos o Options Pattern para
// permitir configuração flexível por ambiente.
// 
// ESTRUTURA:
// - RateLimitSettings: classe raiz que mapeia a seção "RateLimit"
// - GlobalRateLimitRule: configuração do limite global
// - RateLimitPolicy: configuração de políticas específicas por endpoint
// 
// BENEFÍCIOS:
// - Configuração por ambiente (dev, staging, prod)
// - Ajustes sem necessidade de recompilação
// - Resposta rápida a ataques ajustando limites
// - Testabilidade (fácil mockar em testes unitários)
// ============================================================================

namespace RhSensoERP.API.Configuration;

/// <summary>
/// Configurações de Rate Limiting extraídas do appsettings.json.
/// Mapeia a seção "RateLimit" do arquivo de configuração.
/// </summary>
public sealed class RateLimitSettings
{
    /// <summary>
    /// Configuração do limite global aplicado a todas as requisições
    /// que não possuem política específica.
    /// </summary>
    public GlobalRateLimitRule Global { get; set; } = new();

    /// <summary>
    /// Dicionário de políticas específicas por nome.
    /// Chave: nome da política (ex: "login", "refresh", "diagnostics")
    /// Valor: configuração da política
    /// </summary>
    public Dictionary<string, RateLimitPolicy> Policies { get; set; } = new();
}

/// <summary>
/// Configuração do limite global de requisições.
/// Aplica-se a todos os endpoints que não têm política específica.
/// </summary>
public sealed class GlobalRateLimitRule
{
    /// <summary>
    /// Número máximo de requisições permitidas na janela de tempo.
    /// Exemplo: 100 requisições por minuto
    /// </summary>
    public int PermitLimit { get; set; } = 100;

    /// <summary>
    /// Duração da janela de tempo em minutos.
    /// Exemplo: 1 = janela de 1 minuto
    /// </summary>
    public int WindowMinutes { get; set; } = 1;

    /// <summary>
    /// Tipo de janela de tempo:
    /// - "Fixed": janela fixa (reseta completamente a cada período)
    /// - "Sliding": janela deslizante (mais preciso, considera histórico)
    /// </summary>
    public string WindowType { get; set; } = "Fixed";
}

/// <summary>
/// Configuração de uma política de rate limiting específica.
/// Usada para endpoints críticos que precisam de limites customizados.
/// </summary>
public sealed class RateLimitPolicy
{
    /// <summary>
    /// Número máximo de requisições permitidas na janela de tempo.
    /// Exemplo: 5 tentativas de login a cada 5 minutos
    /// </summary>
    public int PermitLimit { get; set; }

    /// <summary>
    /// Duração da janela de tempo em minutos.
    /// Exemplo: 5 = janela de 5 minutos
    /// </summary>
    public int WindowMinutes { get; set; }

    /// <summary>
    /// Tipo de janela de tempo:
    /// - "Fixed": janela fixa (reseta completamente a cada período)
    /// - "Sliding": janela deslizante (mais preciso, considera histórico)
    /// 
    /// Recomendação:
    /// - Use "Sliding" para proteção contra brute force (mais preciso)
    /// - Use "Fixed" para limites gerais (mais performático)
    /// </summary>
    public string WindowType { get; set; } = "Fixed";

    /// <summary>
    /// Número de segmentos da janela deslizante (apenas para WindowType = "Sliding").
    /// Define a granularidade da janela deslizante.
    /// 
    /// Exemplo: WindowMinutes = 5, SegmentsPerWindow = 5
    /// Cada segmento representa 1 minuto, permitindo controle mais fino.
    /// 
    /// Padrão: 5 segmentos
    /// </summary>
    public int SegmentsPerWindow { get; set; } = 5;
}
