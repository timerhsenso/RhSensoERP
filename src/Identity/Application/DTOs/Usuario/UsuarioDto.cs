namespace RhSensoERP.Identity.Application.DTOs.Usuario;

public sealed class UsuarioDto
{
    public string CdUsuario { get; init; } = string.Empty;
    public string DcUsuario { get; init; } = string.Empty;
    public string? Email { get; init; }
    public char FlAtivo { get; init; }
    public string? NoMatric { get; init; }
    public int? CdEmpresa { get; init; }
    public int? CdFilial { get; init; }
    public bool EmailConfirmed { get; init; }
    public bool TwoFactorEnabled { get; init; }
    public DateTime? LastLoginAt { get; init; }
}
