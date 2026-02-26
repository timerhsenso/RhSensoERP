// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: Temp1
// Module: TabelasCompartilhadas
// Data: 2026-02-25 19:38:48
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.TabelasCompartilhadas.Temp1;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.TabelasCompartilhadas.Temp1;

/// <summary>
/// Interface do serviço de API para Empresa.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface ITemp1ApiService 
    : IApiService<Temp1Dto, CreateTemp1Request, UpdateTemp1Request, Guid>,
      IBatchDeleteService<Guid>
{

}
