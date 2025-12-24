// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.6
// Entity: CapContratosFornecedor
// Module: ControleAcessoPortaria
// Data: 2025-12-24 01:21:44
// =============================================================================

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapContratosFornecedor;

/// <summary>
/// DTO de leitura para CapContratosFornecedor.
/// Compatível com backend: RhSensoERP.Modules.ControleAcessoPortaria.Application.DTOs.CapContratosFornecedorDto
/// </summary>
public class CapContratosFornecedorDto
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
    /// IdFornecedor
    /// </summary>
    public int IdFornecedor { get; set; }

    /// <summary>
    /// NumeroContrato
    /// </summary>
    public string NumeroContrato { get; set; } = string.Empty;

    /// <summary>
    /// DataInicio
    /// </summary>
    public DateOnly DataInicio { get; set; }

    /// <summary>
    /// DataFim
    /// </summary>
    public DateOnly? DataFim { get; set; }

    /// <summary>
    /// Valor
    /// </summary>
    public decimal? Valor { get; set; }

    /// <summary>
    /// Descrição
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Data Criação (UTC)
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Criado Por
    /// </summary>
    public Guid? CreatedByUserId { get; set; }

    /// <summary>
    /// Data Atualização (UTC)
    /// </summary>
    public DateTime UpdatedAtUtc { get; set; }

    /// <summary>
    /// Atualizado Por
    /// </summary>
    public Guid? UpdatedByUserId { get; set; }
}
