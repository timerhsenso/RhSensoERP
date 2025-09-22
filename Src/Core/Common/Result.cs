namespace RhSensoERP.Core.Common;

/// <summary>
/// Representa o resultado de uma operação que pode ser bem-sucedida ou falhar
/// Implementação imutável seguindo princípios de Domain-Driven Design
/// </summary>
/// <typeparam name="T">Tipo do valor retornado em caso de sucesso</typeparam>
public class Result<T>
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida
    /// </summary>
    public bool IsSuccess { get; private init; }

    /// <summary>
    /// Indica se a operação falhou
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Valor retornado em caso de sucesso
    /// </summary>
    public T? Value { get; private init; }

    /// <summary>
    /// Mensagem de erro em caso de falha
    /// </summary>
    public string? ErrorMessage { get; private init; }

    /// <summary>
    /// Erros de validação por campo
    /// </summary>
    public Dictionary<string, List<string>>? ValidationErrors { get; private init; }

    /// <summary>
    /// Construtor privado para garantir uso dos métodos factory
    /// </summary>
    private Result(bool isSuccess, T? value, string? errorMessage, Dictionary<string, List<string>>? validationErrors)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
        ValidationErrors = validationErrors;
    }

    /// <summary>
    /// Cria um resultado de sucesso
    /// </summary>
    /// <param name="value">Valor de retorno</param>
    /// <returns>Resultado de sucesso</returns>
    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, null, null);
    }

    /// <summary>
    /// Cria um resultado de falha com mensagem de erro
    /// </summary>
    /// <param name="errorMessage">Mensagem de erro</param>
    /// <returns>Resultado de falha</returns>
    public static Result<T> Failure(string errorMessage)
    {
        return new Result<T>(false, default, errorMessage, null);
    }

    /// <summary>
    /// Cria um resultado de falha com erros de validação
    /// </summary>
    /// <param name="validationErrors">Erros de validação por campo</param>
    /// <returns>Resultado de falha</returns>
    public static Result<T> Failure(Dictionary<string, List<string>> validationErrors)
    {
        return new Result<T>(false, default, "Erro de validação", validationErrors);
    }

    /// <summary>
    /// Cria um resultado de falha com mensagem e erros de validação
    /// </summary>
    /// <param name="errorMessage">Mensagem de erro</param>
    /// <param name="validationErrors">Erros de validação</param>
    /// <returns>Resultado de falha</returns>
    public static Result<T> Failure(string errorMessage, Dictionary<string, List<string>> validationErrors)
    {
        return new Result<T>(false, default, errorMessage, validationErrors);
    }

    /// <summary>
    /// Conversão implícita de valor para Result
    /// </summary>
    /// <param name="value">Valor a ser convertido</param>
    public static implicit operator Result<T>(T value)
    {
        return Success(value);
    }

    /// <summary>
    /// Aplica uma função ao valor se o resultado for bem-sucedido
    /// </summary>
    /// <typeparam name="TNew">Tipo do novo valor</typeparam>
    /// <param name="func">Função a ser aplicada</param>
    /// <returns>Novo resultado</returns>
    public Result<TNew> Map<TNew>(Func<T, TNew> func)
    {
        if (IsFailure)
            return Result<TNew>.Failure(ErrorMessage!, ValidationErrors);

        try
        {
            var newValue = func(Value!);
            return Result<TNew>.Success(newValue);
        }
        catch (Exception ex)
        {
            return Result<TNew>.Failure($"Erro ao processar resultado: {ex.Message}");
        }
    }

    /// <summary>
    /// Aplica uma função que retorna Result ao valor se o resultado for bem-sucedido
    /// </summary>
    /// <typeparam name="TNew">Tipo do novo valor</typeparam>
    /// <param name="func">Função que retorna Result</param>
    /// <returns>Novo resultado</returns>
    public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> func)
    {
        if (IsFailure)
            return Result<TNew>.Failure(ErrorMessage!, ValidationErrors);

        try
        {
            return func(Value!);
        }
        catch (Exception ex)
        {
            return Result<TNew>.Failure($"Erro ao processar resultado: {ex.Message}");
        }
    }
}

/// <summary>
/// Resultado sem valor de retorno (apenas sucesso/falha)
/// </summary>
public class Result
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida
    /// </summary>
    public bool IsSuccess { get; private init; }

    /// <summary>
    /// Indica se a operação falhou
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Mensagem de erro em caso de falha
    /// </summary>
    public string? ErrorMessage { get; private init; }

    /// <summary>
    /// Erros de validação por campo
    /// </summary>
    public Dictionary<string, List<string>>? ValidationErrors { get; private init; }

    /// <summary>
    /// Construtor privado para garantir uso dos métodos factory
    /// </summary>
    private Result(bool isSuccess, string? errorMessage, Dictionary<string, List<string>>? validationErrors)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ValidationErrors = validationErrors;
    }

    /// <summary>
    /// Cria um resultado de sucesso
    /// </summary>
    /// <returns>Resultado de sucesso</returns>
    public static Result Success()
    {
        return new Result(true, null, null);
    }

    /// <summary>
    /// Cria um resultado de falha com mensagem de erro
    /// </summary>
    /// <param name="errorMessage">Mensagem de erro</param>
    /// <returns>Resultado de falha</returns>
    public static Result Failure(string errorMessage)
    {
        return new Result(false, errorMessage, null);
    }

    /// <summary>
    /// Cria um resultado de falha com erros de validação
    /// </summary>
    /// <param name="validationErrors">Erros de validação por campo</param>
    /// <returns>Resultado de falha</returns>
    public static Result Failure(Dictionary<string, List<string>> validationErrors)
    {
        return new Result(false, "Erro de validação", validationErrors);
    }
}