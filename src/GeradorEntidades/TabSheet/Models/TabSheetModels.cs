// =============================================================================
// TABSHEET GENERATOR - MODELOS DE CONFIGURAÇÃO
// Versão: 1.0.1 (Corrigido)
// =============================================================================

namespace GeradorEntidades.TabSheet.Models;

#region Helpers Compartilhados

internal static class TabSheetHelpers
{
    public static string CleanTableName(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        var prefixes = new[] { "tbl_", "tb_", "tab_", "t_" };
        foreach (var prefix in prefixes)
            if (name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return name[prefix.Length..];
        return name;
    }

    public static string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        if (input.Contains('_'))
        {
            var parts = input.Split('_', StringSplitOptions.RemoveEmptyEntries);
            return string.Concat(parts.Select(p => char.ToUpper(p[0]) + p[1..].ToLower()));
        }
        return char.ToUpper(input[0]) + input[1..];
    }

    public static string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return char.ToLower(input[0]) + input[1..];
    }

    public static string Pluralize(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        if (name.EndsWith("ao", StringComparison.OrdinalIgnoreCase)) return name[..^2] + "oes";
        if (name.EndsWith("r", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith("z", StringComparison.OrdinalIgnoreCase)) return name + "es";
        if (name.EndsWith("l", StringComparison.OrdinalIgnoreCase)) return name[..^1] + "is";
        if (name.EndsWith("m", StringComparison.OrdinalIgnoreCase)) return name[..^1] + "ns";
        return name + "s";
    }
}

#endregion

#region Configuração Principal

public class TabSheetConfiguration
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MasterTableConfig MasterTable { get; set; } = new();
    public List<TabDefinition> Tabs { get; set; } = [];
    public TabSheetGenerationOptions GenerationOptions { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = [];

    public string SuggestedFileName => $"{Id}.tabsheet.json";
    public bool IsValid => !string.IsNullOrWhiteSpace(MasterTable.TableName)
                           && !string.IsNullOrWhiteSpace(MasterTable.PrimaryKey)
                           && Tabs.Count > 0 && Tabs.All(t => t.IsValid);

    public List<string> ValidationErrors
    {
        get
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Id)) errors.Add("Id é obrigatório.");
            if (string.IsNullOrWhiteSpace(Title)) errors.Add("Title é obrigatório.");
            if (string.IsNullOrWhiteSpace(MasterTable.TableName)) errors.Add("MasterTable.TableName é obrigatório.");
            if (string.IsNullOrWhiteSpace(MasterTable.PrimaryKey)) errors.Add("MasterTable.PrimaryKey é obrigatório.");
            if (Tabs.Count == 0) errors.Add("Pelo menos uma Tab é obrigatória.");
            for (int i = 0; i < Tabs.Count; i++)
                if (!Tabs[i].IsValid)
                    errors.Add($"Tab[{i}] '{Tabs[i].Title}': {string.Join(", ", Tabs[i].ValidationErrors)}");
            return errors;
        }
    }
}

#endregion

#region Tabela Mestre

public class MasterTableConfig
{
    public string TableName { get; set; } = string.Empty;
    public string Schema { get; set; } = "dbo";
    public string PrimaryKey { get; set; } = string.Empty;
    public string? PrimaryKeyType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? DisplayColumn { get; set; }
    public string Icon { get; set; } = "fas fa-table";
    public bool IsLegacyTable { get; set; } = true;

    public string EntityName => TabSheetHelpers.ToPascalCase(TabSheetHelpers.CleanTableName(TableName));
    public string EntityNameCamel => TabSheetHelpers.ToCamelCase(EntityName);
    public string PluralName => TabSheetHelpers.Pluralize(EntityName);
    public string FullTableName => $"[{Schema}].[{TableName}]";
}

#endregion

#region Tab Definition

