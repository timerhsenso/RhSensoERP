// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: BotaoFuncao
// Module: Seguranca
// Data: 2026-03-02 17:53:39
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Seguranca.BotaoFuncao;

/// <summary>
/// ViewModel para listagem de Botões de Função.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class BotaoFuncaoListViewModel : BaseListViewModel
{
    public BotaoFuncaoListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("BotaoFuncao", "Botões de Função");
        
        // Configurações específicas
        PageTitle = "Botões de Função";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<BotaoFuncaoDto> Items { get; set; } = new();
}
