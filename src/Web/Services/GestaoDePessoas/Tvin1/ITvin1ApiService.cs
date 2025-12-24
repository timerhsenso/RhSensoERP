// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.3
// Entity: Tvin1
// Module: GestaoDePessoas
// Data: 2025-12-24 00:36:02
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.GestaoDePessoas.Tvin1;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.GestaoDePessoas.Tvin1;

/// <summary>
/// Interface do serviço de API para Vínculo Empregatício.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ITvin1ApiService 
    : IApiService<Tvin1Dto, CreateTvin1Request, UpdateTvin1Request, Guid>,
      IBatchDeleteService<Guid>
{
}
