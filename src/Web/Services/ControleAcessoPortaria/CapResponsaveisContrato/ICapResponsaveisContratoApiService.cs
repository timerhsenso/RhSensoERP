// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.3
// Entity: CapResponsaveisContrato
// Module: ControleAcessoPortaria
// Data: 2025-12-24 01:15:50
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.ControleAcessoPortaria.CapResponsaveisContrato;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.ControleAcessoPortaria.CapResponsaveisContrato;

/// <summary>
/// Interface do serviço de API para CapResponsaveisContrato.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ICapResponsaveisContratoApiService 
    : IApiService<CapResponsaveisContratoDto, CreateCapResponsaveisContratoRequest, UpdateCapResponsaveisContratoRequest, int>,
      IBatchDeleteService<int>
{
}
