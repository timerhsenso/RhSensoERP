// Src/Core/MSO/Entities/Temp2.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_temp2")]
public class Temp2
{
    [Key]
    [Column("cod_temp2")]
    public int CodTemp2 { get; set; }

    [Column("dados_temp")]
    [StringLength(4000)]
    public string? DadosTemp { get; set; }

    [Column("tipo_operacao")]
    [StringLength(50)]
    public string? TipoOperacao { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}