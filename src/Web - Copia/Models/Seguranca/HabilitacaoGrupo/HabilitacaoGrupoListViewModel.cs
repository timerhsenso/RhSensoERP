// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: HabilitacaoGrupo
// Module: Seguranca
// Data: 2026-03-02 17:57:18
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Seguranca.HabilitacaoGrupo;

/// <summary>
/// ViewModel para listagem de Habilitação de Grupo.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class HabilitacaoGrupoListViewModel : BaseListViewModel
{
    public HabilitacaoGrupoListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("HabilitacaoGrupo", "Habilitação de Grupo");
        
        // Configurações específicas
        PageTitle = "Habilitação de Grupo";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<HabilitacaoGrupoDto> Items { get; set; } = new();
}
