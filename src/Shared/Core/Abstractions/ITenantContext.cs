// RhSensoERP.Shared.Core — ITenantContext
// Finalidade: Expor contexto do tenant (multi-empresa/filial).
// Uso: Injetar em serviços/handlers para obter Tenant/Empresa/Filial correntes.

namespace RhSensoERP.Shared.Core.Abstractions;

/// <summary>
/// Contexto mínimo de multi-tenant para ser consumido pela aplicação.
/// </summary>
public interface ITenantContext
{
    /// <summary>Identificador lógico do tenant (SaaS).</summary>
    string? TenantId { get; }

    /// <summary>Identificador da empresa (quando aplicável).</summary>
    int? EmpresaId { get; }

    /// <summary>Identificador da filial (quando aplicável).</summary>
    int? FilialId { get; }
}
