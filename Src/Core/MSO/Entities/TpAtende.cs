// Src/Core/MSO/Entities/TpAtende.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_tpatende")]
public class TpAtende
{
    [Key]
    [Column("cod_tpatende")]
    public int CodTpAtende { get; set; }

    [Column("desc_tpatende")]
    [StringLength(100)]
    public string DescTpAtende { get; set; } = string.Empty;

    [Column("categoria")]
    [StringLength(50)]
    public string? Categoria { get; set; }

    [Column("prioridade")]
    public int? Prioridade { get; set; }

    [Column("cor_identificacao")]
    [StringLength(10)]
    public string? CorIdentificacao { get; set; }

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}