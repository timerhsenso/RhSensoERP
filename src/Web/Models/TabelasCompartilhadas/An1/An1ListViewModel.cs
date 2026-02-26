// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: An1
// Module: TabelasCompartilhadas
// Data: 2026-02-25 16:30:15
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.TabelasCompartilhadas.An1;

/// <summary>
/// ViewModel para listagem de Tabela de Bancos.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class An1ListViewModel : BaseListViewModel
{
    public An1ListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("An1", "Tabela de Bancos");
        
        // Configurações específicas
        PageTitle = "Tabela de Bancos";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<An1Dto> Items { get; set; } = new();
}
