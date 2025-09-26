// Src/Core/MSO/Entities/Temp3.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_temp3")]
public class Temp3
{
    [Key]
    [Column("cod_temp3")]
    public int CodTemp3 { get; set; }

    [Column("dados_temp")]
    [StringLength(4000)]
    public string? DadosTemp { get; set; }

    [Column("tipo_operacao")]
    [StringLength(50)]
    public string? TipoOperacao { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
