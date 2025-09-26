// Src/Core/MSO/Entities/GrupoEmpregado.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_grupo_empregado")]
public class GrupoEmpregado
{
    [Key]
    [Column("cod_grupo_empregado")]
    public int CodGrupoEmpregado { get; set; }

    [Column("cod_grupo")]
    public int CodGrupo { get; set; }

    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("data_inclusao")]
    public DateTime DataInclusao { get; set; }

    [Column("data_exclusao")]
    public DateTime? DataExclusao { get; set; }

    [Column("motivo_exclusao")]
    [StringLength(200)]
    public string? MotivoExclusao { get; set; }

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Grupo? Grupo { get; set; }
    public virtual Empregado? Empregado { get; set; }
}