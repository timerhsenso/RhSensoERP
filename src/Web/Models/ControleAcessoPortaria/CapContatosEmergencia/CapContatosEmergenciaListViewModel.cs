// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapContatosEmergencia
// Module: ControleAcessoPortaria
// Data: 2025-12-30 21:38:31
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapContatosEmergencia;

/// <summary>
/// ViewModel para listagem de CapContatosEmergencia.
/// Herda de BaseListViewModel que ja contem permissoes e configuracoes de DataTables.
/// </summary>
public class CapContatosEmergenciaListViewModel : BaseListViewModel
{
    public CapContatosEmergenciaListViewModel()
    {
        // Inicializa propriedades padrao
        InitializeDefaults("CapContatosEmergencia", "CapContatosEmergencia");
        
        // Configuracoes especificas
        PageTitle = "CapContatosEmergencia";
        PageIcon = "fas fa-table";
        CdFuncao = "CAP_FM_CONTATOSEMERGENCIA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapContatosEmergenciaDto> Items { get; set; } = new();
}
