// Src/Core/MSO/Entities/Cid.cs
using RhSensoERP.Core.RHU.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_cid")]
public class Cid
{
    [Key]
    [Column("cod_cid")]
    [StringLength(10)]
    public string CodCid { get; set; } = string.Empty;

    [Column("descricao")]
    [StringLength(500)]
    public string Descricao { get; set; } = string.Empty;

    [Column("categoria")]
    [StringLength(5)]
    public string? Categoria { get; set; }

    [Column("subcategoria")]
    [StringLength(10)]
    public string? Subcategoria { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ICollection<Atendimento> Atendimentos { get; set; } = new List<Atendimento>();
    public virtual ICollection<Afastamento> Afastamentos { get; set; } = new List<Afastamento>();
}