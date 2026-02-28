// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: Gurh1
// Module: Seguranca
// Data: 2026-02-28 09:59:07
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.Seguranca.Gurh1;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Seguranca.Gurh1;

/// <summary>
/// Interface do serviço de API para Grupo de Usuários.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface IGurh1ApiService 
    : IApiService<Gurh1Dto, CreateGurh1Request, UpdateGurh1Request, string>,
      IBatchDeleteService<string>
{

}
