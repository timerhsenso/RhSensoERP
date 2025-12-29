// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: TestEntityComplete
// Module: GestaoDePessoas
// Data: 2025-12-28 14:22:46
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.GestaoDePessoas.TestEntityComplete;

/// <summary>
/// ViewModel para listagem de Test Entity Complete.
/// Herda de BaseListViewModel que ja contem permissoes e configuracoes de DataTables.
/// </summary>
public class TestEntityCompleteListViewModel : BaseListViewModel
{
    public TestEntityCompleteListViewModel()
    {
        // Inicializa propriedades padrao
        InitializeDefaults("TestEntityComplete", "Test Entity Complete");
        
        // Configuracoes especificas
        PageTitle = "Test Entity Complete";
        PageIcon = "fas fa-table";
        CdFuncao = "TEST_FM_COMPLETE";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<TestEntityCompleteDto> Items { get; set; } = new();
}
