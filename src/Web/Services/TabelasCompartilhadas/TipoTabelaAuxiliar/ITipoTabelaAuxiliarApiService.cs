// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.3
// Entity: TipoTabelaAuxiliar
// Module: TabelasCompartilhadas
// Data: 2026-03-04 00:11:12
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.TabelasCompartilhadas.TipoTabelaAuxiliar;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.TabelasCompartilhadas.TipoTabelaAuxiliar;

/// <summary>
/// Interface do serviço de API para Tabela Auxiliar.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface ITipoTabelaAuxiliarApiService 
    : IApiService<TipoTabelaAuxiliarDto, CreateTipoTabelaAuxiliarRequest, UpdateTipoTabelaAuxiliarRequest, string>,
      IBatchDeleteService<string>
{

}
