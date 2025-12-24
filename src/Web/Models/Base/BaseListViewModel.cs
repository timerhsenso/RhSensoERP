// ============================================================================
// BASE LIST VIEW MODEL V2 - Aprimorado
// ============================================================================
// Arquivo: src/Web/Models/Base/BaseListViewModel.cs
// Versão: 2.0
// Data: 24/11/2025
//
// ViewModel base para páginas de listagem com DataTables.
// Inclui configurações completas para exportação, validação e customização.
//
// ============================================================================

namespace RhSensoERP.Web.Models.Base;

/// <summary>
/// ViewModel base para páginas de listagem com DataTables.
/// </summary>
public abstract class BaseListViewModel
{
    #region Propriedades de Página

    /// <summary>
    /// Título da página.
    /// </summary>
    public string PageTitle { get; set; } = string.Empty;

    /// <summary>
    /// Subtítulo da página.
    /// </summary>
    public string? PageSubtitle { get; set; }

    /// <summary>
    /// Ícone da página (Font Awesome).
    /// Exemplo: "fas fa-list", "fas fa-users", "fas fa-building"
    /// </summary>
    public string PageIcon { get; set; } = "fas fa-list";

    #endregion

    #region Propriedades de Controller e Actions

    /// <summary>
    /// Nome do controller.
    /// </summary>
    public string ControllerName { get; set; } = string.Empty;

    /// <summary>
    /// Nome da action para listagem (DataTables Ajax).
    /// </summary>
    public string ListActionName { get; set; } = "List";

    /// <summary>
    /// Nome da action para criar.
    /// </summary>
    public string CreateActionName { get; set; } = "Create";

    /// <summary>
    /// Nome da action para editar.
    /// </summary>
    public string EditActionName { get; set; } = "Edit";

    /// <summary>
    /// Nome da action para visualizar.
    /// </summary>
    public string ViewActionName { get; set; } = "GetById";

    /// <summary>
    /// Nome da action para excluir.
    /// </summary>
    public string DeleteActionName { get; set; } = "Delete";

    /// <summary>
    /// Nome da action para excluir múltiplos.
    /// </summary>
    public string DeleteMultipleActionName { get; set; } = "DeleteMultiple";

    #endregion

    #region Propriedades de Permissões

    /// <summary>
    /// Código da função para controle de permissões.
    /// </summary>
    public string? CdFuncao { get; set; }

    /// <summary>
    /// Permissões do usuário para esta funcionalidade (ex: "IAEC").
    /// I = Incluir, A = Alterar, E = Excluir, C = Consultar
    /// </summary>
    public string? UserPermissions { get; set; }

    /// <summary>
    /// Indica se o usuário pode incluir registros.
    /// </summary>
    public bool CanCreate => UserPermissions?.Contains('I') == true;

    /// <summary>
    /// Indica se o usuário pode alterar registros.
    /// </summary>
    public bool CanEdit => UserPermissions?.Contains('A') == true;

    /// <summary>
    /// Indica se o usuário pode excluir registros.
    /// </summary>
    public bool CanDelete => UserPermissions?.Contains('E') == true;

    /// <summary>
    /// Indica se o usuário pode consultar registros.
    /// </summary>
    public bool CanView => UserPermissions?.Contains('C') == true;

    #endregion

    #region Propriedades de Exibição de Elementos

    /// <summary>
    /// Indica se deve exibir o botão de criar.
    /// </summary>
    public bool ShowCreateButton { get; set; } = true;

    /// <summary>
    /// Indica se deve exibir o botão de excluir múltiplos.
    /// </summary>
    public bool ShowDeleteMultipleButton { get; set; } = true;

    /// <summary>
    /// Indica se deve exibir o botão de atualizar.
    /// </summary>
    public bool ShowRefreshButton { get; set; } = true;

    /// <summary>
    /// Indica se deve exibir os botões de exportação.
    /// </summary>
    public bool ShowExportButtons { get; set; } = true;

    /// <summary>
    /// Indica se deve exibir a coluna de seleção (checkbox).
    /// </summary>
    public bool ShowSelectColumn { get; set; } = true;

    /// <summary>
    /// Indica se deve exibir a coluna de ações.
    /// </summary>
    public bool ShowActionsColumn { get; set; } = true;

