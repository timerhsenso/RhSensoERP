// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TreTiposTreinamento
// Module: TreinamentoDesenvolvimento
// Data: 2026-02-16 23:23:32
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.TreinamentoDesenvolvimento.TreTiposTreinamento;

/// <summary>
/// ViewModel para listagem de Cadastro de Tipo de Treinamento.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class TreTiposTreinamentoListViewModel : BaseListViewModel
{
    public TreTiposTreinamentoListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("TreTiposTreinamento", "Cadastro de Tipo de Treinamento");
        
        // Configurações específicas
        PageTitle = "Cadastro de Tipo de Treinamento";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<TreTiposTreinamentoDto> Items { get; set; } = new();
}
