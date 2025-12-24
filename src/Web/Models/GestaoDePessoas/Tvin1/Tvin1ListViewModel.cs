// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: Tvin1
// Module: GestaoDePessoas
// Data: 2025-12-24 00:36:02
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.GestaoDePessoas.Tvin1;

/// <summary>
/// ViewModel para listagem de Vínculo Empregatício.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class Tvin1ListViewModel : BaseListViewModel
{
    public Tvin1ListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("Tvin1", "Vínculo Empregatício");
        
        // Configurações específicas
        PageTitle = "Vínculo Empregatício";
        PageIcon = "fas fa-table";
        CdFuncao = "RHU_FM_VINCULO";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<Tvin1Dto> Items { get; set; } = new();
}
