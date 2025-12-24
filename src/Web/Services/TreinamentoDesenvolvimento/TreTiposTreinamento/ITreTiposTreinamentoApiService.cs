// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.3
// Entity: TreTiposTreinamento
// Module: TreinamentoDesenvolvimento
// Data: 2025-12-22 12:51:16
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.TreinamentoDesenvolvimento.TreTiposTreinamento;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.TreinamentoDesenvolvimento.TreTiposTreinamento;

/// <summary>
/// Interface do serviço de API para Tipos de Treinamento.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ITreTiposTreinamentoApiService 
    : IApiService<TreTiposTreinamentoDto, CreateTreTiposTreinamentoRequest, UpdateTreTiposTreinamentoRequest, int>,
      IBatchDeleteService<int>
{
}
