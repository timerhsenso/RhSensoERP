namespace RhSensoERP.Shared.Core.Common;

/// <summary>
/// Classe para validações e guard clauses.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Garante que o valor não é nulo.
    /// </summary>
    public static T Against<T>(
        T? value,
        string parameterName,
        string? message = null)
        where T : class
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName, message ?? $"Parameter {parameterName} cannot be null");
        }

        return value;
    }

    /// <summary>
    /// Garante que a string não é nula ou vazia.
    /// </summary>
    public static string AgainstNullOrEmpty(
        string? value,
        string parameterName,
        string? message = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(message ?? $"Parameter {parameterName} cannot be null or empty", parameterName);
        }

        return value;
    }
}
