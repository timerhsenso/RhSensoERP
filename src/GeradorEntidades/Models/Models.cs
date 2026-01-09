// =============================================================================
// GERADOR FULL-STACK v3.6 - MODELS (CORRIGIDO)
// Unifica GeradorEntidades + CrudTool Templates
// v3.6 - CORREÇÃO: Módulo TreinamentoDesenvolvimento + Exclusão de Auditoria
// v3.3 - Suporte a Tabs no formulário (FormLayout, Tab, Group)
// v3.2 - Adicionado ApiRoute para usar rota do manifesto
//      - Organização por módulo nos RelativePaths
// =============================================================================

namespace GeradorEntidades.Models;

#region Tabela e Colunas (Database Metadata)

/// <summary>
/// Informações de uma tabela do banco.
/// </summary>
public class TabelaInfo
{
    public string Schema { get; set; } = "dbo";
    public string NomeTabela { get; set; } = string.Empty;
    public string NomeLimpo => LimparNome(NomeTabela);
    public string NomePascalCase => ToPascalCase(NomeLimpo);
    public string NomePlural => Pluralizar(NomePascalCase);
    public string NomePluralLower => NomePlural.ToLowerInvariant();
    public string NomeLower => NomePascalCase.ToLowerInvariant();
    public string Descricao { get; set; } = string.Empty;
    public List<ColunaInfo> Colunas { get; set; } = [];
    public List<ForeignKeyInfo> ForeignKeys { get; set; } = [];
    public List<IndexInfo> Indices { get; set; } = [];

    /// <summary>
    /// Coluna PK (ou primeira coluna como fallback).
    /// </summary>
    public ColunaInfo? PrimaryKey => Colunas.FirstOrDefault(c => c.IsPrimaryKey)
                                     ?? Colunas.FirstOrDefault();

    /// <summary>
    /// Se tem Primary Key definida.
    /// </summary>
    public bool HasPrimaryKey => Colunas.Any(c => c.IsPrimaryKey);

    /// <summary>
    /// Se tem PK composta (mais de uma coluna).
    /// </summary>
    public bool HasCompositePrimaryKey => Colunas.Count(c => c.IsPrimaryKey) > 1;

    /// <summary>
    /// Colunas que fazem parte da PK.
    /// </summary>
    public List<ColunaInfo> PrimaryKeyColumns => Colunas.Where(c => c.IsPrimaryKey).ToList();

    /// <summary>
    /// Se tem FKs compostas.
    /// </summary>
    public bool HasCompositeForeignKeys => ForeignKeys.Any(fk => fk.IsParteDeFkComposta);

    /// <summary>
    /// Remove prefixos comuns de tabelas legadas.
    /// </summary>
    private static string LimparNome(string nome)
    {
        var prefixes = new[] { "tbl_", "tb_", "tab_", "t_", "tbl", "tb" };
        var resultado = nome;

        foreach (var prefix in prefixes)
        {
            if (resultado.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                && resultado.Length > prefix.Length + 2)
            {
                resultado = resultado[prefix.Length..];
                break;
            }
        }

        return resultado;
    }

    /// <summary>
    /// Converte para PascalCase.
    /// </summary>
    public static string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        // Se já está em PascalCase ou camelCase, apenas garante primeira maiúscula
        if (input.Any(char.IsUpper))
        {
            return char.ToUpper(input[0]) + input[1..];
        }

        // snake_case para PascalCase
        var parts = input.Split('_', StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(parts.Select(p =>
            char.ToUpper(p[0]) + p[1..].ToLower()));
    }

    /// <summary>
    /// Pluraliza o nome (português simplificado).
    /// </summary>
    private static string Pluralizar(string nome)
    {
        if (string.IsNullOrEmpty(nome)) return nome;

        if (nome.EndsWith("ao", StringComparison.OrdinalIgnoreCase))
            return nome[..^2] + "oes";
        if (nome.EndsWith("r", StringComparison.OrdinalIgnoreCase) ||
            nome.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
            nome.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            return nome + "es";
        if (nome.EndsWith("l", StringComparison.OrdinalIgnoreCase))
            return nome[..^1] + "is";
        if (nome.EndsWith("m", StringComparison.OrdinalIgnoreCase))
            return nome[..^1] + "ns";

        return nome + "s";
    }
}

/// <summary>
/// Informações de uma coluna.
/// </summary>
public class ColunaInfo
{
    public string Nome { get; set; } = string.Empty;
    public string NomePascalCase => ToPascalCase(Nome);
    public string NomeCamelCase => ToCamelCase(Nome);
    public string Tipo { get; set; } = string.Empty;
    public int? Tamanho { get; set; }
    public int? Precisao { get; set; }
    public int? Escala { get; set; }
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsIdentity { get; set; }
    public bool IsComputed { get; set; }
    public string? DefaultValue { get; set; }
    public string? Descricao { get; set; }
    public int OrdinalPosition { get; set; }

    /// <summary>
    /// FK relacionada (se houver).
    /// </summary>
    public ForeignKeyInfo? ForeignKey { get; set; }

    /// <summary>
    /// Tipo C# correspondente.
    /// </summary>
    public string TipoCSharp => GetCSharpType();

    /// <summary>
    /// Tipo C# simples (sem nullable marker).
    /// </summary>
    public string TipoCSharpSimples => GetCSharpTypeSimple();

    /// <summary>
    /// Tipo JSON correspondente.
    /// </summary>
    public string TipoJson => GetJsonType();

    /// <summary>
    /// Se é tipo texto.
    /// </summary>
    public bool IsTexto => Tipo.ToLower() is "varchar" or "nvarchar" or "char" or "nchar" or "text" or "ntext";

    /// <summary>
    /// Se é tipo numérico.
    /// </summary>
    public bool IsNumerico => Tipo.ToLower() is "int" or "bigint" or "smallint" or "tinyint"
                              or "decimal" or "numeric" or "money" or "float" or "real";

    /// <summary>
    /// Se é tipo inteiro.
    /// </summary>
    public bool IsInt => Tipo.ToLower() is "int" or "smallint" or "tinyint";

    /// <summary>
    /// Se é tipo long.
    /// </summary>
    public bool IsLong => Tipo.ToLower() is "bigint";

    /// <summary>
    /// Se é tipo decimal.
    /// </summary>
    public bool IsDecimal => Tipo.ToLower() is "decimal" or "numeric" or "money" or "smallmoney" or "float" or "real";

    /// <summary>
    /// Se é tipo boolean.
    /// </summary>
    public bool IsBool => Tipo.ToLower() is "bit";

    /// <summary>
    /// Se é tipo data.
    /// </summary>
    public bool IsData => Tipo.ToLower() is "datetime" or "datetime2" or "date" or "smalldatetime" or "time";

    /// <summary>
    /// Se é tipo DateTime.
    /// </summary>
    public bool IsDateTime => Tipo.ToLower() is "datetime" or "datetime2" or "smalldatetime";

    /// <summary>
    /// Se é tipo Guid.
    /// </summary>
    public bool IsGuid => Tipo.ToLower() == "uniqueidentifier";

    /// <summary>
    /// Se é tipo string (para C#).
    /// </summary>
    public bool IsString => IsTexto;

    /// <summary>
    /// Se é tipo binário (byte[]).
    /// </summary>
    public bool IsBinary => Tipo.ToLower() is "binary" or "varbinary" or "image" or "timestamp" or "rowversion";

    // Prefixos conhecidos para melhor conversão PascalCase
    private static readonly string[] PrefixosConhecidos = new[]
    {
        "cd", "dc", "dt", "nr", "nm", "fl", "vl", "qt", "sg", "no", "id", "tp", "st", "ds", "tx", "pc", "hr"
    };

