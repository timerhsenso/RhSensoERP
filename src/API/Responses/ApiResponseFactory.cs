// =============================================================================
// RHSENSOERP - SHARED API
// =============================================================================
// Arquivo: src/Shared/RhSensoERP.Shared.Api/Responses/ApiResponseFactory.cs
// Descrição: Factory para respostas padronizadas da API
// =============================================================================

using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Shared.Api.Responses;

/// <summary>
/// Resposta padrão da API.
/// </summary>
/// <typeparam name="T">Tipo dos dados</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Dados retornados.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Mensagem de sucesso ou erro.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Código do erro (se houver).
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Lista de erros de validação.
    /// </summary>
    public Dictionary<string, string[]>? ValidationErrors { get; set; }

    /// <summary>
    /// Timestamp da resposta.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Cria uma resposta de sucesso.
    /// </summary>
    public static ApiResponse<T> Ok(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    /// <summary>
    /// Cria uma resposta de erro.
    /// </summary>
    public static ApiResponse<T> Fail(string message, string? errorCode = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode
        };
    }

    /// <summary>
    /// Cria uma resposta de erro de validação.
    /// </summary>
    public static ApiResponse<T> ValidationFail(Dictionary<string, string[]> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = "Erro de validação",
            ErrorCode = "ValidationError",
            ValidationErrors = errors
        };
    }

    /// <summary>
    /// Cria uma resposta a partir de um Result.
    /// </summary>
    public static ApiResponse<T> FromResult(Result<T> result, string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value!, successMessage);
        }

        return Fail(result.Error.Message, result.Error.Code);
    }
}

/// <summary>
/// Resposta padrão sem dados.
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    /// <summary>
    /// Cria uma resposta de sucesso sem dados.
    /// </summary>
    public static ApiResponse Ok(string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    /// <summary>
    /// Cria uma resposta de erro.
    /// </summary>
    public new static ApiResponse Fail(string message, string? errorCode = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode
        };
    }

    /// <summary>
    /// Cria uma resposta a partir de um Result.
    /// </summary>
    public static ApiResponse FromResult(Result result, string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            return Ok(successMessage);
        }

        return Fail(result.Error.Message, result.Error.Code);
    }
}

/// <summary>
/// Extensões para converter Result em ActionResult.
/// </summary>
public static class ResultActionExtensions
{
    /// <summary>
    /// Converte um Result em ActionResult.
    /// </summary>
    public static ActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return new OkResult();
        }

        return result.Error.Code switch
        {
            "NotFound" => new NotFoundObjectResult(new { message = result.Error.Message }),
            "Validation" => new BadRequestObjectResult(new { message = result.Error.Message }),
            "Unauthorized" => new UnauthorizedObjectResult(new { message = result.Error.Message }),
            "Forbidden" => new ForbidResult(),
            _ => new BadRequestObjectResult(new { message = result.Error.Message, code = result.Error.Code })
        };
    }

    /// <summary>
    /// Converte um Result de T em ActionResult.
    /// </summary>
    public static ActionResult<T> ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value);
        }

        return result.Error.Code switch
        {
            "NotFound" => new NotFoundObjectResult(new { message = result.Error.Message }),
            "Validation" => new BadRequestObjectResult(new { message = result.Error.Message }),
            "Unauthorized" => new UnauthorizedObjectResult(new { message = result.Error.Message }),
            "Forbidden" => new ForbidResult(),
            _ => new BadRequestObjectResult(new { message = result.Error.Message, code = result.Error.Code })
        };
    }

    /// <summary>
    /// Converte um Result em ActionResult com ApiResponse.
    /// </summary>
    public static ActionResult<ApiResponse> ToApiResponse(this Result result, string? successMessage = null)
    {
        var response = ApiResponse.FromResult(result, successMessage);

        if (result.IsSuccess)
        {
            return new OkObjectResult(response);
        }

        return result.Error.Code switch
        {
            "NotFound" => new NotFoundObjectResult(response),
            _ => new BadRequestObjectResult(response)
        };
    }

    /// <summary>
    /// Converte um Result de T em ActionResult com ApiResponse.
    /// </summary>
    public static ActionResult<ApiResponse<T>> ToApiResponse<T>(this Result<T> result, string? successMessage = null)
    {
        var response = ApiResponse<T>.FromResult(result, successMessage);

        if (result.IsSuccess)
        {
            return new OkObjectResult(response);
        }

        return result.Error.Code switch
        {
            "NotFound" => new NotFoundObjectResult(response),
            _ => new BadRequestObjectResult(response)
        };
    }
}
