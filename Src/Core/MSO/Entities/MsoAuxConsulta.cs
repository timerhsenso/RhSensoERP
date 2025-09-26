// Src/Core/MSO/Entities/AuxConsulta.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_aux_consulta")]
public class AuxConsulta
{
    [Key]
    [Column("cod_aux_consulta")]
    public int CodAuxConsulta { get; set; }

    [Column("desc_consulta")]
    [StringLength(100)]
    public string DescConsulta { get; set; } = string.Empty;

    [Column("tipo_consulta")]
    [StringLength(20)]
    public string? TipoConsulta { get; set; }

    [Column("obrigatoria")]
    public bool Obrigatoria { get; set; } = false;

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}