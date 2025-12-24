// =============================================================================
// RHSENSOERP CRUD TOOL - CONFIGURATION MODELS
// Versão: 2.0
// =============================================================================
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhSensoERP.CrudTool.Models;

/// <summary>
/// Configuração raiz do arquivo crud-config.json
/// </summary>
public class CrudConfig
{
    [JsonPropertyName("solutionRoot")]
    public string SolutionRoot { get; set; } = ".";

    [JsonPropertyName("apiProject")]
    public string ApiProject { get; set; } = "src/RhSensoERP.API";

    [JsonPropertyName("webProject")]
    public string WebProject { get; set; } = "src/Web";

    [JsonPropertyName("entities")]
    public List<EntityConfig> Entities { get; set; } = new();
}

/// <summary>
/// Configuração de uma Entity para geração de CRUD
/// </summary>
public class EntityConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("pluralName")]
    public string PluralName { get; set; } = string.Empty;

    [JsonPropertyName("module")]
    public string Module { get; set; } = "Identity";

    /// <summary>
    /// Nome do módulo para rota da API (ex: "controledeponto")
    /// Se não informado, usa Module em lowercase
    /// </summary>
    [JsonPropertyName("moduleRoute")]
    public string? ModuleRoute { get; set; }

    /// <summary>
    /// Namespace completo do backend onde estão os DTOs
    /// Ex: "RhSensoERP.Modules.ControleDePonto.Application.DTOs.Sitc2s"
    /// </summary>
    [JsonPropertyName("backendNamespace")]
    public string? BackendNamespace { get; set; }

    [JsonPropertyName("tableName")]
    public string TableName { get; set; } = string.Empty;

    [JsonPropertyName("cdSistema")]
    public string CdSistema { get; set; } = string.Empty;

    [JsonPropertyName("cdFuncao")]
    public string CdFuncao { get; set; } = string.Empty;

    [JsonPropertyName("primaryKey")]
    public PrimaryKeyConfig PrimaryKey { get; set; } = new();

    [JsonPropertyName("properties")]
    public List<PropertyConfig> Properties { get; set; } = new();

    [JsonPropertyName("generate")]
    public GenerateConfig Generate { get; set; } = new();

    // Computed properties
    public string NameLower => Name.ToLower();
    public string PluralNameLower => PluralName.ToLower();
    public string ModuleLower => Module.ToLower();
    public string ModuleRouteLower => (ModuleRoute ?? Module).ToLower();
    
    /// <summary>
    /// Tipo da PK simplificado para uso em código (ex: "Guid" ao invés de "System.Guid")
    /// </summary>
    public string PkTypeSimple => PrimaryKey.Type.Replace("System.", "");
}

/// <summary>
/// Configuração da chave primária
/// </summary>
public class PrimaryKeyConfig
{
    [JsonPropertyName("property")]
    public string Property { get; set; } = "Id";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "System.Guid";

    [JsonPropertyName("column")]
    public string Column { get; set; } = "id";
    
    /// <summary>
    /// Tipo simplificado (sem namespace)
    /// </summary>
    public string TypeSimple => Type.Replace("System.", "");
}

