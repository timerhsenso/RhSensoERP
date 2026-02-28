// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Gurh1
// Module: Seguranca
// Data: 2026-02-28 09:59:07
// =============================================================================

namespace RhSensoERP.Web.Models.Seguranca.Gurh1;

/// <summary>
/// DTO de leitura para Grupo de Usuários.
/// Compatível com backend: RhSensoERP.Modules.Seguranca.Application.DTOs.Gurh1Dto
/// </summary>
public class Gurh1Dto
{
    /// <summary>
    /// Código de Sistema
    /// </summary>
    public string CdsiStema { get; set; } = string.Empty;

    /// <summary>
    /// Código de Gr User
    /// </summary>
    public string Cdgruser { get; set; } = string.Empty;

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
    public string Dcgruser { get; set; } = string.Empty;
}
