// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapOcorrencias
// Module: ControleAcessoPortaria
// Data: 2025-12-28 20:36:06
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapOcorrencias;

/// <summary>
/// ViewModel para listagem de CapOcorrencias.
/// Herda de BaseListViewModel que ja contem permissoes e configuracoes de DataTables.
/// </summary>
public class CapOcorrenciasListViewModel : BaseListViewModel
{
    public CapOcorrenciasListViewModel()
    {
        // Inicializa propriedades padrao
        InitializeDefaults("CapOcorrencias", "CapOcorrencias");
        
        // Configuracoes especificas
        PageTitle = "CapOcorrencias";
        PageIcon = "fas fa-table";
        CdFuncao = "CAP_FM_OCORRENCIAS";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapOcorrenciasDto> Items { get; set; } = new();
}
