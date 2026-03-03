// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: CapColaboradoresFornecedor
// Module: GestaoTerceirosPrestadores
// Data: 2026-03-02 21:41:08
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.GestaoTerceirosPrestadores.CapColaboradoresFornecedor;

/// <summary>
/// ViewModel para listagem de Cadastro de prestadores.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class CapColaboradoresFornecedorListViewModel : BaseListViewModel
{
    public CapColaboradoresFornecedorListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("CapColaboradoresFornecedor", "Cadastro de prestadores");
        
        // Configurações específicas
        PageTitle = "Cadastro de prestadores";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapColaboradoresFornecedorDto> Items { get; set; } = new();
}
