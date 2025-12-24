// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: CapContratosFornecedor
// Module: ControleAcessoPortaria
// Data: 2025-12-24 01:21:44
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapContratosFornecedor;

/// <summary>
/// ViewModel para listagem de CapContratosFornecedor.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class CapContratosFornecedorListViewModel : BaseListViewModel
{
    public CapContratosFornecedorListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("CapContratosFornecedor", "CapContratosFornecedor");
        
        // Configurações específicas
        PageTitle = "CapContratosFornecedor";
        PageIcon = "fas fa-table";
        CdFuncao = "CAP_FM_CONTRATOS";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapContratosFornecedorDto> Items { get; set; } = new();
}
