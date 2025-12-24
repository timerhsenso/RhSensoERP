// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: CapVisitantes
// Module: ControleAcessoPortaria
// Data: 2025-12-24 01:10:14
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapVisitantes;

/// <summary>
/// ViewModel para listagem de CapVisitantes.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class CapVisitantesListViewModel : BaseListViewModel
{
    public CapVisitantesListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("CapVisitantes", "CapVisitantes");
        
        // Configurações específicas
        PageTitle = "CapVisitantes";
        PageIcon = "fas fa-table";
        CdFuncao = "CAP_FM_VISITANTES";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapVisitantesDto> Items { get; set; } = new();
}
