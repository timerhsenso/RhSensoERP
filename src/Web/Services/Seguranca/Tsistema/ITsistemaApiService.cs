// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: Tsistema
// Module: Seguranca
// Data: 2026-02-28 01:05:14
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.Seguranca.Tsistema;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Seguranca.Tsistema;

/// <summary>
/// Interface do serviço de API para Tabela de Sistemas.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface ITsistemaApiService 
    : IApiService<TsistemaDto, CreateTsistemaRequest, UpdateTsistemaRequest, string>,
      IBatchDeleteService<string>
{

}
