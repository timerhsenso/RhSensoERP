// =============================================================================
// RHSENSOERP WEB - API SETTINGS
// =============================================================================
// Arquivo: src/Web/Configuration/ApiSettings.cs
// Descrição: Configurações tipadas para conexão com a API REST
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Configuration;

/// <summary>
/// Configurações de conexão com a API REST do RhSensoERP.
/// Mapeada da seção "ApiSettings" do appsettings.json.
/// </summary>
public sealed class ApiSettings
{
    /// <summary>
    /// Nome da seção no appsettings.json.
    /// </summary>
    public const string SectionName = "ApiSettings";

    /// <summary>
    /// URL base da API REST.
    /// </summary>
    /// <example>https://localhost:7193</example>
    [Required(ErrorMessage = "ApiSettings:BaseUrl é obrigatório")]
    [Url(ErrorMessage = "ApiSettings:BaseUrl deve ser uma URL válida")]
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Timeout padrão para requisições HTTP em segundos.
    /// </summary>
    [Range(5, 300, ErrorMessage = "ApiSettings:TimeoutSeconds deve estar entre 5 e 300")]
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Timeout específico para operações de autenticação em segundos.
    /// Maior devido a operações de hash de senha.
    /// </summary>
    [Range(10, 300, ErrorMessage = "ApiSettings:AuthTimeoutSeconds deve estar entre 10 e 300")]
    public int AuthTimeoutSeconds { get; set; } = 60;

    /// <summary>
    /// Número máximo de tentativas de retry para falhas transitórias.
    /// </summary>
    [Range(0, 10, ErrorMessage = "ApiSettings:RetryCount deve estar entre 0 e 10")]
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Tempo base para espera entre retries em segundos.
    /// O tempo real aumenta exponencialmente: base * 2^tentativa.
    /// </summary>
    [Range(1, 30, ErrorMessage = "ApiSettings:RetryBaseDelaySeconds deve estar entre 1 e 30")]
    public int RetryBaseDelaySeconds { get; set; } = 2;

    /// <summary>
    /// Número de falhas consecutivas para abrir o circuit breaker.
    /// </summary>
    [Range(3, 20, ErrorMessage = "ApiSettings:CircuitBreakerThreshold deve estar entre 3 e 20")]
    public int CircuitBreakerThreshold { get; set; } = 5;

    /// <summary>
    /// Tempo que o circuit breaker permanece aberto em segundos.
    /// </summary>
    [Range(10, 300, ErrorMessage = "ApiSettings:CircuitBreakerDurationSeconds deve estar entre 10 e 300")]
    public int CircuitBreakerDurationSeconds { get; set; } = 30;

    /// <summary>
    /// Habilita logging detalhado de requisições HTTP.
    /// </summary>
    public bool EnableDetailedLogging { get; set; }

    /// <summary>
    /// User-Agent enviado nas requisições HTTP.
    /// </summary>
    public string UserAgent { get; set; } = "RhSensoERP.Web/3.0";
}