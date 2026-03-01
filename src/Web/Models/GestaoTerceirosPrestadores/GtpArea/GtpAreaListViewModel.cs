// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: GtpArea
// Module: GestaoTerceirosPrestadores
// Data: 2026-02-28 22:25:01
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.GestaoTerceirosPrestadores.GtpArea;

/// <summary>
/// ViewModel para listagem de Áreas.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class GtpAreaListViewModel : BaseListViewModel
{
    public GtpAreaListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("GtpArea", "Áreas");
        
        // Configurações específicas
        PageTitle = "Áreas";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<GtpAreaDto> Items { get; set; } = new();
}
