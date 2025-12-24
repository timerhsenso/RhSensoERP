namespace RhSensoERP.Web.Models.Account;

/// <summary>
/// ViewModel para o Dashboard do usuário.
/// </summary>
public sealed class DashboardViewModel
{
    /// <summary>
    /// Informações do usuário autenticado.
    /// </summary>
    public UserInfoViewModel UserInfo { get; set; } = new();

    /// <summary>
    /// Permissões do usuário.
    /// </summary>
    public UserPermissionsViewModel Permissions { get; set; } = new();

    /// <summary>
    /// Indica se houve erro ao carregar as permissões.
    /// </summary>
    public bool HasPermissionsError { get; set; }

    /// <summary>
    /// Mensagem de erro, se houver.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
