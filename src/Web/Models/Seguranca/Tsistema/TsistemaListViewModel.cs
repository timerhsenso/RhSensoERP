// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Tsistema
// Module: Seguranca
// Data: 2026-02-28 01:05:14
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Seguranca.Tsistema;

/// <summary>
/// ViewModel para listagem de Tabela de Sistemas.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class TsistemaListViewModel : BaseListViewModel
{
    public TsistemaListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("Tsistema", "Tabela de Sistemas");
        
        // Configurações específicas
        PageTitle = "Tabela de Sistemas";
        PageIcon = "fas fa-table";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<TsistemaDto> Items { get; set; } = new();
}
