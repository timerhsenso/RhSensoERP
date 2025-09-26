// Src/Core/MSO/Entities/ExameProgConsulta.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_exameprogconsulta")]
public class ExameProgConsulta
{
    [Key]
    [Column("cod_prog_consulta")]
    public int CodProgConsulta { get; set; }

    [Column("cod_prog")]
    public int CodProg { get; set; }

    [Column("cod_tpconsulta")]
    public int CodTpConsulta { get; set; }

    [Column("cod_grupo")]
    public int? CodGrupo { get; set; }

    [Column("obrigatorio")]
    public bool Obrigatorio { get; set; } = false;

    [Column("ordem_execucao")]
    public int? OrdemExecucao { get; set; }

    [Column("observacoes")]
    [StringLength(500)]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ExameProg? ExameProg { get; set; }
    public virtual AuxTpConsulta? TpConsulta { get; set; }
    public virtual ExameGrupo? ExameGrupo { get; set; }
}