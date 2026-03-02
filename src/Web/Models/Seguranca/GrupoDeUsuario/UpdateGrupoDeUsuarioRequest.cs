// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: GrupoDeUsuario
// Module: Seguranca
// Data: 2026-03-01 22:11:05
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Seguranca.GrupoDeUsuario;

/// <summary>
/// Request para atualização de Grupo de Usuários.
/// Compatível com backend: UpdateGrupoDeUsuarioRequest
/// </summary>
public class UpdateGrupoDeUsuarioRequest
{
    /// <summary>
    /// Descrição de Gr User
    /// </summary>
    [Display(Name = "Descrição de Gr User")]
    [StringLength(60, ErrorMessage = "Descrição de Gr User deve ter no máximo {1} caracteres")]
    public string DcGrUser { get; set; } = string.Empty;
}
