// =============================================================================
// RHSENSOERP CRUD TOOL - WEB MODELS TEMPLATE
// Versão: 2.0 - Compatível com estrutura existente
// =============================================================================
using RhSensoERP.CrudTool.Models;
using System.Text;

namespace RhSensoERP.CrudTool.Templates;

/// <summary>
/// Gera Models para o projeto Web.
/// NÃO gera classes base (Result, ApiResponse, etc.) pois já existem.
/// </summary>
public static class WebModelsTemplate
{
    /// <summary>
    /// Gera o DTO de leitura.
    /// </summary>
    public static string GenerateDto(EntityConfig entity)
    {
        var properties = GenerateProperties(entity.Properties);

        return $@"// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool v2.0
// Entity: {entity.Name}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================

namespace RhSensoERP.Web.Models.{entity.PluralName};

/// <summary>
/// DTO de leitura para {entity.DisplayName}.
/// Compatível com backend: {entity.BackendNamespace ?? $"RhSensoERP.Modules.{entity.Module}.Application.DTOs.{entity.PluralName}"}.{entity.Name}Dto
/// </summary>
public class {entity.Name}Dto
{{
{properties}
}}
";
    }

    /// <summary>
    /// Gera o Request de criação.
    /// </summary>
    public static string GenerateCreateRequest(EntityConfig entity)
    {
        var createProps = entity.Properties
            .Where(p => !p.IsReadOnly && !p.IsPrimaryKey)
            .ToList();

        var properties = GeneratePropertiesWithValidation(createProps);

        return $@"// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool v2.0
// Entity: {entity.Name}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.{entity.PluralName};

/// <summary>
/// Request para criação de {entity.DisplayName}.
/// Compatível com backend: Create{entity.Name}Request
/// </summary>
public class Create{entity.Name}Request
{{
{properties}
}}
";
    }

    /// <summary>
    /// Gera o Request de atualização.
    /// </summary>
    public static string GenerateUpdateRequest(EntityConfig entity)
    {
        var updateProps = entity.Properties
            .Where(p => !p.IsPrimaryKey && !p.IsReadOnly)
            .ToList();

        var properties = GeneratePropertiesWithValidation(updateProps);

        return $@"// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool v2.0
// Entity: {entity.Name}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.{entity.PluralName};

/// <summary>
/// Request para atualização de {entity.DisplayName}.
/// Compatível com backend: Update{entity.Name}Request
/// </summary>
public class Update{entity.Name}Request
{{
{properties}
}}
";
    }

    /// <summary>
    /// Gera o ListViewModel que herda de BaseListViewModel.
    /// </summary>
    public static string GenerateListViewModel(EntityConfig entity)
    {
        return $@"// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool v2.0
// Entity: {entity.Name}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.{entity.PluralName};

/// <summary>
/// ViewModel para listagem de {entity.DisplayName}.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class {entity.PluralName}ListViewModel : BaseListViewModel
{{
    public {entity.PluralName}ListViewModel()
    {{
        // Inicializa propriedades padrão
        InitializeDefaults(""{entity.PluralName}"", ""{entity.DisplayName}"");
        
        // Configurações específicas
        PageTitle = ""{entity.DisplayName}"";
        PageIcon = ""fas fa-list"";
        CdFuncao = ""{entity.CdFuncao}"";
    }}

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<{entity.Name}Dto> Items {{ get; set; }} = new();
}}
";
    }

    #region Helper Methods

    /// <summary>
    /// Gera propriedades sem validação (para DTOs de leitura).
    /// </summary>
    private static string GenerateProperties(List<PropertyConfig> properties)
    {
        var sb = new StringBuilder();

        foreach (var prop in properties)
        {
            // Comentário XML com DisplayName
            if (!string.IsNullOrEmpty(prop.DisplayName))
            {
                sb.AppendLine($"    /// <summary>");
                sb.AppendLine($"    /// {prop.DisplayName}");
                sb.AppendLine($"    /// </summary>");
            }

            // Propriedade
            sb.AppendLine($"    {prop.GetPropertyDeclaration()}");
            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Gera propriedades com validação (para Requests de Create/Update).
    /// </summary>
    private static string GeneratePropertiesWithValidation(List<PropertyConfig> properties)
    {
        var sb = new StringBuilder();

        foreach (var prop in properties)
        {
            // Comentário XML
            if (!string.IsNullOrEmpty(prop.DisplayName))
            {
                sb.AppendLine($"    /// <summary>");
                sb.AppendLine($"    /// {prop.DisplayName}");
                sb.AppendLine($"    /// </summary>");
                sb.AppendLine($"    [Display(Name = \"{prop.DisplayName}\")]");
            }

            // Required
            if (prop.Required && !prop.IsNullable)
            {
                var errorMsg = !string.IsNullOrEmpty(prop.DisplayName)
                    ? prop.DisplayName
                    : prop.Name;
                sb.AppendLine($"    [Required(ErrorMessage = \"{errorMsg} é obrigatório\")]");
            }

            // StringLength
            if (prop.MaxLength.HasValue && prop.IsString)
            {
                if (prop.MinLength.HasValue && prop.MinLength.Value > 0)
                {
                    sb.AppendLine($"    [StringLength({prop.MaxLength.Value}, MinimumLength = {prop.MinLength.Value}, " +
                                 $"ErrorMessage = \"{prop.DisplayName ?? prop.Name} deve ter entre {{2}} e {{1}} caracteres\")]");
                }
                else
                {
                    sb.AppendLine($"    [StringLength({prop.MaxLength.Value}, " +
                                 $"ErrorMessage = \"{prop.DisplayName ?? prop.Name} deve ter no máximo {{1}} caracteres\")]");
                }
            }

            // Propriedade
            sb.AppendLine($"    {prop.GetPropertyDeclaration()}");
            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }

    #endregion
}
