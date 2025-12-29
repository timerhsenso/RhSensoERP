// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapResponsaveisContrato
// Module: ControleAcessoPortaria
// Data: 2025-12-28 20:29:17
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapResponsaveisContrato;

/// <summary>
/// ViewModel para listagem de CapResponsaveisContrato.
/// Herda de BaseListViewModel que ja contem permissoes e configuracoes de DataTables.
/// </summary>
public class CapResponsaveisContratoListViewModel : BaseListViewModel
{
    public CapResponsaveisContratoListViewModel()
    {
        // Inicializa propriedades padrao
        InitializeDefaults("CapResponsaveisContrato", "CapResponsaveisContrato");
        
        // Configuracoes especificas
        PageTitle = "CapResponsaveisContrato";
        PageIcon = "fas fa-table";
        CdFuncao = "CAP_FM_RESPONSAVEIS";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapResponsaveisContratoDto> Items { get; set; } = new();
}
