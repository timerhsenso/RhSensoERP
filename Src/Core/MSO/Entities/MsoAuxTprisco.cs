// Src/Core/MSO/Entities/AuxTpRisco.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_aux_tprisco")]
public class AuxTpRisco
{
    [Key]
    [Column("cod_tprisco")]
    public int CodTpRisco { get; set; }

    [Column("desc_tprisco")]
    [StringLength(100)]
    public string DescTpRisco { get; set; } = string.Empty;

    [Column("classificacao")]
    [StringLength(20)]
    public string? Classificacao { get; set; } // FISICO, QUIMICO, BIOLOGICO, etc.

    [Column("grau_risco")]
    public int? GrauRisco { get; set; } // 1-5

    [Column("cor_identificacao")]
    [StringLength(10)]
    public string? CorIdentificacao { get; set; }

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ICollection<Risco> Riscos { get; set; } = new List<Risco>();
}