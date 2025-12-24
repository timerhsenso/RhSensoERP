namespace RhSensoERP.Shared.Core.Abstractions;

/// <summary>
/// Interface para obter informações do usuário autenticado.
/// ✅ ATUALIZADO: Incluído TenantId para multi-tenancy.
/// </summary>
public interface ICurrentUser
{
    /// <summary>ID do usuário autenticado (Guid).</summary>
    Guid? UserId { get; }

    /// <summary>Nome/login do usuário autenticado.</summary>
    string? UserName { get; }

    /// <summary>
    /// ✅ NOVO: TenantId do usuário para isolamento multi-tenant.
    /// Retorna Guid.Empty se não estiver autenticado ou TenantId não estiver presente.
    /// </summary>
    Guid TenantId { get; }
}