// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: TreTiposTreinamento
// Module: GestaoDePessoas
// Data: 2025-12-30 05:27:05
// =============================================================================

namespace RhSensoERP.Web.Models.GestaoDePessoas.TreTiposTreinamento;

/// <summary>
/// DTO de leitura para Tipos de Treinamento.
/// Compativel com backend: RhSensoERP.Modules.GestaoDePessoas.Application.DTOs.TreTiposTreinamentoDto
/// </summary>
public class TreTiposTreinamentoDto
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
    /// CodigoNr
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
