// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: TreTiposTreinamento
// Module: GestaoDePessoas
// Data: 2025-12-28 14:07:48
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.GestaoDePessoas.TreTiposTreinamento;

/// <summary>
/// ViewModel para listagem de Tipos de Treinamento.
/// Herda de BaseListViewModel que ja contem permissoes e configuracoes de DataTables.
/// </summary>
public class TreTiposTreinamentoListViewModel : BaseListViewModel
{
    public TreTiposTreinamentoListViewModel()
    {
        // Inicializa propriedades padrao
        InitializeDefaults("TreTiposTreinamento", "Tipos de Treinamento");
        
        // Configuracoes especificas
        PageTitle = "Tipos de Treinamento";
        PageIcon = "fas fa-table";
        CdFuncao = "SGT_FM_TRETIPOSTREINAMENTO";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<TreTiposTreinamentoDto> Items { get; set; } = new();
}
