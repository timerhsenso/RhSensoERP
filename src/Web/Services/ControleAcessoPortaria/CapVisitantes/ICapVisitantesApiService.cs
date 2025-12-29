// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.3
// Entity: CapVisitantes
// Module: ControleAcessoPortaria
// Data: 2025-12-28 19:08:36
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.ControleAcessoPortaria.CapVisitantes;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.ControleAcessoPortaria.CapVisitantes;

/// <summary>
/// Interface do serviço de API para CapVisitantes.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ICapVisitantesApiService 
    : IApiService<CapVisitantesDto, CreateCapVisitantesRequest, UpdateCapVisitantesRequest, int>,
      IBatchDeleteService<int>
{
}
