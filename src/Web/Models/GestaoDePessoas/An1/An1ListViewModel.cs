// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: An1
// Module: GestaoDePessoas
// Data: 2025-12-28 21:00:16
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.GestaoDePessoas.An1;

/// <summary>
/// ViewModel para listagem de Banco.
/// Herda de BaseListViewModel que ja contem permissoes e configuracoes de DataTables.
/// </summary>
public class An1ListViewModel : BaseListViewModel
{
    public An1ListViewModel()
    {
        // Inicializa propriedades padrao
        InitializeDefaults("An1", "Banco");
        
        // Configuracoes especificas
        PageTitle = "Banco";
        PageIcon = "fas fa-table";
        CdFuncao = "RHU_FM_BANCO";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<An1Dto> Items { get; set; } = new();
}
