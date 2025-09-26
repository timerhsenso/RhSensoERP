// Src/Core/MSO/Entities/FuncaoRisco.cs
using RhSensoERP.Core.Security.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_funcaorisco")]
public class FuncaoRisco
{
    [Key]
    [Column("cod_funcao_risco")]
    public int CodFuncaoRisco { get; set; }

    [Column("cod_funcao")]
    public int CodFuncao { get; set; }

    [Column("cod_risco")]
    public int CodRisco { get; set; }

    [Column("nivel_exposicao")]
    [StringLength(20)]
    public string? NivelExposicao { get; set; }

    [Column("tempo_exposicao_horas")]
    public decimal? TempoExposicaoHoras { get; set; }

    [Column("medidas_controle")]
    [StringLength(1000)]
    public string? MedidasControle { get; set; }

    [Column("epi_necessario")]
    [StringLength(1)]
    public string? EpiNecessario { get; set; }

    [Column("epi_descricao")]
    [StringLength(500)]
    public string? EpiDescricao { get; set; }

    [Column("observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Funcao? Funcao { get; set; }
    public virtual Risco? Risco { get; set; }
}