public class TabDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string Schema { get; set; } = "dbo";
    public string ForeignKey { get; set; } = string.Empty;
    public string? PrimaryKey { get; set; }
    public RelationshipType RelationType { get; set; } = RelationshipType.OneToMany;
    public string Icon { get; set; } = "fas fa-list";
    public int Order { get; set; } = 0;
    public bool Enabled { get; set; } = true;
    public bool LazyLoad { get; set; } = true;
    public bool AllowCreate { get; set; } = true;
    public bool AllowEdit { get; set; } = true;
    public bool AllowDelete { get; set; } = true;
    public bool ShowBadge { get; set; } = true;
    public TabAdvancedOptions AdvancedOptions { get; set; } = new();

    public bool IsValid => !string.IsNullOrWhiteSpace(Title)
                           && !string.IsNullOrWhiteSpace(TableName)
                           && !string.IsNullOrWhiteSpace(ForeignKey);

    public List<string> ValidationErrors
    {
        get
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Title)) errors.Add("Title é obrigatório.");
            if (string.IsNullOrWhiteSpace(TableName)) errors.Add("TableName é obrigatório.");
            if (string.IsNullOrWhiteSpace(ForeignKey)) errors.Add("ForeignKey é obrigatório.");
            return errors;
        }
    }

    public string TabId => !string.IsNullOrWhiteSpace(Id) ? Id : $"tab-{TableName.ToLower()}";
    public string EntityName => TabSheetHelpers.ToPascalCase(TabSheetHelpers.CleanTableName(TableName));
    public string NavigationPropertyName => !string.IsNullOrWhiteSpace(AdvancedOptions.NavigationName)
        ? AdvancedOptions.NavigationName : TabSheetHelpers.Pluralize(EntityName);
}

#endregion

#region Advanced Options

public class TabAdvancedOptions
{
    public string? NavigationName { get; set; }
    public List<string> VisibleColumns { get; set; } = [];
    public List<string> HiddenColumns { get; set; } = [];
    public string? DefaultSortColumn { get; set; }
    public SortDirection DefaultSortDirection { get; set; } = SortDirection.Ascending;
    public Dictionary<string, object> FixedFilters { get; set; } = [];
    public string? CssClass { get; set; }
    public string? CustomTemplate { get; set; }
}

#endregion

#region Generation Options

public class TabSheetGenerationOptions
{
    public bool GenerateMasterEntity { get; set; } = true;
    public bool GenerateDetailEntities { get; set; } = true;
    public bool IncludeNavigationProperties { get; set; } = true;
    public bool GenerateRelationshipAttributes { get; set; } = true;
    public bool GenerateMasterView { get; set; } = true;
    public bool GenerateTabPartials { get; set; } = true;
    public bool GenerateJavaScript { get; set; } = true;
    public bool GenerateCss { get; set; } = false;
    public bool GenerateMasterController { get; set; } = true;
    public bool GenerateTabEndpoints { get; set; } = true;
    public string Module { get; set; } = "GestaoDePessoas";
    public string ModuleRoute { get; set; } = "gestaodepessoas";
    public string? BaseNamespace { get; set; }
    public bool UseDataTables { get; set; } = true;
    public bool UseAdminLTE { get; set; } = true;
}

#endregion

#region Enums

public enum RelationshipType { OneToMany, OneToOne, ManyToMany }
public enum SortDirection { Ascending, Descending }

#endregion

#region Result

public class TabSheetGenerationResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public TabSheetConfiguration? Configuration { get; set; }
    public List<GeneratedTabSheetFile> GeneratedFiles { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public long ElapsedMilliseconds { get; set; }

    public static TabSheetGenerationResult Ok(TabSheetConfiguration config, List<GeneratedTabSheetFile> files)
        => new() { Success = true, Configuration = config, GeneratedFiles = files };
    public static TabSheetGenerationResult Fail(string error)
        => new() { Success = false, Error = error };
}

public class GeneratedTabSheetFile
{
    public string FileName { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public TabSheetFileType FileType { get; set; }
    public string? EntityName { get; set; }
    public bool IsMasterFile { get; set; }
    public int ContentSize => Content?.Length ?? 0;
}

public enum TabSheetFileType
{
    Entity, Controller, View, PartialView, JavaScript, Css, Dto, ServiceInterface, ServiceImplementation, Configuration
}

#endregion
