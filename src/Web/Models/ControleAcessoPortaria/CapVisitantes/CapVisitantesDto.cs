// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: CapVisitantes
// Module: ControleAcessoPortaria
// Data: 2026-02-16 23:34:17
// =============================================================================

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapVisitantes;

/// <summary>
/// DTO de leitura para teste1.
/// Compatível com backend: RhSensoERP.Modules.ControleAcessoPortaria.Application.DTOs.CapVisitantesDto
/// </summary>
public class CapVisitantesDto
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
    /// Nome
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// CPF
    /// </summary>
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// RG
    /// </summary>
    public string Rg { get; set; } = string.Empty;

    /// <summary>
    /// E-mail
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Telefone
    /// </summary>
    public string Telefone { get; set; } = string.Empty;

    /// <summary>
    /// Empresa
    /// </summary>
    public string Empresa { get; set; } = string.Empty;

    /// <summary>
    /// ID Funcionário Responsável
    /// </summary>
    public int? IdFuncionarioResponsavel { get; set; }

    /// <summary>
    /// Requer Responsável
    /// </summary>
    public bool RequerResponsavel { get; set; }

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
