// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapOcorrencias
// Module: ControleAcessoPortaria
// Data: 2025-12-30 21:29:17
// =============================================================================

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapOcorrencias;

/// <summary>
/// DTO de leitura para CapOcorrencias.
/// Compativel com backend: RhSensoERP.Modules.ControleAcessoPortaria.Application.DTOs.CapOcorrenciasDto
/// </summary>
public class CapOcorrenciasDto
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
    /// IdTipoOcorrencia
    /// </summary>
    public int IdTipoOcorrencia { get; set; }

    /// <summary>
    /// IdAcessoPortaria
    /// </summary>
    public int? IdAcessoPortaria { get; set; }

    /// <summary>
    /// IdFuncionarioResponsavel
    /// </summary>
    public int? IdFuncionarioResponsavel { get; set; }

    /// <summary>
    /// IdColaboradorResponsavel
    /// </summary>
    public int? IdColaboradorResponsavel { get; set; }

    /// <summary>
    /// Descricao
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// DataOcorrencia
    /// </summary>
    public DateTime DataOcorrencia { get; set; }

    /// <summary>
    /// Local
    /// </summary>
    public string Local { get; set; } = string.Empty;

    /// <summary>
    /// Observacoes
    /// </summary>
    public string Observacoes { get; set; } = string.Empty;

    /// <summary>
    /// Status
    /// </summary>
    public string Status { get; set; } = string.Empty;

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
