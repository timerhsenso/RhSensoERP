// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: GTP_Area
// Module: GestaoTerceirosPrestadores
// Data: 2026-02-24 21:34:17
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.GestaoTerceirosPrestadores.GTP_Area;

/// <summary>
/// Request para atualização de Cadastro de Terceiros teste.
/// Compatível com backend: UpdateGTP_AreaRequest
/// </summary>
public class UpdateGTP_AreaRequest
{
    /// <summary>
    /// Unidade
    /// </summary>
    [Display(Name = "Unidade")]
    [Required(ErrorMessage = "Unidade é obrigatório")]
    public int IdUnidade { get; set; }

    /// <summary>
    /// Código
    /// </summary>
    [Display(Name = "Código")]
    [Required(ErrorMessage = "Código é obrigatório")]
    [StringLength(20, ErrorMessage = "Código deve ter no máximo {1} caracteres")]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Nome
    /// </summary>
    [Display(Name = "Nome")]
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(150, ErrorMessage = "Nome deve ter no máximo {1} caracteres")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Grau de Risco
    /// </summary>
    [Display(Name = "Grau de Risco")]
    public int? GrauRisco { get; set; }

    /// <summary>
    /// NRs Exigidas
    /// </summary>
    [Display(Name = "NRs Exigidas")]
    [StringLength(100, ErrorMessage = "NRs Exigidas deve ter no máximo {1} caracteres")]
    public string ExigeNR { get; set; } = string.Empty;

    /// <summary>
    /// Responsável
    /// </summary>
    [Display(Name = "Responsável")]
    [StringLength(150, ErrorMessage = "Responsável deve ter no máximo {1} caracteres")]
    public string ResponsavelArea { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo é obrigatório")]
    public bool Ativo { get; set; }

    /// <summary>
    /// Row Ver
    /// </summary>
    [Display(Name = "Row Ver")]
    [Required(ErrorMessage = "Row Ver é obrigatório")]
    public byte[] RowVer { get; set; }
}