    private static string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        // Se tem underscore, processa como snake_case
        if (input.Contains('_'))
        {
            var parts = input.Split('_', StringSplitOptions.RemoveEmptyEntries);
            var sb = new System.Text.StringBuilder();
            foreach (var part in parts)
            {
                sb.Append(ProcessPascalCasePart(part.ToLowerInvariant()));
            }
            return sb.ToString();
        }

        // Se já tem letras maiúsculas misturadas, provavelmente já está em PascalCase
        if (HasMixedCase(input))
        {
            return char.ToUpper(input[0]) + input[1..];
        }

        // Processa como string contínua
        return ProcessPascalCasePart(input.ToLowerInvariant());
    }

    private static bool HasMixedCase(string input)
    {
        bool hasUpper = false;
        bool hasLower = false;
        foreach (var c in input)
        {
            if (char.IsUpper(c)) hasUpper = true;
            if (char.IsLower(c)) hasLower = true;
            if (hasUpper && hasLower) return true;
        }
        return false;
    }

    private static string ProcessPascalCasePart(string part)
    {
        if (string.IsNullOrEmpty(part)) return part;

        var result = new System.Text.StringBuilder();
        var i = 0;

        while (i < part.Length)
        {
            var prefixoEncontrado = false;
            foreach (var prefixo in PrefixosConhecidos)
            {
                if (i + prefixo.Length <= part.Length &&
                    part.Substring(i, prefixo.Length) == prefixo)
                {
                    result.Append(char.ToUpper(prefixo[0]));
                    result.Append(prefixo[1..]);
                    i += prefixo.Length;
                    prefixoEncontrado = true;
                    break;
                }
            }

            if (!prefixoEncontrado)
            {
                result.Append(result.Length == 0 ? char.ToUpper(part[i]) : part[i]);
                i++;
            }
        }

        return result.ToString();
    }

    private static string ToCamelCase(string input)
    {
        var pascal = ToPascalCase(input);
        if (string.IsNullOrEmpty(pascal)) return pascal;
        return char.ToLower(pascal[0]) + pascal[1..];
    }

    private string GetCSharpType()
    {
        var sqlType = Tipo.ToLowerInvariant();

        string baseType = sqlType switch
        {
            "varchar" or "nvarchar" or "char" or "nchar" or "text" or "ntext" => "string",
            "int" => "int",
            "smallint" => "short",
            "tinyint" => "byte",
            "bigint" => "long",
            "bit" => "bool",
            "decimal" or "numeric" or "money" or "smallmoney" => "decimal",
            "float" => "double",
            "real" => "float",
            "datetime" or "smalldatetime" or "datetime2" => "DateTime",
            "date" => "DateOnly",
            "time" => "TimeOnly",
            "datetimeoffset" => "DateTimeOffset",
            "uniqueidentifier" => "Guid",
            "varbinary" or "binary" or "image" => "byte[]",
            _ => "string"
        };

        if (baseType == "string" || baseType == "byte[]")
            return baseType;

        return IsNullable ? $"{baseType}?" : baseType;
    }

    private string GetCSharpTypeSimple()
    {
        var sqlType = Tipo.ToLowerInvariant();

        return sqlType switch
        {
            "varchar" or "nvarchar" or "char" or "nchar" or "text" or "ntext" => "string",
            "int" => "int",
            "smallint" => "short",
            "tinyint" => "byte",
            "bigint" => "long",
            "bit" => "bool",
            "decimal" or "numeric" or "money" or "smallmoney" => "decimal",
            "float" => "double",
            "real" => "float",
            "datetime" or "smalldatetime" or "datetime2" => "DateTime",
            "date" => "DateOnly",
            "time" => "TimeOnly",
            "datetimeoffset" => "DateTimeOffset",
            "uniqueidentifier" => "Guid",
            "varbinary" or "binary" or "image" => "byte[]",
            _ => "string"
        };
    }

    private string GetJsonType()
    {
        var sqlType = Tipo.ToLowerInvariant();

        return sqlType switch
        {
            "varchar" or "nvarchar" or "char" or "nchar" or "text" or "ntext" => "string",
            "int" or "smallint" or "tinyint" or "bigint" => "number",
            "decimal" or "numeric" or "money" or "smallmoney" or "float" or "real" => "number",
            "bit" => "boolean",
            "datetime" or "smalldatetime" or "datetime2" or "date" => "date",
            "time" => "time",
            "uniqueidentifier" => "guid",
            _ => "string"
        };
    }
}

/// <summary>
/// Informações de Foreign Key - com Navigation Properties.
/// </summary>
public class ForeignKeyInfo
{
    public string Nome { get; set; } = string.Empty;
    public string ColunaOrigem { get; set; } = string.Empty;
    public string TabelaDestino { get; set; } = string.Empty;
    public string ColunaDestino { get; set; } = string.Empty;

    /// <summary>
    /// Se esta FK faz parte de uma FK composta (mais de uma coluna).
    /// </summary>
    public bool IsParteDeFkComposta { get; set; }

    /// <summary>
    /// Lista de todas as colunas desta FK (para FKs compostas).
    /// </summary>
    public List<string> TodasColunas { get; set; } = [];

    /// <summary>
    /// Nome da entidade destino em PascalCase.
    /// </summary>
    public string EntidadeDestino => TabelaInfo.ToPascalCase(TabelaDestino);

    /// <summary>
    /// Nome sugerido para a propriedade de navegação.
    /// </summary>
    public string NavigationPropertyName => GerarNavigationPropertyName();

    /// <summary>
    /// Coluna de display provável na tabela destino.
    /// </summary>
    public string? ColunaDisplay { get; set; }

