// =============================================================================
// TABSHEET GENERATOR - TEMPLATE DE ENTIDADES
// Versão: 1.0.0
// Autor: RhSensoERP Team
// Data: 2024
// 
// Gera entidades C# com atributos de relacionamento para mestre/detalhe.
// =============================================================================

using System.Text;
using GeradorEntidades.Models;
using GeradorEntidades.TabSheet.Models;

namespace GeradorEntidades.TabSheet.Templates;

/// <summary>
/// Template para geração de entidades com relacionamentos mestre/detalhe.
/// </summary>
public static class TabSheetEntityTemplate
{
    #region Entidade Mestre

    /// <summary>
    /// Gera a entidade mestre com Navigation Properties para os detalhes.
    /// </summary>
    public static GeneratedTabSheetFile GenerateMasterEntity(
        TabSheetConfiguration config,
        TabelaInfo tabela)
    {
        var sb = new StringBuilder();
        var entityName = config.MasterTable.EntityName;
        var options = config.GenerationOptions;
        var baseNamespace = options.BaseNamespace ?? $"RhSensoERP.Modules.{options.Module}.Core.Entities";

        // Cabeçalho
        sb.AppendLine("// =============================================================================");
        sb.AppendLine($"// ENTIDADE: {entityName}");
        sb.AppendLine($"// Tabela: {tabela.NomeTabela}");
        sb.AppendLine("// Gerado por: TabSheet Generator");
        sb.AppendLine($"// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();

        // Usings
        sb.AppendLine("using System.ComponentModel.DataAnnotations;");
        sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
        if (options.GenerateRelationshipAttributes)
        {
            sb.AppendLine("using GeradorEntidades.TabSheet.Attributes;");
        }
        sb.AppendLine();

        // Namespace
        sb.AppendLine($"namespace {baseNamespace};");
        sb.AppendLine();

        // XML Doc
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// {config.MasterTable.DisplayName}.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("/// <remarks>");
        sb.AppendLine($"/// Tabela: {tabela.NomeTabela}");
        sb.AppendLine("/// Relacionamentos:");
        foreach (var tab in config.Tabs)
        {
            sb.AppendLine($"/// - 1:N → {tab.EntityName} ({tab.Title})");
        }
        sb.AppendLine("/// </remarks>");

        // Atributos da classe
        if (options.GenerateRelationshipAttributes)
        {
            var displayName = EscapeString(config.MasterTable.DisplayName);
            sb.AppendLine($"[MasterEntity(\"{displayName}\")]");
        }
        sb.AppendLine($"[Table(\"{tabela.NomeTabela}\")]");

        // Classe
        sb.AppendLine($"public class {entityName}");
        sb.AppendLine("{");

        // Região: Chave Primária
        sb.AppendLine("    #region Chave Primária");
        sb.AppendLine();

        var pk = tabela.PrimaryKey;
        if (pk != null)
        {
            sb.AppendLine("    /// <summary>");
            sb.AppendLine($"    /// {pk.Descricao ?? "Chave primária"}.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    [Key]");
            if (pk.Nome.ToLower() != pk.NomePascalCase.ToLower())
            {
                sb.AppendLine($"    [Column(\"{pk.Nome}\")]");
            }
            if (pk.IsIdentity)
            {
                sb.AppendLine("    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
            }
            sb.AppendLine($"    public {pk.TipoCSharp} {pk.NomePascalCase} {{ get; set; }}");
            sb.AppendLine();
        }

        sb.AppendLine("    #endregion");
        sb.AppendLine();

        // Região: Propriedades
        sb.AppendLine("    #region Propriedades");
        sb.AppendLine();

        foreach (var coluna in tabela.Colunas.Where(c => !c.IsPrimaryKey))
        {
            GenerateProperty(sb, coluna);
        }

        sb.AppendLine("    #endregion");
        sb.AppendLine();

        // Região: Navigation Properties
        sb.AppendLine("    #region Navigation Properties");
        sb.AppendLine();

        foreach (var tab in config.Tabs)
        {
            sb.AppendLine("    /// <summary>");
            sb.AppendLine($"    /// {tab.Title} relacionados.");
            sb.AppendLine("    /// </summary>");

            if (options.GenerateRelationshipAttributes)
            {
                var tabTitle = EscapeString(tab.Title);
                var tabIcon = EscapeString(tab.Icon);
                sb.AppendLine($"    [DetailCollection(typeof({tab.EntityName}), \"{tab.ForeignKey}\",");
                sb.AppendLine($"        TabTitle = \"{tabTitle}\",");
                sb.AppendLine($"        TabIcon = \"{tabIcon}\",");
                sb.AppendLine($"        TabOrder = {tab.Order})]");
            }

            sb.AppendLine($"    public virtual ICollection<{tab.EntityName}> {tab.NavigationPropertyName} {{ get; set; }} = new List<{tab.EntityName}>();");
            sb.AppendLine();
        }

        sb.AppendLine("    #endregion");

        // Fechar classe
        sb.AppendLine("}");

        return new GeneratedTabSheetFile
        {
            FileName = $"{entityName}.cs",
            RelativePath = $"Modules/{options.Module}/Core/Entities/",
            Content = sb.ToString(),
            FileType = TabSheetFileType.Entity,
            EntityName = entityName,
            IsMasterFile = true
        };
    }

    #endregion

    #region Entidade Detalhe

    /// <summary>
    /// Gera a entidade detalhe com FK e Navigation Property para o mestre.
    /// </summary>
    public static GeneratedTabSheetFile GenerateDetailEntity(
        TabSheetConfiguration config,
        TabDefinition tab,
        TabelaInfo tabela)
    {
        var sb = new StringBuilder();
        var entityName = tab.EntityName;
        var masterEntityName = config.MasterTable.EntityName;
        var options = config.GenerationOptions;
        var baseNamespace = options.BaseNamespace ?? $"RhSensoERP.Modules.{options.Module}.Core.Entities";

        // Cabeçalho
        sb.AppendLine("// =============================================================================");
        sb.AppendLine($"// ENTIDADE: {entityName}");
        sb.AppendLine($"// Tabela: {tabela.NomeTabela}");
        sb.AppendLine($"// Mestre: {masterEntityName}");
        sb.AppendLine("// Gerado por: TabSheet Generator");
        sb.AppendLine($"// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();

        // Usings
        sb.AppendLine("using System.ComponentModel.DataAnnotations;");
        sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
        if (options.GenerateRelationshipAttributes)
        {
            sb.AppendLine("using GeradorEntidades.TabSheet.Attributes;");
        }
        sb.AppendLine();

        // Namespace
        sb.AppendLine($"namespace {baseNamespace};");
        sb.AppendLine();

        // XML Doc
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// {tab.Title} de {config.MasterTable.DisplayName}.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("/// <remarks>");
        sb.AppendLine($"/// Tabela: {tabela.NomeTabela}");
        sb.AppendLine($"/// FK: {tab.ForeignKey} → {config.MasterTable.TableName}.{config.MasterTable.PrimaryKey}");
        sb.AppendLine("/// </remarks>");

        // Atributos da classe
        if (options.GenerateRelationshipAttributes)
        {
            var displayName = EscapeString(tab.Title);
            sb.AppendLine($"[DetailEntity(typeof({masterEntityName}), \"{tab.ForeignKey}\",");
            sb.AppendLine($"    DisplayName = \"{displayName}\",");
            sb.AppendLine($"    OnDelete = DeleteBehavior.Cascade)]");
        }
        sb.AppendLine($"[Table(\"{tabela.NomeTabela}\")]");

        // Classe
        sb.AppendLine($"public class {entityName}");
        sb.AppendLine("{");

        // Região: Chave Primária
        sb.AppendLine("    #region Chave Primária");
        sb.AppendLine();

        var pk = tabela.PrimaryKey;
        if (pk != null)
        {
            sb.AppendLine("    /// <summary>");
            sb.AppendLine($"    /// {pk.Descricao ?? "Chave primária"}.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    [Key]");
            if (pk.Nome.ToLower() != pk.NomePascalCase.ToLower())
            {
                sb.AppendLine($"    [Column(\"{pk.Nome}\")]");
            }
            if (pk.IsIdentity)
            {
                sb.AppendLine("    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
            }
            sb.AppendLine($"    public {pk.TipoCSharp} {pk.NomePascalCase} {{ get; set; }}");
            sb.AppendLine();
        }

        sb.AppendLine("    #endregion");
        sb.AppendLine();

        // Região: Chave Estrangeira
        sb.AppendLine("    #region Chave Estrangeira");
        sb.AppendLine();

        var fkColumn = tabela.Colunas.FirstOrDefault(c =>
            c.Nome.Equals(tab.ForeignKey, StringComparison.OrdinalIgnoreCase));

        if (fkColumn != null)
        {
            sb.AppendLine("    /// <summary>");
            sb.AppendLine($"    /// FK para {config.MasterTable.DisplayName}.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    [Required]");
            if (fkColumn.Nome.ToLower() != fkColumn.NomePascalCase.ToLower())
            {
                sb.AppendLine($"    [Column(\"{fkColumn.Nome}\")]");
            }
            sb.AppendLine($"    public {fkColumn.TipoCSharp} {fkColumn.NomePascalCase} {{ get; set; }}");
            sb.AppendLine();
        }

        sb.AppendLine("    #endregion");
        sb.AppendLine();

        // Região: Propriedades
        sb.AppendLine("    #region Propriedades");
        sb.AppendLine();

        foreach (var coluna in tabela.Colunas.Where(c =>
            !c.IsPrimaryKey &&
            !c.Nome.Equals(tab.ForeignKey, StringComparison.OrdinalIgnoreCase)))
        {
            GenerateProperty(sb, coluna);
        }

        sb.AppendLine("    #endregion");
        sb.AppendLine();

        // Região: Navigation Properties
        sb.AppendLine("    #region Navigation Properties");
        sb.AppendLine();

        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// {config.MasterTable.DisplayName} relacionado.");
        sb.AppendLine("    /// </summary>");

        if (options.GenerateRelationshipAttributes)
        {
            var fkPropName = fkColumn?.NomePascalCase ?? tab.ForeignKey;
            sb.AppendLine($"    [MasterReference(\"{fkPropName}\")]");
        }

        var fkProp = fkColumn?.NomePascalCase ?? tab.ForeignKey;
        sb.AppendLine($"    [ForeignKey(\"{fkProp}\")]");
        sb.AppendLine($"    public virtual {masterEntityName} {masterEntityName} {{ get; set; }} = null!;");
        sb.AppendLine();

        sb.AppendLine("    #endregion");

        // Fechar classe
        sb.AppendLine("}");

        return new GeneratedTabSheetFile
        {
            FileName = $"{entityName}.cs",
            RelativePath = $"Modules/{options.Module}/Core/Entities/",
            Content = sb.ToString(),
            FileType = TabSheetFileType.Entity,
            EntityName = entityName,
            IsMasterFile = false
        };
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Gera uma propriedade com seus atributos.
    /// </summary>
    private static void GenerateProperty(StringBuilder sb, ColunaInfo coluna)
    {
        // XML Doc
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// {coluna.Descricao ?? coluna.NomePascalCase}.");
        sb.AppendLine("    /// </summary>");

        // Atributos
        if (!coluna.IsNullable && coluna.IsTexto)
        {
            sb.AppendLine("    [Required]");
        }

        if (coluna.Nome.ToLower() != coluna.NomePascalCase.ToLower())
        {
            sb.AppendLine($"    [Column(\"{coluna.Nome}\")]");
        }

        if (coluna.IsTexto && coluna.Tamanho.HasValue && coluna.Tamanho.Value > 0)
        {
            sb.AppendLine($"    [StringLength({coluna.Tamanho.Value})]");
        }

        // Propriedade
        var defaultValue = GetDefaultValue(coluna);
        sb.AppendLine($"    public {coluna.TipoCSharp} {coluna.NomePascalCase} {{ get; set; }}{defaultValue}");
        sb.AppendLine();
    }

    /// <summary>
    /// Obtém o valor padrão para inicialização.
    /// </summary>
    private static string GetDefaultValue(ColunaInfo coluna)
    {
        if (coluna.IsNullable) return "";

        return coluna.TipoCSharp switch
        {
            "string" => " = string.Empty;",
            "byte[]" => " = Array.Empty<byte>();",
            _ => ""
        };
    }

    /// <summary>
    /// Escapa string para uso em atributos.
    /// </summary>
    private static string EscapeString(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        return value.Replace("\"", "\\\"");
    }

    #endregion
}
