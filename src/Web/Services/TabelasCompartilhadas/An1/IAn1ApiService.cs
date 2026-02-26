// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: An1
// Module: TabelasCompartilhadas
// Data: 2026-02-25 16:30:15
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.TabelasCompartilhadas.An1;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.TabelasCompartilhadas.An1;

/// <summary>
/// Interface do serviço de API para Tabela de Bancos.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface IAn1ApiService 
    : IApiService<An1Dto, CreateAn1Request, UpdateAn1Request, Guid>,
      IBatchDeleteService<Guid>
{

}
