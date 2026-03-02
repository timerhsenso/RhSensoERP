// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TipoParentesco
// Module: Shared
// Data: 2026-03-02 17:59:26
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Shared.TipoParentesco;

/// <summary>
/// ViewModel para listagem de Tipo de Parentesco.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class TipoParentescoListViewModel : BaseListViewModel
{
    public TipoParentescoListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("TipoParentesco", "Tipo de Parentesco");
        
        // Configurações específicas
        PageTitle = "Tipo de Parentesco";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<TipoParentescoDto> Items { get; set; } = new();
}
