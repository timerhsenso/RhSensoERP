using RhSensoWeb.Models.SEG;
using RhSensoWeb.Models.Shared;

namespace RhSensoWeb.Services.Interfaces;

/// <summary>
/// Interface para serviços de usuários com a API
/// </summary>
public interface IUsuarioApiService
{
    /// <summary>
    /// Obtém lista paginada de usuários
    /// </summary>
    /// <param name="request">Parâmetros de paginação</param>
    /// <param name="filtros">Filtros de pesquisa</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de usuários</returns>
    Task<DataTablesResponse<UsuarioListDto>> GetUsuariosAsync(
        DataTablesRequest request, 
        UsuarioFiltroDto? filtros = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém usuário por código
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do usuário</returns>
    Task<ApiResponse<UsuarioDetalheDto>> GetUsuarioByIdAsync(
        string cdUsuario, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria novo usuário
    /// </summary>
    /// <param name="usuario">Dados do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<ApiResponse<UsuarioDetalheDto>> CreateUsuarioAsync(
        UsuarioViewModel usuario, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza usuário existente
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="usuario">Dados atualizados</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<ApiResponse<UsuarioDetalheDto>> UpdateUsuarioAsync(
        string cdUsuario, 
        UsuarioViewModel usuario, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Exclui usuário
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<ApiResponse> DeleteUsuarioAsync(
        string cdUsuario, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ativa/desativa usuário
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="ativo">Status ativo</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<ApiResponse> ToggleUsuarioStatusAsync(
        string cdUsuario, 
        bool ativo, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Altera senha do usuário
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="novaSenha">Nova senha</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    Task<ApiResponse> AlterarSenhaAsync(
        string cdUsuario, 
        string novaSenha, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém grupos disponíveis para usuários
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de grupos</returns>
    Task<ApiResponse<List<GrupoDto>>> GetGruposAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém empresas disponíveis
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de empresas</returns>
    Task<ApiResponse<List<EmpresaDto>>> GetEmpresasAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém filiais de uma empresa
    /// </summary>
    /// <param name="cdEmpresa">Código da empresa</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de filiais</returns>
    Task<ApiResponse<List<FilialDto>>> GetFiliaisAsync(
        string cdEmpresa, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// DTO para grupos de usuários
/// </summary>
public class GrupoDto
{
    public string CdGrUser { get; set; } = string.Empty;
    public string DcGrUser { get; set; } = string.Empty;
    public char FlAtivo { get; set; }
    public string? Descricao { get; set; }
}

/// <summary>
/// DTO para empresas
/// </summary>
public class EmpresaDto
{
    public string CdEmpresa { get; set; } = string.Empty;
    public string DcEmpresa { get; set; } = string.Empty;
    public string? CnpjEmpresa { get; set; }
    public char FlAtivo { get; set; }
}

/// <summary>
/// DTO para filiais
/// </summary>
public class FilialDto
{
    public string CdFilial { get; set; } = string.Empty;
    public string DcFilial { get; set; } = string.Empty;
    public string CdEmpresa { get; set; } = string.Empty;
    public string? CnpjFilial { get; set; }
    public char FlAtivo { get; set; }
}
