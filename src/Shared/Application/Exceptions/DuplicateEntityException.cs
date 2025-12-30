using System;

namespace RhSensoERP.Shared.Application.Exceptions;

/// <summary>
/// Exception lançada quando há tentativa de inserir/atualizar registro duplicado.
/// </summary>
public class DuplicateEntityException : Exception
{
    public string PropertyName { get; }
    public object? PropertyValue { get; }
    public string EntityName { get; }

    public DuplicateEntityException(
        string entityName,
        string propertyName,
        object? propertyValue,
        string? customMessage = null)
        : base(customMessage ?? $"{propertyName} '{propertyValue}' já está cadastrado.")
    {
        EntityName = entityName;
        PropertyName = propertyName;
        PropertyValue = propertyValue;
    }
}