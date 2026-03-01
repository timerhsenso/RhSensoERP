// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Municipio
// Module: AdministracaoPessoal
// Data: 2026-02-28 22:07:57
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.Municipio;

/// <summary>
/// ViewModel para listagem de Municipios.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class MunicipioListViewModel : BaseListViewModel
{
    public MunicipioListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("Municipio", "Municipios");
        
        // Configurações específicas
        PageTitle = "Municipios";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<MunicipioDto> Items { get; set; } = new();
}
