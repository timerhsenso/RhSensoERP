// Src/Core/MSO/Entities/ExameGrupoTpConsulta.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_exame_grupo_tpconsulta")]
public class ExameGrupoTpConsulta
{
    [Key]
    [Column("cod_grupo_tpconsulta")]
    public int CodGrupoTpConsulta { get; set; }

    [Column("cod_grupo")]
    public int CodGrupo { get; set; }

    [Column("cod_tpconsulta")]
    public int CodTpConsulta { get; set; }

    [Column("obrigatorio")]
    public bool Obrigatorio { get; set; } = false;

    [Column("ordem")]
    public int? Ordem { get; set; }

    [Column("observacoes")]
    [StringLength(500)]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ExameGrupo? ExameGrupo { get; set; }
    public virtual AuxTpConsulta? TpConsulta { get; set; }
}