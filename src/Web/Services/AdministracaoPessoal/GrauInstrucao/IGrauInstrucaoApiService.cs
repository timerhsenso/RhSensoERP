// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: GrauInstrucao
// Module: AdministracaoPessoal
// Data: 2026-02-28 19:50:42
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.AdministracaoPessoal.GrauInstrucao;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.AdministracaoPessoal.GrauInstrucao;

/// <summary>
/// Interface do serviço de API para Grau de instrucao.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface IGrauInstrucaoApiService 
    : IApiService<GrauInstrucaoDto, CreateGrauInstrucaoRequest, UpdateGrauInstrucaoRequest, Guid>,
      IBatchDeleteService<Guid>
{

}
