// Src/Core/MSO/Entities/GrupoRisco.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_grupo_risco")]
public class GrupoRisco
{
    [Key]
    [Column("cod_grupo_risco")]
    public int CodGrupoRisco { get; set; }

    [Column("cod_grupo")]
    public int CodGrupo { get; set; }

    [Column("cod_risco")]
    public int CodRisco { get; set; }

    [Column("nivel_exposicao")]
    [StringLength(20)]
    public string? NivelExposicao { get; set; }

    [Column("medidas_controle")]
    [StringLength(1000)]
    public string? MedidasControle { get; set; }

    [Column("observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Grupo? Grupo { get; set; }
    public virtual Risco? Risco { get; set; }
}