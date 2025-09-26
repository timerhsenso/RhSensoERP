// Src/Core/MSO/Entities/ExameExtra2.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_exame_extra2")]
public class ExameExtra2
{
    [Key]
    [Column("cod_exame_extra2")]
    public int CodExameExtra2 { get; set; }

    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("data_exame")]
    public DateTime DataExame { get; set; }

    [Column("tipo_exame")]
    [StringLength(50)]
    public string TipoExame { get; set; } = string.Empty;

    [Column("resultado")]
    [StringLength(4000)]
    public string? Resultado { get; set; }

    [Column("laudo_medico")]
    [StringLength(4000)]
    public string? LaudoMedico { get; set; }

    [Column("cod_profsaude")]
    public int? CodProfSaude { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Empregado? Empregado { get; set; }
    public virtual ProfSaude? ProfSaude { get; set; }
}
