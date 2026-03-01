// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: AgenciaBancaria
// Module: AdministracaoPessoal
// Data: 2026-02-28 20:05:58
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.AdministracaoPessoal.AgenciaBancaria;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.AdministracaoPessoal.AgenciaBancaria;

/// <summary>
/// Interface do serviço de API para Tabela de Agências.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface IAgenciaBancariaApiService 
    : IApiService<AgenciaBancariaDto, CreateAgenciaBancariaRequest, UpdateAgenciaBancariaRequest, Guid>,
      IBatchDeleteService<Guid>
{

}
