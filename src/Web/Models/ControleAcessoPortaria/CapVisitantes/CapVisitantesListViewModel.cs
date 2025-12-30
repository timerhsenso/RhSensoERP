// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapVisitantes
// Module: ControleAcessoPortaria
// Data: 2025-12-30 04:08:11
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapVisitantes;

/// <summary>
/// ViewModel para listagem de CapVisitantes.
/// Herda de BaseListViewModel que ja contem permissoes e configuracoes de DataTables.
/// </summary>
public class CapVisitantesListViewModel : BaseListViewModel
{
    public CapVisitantesListViewModel()
    {
        // Inicializa propriedades padrao
        InitializeDefaults("CapVisitantes", "CapVisitantes");
        
        // Configuracoes especificas
        PageTitle = "CapVisitantes";
        PageIcon = "fas fa-table";
        CdFuncao = "CAP_FM_VISITANTES";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapVisitantesDto> Items { get; set; } = new();
}
