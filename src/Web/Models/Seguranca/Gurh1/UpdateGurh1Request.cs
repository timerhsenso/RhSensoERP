// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Gurh1
// Module: Seguranca
// Data: 2026-02-28 09:59:07
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Seguranca.Gurh1;

/// <summary>
/// Request para atualização de Grupo de Usuários.
/// Compatível com backend: UpdateGurh1Request
/// </summary>
public class UpdateGurh1Request
{
    /// <summary>
    /// Descrição de Gr User
    /// </summary>
    [Display(Name = "Descrição de Gr User")]
    [StringLength(60, ErrorMessage = "Descrição de Gr User deve ter no máximo {1} caracteres")]
    public string Dcgruser { get; set; } = string.Empty;
}
