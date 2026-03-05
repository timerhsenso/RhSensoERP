// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: GTP_TipoDocumentoFornecedor
// Module: GestaoTerceirosPrestadores
// Data: 2026-03-04 22:56:13
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.GestaoTerceirosPrestadores.GTP_TipoDocumentoFornecedor;

/// <summary>
/// Request para atualização de Tipo de Documento de Fornecedor.
/// Compatível com backend: UpdateGTP_TipoDocumentoFornecedorRequest
/// </summary>
public class UpdateGTP_TipoDocumentoFornecedorRequest
{
    /// <summary>
    /// Código
    /// </summary>
    [Display(Name = "Código")]
    [Required(ErrorMessage = "Código é obrigatório")]
    [StringLength(40, ErrorMessage = "Código deve ter no máximo {1} caracteres")]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [Required(ErrorMessage = "Descrição é obrigatório")]
    [StringLength(150, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Categoria
    /// </summary>
    [Display(Name = "Categoria")]
    [StringLength(30, ErrorMessage = "Categoria deve ter no máximo {1} caracteres")]
    public string Categoria { get; set; } = string.Empty;

    /// <summary>
    /// Obrigatório
    /// </summary>
    [Display(Name = "Obrigatório")]
    [Required(ErrorMessage = "Obrigatório é obrigatório")]
    public bool Obrigatorio { get; set; }

    /// <summary>
    /// Controla Vencimento
    /// </summary>
    [Display(Name = "Controla Vencimento")]
    [Required(ErrorMessage = "Controla Vencimento é obrigatório")]
    public bool ControlVencimento { get; set; }

    /// <summary>
    /// Alertar (dias antes)
    /// </summary>
    [Display(Name = "Alertar (dias antes)")]
    public int? AlertarDiasAntes { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo é obrigatório")]
    public bool Ativo { get; set; }

    /// <summary>
    /// Ordem
    /// </summary>
    [Display(Name = "Ordem")]
    [Required(ErrorMessage = "Ordem é obrigatório")]
    public int Ordem { get; set; }

    /// <summary>
    /// Row Ver
    /// </summary>
    [Display(Name = "Row Ver")]
    [Required(ErrorMessage = "Row Ver é obrigatório")]
    public byte[] RowVer { get; set; }
}
