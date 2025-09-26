// Src/Core/MSO/Entities/AtendimentoMedicamento.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_atendimento_medicamento")]
public class AtendimentoMedicamento
{
    [Key]
    [Column("cod_medicamento_atend")]
    public int CodMedicamentoAtend { get; set; }

    [Column("cod_atendimento")]
    public int CodAtendimento { get; set; }

    [Column("cod_medicamento")]
    public int CodMedicamento { get; set; }

    [Column("dosagem")]
    [StringLength(100)]
    public string? Dosagem { get; set; }

    [Column("posologia")]
    [StringLength(500)]
    public string? Posologia { get; set; }

    [Column("duracao")]
    [StringLength(50)]
    public string? Duracao { get; set; }

    [Column("quantidade")]
    public decimal? Quantidade { get; set; }

    [Column("observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Atendimento? Atendimento { get; set; }
    public virtual Medicamento? Medicamento { get; set; }
}
