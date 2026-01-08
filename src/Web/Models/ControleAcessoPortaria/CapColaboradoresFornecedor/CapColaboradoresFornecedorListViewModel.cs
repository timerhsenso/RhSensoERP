// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: CapColaboradoresFornecedor
// Module: ControleAcessoPortaria
// Data: 2026-01-07 21:15:52
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapColaboradoresFornecedor;

/// <summary>
/// ViewModel para listagem de Cadastro de Terceiros.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class CapColaboradoresFornecedorListViewModel : BaseListViewModel
{
    public CapColaboradoresFornecedorListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("CapColaboradoresFornecedor", "Cadastro de Terceiros");
        
        // Configurações específicas
        PageTitle = "Cadastro de Terceiros";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapColaboradoresFornecedorDto> Items { get; set; } = new();
}
