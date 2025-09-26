// Src/Core/MSO/Entities/ResultadoExameEsp.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_resultado_exame_esp")]
public class ResultadoExameEsp
{
    [Key]
    [Column("cod_resultado_esp")]
    public int CodResultadoEsp { get; set; }

    [Column("cod_resultado")]
    public int CodResultado { get; set; }

    [Column("cod_param")]
    public int CodParam { get; set; }

    [Column("valor_resultado")]
    [StringLength(100)]
    public string? ValorResultado { get; set; }

    [Column("valor_numerico")]
    public decimal? ValorNumerico { get; set; }

    [Column("status_resultado")]
    [StringLength(20)]
    public string? StatusResultado { get; set; } // NORMAL, ALTERADO, CRITICO

    [Column("observacoes")]
    [StringLength(500)]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ResultadoExame? ResultadoExame { get; set; }
    public virtual ParamExameEsp? ParametroExame { get; set; }
}