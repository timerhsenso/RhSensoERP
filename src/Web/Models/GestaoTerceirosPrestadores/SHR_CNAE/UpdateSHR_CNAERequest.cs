// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: SHR_CNAE
// Module: GestaoTerceirosPrestadores
// Data: 2026-02-25 11:14:57
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.GestaoTerceirosPrestadores.SHR_CNAE;

/// <summary>
/// Request para atualização de Tabela de CNAE.
/// Compatível com backend: UpdateSHR_CNAERequest
/// </summary>
public class UpdateSHR_CNAERequest
{
    /// <summary>
    /// Código CNAE
    /// </summary>
    [Display(Name = "Código CNAE")]
    [Required(ErrorMessage = "Código CNAE é obrigatório")]
    [StringLength(10, ErrorMessage = "Código CNAE deve ter no máximo {1} caracteres")]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Código Limpo
    /// </summary>
    [Display(Name = "Código Limpo")]
    [Required(ErrorMessage = "Código Limpo é obrigatório")]
    [StringLength(7, ErrorMessage = "Código Limpo deve ter no máximo {1} caracteres")]
    public string CodigoLimpo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [Required(ErrorMessage = "Descrição é obrigatório")]
    [StringLength(300, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Seção
    /// </summary>
    [Display(Name = "Seção")]
    [StringLength(1, ErrorMessage = "Seção deve ter no máximo {1} caracteres")]
    public string Secao { get; set; } = string.Empty;

    /// <summary>
    /// Divisão
    /// </summary>
    [Display(Name = "Divisão")]
    [StringLength(2, ErrorMessage = "Divisão deve ter no máximo {1} caracteres")]
    public string Divisao { get; set; } = string.Empty;

    /// <summary>
    /// Grupo
    /// </summary>
    [Display(Name = "Grupo")]
    [StringLength(3, ErrorMessage = "Grupo deve ter no máximo {1} caracteres")]
    public string Grupo { get; set; } = string.Empty;

    /// <summary>
    /// Classe
    /// </summary>
    [Display(Name = "Classe")]
    [StringLength(5, ErrorMessage = "Classe deve ter no máximo {1} caracteres")]
    public string Classe { get; set; } = string.Empty;

    /// <summary>
    /// Grau de Risco
    /// </summary>
    [Display(Name = "Grau de Risco")]
    public int? GrauRisco { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo é obrigatório")]
    public bool Ativo { get; set; }

    /// <summary>
    /// Versão do Registro
    /// </summary>
    [Display(Name = "Versão do Registro")]
    [Required(ErrorMessage = "Versão do Registro é obrigatório")]
    public byte[] RowVer { get; set; }
}
