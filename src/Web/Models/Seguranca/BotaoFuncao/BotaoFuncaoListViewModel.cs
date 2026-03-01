// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: BotaoFuncao
// Module: Seguranca
// Data: 2026-02-28 19:22:44
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Seguranca.BotaoFuncao;

/// <summary>
/// ViewModel para listagem de Tabela de Botões.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class BotaoFuncaoListViewModel : BaseListViewModel
{
    public BotaoFuncaoListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("BotaoFuncao", "Tabela de Botões");
        
        // Configurações específicas
        PageTitle = "Tabela de Botões";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<BotaoFuncaoDto> Items { get; set; } = new();
}
