// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: HabilitacaoGrupo
// Module: Seguranca
// Data: 2026-03-02 17:57:18
// =============================================================================

namespace RhSensoERP.Web.Models.Seguranca.HabilitacaoGrupo;

/// <summary>
/// DTO de leitura para Habilitação de Grupo.
/// Compatível com backend: RhSensoERP.Modules.Seguranca.Application.DTOs.HabilitacaoGrupoDto
/// </summary>
public class HabilitacaoGrupoDto
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Tenant Id
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Código de Gr User
    /// </summary>
    public string CdGrUser { get; set; } = string.Empty;

    /// <summary>
    /// Código de Funcao
    /// </summary>
    public string CdFuncao { get; set; } = string.Empty;

    /// <summary>
    /// Código de Sistema
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Código de Acoes
    /// </summary>
    public string CdAcoes { get; set; } = string.Empty;

    /// <summary>
    /// Código de Restric
    /// </summary>
    public string CdRestric { get; set; } = string.Empty;

    /// <summary>
    /// Gr Upodeusuario
    /// </summary>
    public Guid? IdGrUpodeusuario { get; set; }
}
