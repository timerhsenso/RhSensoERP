// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.1
// Entity: Tsistema
// Data: 2025-12-02 02:25:04
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Tsistemas;

/// <summary>
/// ViewModel para listagem de Tsistema.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class TsistemasListViewModel : BaseListViewModel
{
    public TsistemasListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("Tsistemas", "Tsistema");
        
        // Configurações específicas
        PageTitle = "Tsistema";
        PageIcon = "fas fa-list";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<TsistemaDto> Items { get; set; } = new();
}
