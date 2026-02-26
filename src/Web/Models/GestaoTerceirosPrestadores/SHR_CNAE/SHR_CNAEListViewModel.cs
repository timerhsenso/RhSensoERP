// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: SHR_CNAE
// Module: GestaoTerceirosPrestadores
// Data: 2026-02-25 11:14:57
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.GestaoTerceirosPrestadores.SHR_CNAE;

/// <summary>
/// ViewModel para listagem de Tabela de CNAE.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class SHR_CNAEListViewModel : BaseListViewModel
{
    public SHR_CNAEListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("SHR_CNAE", "Tabela de CNAE");
        
        // Configurações específicas
        PageTitle = "Tabela de CNAE";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<SHR_CNAEDto> Items { get; set; } = new();
}
