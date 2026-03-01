// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: Cbo
// Module: AdministracaoPessoal
// Data: 2026-02-28 22:21:22
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.AdministracaoPessoal.Cbo;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.AdministracaoPessoal.Cbo;

/// <summary>
/// Interface do serviço de API para CBO.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface ICboApiService 
    : IApiService<CboDto, CreateCboRequest, UpdateCboRequest, Guid>,
      IBatchDeleteService<Guid>
{

}
