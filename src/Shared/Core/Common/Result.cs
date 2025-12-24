namespace RhSensoERP.Shared.Core.Common;

/// <summary>
/// Representa o resultado de uma operação.
/// </summary>
public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
}

/// <summary>
/// Representa o resultado de uma operação com valor de retorno.
/// </summary>
public sealed class Result<T> : Result
{
    private Result(bool isSuccess, T? value, Error error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value) => new(true, value, Error.None);

    public static new Result<T> Failure(Error error) => new(false, default, error);

    /// <summary>
    /// Sobrecarga para manter compatibilidade com código legado.
    /// </summary>
    public static Result<T> Failure(string code, string message) =>
        new(false, default, Error.Failure(code, message)); // ✅ CORRIGIDO: usa Error.Failure()
}