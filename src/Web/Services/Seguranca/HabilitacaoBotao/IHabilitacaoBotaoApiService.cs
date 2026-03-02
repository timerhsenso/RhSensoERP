// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: HabilitacaoBotao
// Module: Seguranca
// Data: 2026-03-01 13:35:46
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.Seguranca.HabilitacaoBotao;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Seguranca.HabilitacaoBotao;

/// <summary>
/// Interface do serviço de API para Hab botao.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface IHabilitacaoBotaoApiService 
    : IApiService<HabilitacaoBotaoDto, CreateHabilitacaoBotaoRequest, UpdateHabilitacaoBotaoRequest, Guid>,
      IBatchDeleteService<Guid>
{

}
