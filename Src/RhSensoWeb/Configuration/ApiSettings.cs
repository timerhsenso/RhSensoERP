namespace RhSensoWeb.Configuration;

/// <summary>
/// Configurações para comunicação com a API RhSensoERP
/// </summary>
public class ApiSettings
{
    public const string SectionName = "ApiSettings";

    /// <summary>
    /// URL base da API (ex: https://localhost:57148/api/v1/)
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Timeout em segundos para requisições HTTP
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Número de tentativas em caso de falha
    /// </summary>
    public int RetryAttempts { get; set; } = 3;

    /// <summary>
    /// Intervalo entre tentativas em milissegundos
    /// </summary>
    public int RetryDelayMs { get; set; } = 1000;

    /// <summary>
    /// Habilitar logs detalhados das requisições
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = false;

    /// <summary>
    /// Headers customizados para todas as requisições
    /// </summary>
    public Dictionary<string, string> DefaultHeaders { get; set; } = new();
}
