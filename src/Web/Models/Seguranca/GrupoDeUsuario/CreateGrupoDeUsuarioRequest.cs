// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: GrupoDeUsuario
// Module: Seguranca
// Data: 2026-03-02 17:55:27
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Seguranca.GrupoDeUsuario;

/// <summary>
/// Request para criação de Grupos de Usuário.
/// Compatível com backend: CreateGrupoDeUsuarioRequest
/// </summary>
public class CreateGrupoDeUsuarioRequest
{
    /// <summary>
    /// Código de Sistema
    /// (Chave Primária - obrigatório na criação)
    /// </summary>
    [Display(Name = "Código de Sistema")]
    [Required(ErrorMessage = "Código de Sistema é obrigatório")]
    [StringLength(10, ErrorMessage = "Código de Sistema deve ter no máximo {1} caracteres")]
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Código de Gr User
    /// (Chave Primária - obrigatório na criação)
    /// </summary>
    [Display(Name = "Código de Gr User")]
    [Required(ErrorMessage = "Código de Gr User é obrigatório")]
    [StringLength(30, ErrorMessage = "Código de Gr User deve ter no máximo {1} caracteres")]
    public string CdGrUser { get; set; } = string.Empty;

    /// <summary>
    /// Descrição de Gr User
    /// </summary>
    [Display(Name = "Descrição de Gr User")]
    [StringLength(60, ErrorMessage = "Descrição de Gr User deve ter no máximo {1} caracteres")]
    public string DcGrUser { get; set; } = string.Empty;
}
