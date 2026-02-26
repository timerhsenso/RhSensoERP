// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: SHR_CNAE
// Module: GestaoTerceirosPrestadores
// Data: 2026-02-25 11:14:57
// =============================================================================

namespace RhSensoERP.Web.Models.GestaoTerceirosPrestadores.SHR_CNAE;

/// <summary>
/// DTO de leitura para Tabela de CNAE.
/// Compatível com backend: RhSensoERP.Modules.GestaoTerceirosPrestadores.Application.DTOs.SHR_CNAEDto
/// </summary>
public class SHR_CNAEDto
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Código CNAE
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Código Limpo
    /// </summary>
    public string CodigoLimpo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Seção
    /// </summary>
    public string Secao { get; set; } = string.Empty;

    /// <summary>
    /// Divisão
    /// </summary>
    public string Divisao { get; set; } = string.Empty;

    /// <summary>
    /// Grupo
    /// </summary>
    public string Grupo { get; set; } = string.Empty;

    /// <summary>
    /// Classe
    /// </summary>
    public string Classe { get; set; } = string.Empty;

    /// <summary>
    /// Grau de Risco
    /// </summary>
    public int? GrauRisco { get; set; }

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
