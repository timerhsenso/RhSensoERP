// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Gurh1
// Module: Seguranca
// Data: 2026-02-28 09:59:07
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Seguranca.Gurh1;

/// <summary>
/// ViewModel para listagem de Grupo de Usuários.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class Gurh1ListViewModel : BaseListViewModel
{
    public Gurh1ListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("Gurh1", "Grupo de Usuários");
        
        // Configurações específicas
        PageTitle = "Grupo de Usuários";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<Gurh1Dto> Items { get; set; } = new();
}
