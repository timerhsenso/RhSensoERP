namespace RhSensoERP.Shared.Application.Exceptions;

/// <summary>
/// Exceção base da aplicação.
/// </summary>
public class AppException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppException"/> class.
    /// </summary>
    public AppException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppException"/> class.
    /// </summary>
    public AppException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppException"/> class.
    /// </summary>
    public AppException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppException"/> class.
    /// </summary>
    public AppException(string message, string details)
        : base(message)
    {
        Details = details;
    }

    /// <summary>
    /// Gets detalhes adicionais do erro.
    /// </summary>
    public string? Details { get; }
}
