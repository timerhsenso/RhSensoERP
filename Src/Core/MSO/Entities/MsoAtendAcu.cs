// Src/Core/MSO/Entities/AtendAcu.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_atend_acu")]
public class AtendAcu
{
    [Key]
    [Column("setor")]
    [StringLength(20)]
    public string Setor { get; set; } = string.Empty;

    [Column("t01")] public int? T01 { get; set; }
    [Column("t02")] public int? T02 { get; set; }
    [Column("t03")] public int? T03 { get; set; }
    [Column("t04")] public int? T04 { get; set; }
    [Column("t05")] public int? T05 { get; set; }
    [Column("t06")] public int? T06 { get; set; }
    [Column("t07")] public int? T07 { get; set; }
    [Column("t08")] public int? T08 { get; set; }
    [Column("t09")] public int? T09 { get; set; }
    [Column("t10")] public int? T10 { get; set; }
    [Column("t11")] public int? T11 { get; set; }
    [Column("t12")] public int? T12 { get; set; }
    [Column("t13")] public int? T13 { get; set; }
    [Column("t14")] public int? T14 { get; set; }

    [Column("ano")]
    public int? Ano { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}