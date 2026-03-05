// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: GTP_TipoDocumentoFornecedor
// Module: GestaoTerceirosPrestadores
// Data: 2026-03-04 22:56:13
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.GestaoTerceirosPrestadores.GTP_TipoDocumentoFornecedor;

/// <summary>
/// ViewModel para listagem de Tipo de Documento de Fornecedor.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class GTP_TipoDocumentoFornecedorListViewModel : BaseListViewModel
{
    public GTP_TipoDocumentoFornecedorListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("GTP_TipoDocumentoFornecedor", "Tipo de Documento de Fornecedor");
        
        // Configurações específicas
        PageTitle = "Tipo de Documento de Fornecedor";
        PageIcon = "fas fa-table";
        CdFuncao = "GTP_WEB_TIPODOCFORNECEDOR";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<GTP_TipoDocumentoFornecedorDto> Items { get; set; } = new();
}
