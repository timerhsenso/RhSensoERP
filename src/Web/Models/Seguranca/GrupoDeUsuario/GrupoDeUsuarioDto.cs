// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: GrupoDeUsuario
// Module: Seguranca
// Data: 2026-03-02 17:55:27
// =============================================================================

namespace RhSensoERP.Web.Models.Seguranca.GrupoDeUsuario;

/// <summary>
/// DTO de leitura para Grupos de Usuário.
/// Compatível com backend: RhSensoERP.Modules.Seguranca.Application.DTOs.GrupoDeUsuarioDto
/// </summary>
public class GrupoDeUsuarioDto
{
    /// <summary>
    /// Código de Sistema
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Código de Gr User
    /// </summary>
    public string CdGrUser { get; set; } = string.Empty;

    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Tenant Id
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Descrição de Gr User
    /// </summary>
    public string DcGrUser { get; set; } = string.Empty;
}
