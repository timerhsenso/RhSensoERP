// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.3
// Entity: Funcao
// Module: Seguranca
// Data: 2026-03-02 19:25:55
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.Seguranca.Funcao;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Seguranca.Funcao;

/// <summary>
/// Interface do serviço de API para Funções do Sistema.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface IFuncaoApiService 
    : IApiService<FuncaoDto, CreateFuncaoRequest, UpdateFuncaoRequest, string>,
      IBatchDeleteService<string>
{

}
