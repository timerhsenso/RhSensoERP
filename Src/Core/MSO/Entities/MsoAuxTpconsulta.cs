// Src/Core/MSO/Entities/AuxTpConsulta.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_aux_tpconsulta")]
public class AuxTpConsulta
{
    [Key]
    [Column("cod_tpconsulta")]
    public int CodTpConsulta { get; set; }

    [Column("desc_tpconsulta")]
    [StringLength(100)]
    public string DescTpConsulta { get; set; } = string.Empty;

    [Column("periodicidade_meses")]
    public int? PeriodicidadeMeses { get; set; }

    [Column("obrigatoria")]
    public bool Obrigatoria { get; set; } = false;

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ICollection<ExameGrupoTpConsulta> ExameGrupoTpConsultas { get; set; } = new List<ExameGrupoTpConsulta>();
}