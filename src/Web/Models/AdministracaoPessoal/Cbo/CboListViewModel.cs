// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Cbo
// Module: AdministracaoPessoal
// Data: 2026-02-28 22:21:22
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.Cbo;

/// <summary>
/// ViewModel para listagem de CBO.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class CboListViewModel : BaseListViewModel
{
    public CboListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("Cbo", "CBO");
        
        // Configurações específicas
        PageTitle = "CBO";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CboDto> Items { get; set; } = new();
}
