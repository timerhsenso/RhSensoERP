// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: CapTiposOcorrencia
// Module: ControleAcessoPortaria
// Data: 2026-01-07 21:19:53
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapTiposOcorrencia;

/// <summary>
/// ViewModel para listagem de Cadastro de Tipo de Ocorrência.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class CapTiposOcorrenciaListViewModel : BaseListViewModel
{
    public CapTiposOcorrenciaListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("CapTiposOcorrencia", "Cadastro de Tipo de Ocorrência");
        
        // Configurações específicas
        PageTitle = "Cadastro de Tipo de Ocorrência";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapTiposOcorrenciaDto> Items { get; set; } = new();
}
