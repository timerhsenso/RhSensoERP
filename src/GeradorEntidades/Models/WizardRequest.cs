// =============================================================================
// WIZARD REQUEST v4.0 - SELECT2 AJAX SUPPORT
// =============================================================================

using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeradorEntidades.Models;

public class WizardRequest
{
    // =========================================================================
    // IDENTIFICAÇÃO
    // =========================================================================

    public string EntityName { get; set; } = string.Empty;
    public string? TableName { get; set; }
    public string CdFuncao { get; set; } = string.Empty;
    public string CdSistema { get; set; } = "RHU";
    public string? DisplayName { get; set; }
    public string Icon { get; set; } = "fas fa-table";

    [JsonConverter(typeof(FlexibleIntConverter))]
    public int MenuOrder { get; set; } = 10;

    // =========================================================================
    // ⭐ v3.4: ROTA DA API DO MANIFESTO
    // =========================================================================

    [JsonPropertyName("apiRoute")]
    public string? ApiRoute { get; set; }

    // =========================================================================
    // ⭐ v3.5: MÓDULO EXPLÍCITO
    // =========================================================================

    [JsonPropertyName("modulo")]
    public string? Modulo { get; set; }

    // =========================================================================
    // OPÇÕES DE GERAÇÃO
    // =========================================================================

    public bool GerarEntidade { get; set; } = false;
    public bool GerarWebController { get; set; } = true;
    public bool GerarWebModels { get; set; } = true;
    public bool GerarWebServices { get; set; } = true;
    public bool GerarView { get; set; } = true;
    public bool GerarJavaScript { get; set; } = true;

    // =========================================================================
    // GRID E FORM
    // =========================================================================

    public List<WizardColumnConfig> GridColumns { get; set; } = new();
    public WizardFormLayout FormLayout { get; set; } = new();
    public List<WizardFieldConfig> FormFields { get; set; } = new();

    // =========================================================================
    // CONVERSÃO PARA FULLSTACKREQUEST
    // =========================================================================

    public FullStackRequest ToFullStackRequest()
    {
        var request = new FullStackRequest
        {
            CdFuncao = CdFuncao,
            CdSistema = CdSistema,
            DisplayName = DisplayName,
            Icone = Icon,
            MenuOrder = MenuOrder,
            ApiRoute = ApiRoute,
            Modulo = Modulo,
            FormLayout = new FormLayoutConfig
            {
                Columns = FormLayout.Columns,
                UseTabs = FormLayout.UseTabs,
                Tabs = FormLayout.Tabs ?? new List<string>()
            },
            GerarEntidade = GerarEntidade,
            GerarWebController = GerarWebController,
            GerarWebModels = GerarWebModels,
            GerarWebServices = GerarWebServices,
            GerarView = GerarView,
            GerarJavaScript = GerarJavaScript
        };

        // Grid columns
        var orderList = 0;
        foreach (var col in GridColumns.Where(c => c.Visible).OrderBy(c => c.Order))
        {
            request.ColunasListagem.Add(new ColumnListConfig
            {
                Nome = col.Name,
                Title = col.Title,
                Visible = col.Visible,
                Order = orderList++,
                Format = col.Format,
                Width = col.Width,
                Sortable = col.Sortable
            });
        }

        // Form fields - ⭐ v4.0: AGORA PASSA SELECT2 CONFIG!
        var orderForm = 0;
        foreach (var field in FormFields.OrderBy(f => f.Order))
        {
            request.ColunasFormulario.Add(new ColumnFormConfig
            {
                Nome = field.Name,
                Label = field.Label,
                Visible = true,
                Order = orderForm++,
                InputType = field.InputType,
                ColSize = field.ColSize,
                Required = field.Required,
                Placeholder = field.Placeholder,
                HelpText = field.HelpText,
                Tab = field.Tab,
                Group = field.Group,

                // ⭐ v4.0: PASSA CONFIGURAÇÃO SELECT2
                IsSelect2Ajax = field.IsSelect2Ajax,
                SelectEndpoint = field.SelectEndpoint,
                SelectApiRoute = field.SelectApiRoute,
                SelectValueField = field.SelectValueField,
                SelectTextField = field.SelectTextField
            });
        }

        return request;
    }
}

// =============================================================================
// WIZARD COLUMN CONFIG
// =============================================================================

public class WizardColumnConfig
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool Visible { get; set; } = true;

    [JsonConverter(typeof(FlexibleIntConverter))]
    public int Order { get; set; }

    public string Format { get; set; } = "text";
    public string? Width { get; set; }
    public string Align { get; set; } = "left";
    public bool Sortable { get; set; } = true;
}

// =============================================================================
// WIZARD FORM LAYOUT
// =============================================================================

public class WizardFormLayout
{
    [JsonConverter(typeof(FlexibleIntConverter))]
    public int Columns { get; set; } = 2;

    public bool UseTabs { get; set; }
    public List<string> Tabs { get; set; } = new();
}

// =============================================================================
// WIZARD FIELD CONFIG - ⭐ v4.0: SELECT2 SUPPORT
// =============================================================================

public class WizardFieldConfig
{
    public string Name { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = "string";
    public string InputType { get; set; } = "text";

    [JsonConverter(typeof(FlexibleIntConverter))]
    public int ColSize { get; set; } = 6;

    [JsonConverter(typeof(FlexibleIntConverter))]
    public int Order { get; set; }

    public string? Tab { get; set; }
    public string? Group { get; set; }

    public bool Required { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? Mask { get; set; }

    [JsonConverter(typeof(FlexibleNullableIntConverter))]
    public int? MaxLength { get; set; }

    // =========================================================================
    // ⭐ v4.0 - SELECT2 AJAX CONFIGURATION
    // =========================================================================

    /// <summary>
    /// ⭐ v4.0: Se true, gera automaticamente DTO, Action e Service para Select2.
    /// </summary>
    [JsonPropertyName("isSelect2Ajax")]
    public bool IsSelect2Ajax { get; set; }

    /// <summary>
    /// Endpoint da API para Select2 AJAX (ex: "/api/fornecedores")
    /// </summary>
    [JsonPropertyName("selectEndpoint")]
    public string? SelectEndpoint { get; set; }

    /// <summary>
    /// Rota da API para Select2 AJAX (alternativa a SelectEndpoint)
    /// </summary>
    [JsonPropertyName("selectApiRoute")]
    public string? SelectApiRoute { get; set; }

    /// <summary>
    /// Campo de valor (ID) para Select2 (ex: "id")
    /// </summary>
    [JsonPropertyName("selectValueField")]
    public string? SelectValueField { get; set; }

    /// <summary>
    /// Campo de texto para Select2 (ex: "nome", "razaoSocial")
    /// </summary>
    [JsonPropertyName("selectTextField")]
    public string? SelectTextField { get; set; }
}

// =============================================================================
// WIZARD RESPONSE
// =============================================================================

public class WizardResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public List<string> Warnings { get; set; } = new();
    public List<WizardGeneratedFile> Files { get; set; } = new();
}

public class WizardGeneratedFile
{
    public string FileName { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

// =============================================================================
// JSON CONVERTERS
// =============================================================================

public class FlexibleIntConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            return int.TryParse(str, out var val) ? val : 0;
        }
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32();
        }
        return 0;
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}

public class FlexibleNullableIntConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            if (string.IsNullOrEmpty(str))
                return null;
            return int.TryParse(str, out var val) ? val : null;
        }
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32();
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteNumberValue(value.Value);
        else
            writer.WriteNullValue();
    }
}