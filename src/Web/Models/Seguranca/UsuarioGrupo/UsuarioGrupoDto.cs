// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: UsuarioGrupo
// Module: Seguranca
// Data: 2026-03-05 01:06:29
// =============================================================================

namespace RhSensoERP.Web.Models.Seguranca.UsuarioGrupo;

/// <summary>
/// DTO de leitura para Usuário-Grupo.
/// Compatível com backend: RhSensoERP.Modules.Seguranca.Application.DTOs.UsuarioGrupoDto
/// </summary>
public class UsuarioGrupoDto
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
    /// Código de Usuario
    /// </summary>
    public string CdUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Código de Gr User
    /// </summary>
    public string CdGrUser { get; set; } = string.Empty;

    /// <summary>
    /// Código de Sistema
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Dt Ini Val
    /// </summary>
    public DateTime DtInival { get; set; }

    /// <summary>
    /// Dt Fim Val
    /// </summary>
    public DateTime? DtFimval { get; set; }

    /// <summary>
    /// Usuario
    /// </summary>
    public Guid? IdUsuario { get; set; }

    /// <summary>
    /// Gr Upodeusuario
    /// </summary>
    public Guid? IdGrUpodeusuario { get; set; }
}
