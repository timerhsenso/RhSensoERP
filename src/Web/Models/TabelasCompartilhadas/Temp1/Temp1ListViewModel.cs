// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Temp1
// Module: TabelasCompartilhadas
// Data: 2026-02-25 19:38:48
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.TabelasCompartilhadas.Temp1;

/// <summary>
/// ViewModel para listagem de Empresa.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class Temp1ListViewModel : BaseListViewModel
{
    public Temp1ListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("Temp1", "Empresa");
        
        // Configurações específicas
        PageTitle = "Empresa";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<Temp1Dto> Items { get; set; } = new();
}
