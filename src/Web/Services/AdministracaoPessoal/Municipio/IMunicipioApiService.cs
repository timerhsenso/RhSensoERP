// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: Municipio
// Module: AdministracaoPessoal
// Data: 2026-02-28 22:07:57
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.AdministracaoPessoal.Municipio;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.AdministracaoPessoal.Municipio;

/// <summary>
/// Interface do serviço de API para Municipios.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface IMunicipioApiService 
    : IApiService<MunicipioDto, CreateMunicipioRequest, UpdateMunicipioRequest, Guid>,
      IBatchDeleteService<Guid>
{

}
