// Src/Core/MSO/Entities/SeriesHistoricas.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_series_historicas")]
public class SeriesHistoricas
{
    [Key]
    [Column("cod_serie_hist")]
    public int CodSerieHist { get; set; }

    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("tipo_serie")]
    [StringLength(50)]
    public string TipoSerie { get; set; } = string.Empty; // BIOMETRICO, CLINICO, LABORATORIAL

    [Column("parametro")]
    [StringLength(50)]
    public string Parametro { get; set; } = string.Empty;

    [Column("data_medicao")]
    public DateTime DataMedicao { get; set; }

    [Column("valor")]
    public decimal? Valor { get; set; }

    [Column("unidade")]
    [StringLength(20)]
    public string? Unidade { get; set; }

    [Column("referencia_min")]
    public decimal? ReferenciaMin { get; set; }

    [Column("referencia_max")]
    public decimal? ReferenciaMax { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string? Status { get; set; } // NORMAL, ALTERADO, CRITICO

    [Column("observacoes")]
    [StringLength(500)]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Empregado? Empregado { get; set; }
}