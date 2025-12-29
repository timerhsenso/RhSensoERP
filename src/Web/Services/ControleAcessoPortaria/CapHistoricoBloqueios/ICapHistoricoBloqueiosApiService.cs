// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.3
// Entity: CapHistoricoBloqueios
// Module: ControleAcessoPortaria
// Data: 2025-12-28 20:51:29
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.ControleAcessoPortaria.CapHistoricoBloqueios;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.ControleAcessoPortaria.CapHistoricoBloqueios;

/// <summary>
/// Interface do serviço de API para CapHistoricoBloqueios.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ICapHistoricoBloqueiosApiService 
    : IApiService<CapHistoricoBloqueiosDto, CreateCapHistoricoBloqueiosRequest, UpdateCapHistoricoBloqueiosRequest, long>,
      IBatchDeleteService<long>
{
}
