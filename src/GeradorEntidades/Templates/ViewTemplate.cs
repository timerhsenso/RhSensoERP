// =============================================================================
// GERADOR FULL-STACK v4.2 - VIEW TEMPLATE (SELECT2 AUTO-DETECÇÃO COMPLETA)
// Baseado em RhSensoERP.CrudTool v2.5
// ⭐ v4.2 - AUTO-DETECÇÃO: Detecta automaticamente campos de FK e aplica Select2
// ⭐ v4.1 - SELECT2 LOOKUP: Gera campos select com data-select2-url automático
// v3.3 - Suporte a Bootstrap Tabs no formulário
// =============================================================================

using GeradorEntidades.Models;
using System.Text;

namespace GeradorEntidades.Templates;

/// <summary>
/// Gera View Razor compatível com CrudBase.js e BaseListViewModel.
/// v4.2: AUTO-DETECÇÃO de campos de FK - aplica Select2 automaticamente
/// v4.1: CORRIGIDO - Atributos Select2 agora são gerados corretamente
/// v3.4: Adiciona checkbox "Selecionar Todos" e renderização de Toggle Switch para campo ATIVO.
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

        // =====================================================================
        // v3.4: Verifica se tem campo "Ativo" para configurar JavaScript
        // =====================================================================
        var hasAtivoField = entity.Properties.Any(p =>
            p.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("IsAtivo", StringComparison.OrdinalIgnoreCase) ||
            p.Name.Equals("Ativo", StringComparison.OrdinalIgnoreCase));

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
    
    @* v3.4: Configuração para Toggle Ativo *@
    window.crudConfig = {{
        hasAtivoField: {hasAtivoField.ToString().ToLower()},
        entityName: ""{entity.Name}"",
        pkField: ""{entity.PrimaryKey?.Name ?? "Id"}""
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
    /// v4.2: AUTO-DETECÇÃO de campos de FK - aplica Select2 automaticamente
    /// v4.1: CORRIGIDO - Atributos Select2 agora são gerados corretamente
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
                        <input type=""checkbox"" class=""form-check-input"" id=""{prop.Name}"" name=""{prop.Name}"" {disabled} />
                        <label class=""form-check-label"" for=""{prop.Name}"">
                            {EscapeHtml(prop.DisplayName)}{requiredStar}{pkBadge}
                        </label>
                    </div>{helpText}
                </div>";
        }

        // =====================================================================
        // TEXTAREA
        // =====================================================================
        if (inputType == "textarea")
        {
            var rows = config.Rows > 0 ? config.Rows : 3;
            return $@"                <div class=""col-md-{colSize} mb-3"">
                    <label for=""{prop.Name}"" class=""form-label"">
                        {EscapeHtml(prop.DisplayName)}{requiredStar}{pkBadge}
                    </label>
                    <textarea class=""form-control"" id=""{prop.Name}"" name=""{prop.Name}"" 
                              rows=""{rows}"" placeholder=""{EscapeHtml(placeholder)}"" {required} {disabled}{maxLength}></textarea>{helpText}
                </div>";
        }

        // =====================================================================
        // SELECT (Combobox) - v4.2 COM AUTO-DETECÇÃO DE FK
        // =====================================================================
        if (inputType == "select")
        {
            // ⭐ v4.2: AUTO-DETECÇÃO de Select2 AJAX
            // Detecta automaticamente se deve usar Select2 AJAX quando:
            // 1. SelectEndpoint está preenchido OU
            // 2. SelectApiRoute está preenchido OU
            // 3. IsSelect2Ajax = true OU
            // 4. ⭐ v4.2 NOVO: Nome do campo indica FK (Id*, Fk*, Cd* + sufixo)
            var isFkField = IsForeignKeyField(prop.Name);
            var isSelect2Ajax = !string.IsNullOrEmpty(config.SelectEndpoint) ||
                                !string.IsNullOrEmpty(config.SelectApiRoute) ||
                                config.IsSelect2Ajax ||
                                isFkField;  // ⭐ v4.2 NOVO

            // ⭐ v4.2 CORRIGIDO: Classe CSS agora é adicionada corretamente
            var select2Class = isSelect2Ajax ? "select2-ajax" : "form-select";

            // ⭐ v4.2: URL do Select2
            // Prioridade: 1) SelectApiRoute, 2) SelectEndpoint, 3) Gerar automaticamente
            var select2Url = !string.IsNullOrEmpty(config.SelectApiRoute)
                ? config.SelectApiRoute
                : !string.IsNullOrEmpty(config.SelectEndpoint)
                    ? config.SelectEndpoint
                    : GenerateSelect2Url(prop, entity);  // ⭐ v4.2 NOVO

            // ⭐ v4.2 CORRIGIDO: Atributos Select2 agora são adicionados corretamente
            var select2Attrs = isSelect2Ajax
                ? $@" data-select2-url=""{select2Url}"" data-value-field=""{config.SelectValueField ?? "id"}"" data-text-field=""{config.SelectTextField ?? "nome"}"" style=""width: 100%"""
                : "";

            return $@"                <div class=""col-md-{colSize} mb-3"">
                    <label for=""{prop.Name}"" class=""form-label"">
                        {EscapeHtml(prop.DisplayName)}{requiredStar}{pkBadge}
                    </label>
                    <select class=""form-control {select2Class}"" id=""{prop.Name}"" name=""{prop.Name}"" {required} {disabled}{pkTextAttr}{select2Attrs}>
                        <option value="""">Selecione...</option>
                        <!-- Preencher via JavaScript -->
                    </select>{helpText}
                </div>";
        }

        // =====================================================================
        // INPUT padrão (text, email, number, date, etc)
        // =====================================================================
        var inputTypeAttr = inputType switch
        {
            "email" => "email",
            "number" => "number",
            "date" => "date",
            "time" => "time",
            "datetime" => "datetime-local",
            "decimal" => "number",
            _ => "text"
        };

        return $@"                <div class=""col-md-{colSize} mb-3"">
                    <label for=""{prop.Name}"" class=""form-label"">
                        {EscapeHtml(prop.DisplayName)}{requiredStar}{pkBadge}
                    </label>
                    <input type=""{inputTypeAttr}"" class=""form-control"" id=""{prop.Name}"" name=""{prop.Name}"" 
                           placeholder=""{EscapeHtml(placeholder)}"" {required} {disabled}{maxLength}{step} />
                    {helpText}
                </div>";
    }

    /// <summary>
    /// ⭐ v4.2 NOVO: Detecta se um campo é uma chave estrangeira (FK).
    /// Padrões: IdXxx, FkXxx, CdXxx (onde Xxx é o nome da entidade)
    /// </summary>
    private static bool IsForeignKeyField(string fieldName)
    {
        // Padrões de FK
        var fkPatterns = new[] { "Id", "Fk", "Cd" };

        foreach (var pattern in fkPatterns)
        {
            if (fieldName.StartsWith(pattern, StringComparison.OrdinalIgnoreCase))
            {
                // Se tem sufixo após o padrão, é provavelmente FK
                var suffix = fieldName.Substring(pattern.Length);
                if (!string.IsNullOrEmpty(suffix) && char.IsUpper(suffix[0]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// ⭐ v4.2 NOVO: Gera URL do Select2 automaticamente baseado no nome do campo.
    /// Ex: IdFornecedor → /ControleAcessoPortaria/capcolaboradoresfornecedor/GetFornecedorForSelect
    /// </summary>
    private static string GenerateSelect2Url(PropertyConfig prop, EntityConfig entity)
    {
        var entityName = GetEntityNameFromProperty(prop.Name, entity);
        var moduleRoute = entity.ModuleRoute ?? entity.Module?.ToLowerInvariant() ?? "common";
        var entityNameLower = entity.NameLower;

        // URL padrão: /{ModuleRoute}/{EntityNameLower}/Get{RelatedEntityName}ForSelect
        return $"/{moduleRoute}/{entityNameLower}/Get{entityName}ForSelect";
    }

    /// <summary>
    /// Extrai o nome da entidade relacionada a partir do nome do campo.
    /// Ex: IdFornecedor → Fornecedor, CdFilial → Filial, FkCliente → Cliente
    /// </summary>
    private static string GetEntityNameFromProperty(string propertyName, EntityConfig entity)
    {
        // Remove prefixo (Id, Fk, Cd)
        var name = propertyName;

        if (name.StartsWith("Id", StringComparison.OrdinalIgnoreCase))
            name = name.Substring(2);
        else if (name.StartsWith("Fk", StringComparison.OrdinalIgnoreCase))
            name = name.Substring(2);
        else if (name.StartsWith("Cd", StringComparison.OrdinalIgnoreCase))
            name = name.Substring(2);

        // Se ficou vazio, usa o nome da entidade
        if (string.IsNullOrEmpty(name))
            return entity.Name;

        return name;
    }



    /// <summary>
    /// Sanitiza ID de tab para uso em HTML.
    /// </summary>
    private static string SanitizeTabId(string tabName)
    {
        return System.Text.RegularExpressions.Regex.Replace(
            tabName.ToLowerInvariant(),
            @"[^a-z0-9]+",
            "-"
        ).Trim('-');
    }

    /// <summary>
    /// Escapa HTML para segurança.
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
    /// Verifica se é uma PK auto-gerada.
    /// </summary>
    private static bool IsAutoGeneratedPrimaryKey(PropertyConfig prop)
    {
        return prop.IsPrimaryKey && (prop.IsIdentity || prop.IsGuid);
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