    /// <summary>
    /// Indica se deve exibir a caixa de pesquisa.
    /// </summary>
    public bool ShowSearchBox { get; set; } = true;

    #endregion

    #region Propriedades de Exportação

    /// <summary>
    /// Indica se a exportação está habilitada.
    /// </summary>
    public bool ExportEnabled { get; set; } = true;

    /// <summary>
    /// Indica se a exportação para Excel está habilitada.
    /// </summary>
    public bool ExportExcel { get; set; } = true;

    /// <summary>
    /// Indica se a exportação para PDF está habilitada.
    /// </summary>
    public bool ExportPdf { get; set; } = true;

    /// <summary>
    /// Indica se a exportação para CSV está habilitada.
    /// </summary>
    public bool ExportCsv { get; set; } = true;

    /// <summary>
    /// Indica se a opção de impressão está habilitada.
    /// </summary>
    public bool ExportPrint { get; set; } = true;

    /// <summary>
    /// Nome do arquivo para exportação (sem extensão).
    /// </summary>
    public string? ExportFilename { get; set; }

    #endregion

    #region Propriedades de Configuração DataTables

    /// <summary>
    /// Número de registros por página (padrão).
    /// </summary>
    public int PageLength { get; set; } = 10;

    /// <summary>
    /// Opções de registros por página.
    /// Exemplo: [10, 25, 50, 100, -1] onde -1 = Todos
    /// </summary>
    public int[] LengthMenuOptions { get; set; } = new[] { 10, 25, 50, 100 };

    /// <summary>
    /// Coluna padrão para ordenação (índice baseado em 0).
    /// </summary>
    public int DefaultOrderColumn { get; set; } = 1;

    /// <summary>
    /// Direção padrão de ordenação ("asc" ou "desc").
    /// </summary>
    public string DefaultOrderDirection { get; set; } = "asc";

    /// <summary>
    /// Indica se deve usar paginação.
    /// </summary>
    public bool UsePagination { get; set; } = true;

    /// <summary>
    /// Indica se deve usar ordenação.
    /// </summary>
    public bool UseOrdering { get; set; } = true;

    /// <summary>
    /// Indica se deve usar busca/filtro.
    /// </summary>
    public bool UseSearching { get; set; } = true;

    /// <summary>
    /// Indica se deve usar modo responsivo.
    /// </summary>
    public bool UseResponsive { get; set; } = true;

    #endregion

    #region Propriedades de Customização

    /// <summary>
    /// Configurações adicionais do DataTables (JSON).
    /// </summary>
    public string? AdditionalDataTablesConfig { get; set; }

    /// <summary>
    /// Mensagem customizada quando não há registros.
    /// </summary>
    public string? EmptyTableMessage { get; set; }

    /// <summary>
    /// CSS classes adicionais para a tabela.
    /// </summary>
    public string? TableCssClasses { get; set; }

    /// <summary>
    /// CSS classes adicionais para o container.
    /// </summary>
    public string? ContainerCssClasses { get; set; }

    #endregion

    #region Métodos Auxiliares

    /// <summary>
    /// Inicializa propriedades padrão baseadas no nome do controller.
    /// Deve ser chamado no construtor das classes filhas.
    /// </summary>
    protected void InitializeDefaults(string controllerName, string entityNamePlural)
    {
        ControllerName = controllerName;
        ExportFilename = entityNamePlural;
    }

    /// <summary>
    /// Obtém o objeto de configuração de exportação para serialização JSON.
    /// </summary>
    public object GetExportConfig()
    {
        return new
        {
            enabled = ExportEnabled,
            excel = ExportExcel,
            pdf = ExportPdf,
            csv = ExportCsv,
            print = ExportPrint,
            filename = ExportFilename
        };
    }

    /// <summary>
    /// Obtém o objeto de configuração DataTables para serialização JSON.
    /// </summary>
    public object GetDataTablesConfig()
    {
        return new
        {
            pageLength = PageLength,
            lengthMenu = LengthMenuOptions,
            defaultOrderColumn = DefaultOrderColumn,
            defaultOrderDirection = DefaultOrderDirection,
            paging = UsePagination,
            ordering = UseOrdering,
            searching = UseSearching,
            responsive = UseResponsive
        };
    }

    #endregion
}
