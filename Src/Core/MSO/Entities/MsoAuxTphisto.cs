// Src/Core/MSO/Entities/AuxTpHisto.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_aux_tphisto")]
public class AuxTpHisto
{
    [Key]
    [Column("cod_tphisto")]
    public int CodTpHisto { get; set; }

    [Column("desc_tphisto")]
    [StringLength(100)]
    public string DescTpHisto { get; set; } = string.Empty;

    [Column("categoria")]
    [StringLength(50)]
    public string? Categoria { get; set; }

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
