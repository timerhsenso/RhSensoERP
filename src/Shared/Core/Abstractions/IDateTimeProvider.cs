namespace RhSensoERP.Shared.Core.Abstractions;

/// <summary>
/// Provedor de data e hora.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets a data e hora atual UTC.
    /// </summary>
    DateTime UtcNow { get; }
}
