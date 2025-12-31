// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v5.2
// Entity: TreTiposTreinamento
// Module: GestaoDePessoas
// Data: 2025-12-30 17:32:28
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.GestaoDePessoas.TreTiposTreinamento;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.GestaoDePessoas.TreTiposTreinamento;

/// <summary>
/// Interface do serviço de API para Tipos de Treinamento.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v5.2: Compatível com BaseApiService genérico.
/// v4.1: Adiciona ToggleAtivoAsync para alternar status dinamicamente.
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
