using RhSensoERP.Core.MSO.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Src / Core / MSO / Entities / ConsultaBiometrico.cs
namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_consulta_biometrico")]
public class ConsultaBiometrico
{
    [Key]
    [Column("cod_biometrico")]
    public int CodBiometrico { get; set; }

    [Column("cod_consulta")]
    public int CodConsulta { get; set; }

    [Column("peso")]
    public decimal? Peso { get; set; }

    [Column("altura")]
    public decimal? Altura { get; set; }

    [Column("imc")]
    public decimal? Imc { get; set; }

    [Column("circunferencia_abdominal")]
    public decimal? CircunferenciaAbdominal { get; set; }

    [Column("pressao_sistolica")]
    public int? PressaoSistolica { get; set; }

    [Column("pressao_diastolica")]
    public int? PressaoDiastolica { get; set; }

    [Column("frequencia_cardiaca")]
    public int? FrequenciaCardiaca { get; set; }

    [Column("frequencia_respiratoria")]
    public int? FrequenciaRespiratoria { get; set; }

    [Column("temperatura")]
    public decimal? Temperatura { get; set; }

    [Column("saturacao_oxigenio")]
    public decimal? SaturacaoOxigenio { get; set; }

    [Column("glicemia")]
    public decimal? Glicemia { get; set; }

    [Column("observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ConsultaGeral? Consulta { get; set; }
}
