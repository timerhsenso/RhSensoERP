// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: CapVisitantes
// Module: ControleAcessoPortaria
// Data: 2026-01-07 21:13:40
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.ControleAcessoPortaria.CapVisitantes;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.ControleAcessoPortaria.CapVisitantes;

/// <summary>
/// Interface do serviço de API para Cadastro de Visitantes.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface ICapVisitantesApiService 
    : IApiService<CapVisitantesDto, CreateCapVisitantesRequest, UpdateCapVisitantesRequest, int>,
      IBatchDeleteService<int>
{

    /// <summary>
    /// Alterna o status Ativo/Desativo de um registro.
    /// </summary>
    Task ToggleAtivoAsync(int id, bool ativo, CancellationToken ct = default);

}
