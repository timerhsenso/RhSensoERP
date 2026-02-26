// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: SHR_CNAE
// Module: GestaoTerceirosPrestadores
// Data: 2026-02-25 11:14:57
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.GestaoTerceirosPrestadores.SHR_CNAE;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.GestaoTerceirosPrestadores.SHR_CNAE;

/// <summary>
/// Interface do serviço de API para Tabela de CNAE.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface ISHR_CNAEApiService 
    : IApiService<SHR_CNAEDto, CreateSHR_CNAERequest, UpdateSHR_CNAERequest, int>,
      IBatchDeleteService<int>
{

    /// <summary>
    /// Alterna o status Ativo/Desativo de um registro.
    /// </summary>
    Task ToggleAtivoAsync(int id, bool ativo, CancellationToken ct = default);

}
