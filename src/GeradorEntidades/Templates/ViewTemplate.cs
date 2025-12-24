// =============================================================================
// GERADOR FULL-STACK v3.3 - VIEW TEMPLATE
// Baseado em RhSensoERP.CrudTool v2.5
// v3.3 - Suporte a Bootstrap Tabs no formulário
// =============================================================================

using GeradorEntidades.Models;
using System.Text;

namespace GeradorEntidades.Templates;

/// <summary>
/// Gera View Razor compatível com CrudBase.js e BaseListViewModel.
/// v3.3: Suporte a Bootstrap Tabs para formulários grandes.
/// </summary>
public static class ViewTemplate
{
    /// <summary>
    /// Alias para GenerateIndex - mantém compatibilidade com FullStackGeneratorService.
    /// </summary>
    public static GeneratedFile Generate(EntityConfig entity) => GenerateIndex(entity);

    /// <summary>
    /// Gera a View Index.cshtml seguindo o padrão do projeto.
    /// </summary>
    public static GeneratedFile GenerateIndex(EntityConfig entity)
    {
        var modulePath = GetModulePath(entity.Module);
        var modulePathLower = modulePath.ToLowerInvariant();
        var iconClass = entity.Icon ?? "fas fa-list";

        // =====================================================================
        // v3.3: Gera formulário com ou sem Tabs
        // =====================================================================
        string formContent;
        if (entity.FormLayout?.UseTabs == true && entity.FormLayout.Tabs?.Count > 1)
        {
            formContent = GenerateFormWithTabs(entity);
        }
        else
        {
            formContent = GenerateFormFields(entity);
        }

        var content = $@"@model RhSensoERP.Web.Models.{modulePath}.{entity.Name}.{entity.Name}ListViewModel
@{{
    ViewData[""Title""] = Model.PageTitle;
    Layout = ""_CrudLayout"";
    ViewData[""EntityName""] = ""{entity.DisplayName}"";
    ViewData[""IconClass""] = ""{iconClass}"";
}}

@* ============================================================================
   Permissões injetadas para JavaScript (ANTES do script específico)
   ============================================================================ *@
<script>
    window.crudPermissions = {{
        canCreate: @Model.CanCreate.ToString().ToLower(),
        canEdit: @Model.CanEdit.ToString().ToLower(),
        canDelete: @Model.CanDelete.ToString().ToLower(),
        canView: @Model.CanView.ToString().ToLower(),
        actions: ""@Model.UserPermissions""
    }};
</script>

@* ============================================================================
   Tabela usando partial _CrudTable (toolbar + datatable)
   ============================================================================ *@
@await Html.PartialAsync(""_CrudTable"", Model)

@* ============================================================================
   Conteúdo do Modal (apenas campos do formulário)
   O wrapper do modal está no _CrudLayout
   ============================================================================ *@
@section ModalContent {{
    <input type=""hidden"" id=""Id"" name=""Id"" />
{formContent}
}}

@* ============================================================================
   Script específico da entidade
   ============================================================================ *@
@section PageScripts {{
    <script src=""~/js/{modulePathLower}/{entity.NameLower}/{entity.NameLower}.js"" asp-append-version=""true""></script>
}}
";

        return new GeneratedFile
        {
            FileName = "Index.cshtml",
            RelativePath = $"Web/Views/{modulePath}/{entity.Name}/Index.cshtml",
            Content = content,
            FileType = "View"
        };
    }

    #region Helper Methods

