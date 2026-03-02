// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: GrupoDeUsuario
// Module: Seguranca
// Data: 2026-03-01 22:11:05
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Seguranca.GrupoDeUsuario;

/// <summary>
/// ViewModel para listagem de Grupo de Usuários.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class GrupoDeUsuarioListViewModel : BaseListViewModel
{
    public GrupoDeUsuarioListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("GrupoDeUsuario", "Grupo de Usuários");
        
        // Configurações específicas
        PageTitle = "Grupo de Usuários";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<GrupoDeUsuarioDto> Items { get; set; } = new();
}
