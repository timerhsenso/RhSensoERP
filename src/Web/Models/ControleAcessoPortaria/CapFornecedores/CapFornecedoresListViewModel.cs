// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: CapFornecedores
// Module: ControleAcessoPortaria
// Data: 2025-12-24 01:02:36
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapFornecedores;

/// <summary>
/// ViewModel para listagem de CapFornecedores.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class CapFornecedoresListViewModel : BaseListViewModel
{
    public CapFornecedoresListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("CapFornecedores", "CapFornecedores");
        
        // Configurações específicas
        PageTitle = "CapFornecedores";
        PageIcon = "fas fa-table";
        CdFuncao = "CAP_FM_FORNECEDORES";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapFornecedoresDto> Items { get; set; } = new();
}
