// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Cargo
// Module: AdministracaoPessoal
// Data: 2026-02-28 22:19:49
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.Cargo;

/// <summary>
/// ViewModel para listagem de Cargos.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class CargoListViewModel : BaseListViewModel
{
    public CargoListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("Cargo", "Cargos");
        
        // Configurações específicas
        PageTitle = "Cargos";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CargoDto> Items { get; set; } = new();
}
