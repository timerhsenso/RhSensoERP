// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: CapResponsaveisContrato
// Module: ControleAcessoPortaria
// Data: 2025-12-24 01:15:50
// =============================================================================

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapResponsaveisContrato;

/// <summary>
/// DTO de leitura para CapResponsaveisContrato.
/// Compatível com backend: RhSensoERP.Modules.ControleAcessoPortaria.Application.DTOs.CapResponsaveisContratoDto
/// </summary>
public class CapResponsaveisContratoDto
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tenant ID
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
