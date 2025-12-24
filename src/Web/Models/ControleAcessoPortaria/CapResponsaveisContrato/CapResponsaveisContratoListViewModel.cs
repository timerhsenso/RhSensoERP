// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: CapResponsaveisContrato
// Module: ControleAcessoPortaria
// Data: 2025-12-24 01:15:50
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapResponsaveisContrato;

/// <summary>
/// ViewModel para listagem de CapResponsaveisContrato.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class CapResponsaveisContratoListViewModel : BaseListViewModel
{
    public CapResponsaveisContratoListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("CapResponsaveisContrato", "CapResponsaveisContrato");
        
        // Configurações específicas
        PageTitle = "CapResponsaveisContrato";
        PageIcon = "fas fa-table";
        CdFuncao = "CAP_FM_RESPONSAVEIS";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapResponsaveisContratoDto> Items { get; set; } = new();
}
