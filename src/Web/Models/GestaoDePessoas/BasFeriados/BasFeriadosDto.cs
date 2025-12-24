// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: BasFeriados
// Module: GestaoDePessoas
// Data: 2025-12-22 23:31:27
// =============================================================================

namespace RhSensoERP.Web.Models.GestaoDePessoas.BasFeriados;

/// <summary>
/// DTO de leitura para BasFeriados.
/// Compatível com backend: RhSensoERP.Modules.GestaoDePessoas.Application.DTOs.BasFeriadosDto
/// </summary>
public class BasFeriadosDto
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
    /// Data
    /// </summary>
    public DateOnly Data { get; set; }

    /// <summary>
    /// Descricao
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Tipo
    /// </summary>
    public string Tipo { get; set; } = string.Empty;

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
