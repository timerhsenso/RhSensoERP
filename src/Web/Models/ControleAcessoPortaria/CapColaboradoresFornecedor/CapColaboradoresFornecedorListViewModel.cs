// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: CapColaboradoresFornecedor
// Module: ControleAcessoPortaria
// Data: 2026-01-04 16:00:38
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapColaboradoresFornecedor;

/// <summary>
/// ViewModel para listagem de CapColaboradoresFornecedor.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class CapColaboradoresFornecedorListViewModel : BaseListViewModel
{
    public CapColaboradoresFornecedorListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("CapColaboradoresFornecedor", "CapColaboradoresFornecedor");
        
        // Configurações específicas
        PageTitle = "CapColaboradoresFornecedor";
        PageIcon = "fas fa-table";
        CdFuncao = "CAP_FM_COLABORADORES";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapColaboradoresFornecedorDto> Items { get; set; } = new();
}
