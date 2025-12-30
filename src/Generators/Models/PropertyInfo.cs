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
}
