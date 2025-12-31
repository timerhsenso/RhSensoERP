// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v5.2
// Entity: Tsistema
// Module: Seguranca
// Data: 2025-12-30 17:48:37
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.Seguranca.Tsistema;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Seguranca.Tsistema;

/// <summary>
/// Interface do serviço de API para Tabela de Sistemas.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v5.2: Compatível com BaseApiService genérico.
/// v4.1: Adiciona ToggleAtivoAsync para alternar status dinamicamente.
/// </summary>
public interface ITsistemaApiService 
    : IApiService<TsistemaDto, CreateTsistemaRequest, UpdateTsistemaRequest, string>,
      IBatchDeleteService<string>
{
}
