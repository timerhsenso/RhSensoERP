// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: CapTiposOcorrencia
// Module: ControleAcessoPortaria
// Data: 2026-01-07 21:19:53
// =============================================================================

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapTiposOcorrencia;

/// <summary>
/// DTO de leitura para Cadastro de Tipo de Ocorrência.
/// Compatível com backend: RhSensoERP.Modules.ControleAcessoPortaria.Application.DTOs.CapTiposOcorrenciaDto
/// </summary>
public class CapTiposOcorrenciaDto
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
    /// Nome
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Severidade
    /// </summary>
    public string Severidade { get; set; } = string.Empty;

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
