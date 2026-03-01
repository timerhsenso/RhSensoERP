// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: GrauInstrucao
// Module: AdministracaoPessoal
// Data: 2026-02-28 19:50:42
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.GrauInstrucao;

/// <summary>
/// ViewModel para listagem de Grau de instrucao.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class GrauInstrucaoListViewModel : BaseListViewModel
{
    public GrauInstrucaoListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("GrauInstrucao", "Grau de instrucao");
        
        // Configurações específicas
        PageTitle = "Grau de instrucao";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<GrauInstrucaoDto> Items { get; set; } = new();
}
