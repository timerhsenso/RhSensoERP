// =============================================================================
// RHSENSOERP - SHARED CORE
// =============================================================================
// Arquivo: src/Shared/RhSensoERP.Shared.Core/Common/ResultExtensions.cs
// Descrição: Extensões para a classe Result existente
// NOTA: NÃO sobrescreve Result.cs, apenas adiciona funcionalidades
// =============================================================================

namespace RhSensoERP.Shared.Core.Common;

/// <summary>
/// Extensões para a classe Result.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converte um Result para Result de T.
    /// </summary>
    public static Result<T> ToResult<T>(this Result result, T value)
    {
        return result.IsSuccess
            ? Result<T>.Success(value)
            : Result<T>.Failure(result.Error);
    }

    /// <summary>
    /// Executa uma ação se o resultado for sucesso.
    /// </summary>
    public static Result OnSuccess(this Result result, Action action)
    {
        if (result.IsSuccess)
            action();
        return result;
    }

    /// <summary>
    /// Executa uma ação assíncrona se o resultado for sucesso.
    /// </summary>
    public static async Task<Result> OnSuccessAsync(this Result result, Func<Task> action)
    {
        if (result.IsSuccess)
            await action();
        return result;
    }

    /// <summary>
    /// Executa uma ação se o resultado for falha.
    /// </summary>
    public static Result OnFailure(this Result result, Action<Error> action)
    {
        if (!result.IsSuccess)
            action(result.Error);
        return result;
    }

    /// <summary>
    /// Mapeia o valor de um Result de T para outro tipo.
    /// </summary>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper)
    {
        return result.IsSuccess && result.Value is not null
            ? Result<TOut>.Success(mapper(result.Value))
            : Result<TOut>.Failure(result.Error);
    }

    /// <summary>
    /// Mapeia o valor de um Result de T para outro Result de TOut.
    /// </summary>
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> mapper)
    {
        return result.IsSuccess && result.Value is not null
            ? mapper(result.Value)
            : Result<TOut>.Failure(result.Error);
    }

    /// <summary>
    /// Retorna o valor ou um valor padrão se falha.
    /// </summary>
    public static T GetValueOrDefault<T>(this Result<T> result, T defaultValue = default!)
    {
        return result.IsSuccess && result.Value is not null
            ? result.Value
            : defaultValue;
    }

    /// <summary>
    /// Combina múltiplos Results em um único.
    /// </summary>
    public static Result Combine(params Result[] results)
    {
        foreach (var result in results)
        {
            if (!result.IsSuccess)
                return result;
        }
        return Result.Success();
    }

    /// <summary>
    /// Combina múltiplos Results em um único (versão assíncrona).
    /// </summary>
    public static async Task<Result> CombineAsync(params Task<Result>[] tasks)
    {
        var results = await Task.WhenAll(tasks);
        return Combine(results);
    }
}

/// <summary>
/// Métodos de fábrica adicionais para Result.
/// </summary>
public static class ResultFactory
{
    /// <summary>
    /// Cria um Result de sucesso a partir de uma condição.
    /// </summary>
    public static Result SuccessIf(bool condition, Error error)
    {
        return condition ? Result.Success() : Result.Failure(error);
    }

    /// <summary>
    /// Cria um Result de falha a partir de uma condição.
    /// </summary>
    public static Result FailureIf(bool condition, Error error)
    {
        return condition ? Result.Failure(error) : Result.Success();
    }

    /// <summary>
    /// Cria um Result a partir de uma exceção.
    /// </summary>
    public static Result FromException(Exception ex)
    {
        return Result.Failure(Error.Failure("Exception", ex.Message)); // ✅ CORRIGIDO
    }

    /// <summary>
    /// Cria um Result de T a partir de um valor nullable.
    /// </summary>
    public static Result<T> FromNullable<T>(T? value, Error errorIfNull) where T : class
    {
        return value is not null
            ? Result<T>.Success(value)
            : Result<T>.Failure(errorIfNull);
    }

    /// <summary>
    /// Executa uma função e captura exceções como Result.
    /// </summary>
    public static Result Try(Action action)
    {
        try
        {
            action();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return FromException(ex);
        }
    }

    /// <summary>
    /// Executa uma função e captura exceções como Result de T.
    /// </summary>
    public static Result<T> Try<T>(Func<T> func)
    {
        try
        {
            return Result<T>.Success(func());
        }
        catch (Exception ex)
        {
            return Result<T>.Failure("Exception", ex.Message);
        }
    }

    /// <summary>
    /// Executa uma função assíncrona e captura exceções como Result.
    /// </summary>
    public static async Task<Result> TryAsync(Func<Task> action)
    {
        try
        {
            await action();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return FromException(ex);
        }
    }

    /// <summary>
    /// Executa uma função assíncrona e captura exceções como Result de T.
    /// </summary>
    public static async Task<Result<T>> TryAsync<T>(Func<Task<T>> func)
    {
        try
        {
            var result = await func();
            return Result<T>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure("Exception", ex.Message);
        }
    }
}