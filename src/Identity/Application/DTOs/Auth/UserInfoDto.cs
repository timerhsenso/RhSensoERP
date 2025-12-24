namespace RhSensoERP.Identity.Application.DTOs.Auth;

/// <summary>
/// Dados básicos do usuário autenticado.
/// ✅ ATUALIZADO: Incluído TenantId para multi-tenancy.
/// </summary>
public sealed record UserInfoDto
{
    /// <summary>ID único do usuário (Guid).</summary>
    public Guid Id { get; init; }

    /// <summary>Código do usuário (login).</summary>
    public string CdUsuario { get; init; } = string.Empty;

    /// <summary>Nome completo do usuário.</summary>
    public string DcUsuario { get; init; } = string.Empty;

    /// <summary>E-mail do usuário.</summary>
    public string? Email { get; init; }

    /// <summary>Matrícula do usuário.</summary>
    public string? NoMatric { get; init; }

    /// <summary>Código da empresa.</summary>
    public int? CdEmpresa { get; init; }

    /// <summary>Código da filial.</summary>
    public int? CdFilial { get; init; }

    /// <summary>
    /// ✅ NOVO: TenantId para isolamento multi-tenant.
    /// Obrigatório para tabelas com campo TenantId.
    /// </summary>
    public Guid? TenantId { get; init; }

    /// <summary>Indica se o usuário está ativo.</summary>
    public bool FlAtivo { get; init; }

    /// <summary>Tipo do usuário (ex: Admin, User, etc).</summary>
    public string? TpUsuario { get; init; }

    /// <summary>Indica se o usuário deve trocar a senha no próximo login.</summary>
    public bool MustChangePassword { get; init; }

    /// <summary>Indica se a autenticação de dois fatores está habilitada.</summary>
    public bool TwoFactorEnabled { get; init; }

    /// <summary>Indica se o e-mail foi confirmado.</summary>
    public bool EmailConfirmed { get; init; }
}