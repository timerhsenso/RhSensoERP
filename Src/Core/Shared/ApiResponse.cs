namespace RhSensoERP.Core.Shared;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }

    public static ApiResponse<T> Ok(T data, string? msg = null) => new() { Success = true, Data = data, Message = msg };
    public static ApiResponse<T> Fail(string msg) => new() { Success = false, Message = msg };
}
