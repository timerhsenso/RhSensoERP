using RhSensoERP.Shared.Contracts.Common;

namespace RhSensoERP.Shared.Core.Common;

/// <summary>
/// Factory para criação de respostas padronizadas da API.
/// </summary>
public static class ApiResponseFactory
{
    /// <summary>
    /// Cria uma resposta de sucesso com dados.
    /// </summary>
    public static ApiResponse<T> Success<T>(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    /// <summary>
    /// Cria uma resposta de falha.
    /// </summary>
    public static ApiResponse<T> Failure<T>(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Data = default,
            Message = message
        };
    }

    /// <summary>
    /// Cria uma resposta de sucesso sem dados.
    /// </summary>
    public static ApiResponse<object> Success(string? message = null)
    {
        return new ApiResponse<object>
        {
            Success = true,
            Data = null,
            Message = message
        };
    }

    /// <summary>
    /// Cria uma resposta de falha sem tipo específico.
    /// </summary>
    public static ApiResponse<object> Failure(string message)
    {
        return new ApiResponse<object>
        {
            Success = false,
            Data = null,
            Message = message
        };
    }
}

/// <summary>
/// Alias para facilitar uso no código.
/// </summary>
public static class ApiResponse
{
    public static ApiResponse<T> Ok<T>(T data, string? message = null)
        => ApiResponseFactory.Success(data, message);

    public static ApiResponse<T> Fail<T>(string message)
        => ApiResponseFactory.Failure<T>(message);

    public static ApiResponse<object> Ok(string? message = null)
        => ApiResponseFactory.Success(message);

    public static ApiResponse<object> Fail(string message)
        => ApiResponseFactory.Failure(message);
}