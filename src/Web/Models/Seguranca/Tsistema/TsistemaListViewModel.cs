// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: Tsistema
// Module: Seguranca
// Data: 2025-12-30 17:48:37
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Seguranca.Tsistema;

/// <summary>
/// ViewModel para listagem de Tabela de Sistemas.
/// Herda de BaseListViewModel que ja contem permissoes e configuracoes de DataTables.
/// </summary>
public class TsistemaListViewModel : BaseListViewModel
{
    public TsistemaListViewModel()
    {
        // Inicializa propriedades padrao
        InitializeDefaults("Tsistema", "Tabela de Sistemas");
        
        // Configuracoes especificas
        PageTitle = "Tabela de Sistemas";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<TsistemaDto> Items { get; set; } = new();
}
