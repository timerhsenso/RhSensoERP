// Src/Core/MSO/Entities/ConsultaHisPessoal.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_consulta_his_pessoal")]
public class ConsultaHisPessoal
{
    [Key]
    [Column("cod_his_pessoal")]
    public int CodHisPessoal { get; set; }

    [Column("cod_consulta")]
    public int CodConsulta { get; set; }

    [Column("doencas_pregressas")]
    [StringLength(2000)]
    public string? DoencasPregressas { get; set; }

    [Column("cirurgias")]
    [StringLength(2000)]
    public string? Cirurgias { get; set; }

    [Column("internacoes")]
    [StringLength(2000)]
    public string? Internacoes { get; set; }

    [Column("alergias")]
    [StringLength(1000)]
    public string? Alergias { get; set; }

    [Column("medicamentos_uso")]
    [StringLength(2000)]
    public string? MedicamentosUso { get; set; }

    [Column("habitos_vida")]
    [StringLength(2000)]
    public string? HabitosVida { get; set; }

    [Column("tabagismo")]
    [StringLength(1)]
    public string? Tabagismo { get; set; } // S/N

    [Column("etilismo")]
    [StringLength(1)]
    public string? Etilismo { get; set; } // S/N

    [Column("atividade_fisica")]
    [StringLength(1)]
    public string? AtividadeFisica { get; set; } // S/N

    [Column("observacoes")]
    [StringLength(2000)]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ConsultaGeral? Consulta { get; set; }
}
