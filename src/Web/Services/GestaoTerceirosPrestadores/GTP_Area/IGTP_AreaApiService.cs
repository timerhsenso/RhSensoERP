// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: GTP_Area
// Module: GestaoTerceirosPrestadores
// Data: 2026-02-24 21:34:17
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.GestaoTerceirosPrestadores.GTP_Area;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.GestaoTerceirosPrestadores.GTP_Area;

/// <summary>
/// Interface do serviço de API para Cadastro de Terceiros teste.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface IGTP_AreaApiService 
    : IApiService<GTP_AreaDto, CreateGTP_AreaRequest, UpdateGTP_AreaRequest, int>,
      IBatchDeleteService<int>
{

    /// <summary>
    /// Alterna o status Ativo/Desativo de um registro.
    /// </summary>
    Task ToggleAtivoAsync(int id, bool ativo, CancellationToken ct = default);

}
