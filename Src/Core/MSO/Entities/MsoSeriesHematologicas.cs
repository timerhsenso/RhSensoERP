// Src/Core/MSO/Entities/SeriesHematologicas.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_series_hematologicas")]
public class SeriesHematologicas
{
    [Key]
    [Column("cod_serie_hemato")]
    public int CodSerieHemato { get; set; }

    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("data_inicio")]
    public DateTime DataInicio { get; set; }

    [Column("data_fim")]
    public DateTime? DataFim { get; set; }

    [Column("parametro")]
    [StringLength(50)]
    public string Parametro { get; set; } = string.Empty; // HEMOGLOBINA, LEUCOCITOS, etc.

    [Column("valor_inicial")]
    public decimal? ValorInicial { get; set; }

    [Column("valor_final")]
    public decimal? ValorFinal { get; set; }

    [Column("variacao_percentual")]
    public decimal? VariacaoPercentual { get; set; }

    [Column("tendencia")]
    [StringLength(20)]
    public string? Tendencia { get; set; } // ESTAVEL, CRESCENTE, DECRESCENTE

    [Column("dentro_referencia")]
    [StringLength(1)]
    public string? DentroReferencia { get; set; } // S/N

    [Column("observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Empregado? Empregado { get; set; }
}