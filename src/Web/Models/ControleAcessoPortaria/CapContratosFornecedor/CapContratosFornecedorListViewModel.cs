// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapContratosFornecedor
// Module: ControleAcessoPortaria
// Data: 2025-12-30 21:36:02
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapContratosFornecedor;

/// <summary>
/// ViewModel para listagem de CapContratosFornecedor.
/// Herda de BaseListViewModel que ja contem permissoes e configuracoes de DataTables.
/// </summary>
public class CapContratosFornecedorListViewModel : BaseListViewModel
{
    public CapContratosFornecedorListViewModel()
    {
        // Inicializa propriedades padrao
        InitializeDefaults("CapContratosFornecedor", "CapContratosFornecedor");
        
        // Configuracoes especificas
        PageTitle = "CapContratosFornecedor";
        PageIcon = "fas fa-table";
        CdFuncao = "CAP_FM_CONTRATOS";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapContratosFornecedorDto> Items { get; set; } = new();
}
