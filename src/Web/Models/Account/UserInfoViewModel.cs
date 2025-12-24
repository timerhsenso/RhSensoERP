namespace RhSensoERP.Web.Models.Account;

/// <summary>
/// ViewModel com informações do usuário autenticado.
/// </summary>
public sealed class UserInfoViewModel
{
    public Guid Id { get; set; }
    public string CdUsuario { get; set; } = string.Empty;
    public string DcUsuario { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? NoMatric { get; set; }
    public int? CdEmpresa { get; set; }
    public int? CdFilial { get; set; }
    public Guid? TenantId { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool MustChangePassword { get; set; }
}