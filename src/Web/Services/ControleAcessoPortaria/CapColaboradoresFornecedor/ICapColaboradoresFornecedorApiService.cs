// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: CapColaboradoresFornecedor
// Module: ControleAcessoPortaria
// Data: 2026-01-02 19:59:34
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.ControleAcessoPortaria.CapColaboradoresFornecedor;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.ControleAcessoPortaria.CapColaboradoresFornecedor;

/// <summary>
/// Interface do serviço de API para CapColaboradoresFornecedor.
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
