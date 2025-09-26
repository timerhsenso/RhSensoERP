
// Src/Core/MSO/Entities/AuxResultado.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_aux_resultado")]
public class AuxResultado
{
    [Key]
    [Column("cod_resultado")]
    public int CodResultado { get; set; }

    [Column("desc_resultado")]
    [StringLength(100)]
    public string DescResultado { get; set; } = string.Empty;

    [Column("tipo_resultado")]
    [StringLength(20)]
    public string? TipoResultado { get; set; }

    [Column("cor_indicativa")]
    [StringLength(10)]
    public string? CorIndicativa { get; set; }

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
