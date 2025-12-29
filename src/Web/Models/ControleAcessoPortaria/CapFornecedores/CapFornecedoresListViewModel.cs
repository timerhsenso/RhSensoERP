// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapFornecedores
// Module: ControleAcessoPortaria
// Data: 2025-12-28 17:46:18
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapFornecedores;

/// <summary>
/// ViewModel para listagem de CapFornecedores.
/// Herda de BaseListViewModel que ja contem permissoes e configuracoes de DataTables.
/// </summary>
public class CapFornecedoresListViewModel : BaseListViewModel
{
    public CapFornecedoresListViewModel()
    {
        // Inicializa propriedades padrao
        InitializeDefaults("CapFornecedores", "CapFornecedores");
        
        // Configuracoes especificas
        PageTitle = "CapFornecedores";
        PageIcon = "fas fa-table";
        CdFuncao = "CAP_FM_FORNECEDORES";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapFornecedoresDto> Items { get; set; } = new();
}
