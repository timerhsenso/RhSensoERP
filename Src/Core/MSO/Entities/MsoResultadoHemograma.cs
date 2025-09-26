// Src/Core/MSO/Entities/ResultadoHemograma.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_resultado_hemograma")]
public class ResultadoHemograma
{
    [Key]
    [Column("cod_resultado_hemo")]
    public int CodResultadoHemo { get; set; }

    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("data_coleta")]
    public DateTime DataColeta { get; set; }

    [Column("data_resultado")]
    public DateTime? DataResultado { get; set; }

    // Série Vermelha
    [Column("eritrocitos")]
    public decimal? Eritrocitos { get; set; }

    [Column("hemoglobina")]
    public decimal? Hemoglobina { get; set; }

    [Column("hematocrito")]
    public decimal? Hematocrito { get; set; }

    [Column("vcm")]
    public decimal? Vcm { get; set; }

    [Column("hcm")]
    public decimal? Hcm { get; set; }

    [Column("chcm")]
    public decimal? Chcm { get; set; }

    [Column("rdw")]
    public decimal? Rdw { get; set; }

    // Série Branca
    [Column("leucocitos")]
    public decimal? Leucocitos { get; set; }

    [Column("neutrofilos")]
    public decimal? Neutrofilos { get; set; }

    [Column("linfocitos")]
    public decimal? Linfocitos { get; set; }

    [Column("monocitos")]
    public decimal? Monocitos { get; set; }

    [Column("eosinofilos")]
    public decimal? Eosinofilos { get; set; }

    [Column("basofilos")]
    public decimal? Basofilos { get; set; }

    // Plaquetas
    [Column("plaquetas")]
    public decimal? Plaquetas { get; set; }

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