// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool
// Modelos compartilhados compatíveis com backend
// Data: 2025-11-28 18:27:36
// =============================================================================

namespace RhSensoERP.Web.Models.Common;

/// <summary>
/// Resultado de operação (compatível com RhSensoERP.Shared.Core.Common.Result).
/// IMPORTANTE: Usa IsSuccess (não Success) para compatibilidade com backend.
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public Error? Error { get; set; }
    public string? Message { get; set; }

    public static Result<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };

    public static Result<T> Failure(Error error) => new()
    {
        IsSuccess = false,
        Error = error
    };
}

/// <summary>
/// Erro de operação.
/// </summary>
public class Error
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public ErrorType Type { get; set; }

    public static Error Failure(string code, string message) => new()
    {
        Code = code,
        Message = message,
        Type = ErrorType.Failure
    };

    public static Error NotFound(string code, string message) => new()
    {
        Code = code,
        Message = message,
        Type = ErrorType.NotFound
    };

    public static Error Validation(string code, string message) => new()
    {
        Code = code,
        Message = message,
        Type = ErrorType.Validation
    };

    public static Error Conflict(string code, string message) => new()
    {
        Code = code,
        Message = message,
        Type = ErrorType.Conflict
    };
}

/// <summary>
/// Tipo de erro.
/// </summary>
public enum ErrorType
{
    Failure,
    NotFound,
    Validation,
    Conflict
}

/// <summary>
/// Resultado paginado.
/// </summary>
/*
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}
*/
/// <summary>
/// Resultado de exclusão em lote para Sitc2.
/// Compatível com estrutura do backend.
/// </summary>
public class Sitc2BatchDeleteResult
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<Sitc2BatchDeleteError> Errors { get; set; } = new();
}

/// <summary>
/// Erro individual de exclusão em lote.
/// </summary>
public class Sitc2BatchDeleteError
{
    public string Id { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
