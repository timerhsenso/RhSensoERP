// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: VinculoEmpregaticio
// Module: AdministracaoPessoal
// Data: 2026-02-28 21:06:39
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.AdministracaoPessoal.VinculoEmpregaticio;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.AdministracaoPessoal.VinculoEmpregaticio;

/// <summary>
/// Interface do serviço de API para Vinculo Empregaticio.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface IVinculoEmpregaticioApiService 
    : IApiService<VinculoEmpregaticioDto, CreateVinculoEmpregaticioRequest, UpdateVinculoEmpregaticioRequest, Guid>,
      IBatchDeleteService<Guid>
{

}
