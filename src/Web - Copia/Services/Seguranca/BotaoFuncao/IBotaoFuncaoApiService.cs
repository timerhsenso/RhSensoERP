// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.3
// Entity: BotaoFuncao
// Module: Seguranca
// Data: 2026-03-02 17:53:39
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.Seguranca.BotaoFuncao;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Seguranca.BotaoFuncao;

/// <summary>
/// Interface do serviço de API para Botões de Função.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface IBotaoFuncaoApiService 
    : IApiService<BotaoFuncaoDto, CreateBotaoFuncaoRequest, UpdateBotaoFuncaoRequest, string>,
      IBatchDeleteService<string>
{

}
