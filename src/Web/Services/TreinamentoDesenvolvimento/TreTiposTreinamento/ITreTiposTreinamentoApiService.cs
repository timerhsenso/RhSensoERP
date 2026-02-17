// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v6.1
// Entity: TreTiposTreinamento
// Module: TreinamentoDesenvolvimento
// Data: 2026-02-16 23:23:32
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.TreinamentoDesenvolvimento.TreTiposTreinamento;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.TreinamentoDesenvolvimento.TreTiposTreinamento;

/// <summary>
/// Interface do serviço de API para Cadastro de Tipo de Treinamento.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v6.1: CORRIGIDO - Lookup usa 'term' para Select2.
/// v6.0: Adiciona métodos Select2 Lookup automáticos.
/// v4.1: Adiciona ToggleAtivoAsync.
/// </summary>
public interface ITreTiposTreinamentoApiService 
    : IApiService<TreTiposTreinamentoDto, CreateTreTiposTreinamentoRequest, UpdateTreTiposTreinamentoRequest, int>,
      IBatchDeleteService<int>
{

    /// <summary>
    /// Alterna o status Ativo/Desativo de um registro.
    /// </summary>
    Task ToggleAtivoAsync(int id, bool ativo, CancellationToken ct = default);

}
