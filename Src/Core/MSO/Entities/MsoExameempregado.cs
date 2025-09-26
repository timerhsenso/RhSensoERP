// Src/Core/MSO/Entities/ExameEmpregado.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_exameempregado")]
public class ExameEmpregado
{
    [Key]
    [Column("cod_exame_empregado")]
    public int CodExameEmpregado { get; set; }

    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("cod_grupo")]
    public int CodGrupo { get; set; }

    [Column("data_solicitacao")]
    public DateTime DataSolicitacao { get; set; }

    [Column("data_realizacao")]
    public DateTime? DataRealizacao { get; set; }

    [Column("resultado")]
    [StringLength(50)]
    public string? Resultado { get; set; }

    [Column("observacoes")]
    [StringLength(2000)]
    public string? Observacoes { get; set; }

    [Column("cod_profsaude")]
    public int? CodProfSaude { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = "SOLICITADO";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Empregado? Empregado { get; set; }
    public virtual ExameGrupo? ExameGrupo { get; set; }
    public virtual ProfSaude? ProfSaude { get; set; }
}