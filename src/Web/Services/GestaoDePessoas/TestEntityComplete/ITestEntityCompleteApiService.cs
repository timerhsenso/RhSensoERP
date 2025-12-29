// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.3
// Entity: TestEntityComplete
// Module: GestaoDePessoas
// Data: 2025-12-28 14:22:46
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.GestaoDePessoas.TestEntityComplete;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.GestaoDePessoas.TestEntityComplete;

/// <summary>
/// Interface do serviço de API para Test Entity Complete.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ITestEntityCompleteApiService 
    : IApiService<TestEntityCompleteDto, CreateTestEntityCompleteRequest, UpdateTestEntityCompleteRequest, int>,
      IBatchDeleteService<int>
{
}
