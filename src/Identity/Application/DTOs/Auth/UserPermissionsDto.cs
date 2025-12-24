// ============================================================================
// ARQUIVO NOVO - FASE 2: src/Identity/Application/DTOs/Auth/UserPermissionsDto.cs
// ============================================================================

namespace RhSensoERP.Identity.Application.DTOs.Auth;

/// <summary>
/// DTO contendo todas as permissões do usuário.
/// </summary>
public sealed class UserPermissionsDto
{
    /// <summary>
    /// Lista de grupos do usuário.
    /// </summary>
    public List<UserGroupDto> Grupos { get; set; } = new();

    /// <summary>
    /// Lista de funções (telas/módulos) que o usuário tem acesso.
    /// </summary>
    public List<UserFuncaoDto> Funcoes { get; set; } = new();

    /// <summary>
    /// Lista de botões permitidos por função.
    /// </summary>
    public List<UserBotaoDto> Botoes { get; set; } = new();

    /// <summary>
    /// Permissões serializadas para incluir nos claims do JWT.
    /// Formato: "funcao:acoes" (ex: "CADUSER:IAEC")
    /// </summary>
    public List<string> PermissionsForClaims { get; set; } = new();
}

/// <summary>
/// DTO de grupo do usuário.
/// </summary>
public sealed class UserGroupDto
{
    public string CdGrUser { get; set; } = string.Empty;
    public string? DcGrUser { get; set; }
    public string CdSistema { get; set; } = string.Empty;
}

/// <summary>
/// DTO de função (tela/módulo) com permissões.
/// </summary>
public sealed class UserFuncaoDto
{
    public string CdFuncao { get; set; } = string.Empty;
    public string? DcFuncao { get; set; }
    public string CdSistema { get; set; } = string.Empty;
    public string CdAcoes { get; set; } = string.Empty; // Ex: "IAEC"
    public char CdRestric { get; set; }

    /// <summary>
    /// Indica se o usuário pode Incluir nesta função.
    /// </summary>
    public bool PodeIncluir => CdAcoes.Contains('I');

    /// <summary>
    /// Indica se o usuário pode Alterar nesta função.
    /// </summary>
    public bool PodeAlterar => CdAcoes.Contains('A');

    /// <summary>
    /// Indica se o usuário pode Excluir nesta função.
    /// </summary>
    public bool PodeExcluir => CdAcoes.Contains('E');

    /// <summary>
    /// Indica se o usuário pode Consultar nesta função.
    /// </summary>
    public bool PodeConsultar => CdAcoes.Contains('C');
}

/// <summary>
/// DTO de botão permitido.
/// </summary>
public sealed class UserBotaoDto
{
    public string CdFuncao { get; set; } = string.Empty;
    public string CdBotao { get; set; } = string.Empty;
    public string? DcBotao { get; set; }
    // FlAtivo removido - não existe em BotaoFuncao
}
