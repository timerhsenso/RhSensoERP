// Src/Core/MSO/Entities/SeriesAudiometricas.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_series_audiometricas")]
public class SeriesAudiometricas
{
    [Key]
    [Column("cod_serie")]
    public int CodSerie { get; set; }

    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("data_inicio")]
    public DateTime DataInicio { get; set; }

    [Column("data_fim")]
    public DateTime? DataFim { get; set; }

    [Column("periodo_anos")]
    public int PeriodoAnos { get; set; }

    [Column("frequencia")]
    public int Frequencia { get; set; } // Hz

    [Column("orelha")]
    [StringLength(1)]
    public string Orelha { get; set; } = string.Empty; // D/E

    [Column("limiar_inicial")]
    public int? LimiarInicial { get; set; }

    [Column("limiar_final")]
    public int? LimiarFinal { get; set; }

    [Column("diferenca")]
    public int? Diferenca { get; set; }

    [Column("evolucao")]
    [StringLength(20)]
    public string? Evolucao { get; set; } // ESTAVEL, MELHORA, PIORA

    [Column("observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Empregado? Empregado { get; set; }
}