// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: CapFornecedores
// Module: GestaoTerceirosPrestadores
// Data: 2026-03-02 21:31:48
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.GestaoTerceirosPrestadores.CapFornecedores;

/// <summary>
/// ViewModel para listagem de Cadastro de Fonecedores.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class CapFornecedoresListViewModel : BaseListViewModel
{
    public CapFornecedoresListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("CapFornecedores", "Cadastro de Fonecedores");
        
        // Configurações específicas
        PageTitle = "Cadastro de Fonecedores";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapFornecedoresDto> Items { get; set; } = new();
}
