// Src/Core/MSO/Entities/Conduta.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_conduta")]
public class Conduta
{
    [Key]
    [Column("cod_conduta")]
    public int CodConduta { get; set; }

    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("data_conduta")]
    public DateTime DataConduta { get; set; }

    [Column("tipo_conduta")]
    [StringLength(50)]
    public string TipoConduta { get; set; } = string.Empty;

    [Column("descricao")]
    [StringLength(2000)]
    public string Descricao { get; set; } = string.Empty;

    [Column("cod_profsaude")]
    public int? CodProfSaude { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = "ATIVO";

    [Column("data_followup")]
    public DateTime? DataFollowUp { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual Empregado? Empregado { get; set; }
    public virtual ProfSaude? ProfSaude { get; set; }
}