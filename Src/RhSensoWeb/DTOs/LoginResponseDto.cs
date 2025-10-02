/*
using System.Text.Json.Serialization;

namespace RhSensoWeb.DTOs;

/// <summary>
/// Resposta da API de autenticação
/// </summary>
public class LoginResponseDto
{
    [JsonPropertyName("accessToken")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("refreshToken")]
    public string? RefreshToken { get; set; }

    [JsonPropertyName("expiresIn")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("userData")]
    public UserDataDto? UserData { get; set; }
}

/// <summary>
/// Dados do usuário autenticado
/// </summary>
public class UserDataDto
{
    [JsonPropertyName("cdUsuario")]
    public string? CdUsuario { get; set; }

    [JsonPropertyName("dcUsuario")]
    public string? DcUsuario { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Código da empresa (inteiro)
    /// </summary>
    [JsonPropertyName("cdEmpresa")]
    public int CdEmpresa { get; set; }

    /// <summary>
    /// Código da filial (inteiro)
    /// </summary>
    [JsonPropertyName("cdFilial")]
    public int CdFilial { get; set; }

    [JsonPropertyName("idFuncionario")]
    public int? IdFuncionario { get; set; }

    [JsonPropertyName("grupos")]
    public List<string>? Grupos { get; set; }

    [JsonPropertyName("permissoes")]
    public List<PermissaoDto>? Permissoes { get; set; }
}

/// <summary>
/// Permissão do usuário
/// </summary>
public class PermissaoDto
{
    [JsonPropertyName("cdSistema")]
    public string? CdSistema { get; set; }

    [JsonPropertyName("cdGrUser")]
    public string? CdGrUser { get; set; }

    [JsonPropertyName("cdFuncao")]
    public string? CdFuncao { get; set; }

    [JsonPropertyName("cdAcoes")]
    public string? CdAcoes { get; set; }

    [JsonPropertyName("cdRestric")]
    public string? CdRestric { get; set; }
}

*/