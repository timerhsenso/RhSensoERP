// Src/Core/MSO/Entities/Parametro.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_parametro")]
public class Parametro
{
    [Key]
    [Column("cod_parametro")]
    [StringLength(50)]
    public string CodParametro { get; set; } = string.Empty;

    [Column("descricao")]
    [StringLength(200)]
    public string Descricao { get; set; } = string.Empty;

    [Column("valor")]
    [StringLength(1000)]
    public string? Valor { get; set; }

    [Column("tipo_dado")]
    [StringLength(20)]
    public string? TipoDado { get; set; } // STRING, INTEGER, DECIMAL, BOOLEAN, DATE

    [Column("categoria")]
    [StringLength(50)]
    public string? Categoria { get; set; }

    [Column("editavel")]
    public bool Editavel { get; set; } = true;

    [Column("obrigatorio")]
    public bool Obrigatorio { get; set; } = false;

    [Column("observacoes")]
    [StringLength(500)]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
