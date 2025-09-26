// Src/Core/MSO/Entities/Grupo.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_grupo")]
public class Grupo
{
    [Key]
    [Column("cod_grupo")]
    public int CodGrupo { get; set; }

    [Column("nome_grupo")]
    [StringLength(100)]
    public string NomeGrupo { get; set; } = string.Empty;

    [Column("descricao")]
    [StringLength(500)]
    public string? Descricao { get; set; }

    [Column("tipo_grupo")]
    [StringLength(20)]
    public string? TipoGrupo { get; set; } // SETOR, DEPARTAMENTO, EQUIPE

    [Column("responsavel")]
    [StringLength(100)]
    public string? Responsavel { get; set; }

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ICollection<GrupoEmpregado> GrupoEmpregados { get; set; } = new List<GrupoEmpregado>();
    public virtual ICollection<GrupoRisco> GrupoRiscos { get; set; } = new List<GrupoRisco>();
}
