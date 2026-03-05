// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.3
// Entity: TabelaAuxiliar
// Module: TabelasCompartilhadas
// Data: 2026-03-04 16:27:03
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.TabelasCompartilhadas.TabelaAuxiliar;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.TabelasCompartilhadas.TabelaAuxiliar;

/// <summary>
/// Interface do serviço de API para Tabela Auxiliar.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface ITabelaAuxiliarApiService 
    : IApiService<TabelaAuxiliarDto, CreateTabelaAuxiliarRequest, UpdateTabelaAuxiliarRequest, string>,
      IBatchDeleteService<string>
{

}
