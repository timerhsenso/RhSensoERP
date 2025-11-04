namespace RhSensoERP.Shared.Core;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
    public object? ValidationErrors { get; init; } // ADICIONAR ESTA LINHA

    public static ApiResponse<T> Ok(T data, string? msg = null) => new() { Success = true, Data = data, Message = msg };
    public static ApiResponse<T> Fail(string msg) => new() { Success = false, Message = msg };
    public static ApiResponse<T> Fail(string msg, object validationErrors) => new() { Success = false, Message = msg, ValidationErrors = validationErrors }; // ADICIONAR ESTA LINHA
}