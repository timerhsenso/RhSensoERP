using RhSensoWeb.Models.SEG;
using RhSensoWeb.Models.Shared;

namespace RhSensoWeb.Services.Interfaces;

/// <summary>
/// Interface para serviços de usuários com a API
/// </summary>
public interface IUsuarioApiService
{
    Task<DataTablesResponse<UsuarioListDto>> GetUsuariosAsync(
        DataTablesRequest request,
        UsuarioFiltroDto? filtros = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<UsuarioDetalheDto>> GetUsuarioByIdAsync(
        string cdUsuario,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<UsuarioDetalheDto>> CreateUsuarioAsync(
        UsuarioViewModel usuario,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<UsuarioDetalheDto>> UpdateUsuarioAsync(
        string cdUsuario,
        UsuarioViewModel usuario,
        CancellationToken cancellationToken = default);

    Task<ApiResponse> DeleteUsuarioAsync(
        string cdUsuario,
        CancellationToken cancellationToken = default);

    Task<ApiResponse> ToggleUsuarioStatusAsync(
        string cdUsuario,
        bool ativo,
        CancellationToken cancellationToken = default);

    Task<ApiResponse> AlterarSenhaAsync(
        string cdUsuario,
        string novaSenha,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<List<GrupoDto>>> GetGruposAsync(
        CancellationToken cancellationToken = default);

    Task<ApiResponse<List<EmpresaDto>>> GetEmpresasAsync(
        CancellationToken cancellationToken = default);

    // Inteiro aqui:
    Task<ApiResponse<List<FilialDto>>> GetFiliaisAsync(
        int cdEmpresa,
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
    public int CdEmpresa { get; set; }
    public string DcEmpresa { get; set; } = string.Empty;
    public string? CnpjEmpresa { get; set; }
    public char FlAtivo { get; set; }
}

/// <summary>
/// DTO para filiais
/// </summary>
public class FilialDto
{
    public int CdFilial { get; set; }
    public string DcFilial { get; set; } = string.Empty;
    public int CdEmpresa { get; set; }
    public string? CnpjFilial { get; set; }
    public char FlAtivo { get; set; }
}
