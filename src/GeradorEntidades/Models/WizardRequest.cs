// =============================================================================
// WIZARD REQUEST v3.5 - Recebe configuração do Wizard Frontend
// =============================================================================
// Arquivo: GeradorEntidades/Models/WizardRequest.cs
// =============================================================================
// CHANGELOG:
// v3.5 - Suporte completo a Tabs no formulário
// v3.4 - Adicionado ApiRoute para receber rota do manifesto
// v3.3 - Versão inicial com suporte a Grid e Form
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

    /// <summary>
    /// Rota da API vinda do manifesto (ex: "api/treinamentos/tipotreinamento").
    /// CRÍTICO: Esta propriedade recebe a rota correta do backend manifest.
    /// Se não fornecida, será construída automaticamente (o que pode gerar rotas erradas).
    /// </summary>
    [JsonPropertyName("apiRoute")]
    public string? ApiRoute { get; set; }

    // =========================================================================
    // ⭐ v3.5: MÓDULO EXPLÍCITO
    // =========================================================================

    /// <summary>
    /// Nome do módulo (ex: "GestaoDeTerceiros", "Treinamento").
    /// Define o diretório onde os arquivos serão gerados.
    /// </summary>
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

    /// <summary>
    /// ⭐ v3.5: Layout do formulário com suporte a Tabs
    /// </summary>
    public WizardFormLayout FormLayout { get; set; } = new();

    public List<WizardFieldConfig> FormFields { get; set; } = new();

    // =========================================================================
    // CONVERSÃO PARA FULLSTACKREQUEST
    // =========================================================================

    /// <summary>
    /// Converte para FullStackRequest.
    /// v3.5: Agora passa FormLayout com Tabs corretamente.
    /// </summary>
    public FullStackRequest ToFullStackRequest()
    {
        var request = new FullStackRequest
        {
            // Identificação
            CdFuncao = CdFuncao,
            CdSistema = CdSistema,
            DisplayName = DisplayName,
            Icone = Icon,
            MenuOrder = MenuOrder,

            // ⭐ v3.4: PASSA A ROTA DA API DO MANIFESTO
            ApiRoute = ApiRoute,

            // ⭐ v3.5: PASSA O MÓDULO EXPLÍCITO
            Modulo = Modulo,

            // ⭐ v3.5: PASSA O LAYOUT COM TABS (usa FormLayoutConfig de Models.cs)
            FormLayout = new FormLayoutConfig
            {
                Columns = FormLayout.Columns,
                UseTabs = FormLayout.UseTabs,
                Tabs = FormLayout.Tabs ?? new List<string>()
            },

            // Opções de geração
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

        // Form fields - ⭐ v3.5: Agora inclui Tab e Group
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
                // ⭐ v3.5: PASSA TAB E GROUP
                Tab = field.Tab,
                Group = field.Group
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
// WIZARD FORM LAYOUT - ⭐ v3.5: Suporte completo a Tabs
// Nota: Esta é a versão do Wizard (recebe do frontend)
//       FormLayoutConfig (em Models.cs) é a versão do FullStackRequest
// =============================================================================

public class WizardFormLayout
{
    [JsonConverter(typeof(FlexibleIntConverter))]
    public int Columns { get; set; } = 2;

    /// <summary>
    /// Se true, gera formulário com Bootstrap Tabs
    /// </summary>
    public bool UseTabs { get; set; }

    /// <summary>
    /// Lista de nomes das abas (ex: ["Dados Gerais", "Contato", "Documentos"])
    /// </summary>
    public List<string> Tabs { get; set; } = new();
}

// =============================================================================
// WIZARD FIELD CONFIG - ⭐ v3.5: Tab e Group
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

    /// <summary>
    /// ⭐ v3.5: Nome da aba onde este campo deve aparecer
    /// </summary>
    public string? Tab { get; set; }

    /// <summary>
    /// ⭐ v3.5: Nome do grupo dentro da aba
    /// </summary>
    public string? Group { get; set; }

    public bool Required { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? Mask { get; set; }

    [JsonConverter(typeof(FlexibleNullableIntConverter))]
    public int? MaxLength { get; set; }
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
// JSON CONVERTERS - Aceita string ou int do JavaScript
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