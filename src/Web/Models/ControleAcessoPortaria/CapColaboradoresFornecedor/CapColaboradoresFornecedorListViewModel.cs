// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapColaboradoresFornecedor
// Module: ControleAcessoPortaria
// Data: 2025-12-30 21:41:02
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapColaboradoresFornecedor;

/// <summary>
/// ViewModel para listagem de CapColaboradoresFornecedor.
/// Herda de BaseListViewModel que ja contem permissoes e configuracoes de DataTables.
/// </summary>
public class CapColaboradoresFornecedorListViewModel : BaseListViewModel
{
    public CapColaboradoresFornecedorListViewModel()
    {
        // Inicializa propriedades padrao
        InitializeDefaults("CapColaboradoresFornecedor", "CapColaboradoresFornecedor");
        
        // Configuracoes especificas
        PageTitle = "CapColaboradoresFornecedor";
        PageIcon = "fas fa-table";
        CdFuncao = "CAP_FM_COLABORADORES";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapColaboradoresFornecedorDto> Items { get; set; } = new();
}
