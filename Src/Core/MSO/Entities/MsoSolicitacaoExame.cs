// Src/Core/MSO/Entities/SolicitacaoExame.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_solicitacao_exame")]
public class SolicitacaoExame
{
    [Key]
    [Column("cod_solicitacao_exame")]
    public int CodSolicitacaoExame { get; set; }

    [Column("cod_solicitacao")]
    public int CodSolicitacao { get; set; }

    [Column("cod_grupo")]
    public int CodGrupo { get; set; }

    [Column("prioridade")]
    [StringLength(20)]
    public string? Prioridade { get; set; } // BAIXA, NORMAL, ALTA, URGENTE

    [Column("observacoes")]
    [StringLength(500)]
    public string? Observacoes { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = "SOLICITADO";

    [Column("data_execucao")]
    public DateTime? DataExecucao { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Solicitacao? Solicitacao { get; set; }
    public virtual ExameGrupo? ExameGrupo { get; set; }
    public virtual ICollection<SolicitacaoExameFoto> SolicitacaoExameFotos { get; set; } = new List<SolicitacaoExameFoto>();
}
