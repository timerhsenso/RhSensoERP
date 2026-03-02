// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: HabilitacaoBotao
// Module: Seguranca
// Data: 2026-03-01 13:35:46
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Seguranca.HabilitacaoBotao;

/// <summary>
/// ViewModel para listagem de Hab botao.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class HabilitacaoBotaoListViewModel : BaseListViewModel
{
    public HabilitacaoBotaoListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("HabilitacaoBotao", "Hab botao");
        
        // Configurações específicas
        PageTitle = "Hab botao";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<HabilitacaoBotaoDto> Items { get; set; } = new();
}
