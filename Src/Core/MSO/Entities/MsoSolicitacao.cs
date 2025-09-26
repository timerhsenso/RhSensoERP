// Src/Core/MSO/Entities/Solicitacao.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_solicitacao")]
public class Solicitacao
{
    [Key]
    [Column("cod_solicitacao")]
    public int CodSolicitacao { get; set; }

    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("data_solicitacao")]
    public DateTime DataSolicitacao { get; set; }

    [Column("cod_profsaude")]
    public int CodProfSaude { get; set; }

    [Column("tipo_solicitacao")]
    [StringLength(50)]
    public string TipoSolicitacao { get; set; } = string.Empty; // ADMISSIONAL, PERIODICO, DEMISSIONAL

    [Column("urgente")]
    public bool Urgente { get; set; } = false;

    [Column("justificativa")]
    [StringLength(1000)]
    public string? Justificativa { get; set; }

    [Column("observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = "PENDENTE"; // PENDENTE, EXECUTANDO, CONCLUIDO, CANCELADO

    [Column("data_conclusao")]
    public DateTime? DataConclusao { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Relacionamentos
    public virtual Empregado? Empregado { get; set; }
    public virtual ProfSaude? ProfSaude { get; set; }
    public virtual ICollection<SolicitacaoExame> SolicitacaoExames { get; set; } = new List<SolicitacaoExame>();
}
