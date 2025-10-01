using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace RhSensoWeb.TagHelpers;

/// <summary>
/// TagHelper para criar DataTables automaticamente
/// </summary>
[HtmlTargetElement("datatable")]
public class DataTableTagHelper : TagHelper
{
    /// <summary>
    /// ID da tabela
    /// </summary>
    [HtmlAttributeName("id")]
    public string Id { get; set; } = "dataTable";

    /// <summary>
    /// URL para carregar dados via AJAX
    /// </summary>
    [HtmlAttributeName("ajax-url")]
    public string? AjaxUrl { get; set; }

    /// <summary>
    /// Colunas da tabela (JSON)
    /// </summary>
    [HtmlAttributeName("columns")]
    public string? Columns { get; set; }

    /// <summary>
    /// Configurações adicionais (JSON)
    /// </summary>
    [HtmlAttributeName("options")]
    public string? Options { get; set; }

    /// <summary>
    /// Mostrar botões de exportação
    /// </summary>
    [HtmlAttributeName("show-export")]
    public bool ShowExport { get; set; } = true;

    /// <summary>
    /// Mostrar filtros
    /// </summary>
    [HtmlAttributeName("show-filters")]
    public bool ShowFilters { get; set; } = false;

    /// <summary>
    /// CSS classes adicionais
    /// </summary>
    [HtmlAttributeName("css-class")]
    public string CssClass { get; set; } = "table table-bordered table-striped table-hover";

    /// <summary>
    /// Título da tabela
    /// </summary>
    [HtmlAttributeName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Processa o TagHelper
    /// </summary>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.Attributes.SetAttribute("class", "card");

        var content = new StringBuilder();

        // Header do card com título
        if (!string.IsNullOrEmpty(Title))
        {
            content.AppendLine("<div class=\"card-header\">");
            content.AppendLine($"<h3 class=\"card-title\">{Title}</h3>");
            
            if (ShowExport)
            {
                content.AppendLine("<div class=\"card-tools\">");
                content.AppendLine($"<div class=\"btn-group\" id=\"{Id}_buttons\"></div>");
                content.AppendLine("</div>");
            }
            
            content.AppendLine("</div>");
        }

        // Filtros (se habilitado)
        if (ShowFilters)
        {
            content.AppendLine("<div class=\"card-header\">");
            content.AppendLine($"<div id=\"{Id}_filters\" class=\"row\">");
            content.AppendLine("<!-- Filtros serão adicionados via JavaScript -->");
            content.AppendLine("</div>");
            content.AppendLine("</div>");
        }

        // Body do card com tabela
        content.AppendLine("<div class=\"card-body\">");
        content.AppendLine("<div class=\"table-responsive\">");
        content.AppendLine($"<table id=\"{Id}\" class=\"{CssClass}\">");
        content.AppendLine("<thead></thead>");
        content.AppendLine("<tbody></tbody>");
        content.AppendLine("</table>");
        content.AppendLine("</div>");
        content.AppendLine("</div>");

        // Script de inicialização
        content.AppendLine("<script>");
        content.AppendLine("$(document).ready(function() {");
        
        // Configuração base
        content.AppendLine($"var {Id}_config = {{");
        content.AppendLine("processing: true,");
        content.AppendLine("serverSide: true,");
        content.AppendLine("responsive: true,");
        content.AppendLine("language: { url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/pt-BR.json' },");
        content.AppendLine("pageLength: 25,");
        content.AppendLine("lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, 'Todos']],");
        
        // AJAX
        if (!string.IsNullOrEmpty(AjaxUrl))
        {
            content.AppendLine("ajax: {");
            content.AppendLine($"url: '{AjaxUrl}',");
            content.AppendLine("type: 'POST',");
            content.AppendLine("contentType: 'application/json',");
            content.AppendLine("data: function(d) {");
            content.AppendLine("return JSON.stringify(d);");
            content.AppendLine("},");
            content.AppendLine("error: function(xhr, error, thrown) {");
            content.AppendLine("console.error('Erro no DataTable:', error);");
            content.AppendLine("toastr.error('Erro ao carregar dados da tabela');");
            content.AppendLine("}");
            content.AppendLine("},");
        }

        // Colunas
        if (!string.IsNullOrEmpty(Columns))
        {
            content.AppendLine($"columns: {Columns},");
        }

        // Botões de exportação
        if (ShowExport)
        {
            content.AppendLine("dom: 'Bfrtip',");
            content.AppendLine("buttons: [");
            content.AppendLine("{ extend: 'excel', text: '<i class=\"fas fa-file-excel\"></i> Excel', className: 'btn btn-success btn-sm' },");
            content.AppendLine("{ extend: 'pdf', text: '<i class=\"fas fa-file-pdf\"></i> PDF', className: 'btn btn-danger btn-sm' },");
            content.AppendLine("{ extend: 'print', text: '<i class=\"fas fa-print\"></i> Imprimir', className: 'btn btn-info btn-sm' }");
            content.AppendLine("],");
        }

        // Opções adicionais
        if (!string.IsNullOrEmpty(Options))
        {
            // Remove as chaves do JSON para mesclar com a configuração
            var optionsClean = Options.Trim();
            if (optionsClean.StartsWith("{") && optionsClean.EndsWith("}"))
            {
                optionsClean = optionsClean.Substring(1, optionsClean.Length - 2);
            }
            content.AppendLine($"{optionsClean}");
        }

        content.AppendLine("};");

        // Inicializar DataTable
        content.AppendLine($"var {Id}_table = $('#{Id}').DataTable({Id}_config);");

        // Mover botões para o header
        if (ShowExport && !string.IsNullOrEmpty(Title))
        {
            content.AppendLine($"{Id}_table.buttons().container().appendTo('#{Id}_buttons');");
        }

        content.AppendLine("});");
        content.AppendLine("</script>");

        output.Content.SetHtmlContent(content.ToString());
    }
}

