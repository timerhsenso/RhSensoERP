namespace RhSensoERP.Shared.Infrastructure.Services;

using RhSensoERP.Shared.Core.Abstractions;

/// <summary>
/// Implementação do IDateTimeProvider.
/// </summary>
public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}