// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapBloqueiosPessoa
// Module: ControleAcessoPortaria
// Data: 2025-12-30 21:42:59
// =============================================================================

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapBloqueiosPessoa;

/// <summary>
/// DTO de leitura para CapBloqueiosPessoa.
/// Compativel com backend: RhSensoERP.Modules.ControleAcessoPortaria.Application.DTOs.CapBloqueiosPessoaDto
/// </summary>
public class CapBloqueiosPessoaDto
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
    /// IdFuncionarioLegado
    /// </summary>
    public int? IdFuncionarioLegado { get; set; }

    /// <summary>
    /// ID Colaborador Fornecedor
    /// </summary>
    public int? IdColaboradorFornecedor { get; set; }

    /// <summary>
    /// IdVisitante
    /// </summary>
    public int? IdVisitante { get; set; }

    /// <summary>
    /// Motivo
    /// </summary>
    public string Motivo { get; set; } = string.Empty;

    /// <summary>
    /// DataBloqueio
    /// </summary>
    public DateTime DataBloqueio { get; set; }

    /// <summary>
    /// Data Desbloqueio (UTC)
    /// </summary>
    public DateTime? DataDesbloqueio { get; set; }

    /// <summary>
    /// Usuário Bloqueio
    /// </summary>
    public Guid? UsuarioBloqueio { get; set; }

    /// <summary>
    /// Usuário Desbloqueio
    /// </summary>
    public Guid? UsuarioDesbloqueio { get; set; }

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
