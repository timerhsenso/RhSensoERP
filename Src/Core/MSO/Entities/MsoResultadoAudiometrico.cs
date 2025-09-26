// Src/Core/MSO/Entities/ResultadoAudiometrico.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_resultado_audiometrico")]
public class ResultadoAudiometrico
{
    [Key]
    [Column("cod_resultado_audio")]
    public int CodResultadoAudio { get; set; }

    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("data_exame")]
    public DateTime DataExame { get; set; }

    [Column("orelha")]
    [StringLength(1)]
    public string Orelha { get; set; } = string.Empty; // D/E

    // Frequências em Hz - Limiares em dB
    [Column("hz_250")] public int? Hz250 { get; set; }
    [Column("hz_500")] public int? Hz500 { get; set; }
    [Column("hz_1000")] public int? Hz1000 { get; set; }
    [Column("hz_2000")] public int? Hz2000 { get; set; }
    [Column("hz_3000")] public int? Hz3000 { get; set; }
    [Column("hz_4000")] public int? Hz4000 { get; set; }
    [Column("hz_6000")] public int? Hz6000 { get; set; }
    [Column("hz_8000")] public int? Hz8000 { get; set; }

    [Column("conclusao")]
    [StringLength(100)]
    public string? Conclusao { get; set; }

    [Column("sugestao")]
    [StringLength(500)]
    public string? Sugestao { get; set; }

    [Column("cod_profsaude")]
    public int? CodProfSaude { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Empregado? Empregado { get; set; }
    public virtual ProfSaude? ProfSaude { get; set; }
}