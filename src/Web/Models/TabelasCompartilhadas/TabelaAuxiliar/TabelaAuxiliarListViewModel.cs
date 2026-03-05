// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TabelaAuxiliar
// Module: TabelasCompartilhadas
// Data: 2026-03-04 16:27:03
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.TabelasCompartilhadas.TabelaAuxiliar;

/// <summary>
/// ViewModel para listagem de Tabela Auxiliar.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class TabelaAuxiliarListViewModel : BaseListViewModel
{
    public TabelaAuxiliarListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("TabelaAuxiliar", "Tabela Auxiliar");
        
        // Configurações específicas
        PageTitle = "Tabela Auxiliar";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<TabelaAuxiliarDto> Items { get; set; } = new();
}
