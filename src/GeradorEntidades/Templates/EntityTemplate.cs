// =============================================================================
// GERADOR FULL-STACK v3.2 - ENTITY TEMPLATE
// Gera entidades do Domain (backend)
// =============================================================================

using GeradorEntidades.Models;
using System.Text;

namespace GeradorEntidades.Templates;

/// <summary>
/// Gera entidade do Domain para o backend.
/// </summary>
public static class EntityTemplate
{
    /// <summary>
    /// Gera a entidade C# para o Domain.
    /// </summary>
    public static GeneratedFile Generate(TabelaInfo tabela, FullStackRequest request, List<string> navigationsGeradas)
    {
        var entityName = tabela.NomePascalCase;
        var tableName = tabela.NomeTabela.ToLower();
        var schema = tabela.Schema ?? "dbo";

        // Determina namespace baseado no módulo
        var moduloConfig = ModuloConfig.GetModulos()
            .FirstOrDefault(m => m.Nome.Equals(request.Modulo, StringComparison.OrdinalIgnoreCase));

        var baseNamespace = moduloConfig?.Namespace ?? $"RhSensoERP.Modules.{request.Modulo}";

        var sb = new StringBuilder();

        // Header
        sb.AppendLine("// =============================================================================");
        sb.AppendLine($"// ARQUIVO GERADO POR GeradorFullStack v3.2");
        sb.AppendLine($"// Entity: {entityName}");
        sb.AppendLine($"// Table: {schema}.{tableName}");
        sb.AppendLine($"// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();

        // Usings
        sb.AppendLine("using System.ComponentModel.DataAnnotations;");
        sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
        sb.AppendLine($"using {baseNamespace}.Domain.Common;");
        sb.AppendLine();

        // Namespace
        sb.AppendLine($"namespace {baseNamespace}.Domain.Entities;");
        sb.AppendLine();

        // Descrição da tabela
        if (!string.IsNullOrEmpty(tabela.Descricao))
        {
            sb.AppendLine("/// <summary>");
            sb.AppendLine($"/// {tabela.Descricao}");
            sb.AppendLine("/// </summary>");
        }

        // Atributos da tabela
        sb.AppendLine($"[Table(\"{tableName}\", Schema = \"{schema}\")]");

        // Classe
        var baseClass = DetermineBaseClass(tabela, request);
        sb.AppendLine($"public class {entityName} : {baseClass}");
        sb.AppendLine("{");

        // Propriedades
        foreach (var coluna in tabela.Colunas)
        {
            GenerateProperty(sb, coluna, tabela, request, navigationsGeradas);
        }

        // Navigation Properties (ManyToOne)
        if (request.GerarNavigation)
        {
            GenerateNavigationProperties(sb, tabela, request, navigationsGeradas);
        }

        sb.AppendLine("}");

        return new GeneratedFile
        {
            FileName = $"{entityName}.cs",
            RelativePath = $"Backend/Domain/Entities/{entityName}.cs",
            Content = sb.ToString(),
            FileType = "Entity"
        };
    }

    /// <summary>
    /// Determina a classe base apropriada.
    /// </summary>
    private static string DetermineBaseClass(TabelaInfo tabela, FullStackRequest request)
    {
        // Se tem IdSaaS, usa BaseMultiTenantEntity
        var hasIdSaas = tabela.Colunas.Any(c =>
            c.Nome.Equals("IdSaaS", StringComparison.OrdinalIgnoreCase) ||
            c.Nome.Equals("idsaas", StringComparison.OrdinalIgnoreCase));

        if (hasIdSaas)
            return "BaseMultiTenantEntity";

        // Se tem colunas de auditoria
        var hasAudit = tabela.Colunas.Any(c =>
            c.Nome.Equals("DtCriacao", StringComparison.OrdinalIgnoreCase) ||
            c.Nome.Equals("DtAtualizacao", StringComparison.OrdinalIgnoreCase));

        if (hasAudit)
            return "BaseAuditableEntity";

        return "BaseEntity";
    }

    /// <summary>
    /// Gera uma propriedade.
    /// </summary>
    private static void GenerateProperty(
        StringBuilder sb,
        ColunaInfo coluna,
        TabelaInfo tabela,
        FullStackRequest request,
        List<string> navigationsGeradas)
    {
        sb.AppendLine();

        // Descrição
        if (!string.IsNullOrEmpty(coluna.Descricao))
        {
            sb.AppendLine($"    /// <summary>");
            sb.AppendLine($"    /// {coluna.Descricao}");
            sb.AppendLine($"    /// </summary>");
        }

        // Key
        if (coluna.IsPrimaryKey)
        {
            sb.AppendLine("    [Key]");
            if (coluna.IsIdentity)
            {
                sb.AppendLine("    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
            }
        }

        // Column
        var columnAttrs = new List<string> { $"\"{coluna.Nome}\"" };

        if (coluna.Tipo.ToLower() is "varchar" or "nvarchar" or "char" or "nchar")
        {
            var typeStr = coluna.Tipo.ToLower().StartsWith("n") ? "NVarChar" : "VarChar";
            columnAttrs.Add($"TypeName = \"{typeStr}({coluna.Tamanho?.ToString() ?? "MAX"})\"");
        }
        else if (coluna.Tipo.ToLower() is "decimal" or "numeric")
        {
            columnAttrs.Add($"TypeName = \"decimal({coluna.Precisao ?? 18},{coluna.Escala ?? 2})\"");
        }

        sb.AppendLine($"    [Column({string.Join(", ", columnAttrs)})]");

        // Required
        if (!coluna.IsNullable && !coluna.IsPrimaryKey && coluna.TipoCSharp != "string")
        {
            sb.AppendLine("    [Required]");
        }

        // StringLength
        if (coluna.Tamanho.HasValue && coluna.IsTexto)
        {
            sb.AppendLine($"    [StringLength({coluna.Tamanho.Value})]");
        }

        // Propriedade
        var defaultValue = coluna.TipoCSharp switch
        {
            "string" => " = string.Empty;",
            _ when coluna.TipoCSharp.EndsWith("?") => "",
            _ => ""
        };

        sb.AppendLine($"    public {coluna.TipoCSharp} {coluna.NomePascalCase} {{ get; set; }}{defaultValue}");
    }

    /// <summary>
    /// Gera Navigation Properties.
    /// </summary>
    private static void GenerateNavigationProperties(
        StringBuilder sb,
        TabelaInfo tabela,
        FullStackRequest request,
        List<string> navigationsGeradas)
    {
        // Agrupa FKs únicas (evita duplicatas)
        var fksUnicas = tabela.ForeignKeys
            .GroupBy(fk => fk.ChaveUnica)
            .Select(g => g.First())
            .Where(fk => !fk.IsParteDeFkComposta) // Ignora FKs compostas
            .ToList();

        // Adiciona FKs das colunas
        foreach (var coluna in tabela.Colunas.Where(c => c.ForeignKey != null))
        {
            var fk = coluna.ForeignKey!;
            if (!fksUnicas.Any(f => f.ChaveUnica == fk.ChaveUnica) && !fk.IsParteDeFkComposta)
            {
                fksUnicas.Add(fk);
            }
        }

        if (!fksUnicas.Any())
            return;

        sb.AppendLine();
        sb.AppendLine("    // =========================================================================");
        sb.AppendLine("    // Navigation Properties");
        sb.AppendLine("    // =========================================================================");

        foreach (var fk in fksUnicas)
        {
            // Filtra se deve gerar apenas por Guid
            if (request.ApenasNavigationPorGuid && !fk.IsFkByGuid)
                continue;

            // Verifica configuração personalizada
            var fkConfig = request.ConfiguracoesFk?
                .FirstOrDefault(c => c.ColunaOrigem.Equals(fk.ColunaOrigem, StringComparison.OrdinalIgnoreCase));

            if (fkConfig?.Ignorar == true)
                continue;

            var navName = fkConfig?.NavigationName ?? fk.NavigationPropertyName;
            var targetEntity = fk.EntidadeDestino;

            sb.AppendLine();
            sb.AppendLine($"    /// <summary>");
            sb.AppendLine($"    /// Navegação para {targetEntity} via {fk.ColunaOrigem}.");
            sb.AppendLine($"    /// </summary>");
            sb.AppendLine($"    [ForeignKey(nameof({TabelaInfo.ToPascalCase(fk.ColunaOrigem)}))]");
            sb.AppendLine($"    public virtual {targetEntity}? {navName} {{ get; set; }}");

            navigationsGeradas.Add($"{navName} -> {targetEntity}");
        }
    }
}
