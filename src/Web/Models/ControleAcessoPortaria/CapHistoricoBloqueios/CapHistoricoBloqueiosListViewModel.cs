// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapHistoricoBloqueios
// Module: ControleAcessoPortaria
// Data: 2025-12-28 20:51:29
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapHistoricoBloqueios;

/// <summary>
/// ViewModel para listagem de CapHistoricoBloqueios.
/// Herda de BaseListViewModel que ja contem permissoes e configuracoes de DataTables.
/// </summary>
public class CapHistoricoBloqueiosListViewModel : BaseListViewModel
{
    public CapHistoricoBloqueiosListViewModel()
    {
        // Inicializa propriedades padrao
        InitializeDefaults("CapHistoricoBloqueios", "CapHistoricoBloqueios");
        
        // Configuracoes especificas
        PageTitle = "CapHistoricoBloqueios";
        PageIcon = "fas fa-table";
        CdFuncao = "CAP_FM_HISTORICOBLOQUEIOS";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapHistoricoBloqueiosDto> Items { get; set; } = new();
}
