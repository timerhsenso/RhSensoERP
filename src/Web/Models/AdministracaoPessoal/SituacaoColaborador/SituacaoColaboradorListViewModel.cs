// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: SituacaoColaborador
// Module: AdministracaoPessoal
// Data: 2026-02-28 22:05:47
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.SituacaoColaborador;

/// <summary>
/// ViewModel para listagem de Situação do Colaborador.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class SituacaoColaboradorListViewModel : BaseListViewModel
{
    public SituacaoColaboradorListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("SituacaoColaborador", "Situação do Colaborador");
        
        // Configurações específicas
        PageTitle = "Situação do Colaborador";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<SituacaoColaboradorDto> Items { get; set; } = new();
}
