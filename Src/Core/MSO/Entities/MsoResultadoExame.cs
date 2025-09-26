// Src/Core/MSO/Entities/ResultadoExame.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_resultado_exame")]
public class ResultadoExame
{
    [Key]
    [Column("cod_resultado")]
    public int CodResultado { get; set; }

    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("cod_grupo")]
    public int CodGrupo { get; set; }

    [Column("data_coleta")]
    public DateTime DataColeta { get; set; }

    [Column("data_resultado")]
    public DateTime? DataResultado { get; set; }

    [Column("resultado_geral")]
    [StringLength(50)]
    public string? ResultadoGeral { get; set; }

    [Column("observacoes")]
    [StringLength(2000)]
    public string? Observacoes { get; set; }

    [Column("laudo")]
    [StringLength(4000)]
    public string? Laudo { get; set; }

    [Column("cod_profsaude")]
    public int? CodProfSaude { get; set; }

    [Column("cod_prestador")]
    public int? CodPrestador { get; set; }

    [Column("arquivo_resultado")]
    [StringLength(500)]
    public string? ArquivoResultado { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Empregado? Empregado { get; set; }
    public virtual ExameGrupo? ExameGrupo { get; set; }
    public virtual ProfSaude? ProfSaude { get; set; }
    public virtual ExamePrestador? Prestador { get; set; }
    public virtual ICollection<ResultadoExameEsp> ResultadosEspecificos { get; set; } = new List<ResultadoExameEsp>();
}