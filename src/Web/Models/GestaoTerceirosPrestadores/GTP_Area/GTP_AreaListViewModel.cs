// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: GTP_Area
// Module: GestaoTerceirosPrestadores
// Data: 2026-02-24 21:34:17
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.GestaoTerceirosPrestadores.GTP_Area;

/// <summary>
/// ViewModel para listagem de Cadastro de Terceiros teste.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class GTP_AreaListViewModel : BaseListViewModel
{
    public GTP_AreaListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("GTP_Area", "Cadastro de Terceiros teste");
        
        // Configurações específicas
        PageTitle = "Cadastro de Terceiros teste";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<GTP_AreaDto> Items { get; set; } = new();
}
