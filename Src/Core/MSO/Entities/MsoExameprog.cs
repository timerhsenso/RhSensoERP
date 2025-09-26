// Src/Core/MSO/Entities/ExameProg.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_exameprog")]
public class ExameProg
{
    [Key]
    [Column("cod_prog")]
    public int CodProg { get; set; }

    [Column("nome_programa")]
    [StringLength(100)]
    public string NomePrograma { get; set; } = string.Empty;

    [Column("descricao")]
    [StringLength(500)]
    public string? Descricao { get; set; }

    [Column("cod_prestador")]
    public int? CodPrestador { get; set; }

    [Column("periodicidade_meses")]
    public int? PeriodicidadeMeses { get; set; }

    [Column("valor_programa")]
    public decimal? ValorPrograma { get; set; }

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ExamePrestador? Prestador { get; set; }
    public virtual ICollection<ExameProgConsulta> ExameProgConsultas { get; set; } = new List<ExameProgConsulta>();
}