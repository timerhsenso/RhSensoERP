// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapBloqueiosPessoa
// Module: ControleAcessoPortaria
// Data: 2025-12-30 21:42:59
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapBloqueiosPessoa;

/// <summary>
/// ViewModel para listagem de CapBloqueiosPessoa.
/// Herda de BaseListViewModel que ja contem permissoes e configuracoes de DataTables.
/// </summary>
public class CapBloqueiosPessoaListViewModel : BaseListViewModel
{
    public CapBloqueiosPessoaListViewModel()
    {
        // Inicializa propriedades padrao
        InitializeDefaults("CapBloqueiosPessoa", "CapBloqueiosPessoa");
        
        // Configuracoes especificas
        PageTitle = "CapBloqueiosPessoa";
        PageIcon = "fas fa-table";
        CdFuncao = "CAP_FM_BLOQUEIOS";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<CapBloqueiosPessoaDto> Items { get; set; } = new();
}
