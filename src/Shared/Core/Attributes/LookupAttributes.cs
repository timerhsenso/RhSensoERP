// src/Shared/RhSensoERP.Shared.Core/Attributes/LookupAttributes.cs

using System;

namespace RhSensoERP.Shared.Core.Attributes;

/// <summary>
/// Marca a propriedade como chave primária para Lookup (id do Select2).
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class LookupKeyAttribute : Attribute
{
}

/// <summary>
/// Marca a propriedade para aparecer no Lookup.
/// - Se AsColumn = false (padrão): concatena no campo "text"
/// - Se AsColumn = true: retorna como propriedade separada no JSON
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class LookupTextAttribute : Attribute
{
    /// <summary>
    /// Ordem de exibição quando concatenados em "text".
    /// Só funciona quando AsColumn = false.
    /// </summary>
    public int Order { get; set; } = 0;

    /// <summary>
    /// Separador entre campos quando concatenados.
    /// Padrão: " - "
    /// </summary>
    public string Separator { get; set; } = " - ";

    /// <summary>
    /// Formato do campo (ex: {0:dd/MM/yyyy}, {0:C}).
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Se true, retorna como coluna separada no JSON.
    /// Se false (padrão), concatena no campo "text".
    /// </summary>
    public bool AsColumn { get; set; } = false;

    /// <summary>
    /// Nome da propriedade no JSON quando AsColumn = true.
    /// Se não informado, usa nome da propriedade em camelCase.
    /// </summary>
    public string? ColumnName { get; set; }
}