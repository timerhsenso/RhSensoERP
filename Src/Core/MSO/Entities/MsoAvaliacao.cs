
// Src/Core/MSO/Entities/Avaliacao.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_avaliacao")]
public class Avaliacao
{
    [Key]
    [Column("cod_avaliacao")]
    public int CodAvaliacao { get; set; }

    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("data_avaliacao")]
    public DateTime DataAvaliacao { get; set; }

    [Column("cod_tpavaliacao")]
    public int CodTpAvaliacao { get; set; }

    [Column("resultado_geral")]
    [StringLength(20)]
    public string? ResultadoGeral { get; set; }

    [Column("observacoes")]
    [StringLength(2000)]
    public string? Observacoes { get; set; }

    [Column("cod_profsaude")]
    public int? CodProfSaude { get; set; }

    [Column("validade")]
    public DateTime? Validade { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Empregado? Empregado { get; set; }
    public virtual TpAvaliacao? TpAvaliacao { get; set; }
    public virtual ProfSaude? ProfSaude { get; set; }
}