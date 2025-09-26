// Src/Core/MSO/Entities/ConsultaRisco.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_consulta_risco")]
public class ConsultaRisco
{
    [Key]
    [Column("cod_consulta_risco")]
    public int CodConsultaRisco { get; set; }

    [Column("cod_consulta")]
    public int CodConsulta { get; set; }

    [Column("cod_risco")]
    public int CodRisco { get; set; }

    [Column("nivel_exposicao")]
    [StringLength(20)]
    public string? NivelExposicao { get; set; } // BAIXO, MEDIO, ALTO

    [Column("tempo_exposicao")]
    [StringLength(50)]
    public string? TempoExposicao { get; set; }

    [Column("epi_utilizado")]
    [StringLength(1)]
    public string? EpiUtilizado { get; set; } // S/N

    [Column("epi_descricao")]
    [StringLength(500)]
    public string? EpiDescricao { get; set; }

    [Column("observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ConsultaGeral? Consulta { get; set; }
    public virtual Risco? Risco { get; set; }
}