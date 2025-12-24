// =============================================================================
// RHSENSOERP CRUD TOOL - VIEW TEMPLATE
// Versão: 2.5 - CORREÇÕES: _CrudLayout, _CrudTable, sections, crudPermissions
// =============================================================================
using RhSensoERP.CrudTool.Models;
using System.Text;

namespace RhSensoERP.CrudTool.Templates;

/// <summary>
/// Gera View Razor compatível com CrudBase.js e BaseListViewModel.
/// 
/// CORREÇÕES v2.5:
/// - Layout = "_CrudLayout" (não "_Layout")
/// - Usa @await Html.PartialAsync("_CrudTable", Model)
/// - window.crudPermissions (não pagePermissions)
/// - Campos do modal em @section ModalContent
/// - Script em @section PageScripts
/// </summary>
public static class ViewTemplate
{
    /// <summary>
    /// Gera a View Index.cshtml seguindo o padrão do projeto.
    /// </summary>
    public static string GenerateIndex(EntityConfig entity)
    {
        var formFields = GenerateFormFields(entity);
        var iconClass = "fas fa-list";

        return $@"@model RhSensoERP.Web.Models.{entity.PluralName}.{entity.PluralName}ListViewModel
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
    <div class=""row"">
{formFields}
    </div>
}}

@* ============================================================================
   Script específico da entidade
   ============================================================================ *@
@section PageScripts {{
    <script src=""~/js/{entity.PluralNameLower}/{entity.NameLower}.js"" asp-append-version=""true""></script>
}}
";
    }

    #region Helper Methods

    /// <summary>
    /// Gera os campos do formulário para o modal.
    /// </summary>
    private static string GenerateFormFields(EntityConfig entity)
    {
        var sb = new StringBuilder();

        var formProps = entity.Properties
            .Where(p => p.Form?.Show == true && !p.IsPrimaryKey)
            .OrderBy(p => p.Form!.Order)
            .ToList();

        foreach (var prop in formProps)
        {
            var config = prop.Form!;
            var colSize = config.ColSize;
            var inputType = config.InputType;
            var placeholder = config.Placeholder ?? $"Digite {prop.DisplayName.ToLower()}...";
            var required = prop.Required ? "required" : "";
            var disabled = config.Disabled ? "disabled" : "";
            var maxLength = prop.MaxLength.HasValue ? $@" maxlength=""{prop.MaxLength.Value}""" : "";
            var step = prop.IsDecimal ? @" step=""0.01""" : "";

            var helpText = !string.IsNullOrEmpty(config.HelpText)
                ? $@"
            <small class=""form-text text-muted"">{config.HelpText}</small>"
                : "";

            string inputHtml;

            if (inputType == "textarea")
            {
                inputHtml = $@"<textarea class=""form-control"" id=""{prop.Name}"" name=""{prop.Name}"" 
                       rows=""{config.Rows}"" placeholder=""{placeholder}"" {required} {disabled}{maxLength}></textarea>";
            }
            else if (inputType == "select")
            {
                inputHtml = $@"<select class=""form-select"" id=""{prop.Name}"" name=""{prop.Name}"" {required} {disabled}>
                <option value="""">Selecione...</option>
                <!-- Preencher via JavaScript -->
            </select>";
            }
            else if (inputType == "checkbox")
            {
                sb.AppendLine($@"        <div class=""col-md-{colSize} mb-3"">
            <div class=""form-check"">
                <input type=""checkbox"" class=""form-check-input"" id=""{prop.Name}"" name=""{prop.Name}"" {disabled} />
                <label class=""form-check-label"" for=""{prop.Name}"">{prop.DisplayName}</label>
            </div>{helpText}
        </div>");
                continue;
            }
            else
            {
                inputHtml = $@"<input type=""{inputType}"" class=""form-control"" id=""{prop.Name}"" name=""{prop.Name}"" 
                   placeholder=""{placeholder}"" {required} {disabled}{maxLength}{step} />";
            }

            var requiredStar = prop.Required ? @" <span class=""text-danger"">*</span>" : "";

            sb.AppendLine($@"        <div class=""col-md-{colSize} mb-3"">
            <label for=""{prop.Name}"" class=""form-label"">
                {prop.DisplayName}{requiredStar}
            </label>
            {inputHtml}{helpText}
        </div>");
        }

        return sb.ToString().TrimEnd('\r', '\n');
    }

    #endregion
}