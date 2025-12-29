// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: CapHistoricoBloqueios
// Module: ControleAcessoPortaria
// Data: 2025-12-28 20:51:29
// =============================================================================

namespace RhSensoERP.Web.Models.ControleAcessoPortaria.CapHistoricoBloqueios;

/// <summary>
/// DTO de leitura para CapHistoricoBloqueios.
/// Compativel com backend: RhSensoERP.Modules.ControleAcessoPortaria.Application.DTOs.CapHistoricoBloqueiosDto
/// </summary>
public class CapHistoricoBloqueiosDto
{
    /// <summary>
    /// Id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Tenant Id
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// IdBloqueio
    /// </summary>
    public int IdBloqueio { get; set; }

    /// <summary>
    /// ID Funcionário Legado
    /// </summary>
    public int? IdFuncionarioLegado { get; set; }

    /// <summary>
    /// ID Colaborador Fornecedor
    /// </summary>
    public int? IdColaboradorFornecedor { get; set; }

    /// <summary>
    /// ID Visitante
    /// </summary>
    public int? IdVisitante { get; set; }

    /// <summary>
    /// Motivo
    /// </summary>
    public string Motivo { get; set; } = string.Empty;

    /// <summary>
    /// DataBloqueio
    /// </summary>
    public DateTime? DataBloqueio { get; set; }

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
    /// Data Registro (UTC)
    /// </summary>
    public DateTime DataRegistro { get; set; }
}
