// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TipoTabelaAuxiliar
// Module: TabelasCompartilhadas
// Data: 2026-03-04 00:11:12
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.TabelasCompartilhadas.TipoTabelaAuxiliar;

/// <summary>
/// ViewModel para listagem de Tabela Auxiliar.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class TipoTabelaAuxiliarListViewModel : BaseListViewModel
{
    public TipoTabelaAuxiliarListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("TipoTabelaAuxiliar", "Tabela Auxiliar");
        
        // Configurações específicas
        PageTitle = "Tabela Auxiliar";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<TipoTabelaAuxiliarDto> Items { get; set; } = new();
}
