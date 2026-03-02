// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.3
// Entity: TabelaSalarial
// Module: AdministracaoPessoal
// Data: 2026-03-02 18:01:50
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.AdministracaoPessoal.TabelaSalarial;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.AdministracaoPessoal.TabelaSalarial;

/// <summary>
/// Interface do serviço de API para Tabela Salarial.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface ITabelaSalarialApiService 
    : IApiService<TabelaSalarialDto, CreateTabelaSalarialRequest, UpdateTabelaSalarialRequest, Guid>,
      IBatchDeleteService<Guid>
{

}
