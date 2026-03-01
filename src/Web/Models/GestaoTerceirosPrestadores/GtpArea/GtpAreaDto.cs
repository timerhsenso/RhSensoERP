// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: GtpArea
// Module: GestaoTerceirosPrestadores
// Data: 2026-02-28 22:25:01
// =============================================================================

namespace RhSensoERP.Web.Models.GestaoTerceirosPrestadores.GtpArea;

/// <summary>
/// DTO de leitura para Áreas.
/// Compatível com backend: RhSensoERP.Modules.GestaoTerceirosPrestadores.Application.DTOs.GtpAreaDto
/// </summary>
public class GtpAreaDto
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
    /// Unidade
    /// </summary>
    public int IdUnidade { get; set; }

    /// <summary>
    /// Código
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Nome
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Grau de Risco
    /// </summary>
    public int? GrauRisco { get; set; }

    /// <summary>
    /// NRs Exigidas
    /// </summary>
    public string ExigeNR { get; set; } = string.Empty;

    /// <summary>
    /// Responsável
    /// </summary>
    public string ResponsavelArea { get; set; } = string.Empty;

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

    /// <summary>
    /// Versão do Registro
    /// </summary>
    public byte[] RowVer { get; set; }
}
