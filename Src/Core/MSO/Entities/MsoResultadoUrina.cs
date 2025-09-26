// Src/Core/MSO/Entities/ResultadoUrina.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_resultado_urina")]
public class ResultadoUrina
{
    [Key]
    [Column("cod_resultado_urina")]
    public int CodResultadoUrina { get; set; }

    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("data_coleta")]
    public DateTime DataColeta { get; set; }

    [Column("data_resultado")]
    public DateTime? DataResultado { get; set; }

    // Exame Físico
    [Column("cor")]
    [StringLength(50)]
    public string";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}