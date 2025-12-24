// =============================================================================
// TABSHEET GENERATOR - TEMPLATE DE VIEWS
// Versão: 1.0.0
// Autor: RhSensoERP Team
// Data: 2024
// 
// Gera Views Razor com tabs (AdminLTE + Bootstrap 5).
// =============================================================================

using System.Text;
using GeradorEntidades.Models;
using GeradorEntidades.TabSheet.Models;

namespace GeradorEntidades.TabSheet.Templates;

/// <summary>
/// Template para geração de Views com TabSheets.
/// </summary>
public static class TabSheetViewTemplate
{
    #region View Principal (Mestre)

    /// <summary>
    /// Gera a View principal com formulário do mestre e tabs para os detalhes.
    /// </summary>
    public static GeneratedTabSheetFile GenerateMasterView(
        TabSheetConfiguration config,
        TabelaInfo tabela)
    {
        var sb = new StringBuilder();
        var entityName = config.MasterTable.EntityName;
        var entityNameLower = config.MasterTable.EntityNameCamel;
        var options = config.GenerationOptions;

        // Diretiva @model
        sb.AppendLine($"@model {entityName}Dto");
        sb.AppendLine("@{");
        sb.AppendLine($"    ViewData[\"Title\"] = \"{config.Title}\";");
        sb.AppendLine($"    var isEdit = Model?.{config.MasterTable.PrimaryKey} != null && Model.{config.MasterTable.PrimaryKey} != default;");
        sb.AppendLine("}");
        sb.AppendLine();

        // CSS específico
        sb.AppendLine("@section Styles {");
        sb.AppendLine("<style>");
        sb.AppendLine("    .nav-tabs .nav-link { color: #6c757d; }");
        sb.AppendLine("    .nav-tabs .nav-link.active { color: #007bff; font-weight: 600; }");
        sb.AppendLine("    .nav-tabs .nav-link .badge { font-size: 0.75em; }");
        sb.AppendLine("    .tab-content { min-height: 300px; }");
        sb.AppendLine("    .tab-pane { padding: 1rem 0; }");
        sb.AppendLine("</style>");
        sb.AppendLine("}");
        sb.AppendLine();

        // Breadcrumb
        sb.AppendLine("<section class=\"content-header\">");
        sb.AppendLine("    <div class=\"container-fluid\">");
        sb.AppendLine("        <div class=\"row mb-2\">");
        sb.AppendLine("            <div class=\"col-sm-6\">");
        sb.AppendLine($"                <h1><i class=\"{config.MasterTable.Icon} mr-2\"></i>@ViewData[\"Title\"]</h1>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class=\"col-sm-6\">");
        sb.AppendLine("                <ol class=\"breadcrumb float-sm-right\">");
        sb.AppendLine($"                    <li class=\"breadcrumb-item\"><a href=\"/{options.ModuleRoute}\">Home</a></li>");
        sb.AppendLine($"                    <li class=\"breadcrumb-item\"><a href=\"/{options.ModuleRoute}/{config.MasterTable.PluralName.ToLower()}\">@ViewData[\"Title\"]</a></li>");
        sb.AppendLine("                    <li class=\"breadcrumb-item active\">@(isEdit ? \"Editar\" : \"Novo\")</li>");
        sb.AppendLine("                </ol>");
        sb.AppendLine("            </div>");
        sb.AppendLine("        </div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("</section>");
        sb.AppendLine();

        // Conteúdo principal
        sb.AppendLine("<section class=\"content\">");
        sb.AppendLine("    <div class=\"container-fluid\">");
        sb.AppendLine();

        // Card do formulário mestre
        sb.AppendLine("        <!-- Card do Mestre -->");
        sb.AppendLine("        <div class=\"card card-primary card-outline\">");
        sb.AppendLine("            <div class=\"card-header\">");
        sb.AppendLine($"                <h3 class=\"card-title\"><i class=\"{config.MasterTable.Icon} mr-2\"></i>Dados do {config.MasterTable.DisplayName}</h3>");
        sb.AppendLine("                <div class=\"card-tools\">");
        sb.AppendLine("                    <button type=\"button\" class=\"btn btn-tool\" data-card-widget=\"collapse\">");
        sb.AppendLine("                        <i class=\"fas fa-minus\"></i>");
        sb.AppendLine("                    </button>");
        sb.AppendLine("                </div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class=\"card-body\">");
        sb.AppendLine($"                <form id=\"form{entityName}\" class=\"needs-validation\" novalidate>");
        sb.AppendLine($"                    <input type=\"hidden\" id=\"{entityNameLower}Id\" name=\"{config.MasterTable.PrimaryKey}\" value=\"@Model?.{config.MasterTable.PrimaryKey}\" />");
        sb.AppendLine();

        // Gerar campos do formulário (simplificado)
        sb.AppendLine("                    <div class=\"row\">");
        GenerateFormFields(sb, tabela, config.MasterTable.PrimaryKey);
        sb.AppendLine("                    </div>");
        sb.AppendLine();

        sb.AppendLine("                    <div class=\"row mt-3\">");
        sb.AppendLine("                        <div class=\"col-12 text-right\">");
        sb.AppendLine($"                            <a href=\"/{options.ModuleRoute}/{config.MasterTable.PluralName.ToLower()}\" class=\"btn btn-secondary\">");
        sb.AppendLine("                                <i class=\"fas fa-arrow-left mr-1\"></i>Voltar");
        sb.AppendLine("                            </a>");
        sb.AppendLine($"                            <button type=\"submit\" class=\"btn btn-primary\" id=\"btnSalvar{entityName}\">");
        sb.AppendLine("                                <i class=\"fas fa-save mr-1\"></i>Salvar");
        sb.AppendLine("                            </button>");
        sb.AppendLine("                        </div>");
        sb.AppendLine("                    </div>");
        sb.AppendLine("                </form>");
        sb.AppendLine("            </div>");
        sb.AppendLine("        </div>");
        sb.AppendLine();

        // Seção de Tabs (só aparece em edição)
        sb.AppendLine("        <!-- Tabs de Detalhes (só aparece em edição) -->");
        sb.AppendLine("        @if (isEdit)");
        sb.AppendLine("        {");
        sb.AppendLine("            <div class=\"card card-outline card-primary\">");
        sb.AppendLine("                <div class=\"card-header p-0 pt-1\">");
        sb.AppendLine("                    <ul class=\"nav nav-tabs\" id=\"tabsDetalhes\" role=\"tablist\">");

        // Gerar botões das tabs
        for (int i = 0; i < config.Tabs.Count; i++)
        {
            var tab = config.Tabs[i];
            var isFirst = i == 0;
            var activeClass = isFirst ? " active" : "";

            sb.AppendLine($"                        <li class=\"nav-item\">");
            sb.AppendLine($"                            <a class=\"nav-link{activeClass}\" id=\"{tab.TabId}-tab\" data-toggle=\"pill\" href=\"#{tab.TabId}\" role=\"tab\">");
            sb.AppendLine($"                                <i class=\"{tab.Icon} mr-1\"></i>{tab.Title}");
            if (tab.ShowBadge)
            {
                sb.AppendLine($"                                <span class=\"badge badge-primary ml-1\" id=\"badge-{tab.TabId}\">0</span>");
            }
            sb.AppendLine("                            </a>");
            sb.AppendLine("                        </li>");
        }

        sb.AppendLine("                    </ul>");
        sb.AppendLine("                </div>");
        sb.AppendLine("                <div class=\"card-body\">");
        sb.AppendLine("                    <div class=\"tab-content\" id=\"tabsDetalhesContent\">");

        // Gerar conteúdo das tabs
        for (int i = 0; i < config.Tabs.Count; i++)
        {
            var tab = config.Tabs[i];
            var isFirst = i == 0;
            var activeClass = isFirst ? " show active" : "";

            sb.AppendLine($"                        <div class=\"tab-pane fade{activeClass}\" id=\"{tab.TabId}\" role=\"tabpanel\">");
            sb.AppendLine($"                            <div id=\"container-{tab.TabId}\">");
            sb.AppendLine("                                <div class=\"text-center py-5\">");
            sb.AppendLine("                                    <i class=\"fas fa-spinner fa-spin fa-2x\"></i>");
            sb.AppendLine("                                    <p class=\"mt-2\">Carregando...</p>");
            sb.AppendLine("                                </div>");
            sb.AppendLine("                            </div>");
            sb.AppendLine("                        </div>");
        }

        sb.AppendLine("                    </div>");
        sb.AppendLine("                </div>");
        sb.AppendLine("            </div>");
        sb.AppendLine("        }");
        sb.AppendLine();

        sb.AppendLine("    </div>");
        sb.AppendLine("</section>");
        sb.AppendLine();

        // Scripts
        sb.AppendLine("@section Scripts {");
        sb.AppendLine($"    <script src=\"~/{options.ModuleRoute}/js/{entityNameLower}-tabsheet.js\"></script>");
        sb.AppendLine("    <script>");
        sb.AppendLine("        $(document).ready(function() {");
        sb.AppendLine($"            TabSheet{entityName}.init({{");
        sb.AppendLine($"                masterId: '@Model?.{config.MasterTable.PrimaryKey}',");
        sb.AppendLine($"                isEdit: @(isEdit ? \"true\" : \"false\"),");
        sb.AppendLine($"                apiBaseUrl: '/{options.ModuleRoute}/api/{config.MasterTable.PluralName.ToLower()}'");
        sb.AppendLine("            });");
        sb.AppendLine("        });");
        sb.AppendLine("    </script>");
        sb.AppendLine("}");

        return new GeneratedTabSheetFile
        {
            FileName = "Edit.cshtml",
            RelativePath = $"Web/Views/{config.MasterTable.PluralName}/",
            Content = sb.ToString(),
            FileType = TabSheetFileType.View,
            EntityName = entityName,
            IsMasterFile = true
        };
    }

    #endregion

    #region Partial Views (Detalhes)

    /// <summary>
    /// Gera Partial View para uma aba (tabela de detalhe).
    /// </summary>
    public static GeneratedTabSheetFile GenerateTabPartial(
        TabSheetConfiguration config,
        TabDefinition tab,
        TabelaInfo tabela)
    {
        var sb = new StringBuilder();
        var entityName = tab.EntityName;
        var entityNameLower = entityName.ToLower();
        var masterPk = config.MasterTable.PrimaryKey;

        sb.AppendLine($"@* Partial View: {tab.Title} *@");
        sb.AppendLine($"@* Tabela: {tab.TableName} *@");
        sb.AppendLine();

        // Toolbar
        sb.AppendLine("<div class=\"row mb-3\">");
        sb.AppendLine("    <div class=\"col-md-6\">");
        sb.AppendLine($"        <h5><i class=\"{tab.Icon} mr-2\"></i>{tab.Title}</h5>");
        sb.AppendLine("    </div>");
        sb.AppendLine("    <div class=\"col-md-6 text-right\">");

        if (tab.AllowCreate)
        {
            sb.AppendLine($"        <button type=\"button\" class=\"btn btn-sm btn-success\" id=\"btnNovo{entityName}\">");
            sb.AppendLine("            <i class=\"fas fa-plus mr-1\"></i>Novo");
            sb.AppendLine("        </button>");
        }

        sb.AppendLine("        <button type=\"button\" class=\"btn btn-sm btn-secondary\" id=\"btnRecarregar{entityName}\">");
        sb.AppendLine("            <i class=\"fas fa-sync-alt mr-1\"></i>Atualizar");
        sb.AppendLine("        </button>");
        sb.AppendLine("    </div>");
        sb.AppendLine("</div>");
        sb.AppendLine();

        // DataTable
        sb.AppendLine("<div class=\"table-responsive\">");
        sb.AppendLine($"    <table id=\"table{entityName}\" class=\"table table-bordered table-striped table-sm\" style=\"width:100%\">");
        sb.AppendLine("        <thead class=\"thead-light\">");
        sb.AppendLine("            <tr>");

        // Colunas da tabela
        var columns = tabela.Colunas
            .Where(c => !c.IsPrimaryKey &&
                       !c.Nome.Equals(tab.ForeignKey, StringComparison.OrdinalIgnoreCase) &&
                       !c.IsBinary)
            .Take(6) // Limitar a 6 colunas visíveis
            .ToList();

        foreach (var col in columns)
        {
            var displayName = col.Descricao ?? col.NomePascalCase;
            sb.AppendLine($"                <th>{displayName}</th>");
        }

        sb.AppendLine("                <th style=\"width: 100px\">Ações</th>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("        </thead>");
        sb.AppendLine("        <tbody>");
        sb.AppendLine("        </tbody>");
        sb.AppendLine("    </table>");
        sb.AppendLine("</div>");
        sb.AppendLine();

        // Modal de Edição
        sb.AppendLine($"<!-- Modal de Edição: {tab.Title} -->");
        sb.AppendLine($"<div class=\"modal fade\" id=\"modal{entityName}\" tabindex=\"-1\" role=\"dialog\">");
        sb.AppendLine("    <div class=\"modal-dialog modal-lg\" role=\"document\">");
        sb.AppendLine("        <div class=\"modal-content\">");
        sb.AppendLine("            <div class=\"modal-header bg-primary\">");
        sb.AppendLine($"                <h5 class=\"modal-title\"><i class=\"{tab.Icon} mr-2\"></i><span id=\"modalTitle{entityName}\">Novo</span></h5>");
        sb.AppendLine("                <button type=\"button\" class=\"close text-white\" data-dismiss=\"modal\">");
        sb.AppendLine("                    <span>&times;</span>");
        sb.AppendLine("                </button>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class=\"modal-body\">");
        sb.AppendLine($"                <form id=\"form{entityName}\" class=\"needs-validation\" novalidate>");
        sb.AppendLine($"                    <input type=\"hidden\" id=\"{entityNameLower}_{tab.PrimaryKey ?? "id"}\" name=\"{tab.PrimaryKey ?? "Id"}\" />");
        sb.AppendLine($"                    <input type=\"hidden\" id=\"{entityNameLower}_{tab.ForeignKey}\" name=\"{tab.ForeignKey}\" />");
        sb.AppendLine();
        sb.AppendLine("                    <div class=\"row\">");

        // Campos do formulário do detalhe
        GenerateFormFields(sb, tabela, tab.PrimaryKey ?? "id", tab.ForeignKey);

        sb.AppendLine("                    </div>");
        sb.AppendLine("                </form>");
        sb.AppendLine("            </div>");
        sb.AppendLine("            <div class=\"modal-footer\">");
        sb.AppendLine("                <button type=\"button\" class=\"btn btn-secondary\" data-dismiss=\"modal\">");
        sb.AppendLine("                    <i class=\"fas fa-times mr-1\"></i>Cancelar");
        sb.AppendLine("                </button>");
        sb.AppendLine($"                <button type=\"button\" class=\"btn btn-primary\" id=\"btnSalvar{entityName}\">");
        sb.AppendLine("                    <i class=\"fas fa-save mr-1\"></i>Salvar");
        sb.AppendLine("                </button>");
        sb.AppendLine("            </div>");
        sb.AppendLine("        </div>");
        sb.AppendLine("    </div>");
        sb.AppendLine("</div>");

        return new GeneratedTabSheetFile
        {
            FileName = $"_{entityName}Tab.cshtml",
            RelativePath = $"Web/Views/{config.MasterTable.PluralName}/Partials/",
            Content = sb.ToString(),
            FileType = TabSheetFileType.PartialView,
            EntityName = entityName,
            IsMasterFile = false
        };
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Gera campos de formulário para as colunas da tabela.
    /// </summary>
    private static void GenerateFormFields(StringBuilder sb, TabelaInfo tabela, string pkName, string? fkName = null)
    {
        var columns = tabela.Colunas
            .Where(c => !c.IsPrimaryKey &&
                       !c.Nome.Equals(pkName, StringComparison.OrdinalIgnoreCase) &&
                       (fkName == null || !c.Nome.Equals(fkName, StringComparison.OrdinalIgnoreCase)) &&
                       !c.IsBinary)
            .ToList();

        foreach (var col in columns)
        {
            var fieldId = col.Nome.ToLower();
            var fieldName = col.NomePascalCase;
            var label = col.Descricao ?? col.NomePascalCase;
            var required = !col.IsNullable ? "required" : "";
            var colSize = col.IsTexto && col.Tamanho > 100 ? "12" : "6";

            sb.AppendLine($"                        <div class=\"col-md-{colSize}\">");
            sb.AppendLine("                            <div class=\"form-group\">");
            sb.AppendLine($"                                <label for=\"{fieldId}\">{label}</label>");

            if (col.IsTexto && col.Tamanho > 200)
            {
                // Textarea
                sb.AppendLine($"                                <textarea class=\"form-control\" id=\"{fieldId}\" name=\"{fieldName}\" rows=\"3\" maxlength=\"{col.Tamanho}\" {required}></textarea>");
            }
            else if (col.IsNumerico)
            {
                sb.AppendLine($"                                <input type=\"number\" class=\"form-control\" id=\"{fieldId}\" name=\"{fieldName}\" {required} />");
            }
            else if (col.IsData)
            {
                var inputType = col.TipoCSharp.Contains("DateTime") ? "datetime-local" : "date";
                sb.AppendLine($"                                <input type=\"{inputType}\" class=\"form-control\" id=\"{fieldId}\" name=\"{fieldName}\" {required} />");
            }
            else if (col.IsBool)
            {
                sb.AppendLine("                                <div class=\"custom-control custom-switch\">");
                sb.AppendLine($"                                    <input type=\"checkbox\" class=\"custom-control-input\" id=\"{fieldId}\" name=\"{fieldName}\" />");
                sb.AppendLine($"                                    <label class=\"custom-control-label\" for=\"{fieldId}\">Sim</label>");
                sb.AppendLine("                                </div>");
            }
            else
            {
                var maxLength = col.Tamanho > 0 ? $"maxlength=\"{col.Tamanho}\"" : "";
                sb.AppendLine($"                                <input type=\"text\" class=\"form-control\" id=\"{fieldId}\" name=\"{fieldName}\" {maxLength} {required} />");
            }

            sb.AppendLine("                            </div>");
            sb.AppendLine("                        </div>");
        }
    }

    #endregion
}