    /// <summary>
    /// Gera formulário COM Bootstrap Tabs.
    /// v3.3: Novo método para formulários com abas.
    /// </summary>
    private static string GenerateFormWithTabs(EntityConfig entity)
    {
        var sb = new StringBuilder();
        var tabs = entity.FormLayout!.Tabs!;
        var entityNameLower = entity.NameLower;

        // =====================================================================
        // NAVEGAÇÃO DAS TABS
        // =====================================================================
        sb.AppendLine($@"    <!-- ================================================================== -->
    <!-- TABS - Navegação -->
    <!-- ================================================================== -->
    <ul class=""nav nav-tabs mb-3"" id=""{entityNameLower}FormTabs"" role=""tablist"">");

        for (int i = 0; i < tabs.Count; i++)
        {
            var tab = tabs[i];
            var tabId = SanitizeTabId(tab);
            var isActive = i == 0 ? "active" : "";
            var ariaSelected = i == 0 ? "true" : "false";

            sb.AppendLine($@"        <li class=""nav-item"" role=""presentation"">
            <button class=""nav-link {isActive}"" id=""{tabId}-tab"" data-bs-toggle=""tab"" 
                    data-bs-target=""#{tabId}"" type=""button"" role=""tab"" 
                    aria-controls=""{tabId}"" aria-selected=""{ariaSelected}"">
                {EscapeHtml(tab)}
            </button>
        </li>");
        }

        sb.AppendLine(@"    </ul>");

        // =====================================================================
        // CONTEÚDO DAS TABS
        // =====================================================================
        sb.AppendLine($@"
    <!-- ================================================================== -->
    <!-- TABS - Conteúdo -->
    <!-- ================================================================== -->
    <div class=""tab-content"" id=""{entityNameLower}FormTabsContent"">");

        for (int i = 0; i < tabs.Count; i++)
        {
            var tab = tabs[i];
            var tabId = SanitizeTabId(tab);
            var isActive = i == 0 ? "show active" : "";

            // Filtra campos desta aba
            var tabFields = entity.Properties
                .Where(p => p.Form?.Show == true)
                .Where(p => (p.Form!.Tab ?? tabs[0]) == tab)
                .Where(p => !IsAutoGeneratedPrimaryKey(p))
                .OrderBy(p => p.Form!.Order)
                .ToList();

            sb.AppendLine($@"        <div class=""tab-pane fade {isActive}"" id=""{tabId}"" role=""tabpanel"" aria-labelledby=""{tabId}-tab"">");
            sb.AppendLine(@"            <div class=""row"">");

            // Gera campos desta aba
            foreach (var prop in tabFields)
            {
                sb.AppendLine(GenerateFieldHtml(prop, entity));
            }

            sb.AppendLine(@"            </div>");
            sb.AppendLine(@"        </div>");
        }

        sb.AppendLine(@"    </div>");

        return sb.ToString();
    }

    /// <summary>
    /// Gera os campos do formulário SEM tabs (layout simples).
    /// PKs de texto aparecem no formulário (readonly na edição).
    /// </summary>
    private static string GenerateFormFields(EntityConfig entity)
    {
        var sb = new StringBuilder();
        sb.AppendLine(@"    <div class=""row"">");

        // PKs auto-geradas (Identity ou Guid) são puladas
        // PKs de texto (código manual) DEVEM aparecer no formulário
        var formProps = entity.Properties
            .Where(p =>
            {
                if (p.Form?.Show != true) return false;
                if (p.IsPrimaryKey && (p.IsIdentity || p.IsGuid)) return false;
                return true;
            })
            .OrderBy(p => p.Form!.Order)
            .ToList();

        foreach (var prop in formProps)
        {
            sb.AppendLine(GenerateFieldHtml(prop, entity));
        }

        sb.AppendLine(@"    </div>");

        return sb.ToString();
    }

    /// <summary>
    /// Gera o HTML de um campo individual.
    /// </summary>
    private static string GenerateFieldHtml(PropertyConfig prop, EntityConfig entity)
    {
        var config = prop.Form!;
        var colSize = config.ColSize;
        var inputType = config.InputType;
        var placeholder = config.Placeholder ?? $"Digite {prop.DisplayName.ToLower()}...";
        var required = prop.Required ? "required" : "";
        var maxLength = prop.MaxLength.HasValue ? $@" maxlength=""{prop.MaxLength.Value}""" : "";
        var step = prop.IsDecimal ? @" step=""0.01""" : "";

        // PKs de texto são obrigatórias e readonly na edição
        var isPkTexto = prop.IsPrimaryKey && !prop.IsIdentity && !prop.IsGuid;

        if (isPkTexto)
        {
            required = "required";
        }

        var disabled = config.Disabled ? "disabled" : "";
        var pkTextAttr = isPkTexto ? @" data-pk-text=""true""" : "";

        var helpText = !string.IsNullOrEmpty(config.HelpText)
            ? $@"
                <small class=""form-text text-muted"">{EscapeHtml(config.HelpText)}</small>"
            : "";

        // Badge para PK de texto
        var pkBadge = isPkTexto
            ? @" <span class=""badge bg-warning text-dark"" title=""Chave primária - editável apenas na criação"">PK</span>"
            : "";

        var requiredStar = prop.Required || isPkTexto ? @" <span class=""text-danger"">*</span>" : "";

        // =====================================================================
        // CHECKBOX
        // =====================================================================
        if (inputType == "checkbox")
        {
            return $@"                <div class=""col-md-{colSize} mb-3"">
                    <div class=""form-check"">
                        <input type=""checkbox"" class=""form-check-input"" id=""{prop.Name}"" name=""{prop.Name}"" {disabled}{pkTextAttr} />
                        <label class=""form-check-label"" for=""{prop.Name}"">{EscapeHtml(prop.DisplayName)}{pkBadge}</label>
                    </div>{helpText}
                </div>";
        }

        // =====================================================================
        // TEXTAREA
        // =====================================================================
        if (inputType == "textarea")
        {
            return $@"                <div class=""col-md-{colSize} mb-3"">
                    <label for=""{prop.Name}"" class=""form-label"">
                        {EscapeHtml(prop.DisplayName)}{requiredStar}{pkBadge}
                    </label>
                    <textarea class=""form-control"" id=""{prop.Name}"" name=""{prop.Name}"" 
                              rows=""{config.Rows}"" placeholder=""{EscapeHtml(placeholder)}"" 
                              {required} {disabled}{maxLength}{pkTextAttr}></textarea>{helpText}
                </div>";
        }

        // =====================================================================
        // SELECT
        // =====================================================================
        if (inputType == "select")
        {
            return $@"                <div class=""col-md-{colSize} mb-3"">
                    <label for=""{prop.Name}"" class=""form-label"">
                        {EscapeHtml(prop.DisplayName)}{requiredStar}{pkBadge}
                    </label>
                    <select class=""form-select"" id=""{prop.Name}"" name=""{prop.Name}"" {required} {disabled}{pkTextAttr}>
                        <option value="""">Selecione...</option>
                        <!-- Preencher via JavaScript -->
                    </select>{helpText}
                </div>";
        }

        // =====================================================================
        // INPUT PADRÃO (text, number, date, email, etc.)
        // =====================================================================
        return $@"                <div class=""col-md-{colSize} mb-3"">
                    <label for=""{prop.Name}"" class=""form-label"">
                        {EscapeHtml(prop.DisplayName)}{requiredStar}{pkBadge}
                    </label>
                    <input type=""{inputType}"" class=""form-control"" id=""{prop.Name}"" name=""{prop.Name}"" 
                           placeholder=""{EscapeHtml(placeholder)}"" {required} {disabled}{maxLength}{step}{pkTextAttr} />{helpText}
                </div>";
    }

    /// <summary>
    /// Verifica se a propriedade é uma PK auto-gerada.
    /// </summary>
    private static bool IsAutoGeneratedPrimaryKey(PropertyConfig prop)
    {
        if (!prop.IsPrimaryKey)
            return false;

        if (prop.IsString)
            return false;

        if (prop.IsIdentity || prop.IsGuid)
            return true;

        if (prop.IsInt || prop.IsLong)
            return true;

        return true;
    }

    /// <summary>
    /// Converte nome da aba para ID válido (sem espaços, acentos, etc.)
    /// </summary>
    private static string SanitizeTabId(string tabName)
    {
        if (string.IsNullOrEmpty(tabName))
            return "tab-default";

        // Remove acentos
        var normalized = tabName
            .ToLowerInvariant()
            .Replace("á", "a").Replace("à", "a").Replace("ã", "a").Replace("â", "a")
            .Replace("é", "e").Replace("ê", "e")
            .Replace("í", "i")
            .Replace("ó", "o").Replace("ô", "o").Replace("õ", "o")
            .Replace("ú", "u").Replace("ü", "u")
            .Replace("ç", "c");

        // Remove caracteres especiais, mantém apenas letras, números e hífen
        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            if (char.IsLetterOrDigit(c))
                sb.Append(c);
            else if (c == ' ' || c == '-' || c == '_')
                sb.Append('-');
        }

        var result = sb.ToString().Trim('-');

        // Garante que não está vazio
        return string.IsNullOrEmpty(result) ? "tab-default" : $"tab-{result}";
    }

    /// <summary>
    /// Escape de HTML para evitar XSS.
    /// </summary>
    private static string EscapeHtml(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }

    /// <summary>
    /// Converte nome do módulo para path de pasta.
    /// </summary>
    private static string GetModulePath(string moduleName)
    {
        if (string.IsNullOrEmpty(moduleName))
            return "Common";

        return moduleName;
    }

    #endregion
}