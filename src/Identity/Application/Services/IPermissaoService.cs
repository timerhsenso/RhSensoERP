// ============================================================================
// ARQUIVO ATUALIZADO - FASE 3: src/Identity/Application/Services/IPermissaoService.cs
// ============================================================================

using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Identity.Application.DTOs.Permissoes;

namespace RhSensoERP.Identity.Application.Services;

/// <summary>
/// Serviço para carregamento de permissões do sistema legado.
/// Responsável por buscar grupos, funções e botões do usuário.
/// </summary>
public interface IPermissaoService
{
    /// <summary>
    /// Carrega todas as permissões do usuário (grupos, funções, botões).
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="cdSistema">Código do sistema (opcional, null = todos os sistemas)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>DTO com todas as permissões do usuário</returns>
    Task<UserPermissionsDto> CarregarPermissoesAsync(
        string cdUsuario, 
        string? cdSistema = null, 
        CancellationToken ct = default);

    /// <summary>
    /// Verifica se o usuário tem permissão para uma ação específica em uma função.
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="cdFuncao">Código da função</param>
    /// <param name="acao">Ação desejada (I=Incluir, A=Alterar, E=Excluir, C=Consultar)</param>
    /// <param name="cdSistema">Código do sistema (opcional)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>True se o usuário tem a permissão</returns>
    Task<bool> TemPermissaoAsync(
        string cdUsuario, 
        string cdFuncao, 
        char acao, 
        string? cdSistema = null, 
        CancellationToken ct = default);

    /// <summary>
    /// Valida permissão específica e retorna informações detalhadas.
    /// Este método é mais completo que TemPermissaoAsync, retornando
    /// não apenas se tem permissão, mas também o motivo da negação e
    /// quais ações estão disponíveis.
    /// </summary>
    /// <param name="request">Request com dados da validação</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Response detalhado com resultado da validação</returns>
    Task<ValidarPermissaoResponse> ValidarPermissaoAsync(
        ValidarPermissaoRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Obtém lista de funções permitidas para o usuário.
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="cdSistema">Código do sistema (opcional)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Lista de códigos de funções permitidas</returns>
    Task<List<string>> ObterFuncoesPermitidasAsync(
        string cdUsuario,
        string? cdSistema = null,
        CancellationToken ct = default);

    /// <summary>
    /// Obtém lista de botões permitidos para o usuário em uma função específica.
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="cdFuncao">Código da função</param>
    /// <param name="cdSistema">Código do sistema (opcional)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Lista de códigos de botões permitidos</returns>
    Task<List<string>> ObterBotoesPermitidosAsync(
        string cdUsuario, 
        string cdFuncao, 
        string? cdSistema = null, 
        CancellationToken ct = default);

    /// <summary>
    /// Habilita ou desabilita uma ação específica para o grupo do usuário em uma função.
    /// </summary>
    /// <param name="request">Request com dados do toggle</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Response com resultado da operação</returns>
    Task<TogglePermissaoResponse> TogglePermissaoAsync(
        TogglePermissaoRequest request,
        CancellationToken ct = default);
}
