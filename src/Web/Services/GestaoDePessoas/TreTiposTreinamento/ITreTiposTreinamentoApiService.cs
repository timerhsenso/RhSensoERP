// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.3
// Entity: TreTiposTreinamento
// Module: GestaoDePessoas
// Data: 2025-12-28 14:07:48
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.GestaoDePessoas.TreTiposTreinamento;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.GestaoDePessoas.TreTiposTreinamento;

/// <summary>
/// Interface do serviço de API para Tipos de Treinamento.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ITreTiposTreinamentoApiService 
    : IApiService<TreTiposTreinamentoDto, CreateTreTiposTreinamentoRequest, UpdateTreTiposTreinamentoRequest, int>,
      IBatchDeleteService<int>
{
}
