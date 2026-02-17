// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: CapTiposOcorrencia
// Module: ControleAcessoPortaria
// Data: 2026-02-16 23:54:18
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapTiposOcorrencia;

/// <summary>
/// ViewModel para listagem de teste2.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class CapTiposOcorrenciaListViewModel : BaseListViewModel
{
    public CapTiposOcorrenciaListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("CapTiposOcorrencia", "teste2");
        
        // Configurações específicas
        PageTitle = "teste2";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapTiposOcorrenciaDto> Items { get; set; } = new();
}
