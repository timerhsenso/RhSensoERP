// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0 FINAL
// Entity: TreTiposTreinamento
// Module: GestaoDePessoas
// Data: 2025-12-30 05:27:05
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.GestaoDePessoas.TreTiposTreinamento;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.GestaoDePessoas.TreTiposTreinamento;

/// <summary>
/// Interface do serviço de API para Tipos de Treinamento.
/// Herda de IApiService e IBatchDeleteService existentes.
/// v4.0 FINAL: Suporta ordenação server-side com sortBy/desc.
/// </summary>
public interface ITreTiposTreinamentoApiService 
    : IApiService<TreTiposTreinamentoDto, CreateTreTiposTreinamentoRequest, UpdateTreTiposTreinamentoRequest, int>,
      IBatchDeleteService<int>
{
}
