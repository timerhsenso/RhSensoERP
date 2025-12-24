using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Shared.Core.Domain.Entities.TabelasBase;

/// <summary>
/// Unidades Federativas do Brasil.
/// Tabela: BASE_UF
/// </summary>
[Table("BASE_UF")]
public class UF
{
    [Key]
    [Column("Id")]
    [Display(Name = "ID")]
    public byte Id { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Dados
    // ═══════════════════════════════════════════════════════════════════

    [Column("Sigla")]
    [StringLength(2)]
    [Required]
    [Display(Name = "Sigla")]
    public string Sigla { get; set; } = string.Empty;

    [Column("Nome")]
    [StringLength(50)]
    [Required]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("CodigoIBGE")]
    [StringLength(2)]
    [Required]
    [Display(Name = "Código IBGE")]
    public string CodigoIBGE { get; set; } = string.Empty;

    [Column("Regiao")]
    [StringLength(20)]
    [Required]
    [Display(Name = "Região")]
    public string Regiao { get; set; } = string.Empty;
}
