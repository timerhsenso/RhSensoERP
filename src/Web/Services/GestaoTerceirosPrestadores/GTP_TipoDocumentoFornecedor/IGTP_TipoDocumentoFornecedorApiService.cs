// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.3
// Entity: GTP_TipoDocumentoFornecedor
// Module: GestaoTerceirosPrestadores
// Data: 2026-03-04 22:56:13
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.GestaoTerceirosPrestadores.GTP_TipoDocumentoFornecedor;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.GestaoTerceirosPrestadores.GTP_TipoDocumentoFornecedor;

/// <summary>
/// Interface do serviço de API para Tipo de Documento de Fornecedor.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface IGTP_TipoDocumentoFornecedorApiService 
    : IApiService<GTP_TipoDocumentoFornecedorDto, CreateGTP_TipoDocumentoFornecedorRequest, UpdateGTP_TipoDocumentoFornecedorRequest, int>,
      IBatchDeleteService<int>
{

    /// <summary>
    /// Alterna o status Ativo/Desativo de um registro.
    /// </summary>
    Task ToggleAtivoAsync(int id, bool ativo, CancellationToken ct = default);

}
