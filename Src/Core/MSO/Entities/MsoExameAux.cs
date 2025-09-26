// Src/Core/MSO/Entities/MsoExameAux.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("MSO_EXAME_AUX")]
public class MsoExameAux
{
    [Key]
    [Column("cod_exame_aux")]
    public int CodExameAux { get; set; }

    [Column("descricao")]
    [StringLength(200)]
    public string Descricao { get; set; } = string.Empty;

    [Column("tipo_exame")]
    [StringLength(50)]
    public string? TipoExame { get; set; }

    [Column("valor_referencia")]
    [StringLength(100)]
    public string? ValorReferencia { get; set; }

    [Column("unidade_medida")]
    [StringLength(20)]
    public string? UnidadeMedida { get; set; }

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}