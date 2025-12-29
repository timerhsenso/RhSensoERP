// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.3
// Entity: An1
// Module: GestaoDePessoas
// Data: 2025-12-28 21:00:16
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.GestaoDePessoas.An1;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.GestaoDePessoas.An1;

/// <summary>
/// Interface do serviço de API para Banco.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface IAn1ApiService 
    : IApiService<An1Dto, CreateAn1Request, UpdateAn1Request, Guid>,
      IBatchDeleteService<Guid>
{
}
