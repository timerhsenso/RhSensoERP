namespace RhSensoERP.Core.Shared;

public class ServiceResult<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? Message { get; private set; }

    private ServiceResult(bool isSuccess, T? value, string? message, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Value = value;
        Message = message;
        ErrorMessage = errorMessage;
    }

    public static ServiceResult<T> Success(T value, string? message = null)
    {
        return new ServiceResult<T>(true, value, message, null);
    }

    public static ServiceResult<T> Fail(string errorMessage)
    {
        return new ServiceResult<T>(false, default, null, errorMessage);
    }
}
