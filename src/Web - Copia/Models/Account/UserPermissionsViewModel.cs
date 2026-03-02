namespace RhSensoERP.Web.Models.Account;

/// <summary>
/// ViewModel contendo todas as permissões do usuário.
/// </summary>
public sealed class UserPermissionsViewModel
{
    /// <summary>
    /// Lista de grupos do usuário.
    /// </summary>
    public List<UserGroupViewModel> Grupos { get; set; } = new();

    /// <summary>
    /// Lista de funções (telas/módulos) que o usuário tem acesso.
    /// </summary>
    public List<UserFuncaoViewModel> Funcoes { get; set; } = new();

    /// <summary>
    /// Lista de botões permitidos por função.
    /// </summary>
    public List<UserBotaoViewModel> Botoes { get; set; } = new();

    /// <summary>
    /// Permissões serializadas para incluir nos claims do JWT.
    /// Formato: "funcao:acoes" (ex: "CADUSER:IAEC")
    /// </summary>
    public List<string> PermissionsForClaims { get; set; } = new();
}

/// <summary>
/// ViewModel de grupo do usuário.
/// </summary>
public sealed class UserGroupViewModel
{
    public string CdGrUser { get; set; } = string.Empty;
    public string? DcGrUser { get; set; }
    public string CdSistema { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel de função (tela/módulo) com permissões.
/// </summary>
public sealed class UserFuncaoViewModel
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
/// ViewModel de botão permitido.
/// </summary>
public sealed class UserBotaoViewModel
{
    public string CdFuncao { get; set; } = string.Empty;
    public string CdBotao { get; set; } = string.Empty;
    public string? DcBotao { get; set; }
}
