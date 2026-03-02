// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.3
// Entity: TipoParentesco
// Module: Shared
// Data: 2026-03-02 17:59:26
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.Shared.TipoParentesco;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Shared.TipoParentesco;

/// <summary>
/// Interface do serviço de API para Tipo de Parentesco.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface ITipoParentescoApiService 
    : IApiService<TipoParentescoDto, CreateTipoParentescoRequest, UpdateTipoParentescoRequest, byte>,
      IBatchDeleteService<byte>
{

    /// <summary>
    /// Alterna o status Ativo/Desativo de um registro.
    /// </summary>
    Task ToggleAtivoAsync(byte id, bool ativo, CancellationToken ct = default);

}
