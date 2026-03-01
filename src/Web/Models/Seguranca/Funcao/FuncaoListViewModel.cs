// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Funcao
// Module: Seguranca
// Data: 2026-02-28 19:04:06
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Seguranca.Funcao;

/// <summary>
/// ViewModel para listagem de Funções do Sistema.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class FuncaoListViewModel : BaseListViewModel
{
    public FuncaoListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("Funcao", "Funções do Sistema");
        
        // Configurações específicas
        PageTitle = "Funções do Sistema";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<FuncaoDto> Items { get; set; } = new();
}
