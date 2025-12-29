// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.3
// Entity: CapContatosEmergencia
// Module: ControleAcessoPortaria
// Data: 2025-12-28 20:54:55
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.ControleAcessoPortaria.CapContatosEmergencia;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.ControleAcessoPortaria.CapContatosEmergencia;

/// <summary>
/// Interface do serviço de API para CapContatosEmergencia.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ICapContatosEmergenciaApiService 
    : IApiService<CapContatosEmergenciaDto, CreateCapContatosEmergenciaRequest, UpdateCapContatosEmergenciaRequest, int>,
      IBatchDeleteService<int>
{
}
