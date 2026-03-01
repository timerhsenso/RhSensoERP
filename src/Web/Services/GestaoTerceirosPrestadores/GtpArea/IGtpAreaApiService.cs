// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: GtpArea
// Module: GestaoTerceirosPrestadores
// Data: 2026-02-28 22:25:01
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.GestaoTerceirosPrestadores.GtpArea;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.GestaoTerceirosPrestadores.GtpArea;

/// <summary>
/// Interface do serviço de API para Áreas.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface IGtpAreaApiService 
    : IApiService<GtpAreaDto, CreateGtpAreaRequest, UpdateGtpAreaRequest, int>,
      IBatchDeleteService<int>
{

    /// <summary>
    /// Alterna o status Ativo/Desativo de um registro.
    /// </summary>
    Task ToggleAtivoAsync(int id, bool ativo, CancellationToken ct = default);

}