/// <summary>
/// TagHelper para botões de ação em DataTables
/// </summary>
[HtmlTargetElement("datatable-actions")]
public class DataTableActionsTagHelper : TagHelper
{
    /// <summary>
    /// Permissão base para as ações
    /// </summary>
    [HtmlAttributeName("permission-base")]
    public string? PermissionBase { get; set; }

    /// <summary>
    /// Mostrar botão de visualizar
    /// </summary>
    [HtmlAttributeName("show-view")]
    public bool ShowView { get; set; } = true;

    /// <summary>
    /// Mostrar botão de editar
    /// </summary>
    [HtmlAttributeName("show-edit")]
    public bool ShowEdit { get; set; } = true;

    /// <summary>
    /// Mostrar botão de excluir
    /// </summary>
    [HtmlAttributeName("show-delete")]
    public bool ShowDelete { get; set; } = true;

    /// <summary>
    /// Controller para as ações
    /// </summary>
    [HtmlAttributeName("controller")]
    public string? Controller { get; set; }

    /// <summary>
    /// Área para as ações
    /// </summary>
    [HtmlAttributeName("area")]
    public string? Area { get; set; }

    /// <summary>
    /// Campo ID do registro
    /// </summary>
    [HtmlAttributeName("id-field")]
    public string IdField { get; set; } = "id";

    /// <summary>
    /// Processa o TagHelper
    /// </summary>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;

        var content = new StringBuilder();
        content.AppendLine("function(data, type, row, meta) {");
        content.AppendLine("if (type === 'display') {");
        content.AppendLine("var actions = '<div class=\"btn-group\" role=\"group\">';");

        // Botão Visualizar
        if (ShowView)
        {
            var viewPermission = !string.IsNullOrEmpty(PermissionBase) ? $"{PermissionBase}.C" : "";
            content.AppendLine($"// Visualizar");
            
            if (!string.IsNullOrEmpty(viewPermission))
            {
                content.AppendLine($"if (hasPermission('{viewPermission}')) {{");
            }
            
            var viewUrl = BuildActionUrl("Details");
            content.AppendLine($"actions += '<a href=\"{viewUrl}' + row.{IdField} + '\" class=\"btn btn-info btn-sm\" title=\"Visualizar\">';");
            content.AppendLine("actions += '<i class=\"fas fa-eye\"></i>';");
            content.AppendLine("actions += '</a>';");
            
            if (!string.IsNullOrEmpty(viewPermission))
            {
                content.AppendLine("}");
            }
        }

        // Botão Editar
        if (ShowEdit)
        {
            var editPermission = !string.IsNullOrEmpty(PermissionBase) ? $"{PermissionBase}.A" : "";
            content.AppendLine($"// Editar");
            
            if (!string.IsNullOrEmpty(editPermission))
            {
                content.AppendLine($"if (hasPermission('{editPermission}')) {{");
            }
            
            var editUrl = BuildActionUrl("Edit");
            content.AppendLine($"actions += '<a href=\"{editUrl}' + row.{IdField} + '\" class=\"btn btn-warning btn-sm\" title=\"Editar\">';");
            content.AppendLine("actions += '<i class=\"fas fa-edit\"></i>';");
            content.AppendLine("actions += '</a>';");
            
            if (!string.IsNullOrEmpty(editPermission))
            {
                content.AppendLine("}");
            }
        }

        // Botão Excluir
        if (ShowDelete)
        {
            var deletePermission = !string.IsNullOrEmpty(PermissionBase) ? $"{PermissionBase}.E" : "";
            content.AppendLine($"// Excluir");
            
            if (!string.IsNullOrEmpty(deletePermission))
            {
                content.AppendLine($"if (hasPermission('{deletePermission}')) {{");
            }
            
            content.AppendLine("actions += '<button type=\"button\" class=\"btn btn-danger btn-sm\" onclick=\"deleteRecord(\\'' + row." + IdField + " + '\\')\" title=\"Excluir\">';");
            content.AppendLine("actions += '<i class=\"fas fa-trash\"></i>';");
            content.AppendLine("actions += '</button>';");
            
            if (!string.IsNullOrEmpty(deletePermission))
            {
                content.AppendLine("}");
            }
        }

        content.AppendLine("actions += '</div>';");
        content.AppendLine("return actions;");
        content.AppendLine("}");
        content.AppendLine("return data;");
        content.AppendLine("}");

        output.Content.SetHtmlContent(content.ToString());
    }

    /// <summary>
    /// Constrói URL para ação
    /// </summary>
    private string BuildActionUrl(string action)
    {
        var url = "/";
        
        if (!string.IsNullOrEmpty(Area))
        {
            url += $"{Area}/";
        }
        
        if (!string.IsNullOrEmpty(Controller))
        {
            url += $"{Controller}/";
        }
        
        url += $"{action}/";
        
        return url;
    }
}
