namespace RhSensoERP.Shared.Application.Exceptions;

/// <summary>
/// Exceção para recurso não encontrado.
/// </summary>
public sealed class NotFoundException : AppException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class.
    /// </summary>
    public NotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class.
    /// </summary>
    public NotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class.
    /// </summary>
    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class.
    /// </summary>
    public NotFoundException(string name, object key)
        : base($"Entidade \"{name}\" ({key}) não foi encontrada.")
    {
    }
}
