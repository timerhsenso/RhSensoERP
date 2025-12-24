// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.3
// Entity: BasFeriados
// Module: GestaoDePessoas
// Data: 2025-12-22 23:31:27
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.GestaoDePessoas.BasFeriados;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.GestaoDePessoas.BasFeriados;

/// <summary>
/// Interface do serviço de API para BasFeriados.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface IBasFeriadosApiService 
    : IApiService<BasFeriadosDto, CreateBasFeriadosRequest, UpdateBasFeriadosRequest, int>,
      IBatchDeleteService<int>
{
}
