// Src/Core/MSO/Entities/AuxAtivo.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_aux_ativo")]
public class AuxAtivo
{
    [Key]
    [Column("cod_ativo")]
    public int CodAtivo { get; set; }

    [Column("desc_ativo")]
    [StringLength(100)]
    public string DescAtivo { get; set; } = string.Empty;

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ICollection<ExameProg> ExameProgs { get; set; } = new List<ExameProg>();
}