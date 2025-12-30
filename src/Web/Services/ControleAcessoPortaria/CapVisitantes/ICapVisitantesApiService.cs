// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: CapVisitantes
// Module: ControleAcessoPortaria
// Data: 2025-12-30 04:08:11
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.ControleAcessoPortaria.CapVisitantes;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.ControleAcessoPortaria.CapVisitantes;

/// <summary>
/// Interface do serviço de API para CapVisitantes.
/// Herda de IApiService e IBatchDeleteService existentes.
/// v4.0: Suporta ordenação server-side.
/// </summary>
public interface ICapVisitantesApiService 
    : IApiService<CapVisitantesDto, CreateCapVisitantesRequest, UpdateCapVisitantesRequest, int>,
      IBatchDeleteService<int>
{
}
