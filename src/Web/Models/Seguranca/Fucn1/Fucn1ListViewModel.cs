// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Fucn1
// Module: Seguranca
// Data: 2026-02-28 01:25:19
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Seguranca.Fucn1;

/// <summary>
/// ViewModel para listagem de Cadastro de Terceiros.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class Fucn1ListViewModel : BaseListViewModel
{
    public Fucn1ListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("Fucn1", "Cadastro de Terceiros");
        
        // Configurações específicas
        PageTitle = "Cadastro de Terceiros";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<Fucn1Dto> Items { get; set; } = new();
}
