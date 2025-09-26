// Src/Core/MSO/Entities/ResultadoFezes.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_resultado_fezes")]
public class ResultadoFezes
{
    [Key]
    [Column("cod_resultado_fezes")]
    public int CodResultadoFezes { get; set; }

    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("data_coleta")]
    public DateTime DataColeta { get; set; }

    [Column("data_resultado")]
    public DateTime? DataResultado { get; set; }

    // Exame Macroscópico
    [Column("cor")]
    [StringLength(50)]
    public string? Cor { get; set; }

    [Column("consistencia")]
    [StringLength(50)]
    public string? Consistencia { get; set; }

    [Column("odor")]
    [StringLength(50)]
    public string? Odor { get; set; }

    [Column("sangue")]
    [StringLength(20)]
    public string? Sangue { get; set; }

    [Column("muco")]
    [StringLength(20)]
    public string? Muco { get; set; }

    // Exame Microscópico
    [Column("leucocitos")]
    [StringLength(50)]
    public string? Leucocitos { get; set; }

    [Column("hemacias")]
    [StringLength(50)]
    public string? Hemacias { get; set; }

    [Column("ovos_parasitas")]
    [StringLength(100)]
    public string? OvosParasitas { get; set; }

    [Column("cistos_parasitas")]
    [StringLength(100)]
    public string? CistosParasitas { get; set; }

    [Column("observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

    [Column("cod_profsaude")]
    public int? CodProfSaude { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Empregado? Empregado { get; set; }
    public virtual ProfSaude? ProfSaude { get; set; }
}