// Src/Core/MSO/Entities/Agenda.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_agenda")]
public class Agenda
{
    [Key]
    [Column("cod_agenda")]
    public int CodAgenda { get; set; }

    [Column("cod_profsaude")]
    public int CodProfSaude { get; set; }

    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("data_consulta")]
    public DateTime DataConsulta { get; set; }

    [Column("hora_inicio")]
    [StringLength(5)]
    public string HoraInicio { get; set; } = string.Empty;

    [Column("hora_fim")]
    [StringLength(5)]
    public string? HoraFim { get; set; }

    [Column("cod_solicitacao")]
    public int? CodSolicitacao { get; set; }

    [Column("cod_consulta")]
    public int? CodConsulta { get; set; }

    [Column("cod_tpconsulta")]
    public int? CodTpConsulta { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = "AGENDADO";

    [Column("observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ProfSaude? ProfSaude { get; set; }
    public virtual Empregado? Empregado { get; set; }
    public virtual TpConsulta? TpConsulta { get; set; }
}