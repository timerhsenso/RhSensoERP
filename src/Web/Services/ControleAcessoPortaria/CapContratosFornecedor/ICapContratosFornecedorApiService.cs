// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.3
// Entity: CapContratosFornecedor
// Module: ControleAcessoPortaria
// Data: 2025-12-24 01:21:44
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.ControleAcessoPortaria.CapContratosFornecedor;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.ControleAcessoPortaria.CapContratosFornecedor;

/// <summary>
/// Interface do serviço de API para CapContratosFornecedor.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ICapContratosFornecedorApiService 
    : IApiService<CapContratosFornecedorDto, CreateCapContratosFornecedorRequest, UpdateCapContratosFornecedorRequest, int>,
      IBatchDeleteService<int>
{
}
