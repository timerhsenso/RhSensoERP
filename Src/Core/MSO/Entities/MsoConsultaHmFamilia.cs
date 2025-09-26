// Src/Core/MSO/Entities/ConsultaHmFamilia.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_consulta_hm_familia")]
public class ConsultaHmFamilia
{
    [Key]
    [Column("cod_hm_familia")]
    public int CodHmFamilia { get; set; }

    [Column("cod_consulta")]
    public int CodConsulta { get; set; }

    [Column("parentesco")]
    [StringLength(50)]
    public string Parentesco { get; set; } = string.Empty;

    [Column("doencas")]
    [StringLength(1000)]
    public string? Doencas { get; set; }

    [Column("idade_obito")]
    public int? IdadeObito { get; set; }

    [Column("causa_obito")]
    [StringLength(200)]
    public string? CausaObito { get; set; }

    [Column("observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ConsultaGeral? Consulta { get; set; }
}