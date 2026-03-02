// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TabelaSalarial
// Module: AdministracaoPessoal
// Data: 2026-03-02 18:01:50
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.TabelaSalarial;

/// <summary>
/// ViewModel para listagem de Tabela Salarial.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class TabelaSalarialListViewModel : BaseListViewModel
{
    public TabelaSalarialListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("TabelaSalarial", "Tabela Salarial");
        
        // Configurações específicas
        PageTitle = "Tabela Salarial";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<TabelaSalarialDto> Items { get; set; } = new();
}
