using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Shared.Core.Domain.Entities.TabelasBase;

/// <summary>
/// Tipos sanguíneos com informações de compatibilidade.
/// Informação crítica para emergências médicas em ambiente industrial.
/// Tabela: BASE_TipoSanguineo
/// </summary>
[Table("BASE_TipoSanguineo")]
public class TipoSanguineo
{
    [Key]
    [Column("Id")]
    [Display(Name = "ID")]
    public byte Id { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Dados
    // ═══════════════════════════════════════════════════════════════════

    [Column("Codigo")]
    [StringLength(5)]
    [Required]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Column("Descricao")]
    [StringLength(20)]
    [Required]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = string.Empty;

    [Column("PodeDoarPara")]
    [StringLength(50)]
    [Display(Name = "Pode Doar Para")]
    public string? PodeDoarPara { get; set; }

    [Column("PodeReceberDe")]
    [StringLength(50)]
    [Display(Name = "Pode Receber De")]
    public string? PodeReceberDe { get; set; }

    [Column("Ordem")]
    [Display(Name = "Ordem")]
    public byte Ordem { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Controle
    // ═══════════════════════════════════════════════════════════════════

    [Column("Ativo")]
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;
}
