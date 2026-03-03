// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.3
// Entity: CapColaboradoresFornecedor
// Module: GestaoTerceirosPrestadores
// Data: 2026-03-02 21:41:08
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.GestaoTerceirosPrestadores.CapColaboradoresFornecedor;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.GestaoTerceirosPrestadores.CapColaboradoresFornecedor;

/// <summary>
/// Interface do serviço de API para Cadastro de prestadores.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface ICapColaboradoresFornecedorApiService 
    : IApiService<CapColaboradoresFornecedorDto, CreateCapColaboradoresFornecedorRequest, UpdateCapColaboradoresFornecedorRequest, int>,
      IBatchDeleteService<int>
{

    /// <summary>
    /// Alterna o status Ativo/Desativo de um registro.
    /// </summary>
    Task ToggleAtivoAsync(int id, bool ativo, CancellationToken ct = default);

}