/// <summary>
/// Configuração de uma propriedade
/// </summary>
public class PropertyConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = "string";

    [JsonPropertyName("column")]
    public string Column { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("maxLength")]
    public int? MaxLength { get; set; }

    [JsonPropertyName("minLength")]
    public int? MinLength { get; set; }

    [JsonPropertyName("required")]
    public bool Required { get; set; }

    [JsonPropertyName("isPrimaryKey")]
    public bool IsPrimaryKey { get; set; }

    [JsonPropertyName("isReadOnly")]
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Valor padrão - aceita string, bool, int, etc.
    /// </summary>
    [JsonPropertyName("defaultValue")]
    public JsonElement? DefaultValue { get; set; }

    /// <summary>
    /// Configuração de exibição em lista
    /// </summary>
    [JsonPropertyName("list")]
    public ListConfig? List { get; set; }

    /// <summary>
    /// Configuração de exibição em formulário
    /// </summary>
    [JsonPropertyName("form")]
    public FormConfig? Form { get; set; }

    /// <summary>
    /// Configuração de filtro
    /// </summary>
    [JsonPropertyName("filter")]
    public FilterConfig? Filter { get; set; }

    // Computed
    public bool IsNullable => Type.EndsWith("?");
    public string BaseType => Type.TrimEnd('?').Replace("System.", "");
    public bool IsString => BaseType.Equals("string", StringComparison.OrdinalIgnoreCase);
    public bool IsBool => BaseType.Equals("bool", StringComparison.OrdinalIgnoreCase);
    public bool IsInt => BaseType.Equals("int", StringComparison.OrdinalIgnoreCase);
    public bool IsLong => BaseType.Equals("long", StringComparison.OrdinalIgnoreCase);
    public bool IsDecimal => BaseType.Equals("decimal", StringComparison.OrdinalIgnoreCase);
    public bool IsDateTime => BaseType.Equals("DateTime", StringComparison.OrdinalIgnoreCase);
    public bool IsGuid => BaseType.Equals("Guid", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Tipo simplificado para uso em código C#
    /// </summary>
    public string TypeSimple => Type.Replace("System.", "");

    /// <summary>
    /// Verifica se há valor padrão definido
    /// </summary>
    public bool HasDefaultValue => DefaultValue.HasValue &&
                                   DefaultValue.Value.ValueKind != JsonValueKind.Null &&
                                   DefaultValue.Value.ValueKind != JsonValueKind.Undefined;

    /// <summary>
    /// Retorna o valor padrão formatado como código C#
    /// </summary>
    public string? GetDefaultValueAsCode()
    {
        if (!HasDefaultValue)
            return null;

        var value = DefaultValue!.Value;

        return value.ValueKind switch
        {
            JsonValueKind.String => $"\"{value.GetString()}\"",
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Number when IsInt => value.GetInt32().ToString(),
            JsonValueKind.Number when IsLong => $"{value.GetInt64()}L",
            JsonValueKind.Number when IsDecimal => $"{value.GetDecimal()}m",
            JsonValueKind.Number => value.GetDouble().ToString(System.Globalization.CultureInfo.InvariantCulture),
            _ => value.GetRawText()
        };
    }

    /// <summary>
    /// Retorna a declaração completa da propriedade para DTOs.
    /// Exemplo: "public string Nome { get; set; } = string.Empty;"
    /// </summary>
    public string GetPropertyDeclaration()
    {
        // Monta a parte do valor padrão
        string defaultPart;
        
        if (HasDefaultValue)
        {
            defaultPart = $" = {GetDefaultValueAsCode()};";
        }
        else if (IsString && !IsNullable)
        {
            // Strings não-nullable sem default usam string.Empty
            defaultPart = " = string.Empty;";
        }
        else
        {
            // Sem valor padrão
            defaultPart = "";
        }

        return $"public {TypeSimple} {Name} {{ get; set; }}{defaultPart}";
    }
}

/// <summary>
/// Configuração de exibição em lista
/// </summary>
public class ListConfig
{
    [JsonPropertyName("show")]
    public bool Show { get; set; } = true;

    [JsonPropertyName("order")]
    public int Order { get; set; }

    [JsonPropertyName("sortable")]
    public bool Sortable { get; set; } = true;

    [JsonPropertyName("align")]
    public string Align { get; set; } = "left";

    [JsonPropertyName("format")]
    public string Format { get; set; } = "text";

    [JsonPropertyName("width")]
    public string? Width { get; set; }
}

/// <summary>
/// Configuração de exibição em formulário
/// </summary>
public class FormConfig
{
    [JsonPropertyName("show")]
    public bool Show { get; set; } = true;

    [JsonPropertyName("showOnCreate")]
    public bool ShowOnCreate { get; set; } = true;

    [JsonPropertyName("showOnEdit")]
    public bool ShowOnEdit { get; set; } = true;

    [JsonPropertyName("order")]
    public int Order { get; set; }

    [JsonPropertyName("inputType")]
    public string InputType { get; set; } = "text";

    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    [JsonPropertyName("helpText")]
    public string? HelpText { get; set; }

    [JsonPropertyName("colSize")]
    public int ColSize { get; set; } = 6;

    [JsonPropertyName("rows")]
    public int Rows { get; set; } = 3;

    [JsonPropertyName("disabled")]
    public bool Disabled { get; set; }
}

/// <summary>
/// Configuração de filtro
/// </summary>
public class FilterConfig
{
    [JsonPropertyName("show")]
    public bool Show { get; set; }

    [JsonPropertyName("filterType")]
    public string FilterType { get; set; } = "text";

    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    [JsonPropertyName("order")]
    public int Order { get; set; }
}

/// <summary>
/// Flags de geração
/// </summary>
public class GenerateConfig
{
    [JsonPropertyName("apiController")]
    public bool ApiController { get; set; } = false; // Backend já gera via Source Generator

    [JsonPropertyName("webController")]
    public bool WebController { get; set; } = true;

    [JsonPropertyName("webModels")]
    public bool WebModels { get; set; } = true;

    [JsonPropertyName("webServices")]
    public bool WebServices { get; set; } = true;

    [JsonPropertyName("view")]
    public bool View { get; set; } = true;

    [JsonPropertyName("javascript")]
    public bool JavaScript { get; set; } = true;
}
