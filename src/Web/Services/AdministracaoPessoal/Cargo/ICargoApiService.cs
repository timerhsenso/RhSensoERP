// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: Cargo
// Module: AdministracaoPessoal
// Data: 2026-02-28 22:19:49
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.AdministracaoPessoal.Cargo;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.AdministracaoPessoal.Cargo;

/// <summary>
/// Interface do serviço de API para Cargos.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface ICargoApiService 
    : IApiService<CargoDto, CreateCargoRequest, UpdateCargoRequest, Guid>,
      IBatchDeleteService<Guid>
{

}
