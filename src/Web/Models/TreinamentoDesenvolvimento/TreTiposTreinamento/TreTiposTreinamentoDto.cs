// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: TreTiposTreinamento
// Module: TreinamentoDesenvolvimento
// Data: 2026-02-16 23:23:32
// =============================================================================

namespace RhSensoERP.Web.Models.TreinamentoDesenvolvimento.TreTiposTreinamento;

/// <summary>
/// DTO de leitura para Cadastro de Tipo de Treinamento.
/// Compatível com backend: RhSensoERP.Modules.TreinamentoDesenvolvimento.Application.DTOs.TreTiposTreinamentoDto
/// </summary>
public class TreTiposTreinamentoDto
{
    /// <summary>
    /// ID
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
    /// Código NR
    /// </summary>
    public string CodigoNr { get; set; } = string.Empty;

    /// <summary>
    /// Validade (dias)
    /// </summary>
    public int? DiasPrazoValidade { get; set; }

    /// <summary>
    /// Obrigatório
    /// </summary>
    public bool Obrigatorio { get; set; }

    /// <summary>
    /// Aplicável a
    /// </summary>
    public string AplicavelA { get; set; } = string.Empty;

    /// <summary>
    /// Carga horária (h)
    /// </summary>
    public int? CargaHoraria { get; set; }

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
