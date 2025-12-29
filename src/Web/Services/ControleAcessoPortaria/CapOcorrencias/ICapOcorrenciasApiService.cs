// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.3
// Entity: CapOcorrencias
// Module: ControleAcessoPortaria
// Data: 2025-12-28 20:36:06
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.ControleAcessoPortaria.CapOcorrencias;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.ControleAcessoPortaria.CapOcorrencias;

/// <summary>
/// Interface do serviço de API para CapOcorrencias.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ICapOcorrenciasApiService 
    : IApiService<CapOcorrenciasDto, CreateCapOcorrenciasRequest, UpdateCapOcorrenciasRequest, int>,
      IBatchDeleteService<int>
{
}
