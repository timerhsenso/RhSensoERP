// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapTiposOcorrencia
// Module: ControleAcessoPortaria
// Data: 2025-12-28 19:27:50
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapTiposOcorrencia;

/// <summary>
/// ViewModel para listagem de CapTiposOcorrencia.
/// Herda de BaseListViewModel que ja contem permissoes e configuracoes de DataTables.
/// </summary>
public class CapTiposOcorrenciaListViewModel : BaseListViewModel
{
    public CapTiposOcorrenciaListViewModel()
    {
        // Inicializa propriedades padrao
        InitializeDefaults("CapTiposOcorrencia", "CapTiposOcorrencia");
        
        // Configuracoes especificas
        PageTitle = "CapTiposOcorrencia";
        PageIcon = "fas fa-table";
        CdFuncao = "CAP_FM_TIPOSOCORRENCIA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapTiposOcorrenciaDto> Items { get; set; } = new();
}
