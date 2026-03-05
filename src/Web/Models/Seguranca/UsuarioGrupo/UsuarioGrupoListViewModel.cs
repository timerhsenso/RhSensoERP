// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: UsuarioGrupo
// Module: Seguranca
// Data: 2026-03-05 01:06:29
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Seguranca.UsuarioGrupo;

/// <summary>
/// ViewModel para listagem de Usuário-Grupo.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class UsuarioGrupoListViewModel : BaseListViewModel
{
    public UsuarioGrupoListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("UsuarioGrupo", "Usuário-Grupo");
        
        // Configurações específicas
        PageTitle = "Usuário-Grupo";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<UsuarioGrupoDto> Items { get; set; } = new();
}
