// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: VinculoEmpregaticio
// Module: AdministracaoPessoal
// Data: 2026-02-28 21:06:39
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.VinculoEmpregaticio;

/// <summary>
/// ViewModel para listagem de Vinculo Empregaticio.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class VinculoEmpregaticioListViewModel : BaseListViewModel
{
    public VinculoEmpregaticioListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("VinculoEmpregaticio", "Vinculo Empregaticio");
        
        // Configurações específicas
        PageTitle = "Vinculo Empregaticio";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<VinculoEmpregaticioDto> Items { get; set; } = new();
}
