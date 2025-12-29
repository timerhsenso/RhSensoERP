// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapResponsaveisContrato
// Module: ControleAcessoPortaria
// Data: 2025-12-28 20:29:17
// =============================================================================

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapResponsaveisContrato;

/// <summary>
/// DTO de leitura para CapResponsaveisContrato.
/// Compativel com backend: RhSensoERP.Modules.ControleAcessoPortaria.Application.DTOs.CapResponsaveisContratoDto
/// </summary>
public class CapResponsaveisContratoDto
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tenant Id
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// IdContrato
    /// </summary>
    public int IdContrato { get; set; }

    /// <summary>
    /// IdFuncionarioLegado
    /// </summary>
    public int? IdFuncionarioLegado { get; set; }

    /// <summary>
    /// TipoResponsabilidade
    /// </summary>
    public string TipoResponsabilidade { get; set; } = string.Empty;

    /// <summary>
    /// DataInicio
    /// </summary>
    public DateOnly? DataInicio { get; set; }

    /// <summary>
    /// DataFim
    /// </summary>
    public DateOnly? DataFim { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Created At Utc
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Created By User Id
    /// </summary>
    public Guid? CreatedByUserId { get; set; }

    /// <summary>
    /// Updated At Utc
    /// </summary>
    public DateTime UpdatedAtUtc { get; set; }

    /// <summary>
    /// Updated By User Id
    /// </summary>
    public Guid? UpdatedByUserId { get; set; }
}
