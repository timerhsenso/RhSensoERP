// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: AgenciaBancaria
// Module: AdministracaoPessoal
// Data: 2026-02-28 20:05:58
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.AdministracaoPessoal.AgenciaBancaria;

/// <summary>
/// ViewModel para listagem de Tabela de Agências.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class AgenciaBancariaListViewModel : BaseListViewModel
{
    public AgenciaBancariaListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("AgenciaBancaria", "Tabela de Agências");
        
        // Configurações específicas
        PageTitle = "Tabela de Agências";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<AgenciaBancariaDto> Items { get; set; } = new();
}
