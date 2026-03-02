// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.3
// Entity: HabilitacaoGrupo
// Module: Seguranca
// Data: 2026-03-02 17:57:18
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.Seguranca.HabilitacaoGrupo;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Seguranca.HabilitacaoGrupo;

/// <summary>
/// Interface do serviço de API para Habilitação de Grupo.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface IHabilitacaoGrupoApiService 
    : IApiService<HabilitacaoGrupoDto, CreateHabilitacaoGrupoRequest, UpdateHabilitacaoGrupoRequest, Guid>,
      IBatchDeleteService<Guid>
{

}
