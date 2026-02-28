// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: Fucn1
// Module: Seguranca
// Data: 2026-02-28 01:25:19
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.Seguranca.Fucn1;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Seguranca.Fucn1;

/// <summary>
/// Interface do serviço de API para Cadastro de Terceiros.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface IFucn1ApiService 
    : IApiService<Fucn1Dto, CreateFucn1Request, UpdateFucn1Request, string>,
      IBatchDeleteService<string>
{

}
