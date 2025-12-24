// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: TreTiposTreinamento
// Module: TreinamentoDesenvolvimento
// Data: 2025-12-22 12:51:16
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.TreinamentoDesenvolvimento.TreTiposTreinamento;

/// <summary>
/// ViewModel para listagem de Tipos de Treinamento.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class TreTiposTreinamentoListViewModel : BaseListViewModel
{
    public TreTiposTreinamentoListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("TreTiposTreinamento", "Tipos de Treinamento");
        
        // Configurações específicas
        PageTitle = "Tipos de Treinamento";
        PageIcon = "fas fa-table";
        CdFuncao = "SGT_FM_TRETIPOSTREINAMENTO";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<TreTiposTreinamentoDto> Items { get; set; } = new();
}
