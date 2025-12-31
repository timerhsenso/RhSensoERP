// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v5.2
// Entity: CapHistoricoBloqueios
// Module: ControleAcessoPortaria
// Data: 2025-12-30 21:31:32
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.ControleAcessoPortaria.CapHistoricoBloqueios;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.ControleAcessoPortaria.CapHistoricoBloqueios;

/// <summary>
/// Interface do serviço de API para CapHistoricoBloqueios.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v5.2: Compatível com BaseApiService genérico.
/// v4.1: Adiciona ToggleAtivoAsync para alternar status dinamicamente.
/// </summary>
public interface ICapHistoricoBloqueiosApiService 
    : IApiService<CapHistoricoBloqueiosDto, CreateCapHistoricoBloqueiosRequest, UpdateCapHistoricoBloqueiosRequest, long>,
      IBatchDeleteService<long>
{
}
