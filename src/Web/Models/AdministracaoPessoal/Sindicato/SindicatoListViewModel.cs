// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Sindicato
// Module: AdministracaoPessoal
// Data: 2026-02-28 21:56:09
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.Sindicato;

/// <summary>
/// ViewModel para listagem de Sindicato.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class SindicatoListViewModel : BaseListViewModel
{
    public SindicatoListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("Sindicato", "Sindicato");
        
        // Configurações específicas
        PageTitle = "Sindicato";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<SindicatoDto> Items { get; set; } = new();
}
