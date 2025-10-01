using System.Text.Json.Serialization;

namespace RhSensoWeb.Models.Shared;

/// <summary>
/// Resposta padrão da API RhSensoERP
/// </summary>
/// <typeparam name="T">Tipo dos dados retornados</typeparam>
public class ApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    /// <summary>
    /// Cria uma resposta de sucesso
    /// </summary>
    public static ApiResponse<T> Ok(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Cria uma resposta de erro
    /// </summary>
    public static ApiResponse<T> Fail(string message, Dictionary<string, string[]>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Resposta da API sem dados específicos
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    /// <summary>
    /// Cria uma resposta de sucesso sem dados
    /// </summary>
    public static ApiResponse Ok(string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Cria uma resposta de erro sem dados
    /// </summary>
    public new static ApiResponse Fail(string message, Dictionary<string, string[]>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}
