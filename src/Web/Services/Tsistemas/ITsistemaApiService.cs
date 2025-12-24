// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.0
// Entity: Tsistema
// Data: 2025-12-02 02:25:04
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.Tsistemas;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Tsistemas;

/// <summary>
/// Interface do serviço de API para Tsistema.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ITsistemaApiService 
    : IApiService<TsistemaDto, CreateTsistemaRequest, UpdateTsistemaRequest, string>,
      IBatchDeleteService<string>
{
}
