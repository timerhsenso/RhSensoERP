// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: GTP_TipoDocumentoFornecedor
// Module: GestaoTerceirosPrestadores
// Data: 2026-03-04 22:56:13
// =============================================================================

namespace RhSensoERP.Web.Models.GestaoTerceirosPrestadores.GTP_TipoDocumentoFornecedor;

/// <summary>
/// DTO de leitura para Tipo de Documento de Fornecedor.
/// Compatível com backend: RhSensoERP.Modules.GestaoTerceirosPrestadores.Application.DTOs.GTP_TipoDocumentoFornecedorDto
/// </summary>
public class GTP_TipoDocumentoFornecedorDto
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
    /// Código
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Categoria
    /// </summary>
    public string Categoria { get; set; } = string.Empty;

    /// <summary>
    /// Obrigatório
    /// </summary>
    public bool Obrigatorio { get; set; }

    /// <summary>
    /// Controla Vencimento
    /// </summary>
    public bool ControlVencimento { get; set; }

    /// <summary>
    /// Alertar (dias antes)
    /// </summary>
    public int? AlertarDiasAntes { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Ordem
    /// </summary>
    public int Ordem { get; set; }

    /// <summary>
    /// Created At Utc
    /// </summary>
    public DateTime? CreatedAtUtc { get; set; }

    /// <summary>
    /// Updated At Utc
    /// </summary>
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>
    /// Row Ver
    /// </summary>
    public byte[] RowVer { get; set; }
}
