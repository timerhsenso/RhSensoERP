// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Tuse1
// Module: Identity
// Data: 2026-02-25 21:55:51
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Identity.Tuse1;

/// <summary>
/// ViewModel para listagem de Usuários.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class Tuse1ListViewModel : BaseListViewModel
{
    public Tuse1ListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("Tuse1", "Usuários");
        
        // Configurações específicas
        PageTitle = "Usuários";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<Tuse1Dto> Items { get; set; } = new();
}
