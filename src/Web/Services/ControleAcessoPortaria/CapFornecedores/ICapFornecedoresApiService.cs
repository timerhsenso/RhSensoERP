// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.3
// Entity: CapFornecedores
// Module: ControleAcessoPortaria
// Data: 2025-12-28 17:46:18
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.ControleAcessoPortaria.CapFornecedores;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.ControleAcessoPortaria.CapFornecedores;

/// <summary>
/// Interface do serviço de API para CapFornecedores.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ICapFornecedoresApiService 
    : IApiService<CapFornecedoresDto, CreateCapFornecedoresRequest, UpdateCapFornecedoresRequest, int>,
      IBatchDeleteService<int>
{
}
