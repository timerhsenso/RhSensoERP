// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: BasFeriados
// Module: GestaoDePessoas
// Data: 2025-12-22 23:31:27
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.GestaoDePessoas.BasFeriados;

/// <summary>
/// ViewModel para listagem de BasFeriados.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class BasFeriadosListViewModel : BaseListViewModel
{
    public BasFeriadosListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("BasFeriados", "BasFeriados");
        
        // Configurações específicas
        PageTitle = "BasFeriados";
        PageIcon = "fas fa-table";
        CdFuncao = "BAS_FM_FERIADOS";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<BasFeriadosDto> Items { get; set; } = new();
}
