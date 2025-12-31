// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v5.2
// Entity: CapBloqueiosPessoa
// Module: ControleAcessoPortaria
// Data: 2025-12-30 21:42:59
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.ControleAcessoPortaria.CapBloqueiosPessoa;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.ControleAcessoPortaria.CapBloqueiosPessoa;

/// <summary>
/// Interface do serviço de API para CapBloqueiosPessoa.
/// Herda de IApiService (já implementado por BaseApiService) e IBatchDeleteService.
/// v5.2: Compatível com BaseApiService genérico.
/// v4.1: Adiciona ToggleAtivoAsync para alternar status dinamicamente.
/// </summary>
public interface ICapBloqueiosPessoaApiService 
    : IApiService<CapBloqueiosPessoaDto, CreateCapBloqueiosPessoaRequest, UpdateCapBloqueiosPessoaRequest, int>,
      IBatchDeleteService<int>
{

    /// <summary>
    /// Alterna o status Ativo/Desativo de um registro.
    /// </summary>
    Task ToggleAtivoAsync(int id, bool ativo, CancellationToken ct = default);
}
