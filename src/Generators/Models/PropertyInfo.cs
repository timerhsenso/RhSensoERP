// =============================================================================
// RHSENSOERP GENERATOR - PROPERTY INFO MODEL
// =============================================================================
namespace RhSensoERP.Generators.Models;

/// <summary>
/// Informações de uma propriedade da Entity.
/// </summary>
public class PropertyInfo
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public string ColumnName { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public bool IsNullable { get; set; }
    public bool IsString { get; set; }
    public bool IsBool { get; set; }
    public bool IsNumeric { get; set; }
    public bool IsDateTime { get; set; }
    public bool IsGuid { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsRequired { get; set; }
    public bool IsReadOnly { get; set; }
    public bool IsNavigation { get; set; }
    public bool ExcludeFromDto { get; set; }

    // ✅ ADICIONADO
    public bool IsIdentity { get; set; }
    public string DefaultValue { get; set; } = "";
    public bool RequiredOnCreate { get; set; }

    public int? MaxLength { get; set; }
    public int? MinLength { get; set; }

    // ✅ ADICIONAR estas propriedades ao final da classe PropertyInfo existente:

    // =========================================================================
    // UNIQUE VALIDATION
    // =========================================================================

    /// <summary>
    /// Indica se a propriedade tem [Unique].
    /// </summary>
    public bool IsUnique { get; set; }

    /// <summary>
    /// Escopo da unicidade (Global ou Tenant).
    /// </summary>
    public string UniqueScope { get; set; } = "Tenant";

    /// <summary>
    /// Nome para exibição em erro de duplicata.
    /// </summary>
    public string UniqueDisplayName { get; set; } = "";

    /// <summary>
    /// Mensagem de erro customizada.
    /// </summary>
    public string UniqueErrorMessage { get; set; } = "";

    /// <summary>
    /// Se permite null na validação de unicidade.
    /// </summary>
    public bool UniqueAllowNull { get; set; } = true;

    // =========================================================================
    // LOOKUP (SELECT2)
    // =========================================================================

    /// <summary>
    /// Indica se a propriedade tem [LookupKey].
    /// </summary>
    public bool IsLookupKey { get; set; }

    /// <summary>
    /// Indica se a propriedade tem [LookupText].
    /// </summary>
    public bool IsLookupText { get; set; }

    /// <summary>
    /// Ordem de exibição quando concatenados em "text".
    /// </summary>
    public int LookupTextOrder { get; set; } = 0;

    /// <summary>
    /// Separador entre campos quando concatenados.
    /// </summary>
    public string LookupTextSeparator { get; set; } = " - ";

    /// <summary>
    /// Formato do campo (ex: {0:dd/MM/yyyy}, {0:C}).
    /// </summary>
    public string? LookupTextFormat { get; set; }

    /// <summary>
    /// Se true, retorna como coluna separada no JSON.
    /// </summary>
    public bool LookupAsColumn { get; set; } = false;

    /// <summary>
    /// Nome da propriedade no JSON quando AsColumn = true.
    /// </summary>
    public string? LookupColumnName { get; set; }

}