    /// <summary>
    /// Se a FK é por Guid.
    /// </summary>
    public bool IsFkByGuid => ColunaOrigem.StartsWith("id", StringComparison.OrdinalIgnoreCase)
                              && !ColunaOrigem.Equals("id", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Se a FK é por código legado.
    /// </summary>
    public bool IsFkByCodigo => ColunaOrigem.StartsWith("cd", StringComparison.OrdinalIgnoreCase)
                                || ColunaOrigem.StartsWith("no", StringComparison.OrdinalIgnoreCase)
                                || ColunaOrigem.StartsWith("sg", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Chave única para identificar FKs duplicadas.
    /// </summary>
    public string ChaveUnica => $"{ColunaOrigem.ToLower()}_{TabelaDestino.ToLower()}";

    private string GerarNavigationPropertyName()
    {
        var coluna = ColunaOrigem.ToLower();

        string nomeLimpo;

        if (coluna.StartsWith("id") && coluna.Length > 2)
            nomeLimpo = coluna[2..];
        else if (coluna.StartsWith("cd") && coluna.Length > 2)
            nomeLimpo = coluna[2..];
        else if (coluna.StartsWith("fk") && coluna.Length > 2)
            nomeLimpo = coluna[2..];
        else
            nomeLimpo = coluna;

        return TabelaInfo.ToPascalCase(nomeLimpo);
    }
}

/// <summary>
/// Informações de índice.
/// </summary>
public class IndexInfo
{
    public string Nome { get; set; } = string.Empty;
    public bool IsUnique { get; set; }
    public List<string> Colunas { get; set; } = [];
}

#endregion

#region Request/Response de Geração

/// <summary>
/// Request de geração FULL-STACK com todas as opções.
/// </summary>
public class FullStackRequest
{
    // =========================================================================
    // IDENTIFICAÇÃO
    // =========================================================================
    public string NomeTabela { get; set; } = string.Empty;
    public string CdFuncao { get; set; } = string.Empty;
    public string CdSistema { get; set; } = "RHU";
    public string DisplayName { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;

    // =========================================================================
    // v3.2 - ROTA API DO MANIFESTO
    // =========================================================================

    /// <summary>
    /// Rota da API conforme definida no backend (ex: "api/esocial/lotacaotributaria").
    /// Se não informada, será construída automaticamente como "api/{ModuloRota}/{EntityNameLower}".
    /// </summary>
    public string? ApiRoute { get; set; }

    // =========================================================================
    // MENU E NAVEGAÇÃO
    // =========================================================================
    public string Modulo { get; set; } = string.Empty;
    public string ModuloRota { get; set; } = string.Empty;
    public string Icone { get; set; } = "fas fa-table";
    public int MenuOrder { get; set; } = 10;
    public bool GerarMenuItem { get; set; } = true;

    // =========================================================================
    // OPÇÕES DE GERAÇÃO - BACKEND
    // =========================================================================
    public bool GerarEntidade { get; set; } = true;
    public bool GerarApiController { get; set; } = false;
    public bool GerarNavigation { get; set; } = true;
    public bool ApenasNavigationPorGuid { get; set; } = false;
    public bool IsLegacyTable { get; set; } = true;

    // =========================================================================
    // OPÇÕES DE GERAÇÃO - FRONTEND
    // =========================================================================
    public bool GerarWebController { get; set; } = true;
    public bool GerarWebModels { get; set; } = true;
    public bool GerarWebServices { get; set; } = true;
    public bool GerarView { get; set; } = true;
    public bool GerarJavaScript { get; set; } = true;

    // =========================================================================
    // CONFIGURAÇÃO DE COLUNAS
    // =========================================================================
    public List<ColumnListConfig> ColunasListagem { get; set; } = [];
    public List<ColumnFormConfig> ColunasFormulario { get; set; } = [];
    public List<FkNavigationConfig> ConfiguracoesFk { get; set; } = [];

    // =========================================================================
    // CHAVE PRIMÁRIA DEFINIDA MANUALMENTE (para tabelas sem PK no banco)
    // =========================================================================
    public List<PkColumnConfig> ColunasPkDefinidas { get; set; } = [];

    // =========================================================================
    // ⭐ v3.3 - LAYOUT DO FORMULÁRIO COM TABS
    // =========================================================================

    /// <summary>
    /// Configuração de layout do formulário (colunas e tabs).
    /// </summary>
    public FormLayoutConfig? FormLayout { get; set; }
}

/// <summary>
/// ⭐ v3.3: Configuração de layout do formulário com suporte a Tabs.
/// </summary>
public class FormLayoutConfig
{
    /// <summary>
    /// Número de colunas do formulário (1, 2, 3 ou 4).
    /// </summary>
    public int Columns { get; set; } = 2;

    /// <summary>
    /// Se true, gera formulário com Bootstrap Tabs.
    /// </summary>
    public bool UseTabs { get; set; }

    /// <summary>
    /// Lista de nomes das abas (ex: ["Dados Gerais", "Contato", "Documentos"]).
    /// </summary>
    public List<string> Tabs { get; set; } = [];
}

/// <summary>
/// Configuração de coluna definida como PK pelo usuário.
/// </summary>
public class PkColumnConfig
{
    public string Nome { get; set; } = string.Empty;
    public string NomePascalCase { get; set; } = string.Empty;
}

/// <summary>
/// Configuração de coluna para listagem (DataTable).
/// </summary>
public class ColumnListConfig
{
    public string Nome { get; set; } = string.Empty;
    public bool Visible { get; set; } = true;
    public int Order { get; set; } = 0;
    public string? Title { get; set; }
    public string Format { get; set; } = "text";
    public string? Width { get; set; }
    public string Align { get; set; } = "left";
    public bool Sortable { get; set; } = true;
}

/// <summary>
/// Configuração de coluna para formulário.
/// </summary>
public class ColumnFormConfig
{
    public string Nome { get; set; } = string.Empty;
    public bool Visible { get; set; } = true;
    public int Order { get; set; } = 0;
    public string? Label { get; set; }
    public string InputType { get; set; } = "text";
    public int ColSize { get; set; } = 6;
    public bool Required { get; set; } = false;
    public bool Disabled { get; set; } = false;
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? Mask { get; set; }
    public int? Rows { get; set; }

    // =========================================================================
    // ⭐ v3.3 - SUPORTE A TABS
    // =========================================================================

    /// <summary>
    /// Nome da aba onde este campo deve aparecer.
    /// </summary>
    public string? Tab { get; set; }

    /// <summary>
    /// Nome do grupo dentro da aba.
    /// </summary>
    public string? Group { get; set; }

    // =========================================================================
    // ⭐ v4.4 - CONFIGURAÇÃO DE SELECT/COMBOBOX (AJAX)
    // =========================================================================

    /// <summary>
    /// Se true, gera automaticamente DTO, Action e Service para Select2.
    /// </summary>
    public bool IsSelect2Ajax { get; set; }

    /// <summary>
    /// Rota da API backend para buscar dados.
    /// </summary>
    public string? SelectApiRoute { get; set; }
    public string? SelectEndpoint { get; set; }
    public string? SelectValueField { get; set; }
    public string? SelectTextField { get; set; }
}

/// <summary>
/// Configuração de FK/Navegação.
/// </summary>
public class FkNavigationConfig
{
    public string ColunaOrigem { get; set; } = string.Empty;
    public string TabelaDestino { get; set; } = string.Empty;
    public string NavigationName { get; set; } = string.Empty;
    public string DisplayColumn { get; set; } = string.Empty;
    public bool Ignorar { get; set; } = false;
}

/// <summary>
/// Resultado da geração FULL-STACK.
/// </summary>
public class FullStackResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string NomeTabela { get; set; } = string.Empty;
    public string NomeEntidade { get; set; } = string.Empty;

    // Arquivos gerados
    public GeneratedFile? Entidade { get; set; }
    public GeneratedFile? WebController { get; set; }
    public GeneratedFile? Dto { get; set; }
    public GeneratedFile? CreateRequest { get; set; }
    public GeneratedFile? UpdateRequest { get; set; }
    public GeneratedFile? ListViewModel { get; set; }
    public GeneratedFile? ServiceInterface { get; set; }
    public GeneratedFile? ServiceImplementation { get; set; }
    public GeneratedFile? View { get; set; }
    public GeneratedFile? JavaScript { get; set; }

    public List<GeneratedFile> Select2LookupDtos { get; set; } = [];


    // Metadados
    public List<string> NavigationsGeradas { get; set; } = [];
    public List<string> Warnings { get; set; } = [];

    /// <summary>
    /// Todos os arquivos gerados.
    /// </summary>
    public IEnumerable<GeneratedFile> AllFiles =>
       new[] { Entidade, WebController, Dto, CreateRequest, UpdateRequest,
                ListViewModel, ServiceInterface, ServiceImplementation, View, JavaScript }
           .Where(f => f != null)!
           .Concat(Select2LookupDtos ?? []);
}

/// <summary>
/// Arquivo gerado.
/// </summary>
public class GeneratedFile
{
    public string FileName { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
}

/// <summary>
/// ✅ v4.1 NOVO: Configuração de propriedade de navegação para exibição no DTO.
/// Representa um campo calculado que vem de uma entidade relacionada via FK.
/// 
/// EXEMPLO:
/// - Name: "FornecedorRazaoSocial"
/// - DisplayName: "Fornecedor"
/// - NavigationName: "Fornecedor"
/// - DisplayField: "RazaoSocial"
/// - ForeignKeyProperty: "IdFornecedor"
/// </summary>
public class NavigationPropertyConfig
{
    /// <summary>
    /// Nome da propriedade no DTO (ex: "FornecedorRazaoSocial")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Nome para exibição na grid (ex: "Fornecedor")
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Nome da navegação na entidade backend (ex: "Fornecedor")
    /// </summary>
    public string NavigationName { get; set; } = string.Empty;

    /// <summary>
    /// Campo a ser exibido da entidade relacionada (ex: "RazaoSocial", "Descricao", "Sigla")
    /// </summary>
    public string DisplayField { get; set; } = string.Empty;

    /// <summary>
    /// Tipo C# da propriedade (sempre string? para navegações)
    /// </summary>
    public string CSharpType => "string?";

    /// <summary>
    /// FK origem (ex: "IdFornecedor")
    /// </summary>
    public string ForeignKeyProperty { get; set; } = string.Empty;

    /// <summary>
    /// Entidade destino (ex: "Fornecedor", "TipoSanguineo")
    /// </summary>
    public string TargetEntity { get; set; } = string.Empty;

    // ✅ v5.1: Adiciona Order para controlar posição na grid
    public int Order { get; set; } = 999;  // ← ADICIONE ESTA LINHA

}

#endregion

#region EntityConfig (Compatível com Templates)

/// <summary>
/// Configuração de entidade para templates.
/// Converte de TabelaInfo + FullStackRequest para formato usado pelos templates.
/// </summary>
public class EntityConfig
{
    public string Name { get; set; } = string.Empty;
    public string NameLower => Name.ToLowerInvariant();
    public string PluralName { get; set; } = string.Empty;
    public string PluralNameLower => PluralName.ToLowerInvariant();
    public string DisplayName { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string CdFuncao { get; set; } = string.Empty;
    public string CdSistema { get; set; } = "RHU";
    public string Module { get; set; } = string.Empty;
    public string ModuleRoute { get; set; } = string.Empty;
    public string ModuleRouteLower => ModuleRoute.ToLowerInvariant();
    public string? BackendNamespace { get; set; }

    // =========================================================================
    // v3.2 - ROTA API DO MANIFESTO
    // =========================================================================

    /// <summary>
    /// Rota da API conforme definida no backend (ex: "api/esocial/lotacaotributaria").
    /// </summary>
    public string ApiRoute { get; set; } = string.Empty;

    // =========================================================================
    // MENU ITEM - Configurações para [MenuItem] no Controller
    // =========================================================================

    /// <summary>
    /// Ícone FontAwesome para o menu (ex: "fas fa-table", "fas fa-users").
    /// </summary>
    public string Icon { get; set; } = "fas fa-table";

    /// <summary>
    /// Ordem de exibição no menu (menor = primeiro).
    /// </summary>
    public int MenuOrder { get; set; } = 10;

    // =========================================================================
    // PK Info
    // =========================================================================
    public PropertyConfig? PrimaryKey { get; set; }
    public string PkType => PrimaryKey?.CSharpType ?? "Guid";
    public string PkTypeSimple => PrimaryKey?.CSharpTypeSimple ?? "Guid";

    // Propriedades
    public List<PropertyConfig> Properties { get; set; } = [];

    // ✅ v4.1: Propriedades de navegação para exibição
    public List<NavigationPropertyConfig> NavigationProperties { get; set; } = [];

    /// <summary>
    /// Lista de lookups Select2 detectados automaticamente.
    /// </summary>
    public List<SelectLookupConfig> Select2Lookups { get; set; } = [];

    // =========================================================================
    // ⭐ v3.3 - LAYOUT DO FORMULÁRIO COM TABS
    // =========================================================================

    /// <summary>
    /// Configuração de layout do formulário (colunas e tabs).
    /// </summary>
    public FormLayoutConfig? FormLayout { get; set; }

    /// <summary>
    /// ✅ v3.6: Helper para identificar campos de auditoria.
    /// Campos de auditoria NÃO devem aparecer em List/Form por padrão.
    /// </summary>
    private static bool IsAuditField(string columnName)
    {
        var auditFields = new[]
        {
            "DataCriacao", "DtCriacao", "data_criacao", "dt_criacao", "created_at", "created_date",
            "UsuarioCriacao", "usuario_criacao", "created_by", "criado_por",
            "DataAtualizacao", "DtAtualizacao", "data_atualizacao", "dt_atualizacao", "updated_at", "modified_at",
            "UsuarioAtualizacao", "usuario_atualizacao", "updated_by", "modified_by", "atualizado_por"
        };

        return auditFields.Any(f => columnName.Equals(f, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Cria EntityConfig a partir de TabelaInfo e FullStackRequest.
    /// </summary>
    public static EntityConfig FromTabela(TabelaInfo tabela, FullStackRequest request)
    {
        // Determinar quais colunas são PK (do banco OU definidas pelo usuário)
        var pkColumnsDefinidas = request.ColunasPkDefinidas
            .Select(p => p.Nome.ToLowerInvariant())
            .ToHashSet();

        var config = new EntityConfig
        {
            Name = tabela.NomePascalCase,
            PluralName = tabela.NomePlural,
            DisplayName = !string.IsNullOrWhiteSpace(request.DisplayName)
                ? request.DisplayName
                : FormatDisplayName(tabela.NomePascalCase),
            TableName = tabela.NomeTabela.ToLower(),
            CdFuncao = request.CdFuncao,
            CdSistema = request.CdSistema,
            Module = request.Modulo,
            ModuleRoute = request.ModuloRota,
            Icon = request.Icone,
            MenuOrder = request.MenuOrder,

            // =========================================================================
            // v3.2 - Usa ApiRoute do request ou constrói automaticamente
            // =========================================================================
            ApiRoute = !string.IsNullOrWhiteSpace(request.ApiRoute)
                ? request.ApiRoute
                : $"api/{request.ModuloRota.ToLowerInvariant()}/{tabela.NomePascalCase.ToLowerInvariant()}",

            // =========================================================================
            // ⭐ v3.3 - FormLayout com Tabs
            // =========================================================================
            FormLayout = request.FormLayout
        };

        // Determina se é primeira execução (sem seleção)
        var hasListSelection = request.ColunasListagem.Count > 0;
        var hasFormSelection = request.ColunasFormulario.Count > 0;
        var isFirstExecution = !hasListSelection && !hasFormSelection;

        // Mapear colunas para propriedades
        var order = 0;
        foreach (var coluna in tabela.Colunas)
        {
            var listConfig = request.ColunasListagem
                .FirstOrDefault(c => c.Nome.Equals(coluna.Nome, StringComparison.OrdinalIgnoreCase));

            var formConfig = request.ColunasFormulario
                .FirstOrDefault(c => c.Nome.Equals(coluna.Nome, StringComparison.OrdinalIgnoreCase));

            // Verificar se esta coluna foi definida como PK pelo usuário
            var isPkDefinida = pkColumnsDefinidas.Contains(coluna.Nome.ToLowerInvariant());
            var isPrimaryKey = coluna.IsPrimaryKey || isPkDefinida;

            // ✅ v3.6: Verifica se é campo de auditoria
            var isAudit = IsAuditField(coluna.Nome);

            var prop = new PropertyConfig
            {
                Name = coluna.NomePascalCase,
                ColumnName = coluna.Nome,
                CSharpType = coluna.TipoCSharp,
                CSharpTypeSimple = coluna.TipoCSharpSimples,
                SqlType = coluna.Tipo,
                DisplayName = listConfig?.Title ?? formConfig?.Label ?? FormatDisplayName(coluna.NomePascalCase),
                IsPrimaryKey = isPrimaryKey,
                IsPrimaryKeyDefinedByUser = isPkDefinida,
                IsIdentity = coluna.IsIdentity,
                IsNullable = coluna.IsNullable,
                IsReadOnly = coluna.IsComputed || coluna.IsIdentity,
                Required = !coluna.IsNullable && !isPrimaryKey,
                MaxLength = coluna.Tamanho,
                IsGuid = coluna.IsGuid,
                IsString = coluna.IsString,
                IsInt = coluna.IsInt,
                IsLong = coluna.IsLong,
                IsDecimal = coluna.IsDecimal,
                IsBool = coluna.IsBool,
                IsDateTime = coluna.IsDateTime,
                HasForeignKey = coluna.ForeignKey != null,
                ForeignKeyTable = coluna.ForeignKey?.TabelaDestino,
                ForeignKeyColumn = coluna.ForeignKey?.ColunaDestino
            };

            // Configuração de listagem
            if (listConfig != null && listConfig.Visible)
            {
                prop.List = new ListConfig
                {
                    Show = true,
                    Order = listConfig.Order,
                    Title = listConfig.Title,
                    Format = listConfig.Format,
                    Width = listConfig.Width,
                    Align = listConfig.Align,
                    Sortable = listConfig.Sortable
                };
            }
            else if (listConfig == null && isFirstExecution && !isPrimaryKey && !coluna.IsGuid && !coluna.IsBinary && !isAudit)
            {
                // ✅ v3.6: Default: mostrar colunas simples (exceto auditoria)
                prop.List = new ListConfig
                {
                    Show = true,
                    Order = order++,
                    Format = GetDefaultFormat(coluna),
                    Sortable = true
                };
            }

            // Configuração de formulário
            if (formConfig != null && formConfig.Visible)
            {
                prop.Form = new FormConfig
                {
                    Show = true,
                    Order = formConfig.Order,
                    InputType = formConfig.InputType,
                    ColSize = formConfig.ColSize,
                    Placeholder = formConfig.Placeholder,
                    HelpText = formConfig.HelpText,
                    Disabled = formConfig.Disabled,
                    Rows = formConfig.Rows ?? 3,
                    Tab = formConfig.Tab,
                    Group = formConfig.Group,
                    SelectEndpoint = formConfig.SelectEndpoint,
                    SelectValueField = formConfig.SelectValueField,
                    SelectTextField = formConfig.SelectTextField,
                    IsSelect2Ajax = formConfig.IsSelect2Ajax || !string.IsNullOrEmpty(formConfig.SelectEndpoint),
                    SelectApiRoute = formConfig.SelectApiRoute
                };
            }
            else if (formConfig == null && isPrimaryKey && !coluna.IsIdentity && !coluna.IsGuid && coluna.IsTexto)
            {
                prop.Form = new FormConfig
                {
                    Show = true,
                    ShowOnCreate = true,
                    ShowOnEdit = false,
                    Order = 0,
                    InputType = "text",
                    ColSize = coluna.Tamanho <= 10 ? 4 : 6,
                    Disabled = false
                };
            }
            else if (formConfig == null && isFirstExecution && !isPrimaryKey && !coluna.IsComputed && !coluna.IsBinary && !isAudit)
            {
                prop.Form = new FormConfig
                {
                    Show = true,
                    Order = order++,
                    InputType = GetDefaultInputType(coluna),
                    ColSize = GetDefaultColSize(coluna)
                };
            }

            // =========================================================================
            // ⭐ v4.5: MAPEIA LOOKUP DO FORMCONFIG (ADICIONAR AQUI!)
            // =========================================================================
            if (prop.Form != null &&
                (prop.Form.IsSelect2Ajax || !string.IsNullOrEmpty(prop.Form.SelectEndpoint)))
            {
                var endpoint = prop.Form.SelectEndpoint ?? prop.Form.SelectApiRoute ?? "";

                if (!string.IsNullOrEmpty(endpoint))
                {
                    prop.Lookup = new LookupConfig
                    {
                        Endpoint = endpoint,
                        ValueField = prop.Form.SelectValueField ?? "id",
                        TextField = prop.Form.SelectTextField ?? "nome",
                        AllowSearch = true,
                        AllowClear = true,
                        MinSearchLength = 0,
                        Multiple = false
                    };

                    Console.WriteLine($"✅ [LOOKUP] Mapeado: {prop.Name} → {endpoint}");
                }
            }

            config.Properties.Add(prop);

            //

            // =========================================================================
            // ⭐ v4.5: MAPEIA LOOKUP DO FORMCONFIG (vem do JSON PropertyMetadata)
            // =========================================================================
            if (formConfig != null &&
                (!string.IsNullOrEmpty(formConfig.SelectEndpoint) || formConfig.IsSelect2Ajax))
            {
                prop.Lookup = new LookupConfig
                {
                    Endpoint = formConfig.SelectEndpoint ?? formConfig.SelectApiRoute ?? "",
                    ValueField = formConfig.SelectValueField ?? "id",
                    TextField = formConfig.SelectTextField ?? "nome",
                    AllowSearch = true,
                    AllowClear = true,
                    MinSearchLength = 0,
                    Multiple = false
                };

                Console.WriteLine($"✅ [LOOKUP] Campo {prop.Name} tem lookup: {prop.Lookup.Endpoint}");
            }

            //

            config.Properties.Add(prop);

            // Definir PrimaryKey (primeira encontrada, ou primeira definida pelo usuário)
            if (isPrimaryKey && config.PrimaryKey == null)
            {
                config.PrimaryKey = prop;
            }
            if (isPrimaryKey && config.PrimaryKey == null)
            {
                config.PrimaryKey = prop;
            }
        }

        // =========================================================================
        // ⭐ v6.0: Popula Select2Lookups baseado nas propriedades configuradas
        // =========================================================================
        // =========================================================================
        // ⭐ v6.1: Popula Select2Lookups a partir do campo "lookup" da PropertyConfig
        // =========================================================================
        Console.WriteLine($"🔍 [SELECT2] Buscando lookups em {config.Properties.Count} propriedades...");

        foreach (var prop in config.Properties.Where(p => p.Lookup != null && !string.IsNullOrEmpty(p.Lookup.Endpoint)))
        {
            var lookup = prop.Lookup!;

            Console.WriteLine($"  ✅ Encontrado: {prop.Name} → {lookup.Endpoint}");

            // Extrai nome da entidade do endpoint ou usa o módulo
            var entityName = lookup.Route ?? prop.ForeignKeyTable ?? "Unknown";

            if (!string.IsNullOrEmpty(prop.ForeignKeyTable))
            {
                entityName = TabelaInfo.ToPascalCase(prop.ForeignKeyTable);
            }
            else if (!string.IsNullOrEmpty(lookup.Route))
            {
                entityName = TabelaInfo.ToPascalCase(lookup.Route);
            }
            else
            {
                // Tenta extrair do endpoint (último segmento)
                var segments = lookup.Endpoint.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length > 0)
                {
                    var lastSegment = segments.Last();
                    // Remove "lookup" se for o último segmento
                    if (lastSegment.Equals("lookup", StringComparison.OrdinalIgnoreCase) && segments.Length > 1)
                    {
                        lastSegment = segments[^2]; // Penúltimo segmento
                    }
                    entityName = TabelaInfo.ToPascalCase(lastSegment);
                }
            }

            var dtoName = $"{entityName}LookupDto";
            var methodName = $"Get{prop.Name}Select2";

            config.Select2Lookups.Add(new SelectLookupConfig
            {
                PropertyName = prop.Name,
                EntityName = entityName,
                DtoName = dtoName,
                ApiRoute = lookup.Endpoint,  // ✅ USA O ENDPOINT DO LOOKUP!
                ValueField = lookup.ValueField,
                TextField = lookup.TextField,
                MethodName = methodName,
                DisplayName = prop.DisplayName,
                Label = prop.DisplayName
            });

            Console.WriteLine($"    → Adicionado ao Select2Lookups: {entityName}LookupDto");
        }

        Console.WriteLine($"✅ [SELECT2] Total de lookups configurados: {config.Select2Lookups.Count}");


        // =========================================================================
        // ✅ v4.1: Popula NavigationProperties baseado nas ForeignKeys da tabela
        // =========================================================================
        // Processa FKs únicas (evita duplicatas) e gera propriedades de navegação
        // para exibição no frontend (ex: FornecedorRazaoSocial, TipoSanguineoDescricao)
        // =========================================================================

        var fksUnicas = tabela.ForeignKeys
            .Where(fk => !fk.IsParteDeFkComposta) // Ignora FKs compostas
            .GroupBy(fk => fk.ChaveUnica)
            .Select(g => g.First())
            .ToList();






        // ============ LOGS DE DEBUG v4.8 ============
        Console.WriteLine("═══════════════════════════════════════════════════════");
        Console.WriteLine("🔍 EntityConfig.FromTabela - DEBUG NavigationProperties");
        Console.WriteLine("═══════════════════════════════════════════════════════");
        Console.WriteLine($"🔍 hasListSelection = {hasListSelection}");
        Console.WriteLine($"🔍 request.ColunasListagem.Count = {request.ColunasListagem.Count}");
        Console.WriteLine("📋 Colunas de listagem recebidas:");
        foreach (var col in request.ColunasListagem)
        {
            Console.WriteLine($"  - {col.Nome} (Visible: {col.Visible})");
        }
        Console.WriteLine("───────────────────────────────────────────────────────");
        Console.WriteLine($"🔍 fksUnicas.Count = {fksUnicas.Count}");
        Console.WriteLine("📋 ForeignKeys detectadas:");

        foreach (var fk in fksUnicas)
        {
            var fkConfig = request.ConfiguracoesFk?
                .FirstOrDefault(c => c.ColunaOrigem.Equals(fk.ColunaOrigem, StringComparison.OrdinalIgnoreCase));

            if (fkConfig?.Ignorar == true)
            {
                Console.WriteLine($"  FK: {fk.ColunaOrigem} → IGNORADA (fkConfig.Ignorar)");
                continue;
            }

            var navigationName = fkConfig?.NavigationName ?? fk.NavigationPropertyName;
            var displayField = fkConfig?.DisplayColumn;

            if (string.IsNullOrEmpty(displayField))
            {
                displayField = DetectDisplayField(fk.TabelaDestino);
            }

            var propertyName = $"{navigationName}{displayField}";

            Console.WriteLine($"  FK: {fk.ColunaOrigem}");
            Console.WriteLine($"    → Tabela destino: {fk.TabelaDestino}");
            Console.WriteLine($"    → navigationName: {navigationName}");
            Console.WriteLine($"    → displayField: {displayField}");
            Console.WriteLine($"    → propertyName: {propertyName}");

            if (hasListSelection)
            {
                var isSelected = request.ColunasListagem.Any(c =>
                    c.Nome.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

                Console.WriteLine($"    → isSelected? {isSelected}");

                if (!isSelected)
                {
                    Console.WriteLine($"    ❌ NÃO selecionada - será IGNORADA");
                }
                else
                {
                    Console.WriteLine($"    ✅ SELECIONADA - será ADICIONADA");
                }
            }
            else
            {
                Console.WriteLine($"    ✅ Modo automático - será ADICIONADA");
            }

            Console.WriteLine();
        }
        Console.WriteLine("═══════════════════════════════════════════════════════");
        // ============ FIM DOS LOGS DE DEBUG ============




        foreach (var fk in fksUnicas)
        {
            // Busca configuração customizada (se definida pelo usuário)
            var fkConfig = request.ConfiguracoesFk?
                .FirstOrDefault(c => c.ColunaOrigem.Equals(fk.ColunaOrigem, StringComparison.OrdinalIgnoreCase));

            // Se usuário marcou para ignorar, pula
            if (fkConfig?.Ignorar == true)
                continue;

            // Nome da navegação (ex: "Fornecedor", "TipoSanguineo")
            var navigationName = fkConfig?.NavigationName ?? fk.NavigationPropertyName;

            // Campo de display da entidade relacionada
            // Tenta usar configuração do usuário, senão detecta automaticamente
            var displayField = fkConfig?.DisplayColumn;

            if (string.IsNullOrEmpty(displayField))
            {
                // Detecta campo de display baseado em padrões comuns
                displayField = DetectDisplayField(fk.TabelaDestino);
            }

            // Nome da propriedade no DTO (ex: "FornecedorRazaoSocial")
            var propertyName = $"{navigationName}{displayField}";

            // DisplayName para a coluna da grid (ex: "Fornecedor")
            var displayName = FormatDisplayName(navigationName);

            // ✅ v4.9: APLICA O FILTRO (mesma lógica dos logs)
            if (hasListSelection)
            {
                // Modo manual: verifica se foi selecionada
                var isSelected = request.ColunasListagem.Any(c =>
                    c.Nome.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

                if (!isSelected)
                {
                    // NÃO selecionada → PULA!
                    continue;
                }
            }
            // Se !hasListSelection (modo automático): adiciona todas
            /*
            config.NavigationProperties.Add(new NavigationPropertyConfig
            {
                Name = propertyName,
                DisplayName = displayName,
                NavigationName = navigationName,
                DisplayField = displayField,
                ForeignKeyProperty = TabelaInfo.ToPascalCase(fk.ColunaOrigem),
                TargetEntity = fk.EntidadeDestino
            });
            */

            // ✅ v5.1: Busca Order da configuração do usuário
            var listConfig = request.ColunasListagem
                .FirstOrDefault(c => c.Nome.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

           var navOrder = listConfig?.Order ?? 999;  // 999 = final da lista

            config.NavigationProperties.Add(new NavigationPropertyConfig
            {
                Name = propertyName,
                DisplayName = displayName,
                NavigationName = navigationName,
                DisplayField = displayField,
                ForeignKeyProperty = TabelaInfo.ToPascalCase(fk.ColunaOrigem),
                TargetEntity = fk.EntidadeDestino,
                Order = navOrder  // ✅ v5.1: ADICIONA ORDER
            });

        }

        return config;  // ✅ LINHA ADICIONADA
    }  // ✅ FECHA O MÉTODO FromTabela

    /// <summary>
    /// ✅ v4.1 NOVO: Detecta o campo de display provável de uma tabela relacionada.
    /// <summary>
    /// ✅ v4.1 NOVO: Detecta o campo de display provável de uma tabela relacionada.
    /// Usa heurísticas baseadas em padrões comuns de nomenclatura.
    /// </summary>
    private static string DetectDisplayField(string tableName)
    {
        var lowerTable = tableName.ToLower();

        // Padrões específicos por tipo de tabela
        if (lowerTable.Contains("fornece") || lowerTable.Contains("cliente") ||
            lowerTable.Contains("empresa") || lowerTable.Contains("pessoa"))
        {
            return "RazaoSocial"; // ou "Nome"
        }

        if (lowerTable.Contains("tipo") || lowerTable.Contains("categoria") ||
            lowerTable.Contains("classe") || lowerTable.Contains("grupo"))
        {
            return "Descricao";
        }

        if (lowerTable.Contains("uf") || lowerTable.Contains("estado") ||
            lowerTable.Contains("pais") || lowerTable.Contains("moeda"))
        {
            return "Sigla";
        }

        if (lowerTable.Contains("cidade") || lowerTable.Contains("bairro") ||
            lowerTable.Contains("logradouro"))
        {
            return "Nome";
        }

        // Padrão genérico
        return "Descricao";
    }

    private static string FormatDisplayName(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        var sb = new System.Text.StringBuilder();

        for (int i = 0; i < name.Length; i++)
        {
            var c = name[i];

            if (i > 0 && char.IsUpper(c) && !char.IsUpper(name[i - 1]))
            {
                sb.Append(' ');
            }

            sb.Append(c);
        }

        var resultado = sb.ToString()
            .Replace("Cd ", "Código ")
            .Replace("Dc ", "Descrição ")
            .Replace("Dt ", "Data ")
            .Replace("Nr ", "Número ")
            .Replace("Nm ", "Nome ")
            .Replace("Fl ", "Flag ")
            .Replace("Vl ", "Valor ")
            .Replace("Qt ", "Quantidade ")
            .Replace("Sg ", "Sigla ")
            .Replace("No ", "Número ")
            .Replace("Tp ", "Tipo ")
            .Replace("St ", "Status ")
            .Replace("Ds ", "Descrição ")
            .Replace("Tx ", "Taxa ")
            .Replace("Pc ", "Percentual ")
            .Replace("Hr ", "Hora ")
            .Replace("Id ", "")
            .Trim();

        return resultado;
    }

    private static string GetDefaultFormat(ColunaInfo coluna)
    {
        if (coluna.IsData) return "date";
        if (coluna.Tipo.ToLower() is "decimal" or "money" or "numeric") return "currency";
        if (coluna.IsBool) return "boolean";
        return "text";
    }

    private static string GetDefaultInputType(ColunaInfo coluna)
    {
        if (coluna.IsData) return coluna.Tipo.ToLower() == "date" ? "date" : "datetime-local";
        if (coluna.IsBool) return "checkbox";
        if (coluna.IsNumerico) return "number";
        if (coluna.IsTexto && coluna.Tamanho > 255) return "textarea";
        if (coluna.ForeignKey != null) return "select";
        return "text";
    }

    private static int GetDefaultColSize(ColunaInfo coluna)
    {
        if (coluna.IsBool) return 2;
        if (coluna.IsData) return 4;
        if (coluna.IsNumerico) return 3;
        if (coluna.Tamanho.HasValue && coluna.Tamanho < 20) return 3;
        if (coluna.Tamanho.HasValue && coluna.Tamanho > 255) return 12;
        return 6;
    }
}

/// <summary>
/// Configuração de propriedade.
/// </summary>
/// <summary>
/// ⭐ v4.0: Configuração de Lookup para Select2.
/// Gerado automaticamente quando um campo tem IsSelect2Ajax = true.
/// </summary>
public class SelectLookupConfig
{
    /// <summary>
    /// Nome da propriedade que usa este lookup (ex: "IdFornecedor")
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Nome da entidade de destino (ex: "Fornecedor")
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// Nome do DTO que será gerado (ex: "FornecedorLookupDto")
    /// </summary>
    public string DtoName { get; set; } = string.Empty;

    /// <summary>
    /// Rota da API backend (ex: "api/gestaoterceirosprestadores/capfornecedores")
    /// </summary>
    public string ApiRoute { get; set; } = string.Empty;

    /// <summary>
    /// Campo que será usado como VALUE no Select2 (ex: "id")
    /// </summary>
    public string ValueField { get; set; } = "id";

    /// <summary>
    /// Campo que será exibido como TEXT no Select2 (ex: "razaoSocial")
    /// </summary>
    public string TextField { get; set; } = "nome";

    /// <summary>
    /// Nome do método que será gerado (ex: "GetFornecedorForSelect")
    /// </summary>
    public string MethodName { get; set; } = string.Empty;

    /// <summary>
    /// DisplayName para mensagens (ex: "Fornecedor")
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Label do campo no formulário
    /// </summary>
    public string Label { get; set; } = string.Empty;
}



/// <summary>
/// ⭐ v4.5 NOVO: Configuração de Lookup (Select2 Ajax) que vem do JSON PropertyMetadata
/// </summary>
public class LookupConfig
{
    /// <summary>
    /// Endpoint da API para buscar opções (ex: "/api/administracaopessoal/ufs/lookup")
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Campo do valor (ID) - padrão "id"
    /// </summary>
    public string ValueField { get; set; } = "id";

    /// <summary>
    /// Campo do texto de exibição - padrão "nome"
    /// </summary>
    public string TextField { get; set; } = "nome";

    /// <summary>
    /// Permite busca/autocomplete
    /// </summary>
    public bool AllowSearch { get; set; } = true;

    /// <summary>
    /// Permite limpar seleção
    /// </summary>
    public bool AllowClear { get; set; } = true;

    /// <summary>
    /// Campo pai (cascata)
    /// </summary>
    public string? DependsOn { get; set; }

    /// <summary>
    /// Parâmetro de filtro do pai
    /// </summary>
    public string? DependsOnParam { get; set; }

    /// <summary>
    /// Mínimo de caracteres para busca
    /// </summary>
    public int MinSearchLength { get; set; } = 0;

    /// <summary>
    /// Permite seleção múltipla
    /// </summary>
    public bool Multiple { get; set; } = false;

    /// <summary>
    /// Módulo da API (ex: "AdministracaoPessoal")
    /// </summary>
    public string? Module { get; set; }

    /// <summary>
    /// Route da entidade na API (ex: "ufs")
    /// </summary>
    public string? Route { get; set; }

    /// <summary>
    /// Campos adicionais a retornar
    /// </summary>
    public List<string>? AdditionalFields { get; set; }
}

// =============================================================================
// ⭐ COPIE ESTE BLOCO E COLE NO FINAL DO SEU Models.cs (ANTES DO #endregion FINAL)


public class PropertyConfig
{
    public string Name { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string CSharpType { get; set; } = string.Empty;
    public string CSharpTypeSimple { get; set; } = string.Empty;
    public string SqlType { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    public bool IsPrimaryKey { get; set; }
    public bool IsPrimaryKeyDefinedByUser { get; set; } // PK definida manualmente pelo usuário (não existe no banco)
    public bool IsIdentity { get; set; }
    public bool IsNullable { get; set; }
    public bool IsReadOnly { get; set; }
    public bool Required { get; set; }
    public int? MaxLength { get; set; }
    public int? MinLength { get; set; }

    public bool IsGuid { get; set; }
    public bool IsString { get; set; }
    public bool IsInt { get; set; }
    public bool IsLong { get; set; }
    public bool IsDecimal { get; set; }
    public bool IsBool { get; set; }
    public bool IsDateTime { get; set; }

    public bool HasForeignKey { get; set; }
    public string? ForeignKeyTable { get; set; }
    public string? ForeignKeyColumn { get; set; }

    public ListConfig? List { get; set; }
    public FormConfig? Form { get; set; }

    // ⭐ v4.5 NOVO: Lookup do JSON PropertyMetadata
    public LookupConfig? Lookup { get; set; }  // ← ADICIONE ESTA LINHA


    /// <summary>
    /// Gera declaração da propriedade C#.
    /// </summary>
    public string GetPropertyDeclaration()
    {
        var defaultValue = CSharpType switch
        {
            "string" => " = string.Empty;",
            _ when CSharpType.EndsWith("?") => "",
            _ => ""
        };

        return $"public {CSharpType} {Name} {{ get; set; }}{defaultValue}";
    }
}

/// <summary>
/// Configuração de exibição na listagem.
/// </summary>
public class ListConfig
{
    public bool Show { get; set; } = true;
    public int Order { get; set; }
    public string? Title { get; set; }
    public string Format { get; set; } = "text";
    public string? Width { get; set; }
    public string Align { get; set; } = "left";
    public bool Sortable { get; set; } = true;
}

/// <summary>
/// Configuração de exibição no formulário.
/// </summary>
public class FormConfig
{
    public bool Show { get; set; } = true;
    public bool ShowOnCreate { get; set; } = true;
    public bool ShowOnEdit { get; set; } = true;
    public int Order { get; set; }
    public string InputType { get; set; } = "text";
    public int ColSize { get; set; } = 6;
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public bool Disabled { get; set; }
    public int Rows { get; set; } = 3;

    // =========================================================================
    // ⭐ v3.3 - SUPORTE A TABS
    // =========================================================================

    /// <summary>
    /// Nome da aba onde este campo deve aparecer.
    /// </summary>
    public string? Tab { get; set; }

    /// <summary>
    /// Nome do grupo dentro da aba.
    /// </summary>
    public string? Group { get; set; }


    // =========================================================================
    // ⭐ v4.4 - CONFIGURAÇÃO DE SELECT/COMBOBOX (AJAX)
    // =========================================================================

    /// <summary>
    /// ⭐ v4.0: Se true, gera automaticamente DTO, Action e Service para Select2.
    /// </summary>
    public bool IsSelect2Ajax { get; set; }

    /// <summary>
    /// Rota da API backend para buscar dados (ex: "api/fornecedores").
    /// </summary>
    public string? SelectApiRoute { get; set; }

    public string? SelectEndpoint { get; set; }
    public string? SelectValueField { get; set; }
    public string? SelectTextField { get; set; }
}

#endregion

#region Módulos e Funções

/// <summary>
/// Configuração de módulos disponíveis.
/// </summary>
public class ModuloConfig
{
    public string Nome { get; set; } = string.Empty;
    public string Rota { get; set; } = string.Empty;
    public string CdSistema { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public string Icone { get; set; } = string.Empty;

    public static List<ModuloConfig> GetModulos() =>
    [
        new() { Nome = "Identity", Rota = "identity", CdSistema = "SEG",
                Namespace = "RhSensoERP.Modules.Identity", Icone = "fas fa-shield-alt" },
        new() { Nome = "GestaoDePessoas", Rota = "gestaodepessoas", CdSistema = "RHU",
                Namespace = "RhSensoERP.Modules.GestaoDePessoas", Icone = "fas fa-users" },
        new() { Nome = "GestaoDeTerceiros", Rota = "gestaodeterceiros", CdSistema = "GTC",
                Namespace = "RhSensoERP.Modules.GestaoDeTerceiros", Icone = "fas fa-building" },
        new() { Nome = "ControleAcessoPortaria", Rota = "controleacessoportaria", CdSistema = "CAP",
                Namespace = "RhSensoERP.Modules.ControleAcessoPortaria", Icone = "fas fa-door-open" },
        new() { Nome = "ControleDePonto", Rota = "controledeponto", CdSistema = "CPO",
                Namespace = "RhSensoERP.Modules.ControleDePonto", Icone = "fas fa-clock" },
        new() { Nome = "TreinamentoDesenvolvimento", Rota = "treinamentodesenvolvimento", CdSistema = "TRE",
                Namespace = "RhSensoERP.Modules.TreinamentoDesenvolvimento", Icone = "fas fa-graduation-cap" },
        new() { Nome = "SaudeOcupacional", Rota = "saudeocupacional", CdSistema = "MSO",
                Namespace = "RhSensoERP.Modules.SaudeOcupacional", Icone = "fas fa-heartbeat" },
        new() { Nome = "Avaliacao", Rota = "avaliacao", CdSistema = "AVA",
                Namespace = "RhSensoERP.Modules.Avaliacao", Icone = "fas fa-chart-line" },
        new() { Nome = "eSocial", Rota = "esocial", CdSistema = "ESO",
                Namespace = "RhSensoERP.Modules.eSocial", Icone = "fas fa-file-contract" },
        new() { Nome = "GestaoDeEpi", Rota = "gestaodeepi", CdSistema = "EPI",
                Namespace = "RhSensoERP.Modules.GestaoDeEpi", Icone = "fas fa-hard-hat" },
        new() { Nome = "Integracoes", Rota = "integracoes", CdSistema = "INT",
                Namespace = "RhSensoERP.Modules.Integracoes", Icone = "fas fa-plug" },
    ];
}

/// <summary>
/// Informações de função do sistema (tabela fucn1).
/// </summary>
public class FuncaoInfo
{
    public string CdFuncao { get; set; } = string.Empty;
    public string DcFuncao { get; set; } = string.Empty;
    public string CdSistema { get; set; } = string.Empty;
}

/// <summary>
/// Informações de coluna de display para tabelas relacionadas.
/// </summary>
public class TabelaDisplayInfo
{
    public string NomeTabela { get; set; } = string.Empty;
    public string? ColunaDisplay { get; set; }
    public string? ColunaDescricao { get; set; }
    public string? ColunaCodigo { get; set; }
}

#endregion

#region Request Legado (compatibilidade)

/// <summary>
/// Request de geração legado (compatibilidade com versão anterior).
/// </summary>
public class GeracaoRequest
{
    public string NomeTabela { get; set; } = string.Empty;
    public string CdFuncao { get; set; } = string.Empty;
    public string CdSistema { get; set; } = "RHU";
    public string Modulo { get; set; } = "GestaoDePessoas";
    public string ModuloRota { get; set; } = "gestaodepessoas";
    public string DisplayName { get; set; } = string.Empty;
    public string Icone { get; set; } = "fas fa-table";
    public int MenuOrder { get; set; } = 10;
    public bool GerarApiController { get; set; } = false;
    public bool GerarWebController { get; set; } = true;
    public bool GerarMenuItem { get; set; } = true;
    public bool IsLegacyTable { get; set; } = true;
    public bool GerarNavigation { get; set; } = true;
    public bool ApenasNavigationPorGuid { get; set; } = false;
    public List<string> ColunasListagem { get; set; } = [];
    public List<string> ColunasFormulario { get; set; } = [];

    /// <summary>
    /// Converte para FullStackRequest.
    /// </summary>
    public FullStackRequest ToFullStackRequest()
    {
        return new FullStackRequest
        {
            NomeTabela = NomeTabela,
            CdFuncao = CdFuncao,
            CdSistema = CdSistema,
            Modulo = Modulo,
            ModuloRota = ModuloRota,
            DisplayName = DisplayName,
            Icone = Icone,
            MenuOrder = MenuOrder,
            GerarApiController = GerarApiController,
            GerarWebController = GerarWebController,
            GerarMenuItem = GerarMenuItem,
            IsLegacyTable = IsLegacyTable,
            GerarNavigation = GerarNavigation,
            ApenasNavigationPorGuid = ApenasNavigationPorGuid
        };
    }
}

/// <summary>
/// Resultado da geração legado (compatibilidade).
/// </summary>
public class GeracaoResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string NomeTabela { get; set; } = string.Empty;
    public string NomeEntidade { get; set; } = string.Empty;
    public string CodigoEntidade { get; set; } = string.Empty;
    public string JsonConfig { get; set; } = string.Empty;
    public string NomeArquivoEntidade => $"{NomeEntidade}.cs";
    public string NomeArquivoJson => $"crud-config-{NomeEntidade}.json";
    public List<string> NavigationsGeradas { get; set; } = [];
}


#endregion