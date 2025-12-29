// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.3
// Entity: CapTiposOcorrencia
// Module: ControleAcessoPortaria
// Data: 2025-12-28 19:27:50
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.ControleAcessoPortaria.CapTiposOcorrencia;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.ControleAcessoPortaria.CapTiposOcorrencia;

/// <summary>
/// Interface do serviço de API para CapTiposOcorrencia.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ICapTiposOcorrenciaApiService 
    : IApiService<CapTiposOcorrenciaDto, CreateCapTiposOcorrenciaRequest, UpdateCapTiposOcorrenciaRequest, int>,
      IBatchDeleteService<int>
{
}
