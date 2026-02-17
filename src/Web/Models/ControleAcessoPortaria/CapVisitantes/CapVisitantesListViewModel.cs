// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: CapVisitantes
// Module: ControleAcessoPortaria
// Data: 2026-02-16 23:34:17
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapVisitantes;

/// <summary>
/// ViewModel para listagem de teste1.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class CapVisitantesListViewModel : BaseListViewModel
{
    public CapVisitantesListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("CapVisitantes", "teste1");
        
        // Configurações específicas
        PageTitle = "teste1";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapVisitantesDto> Items { get; set; } = new();
}
