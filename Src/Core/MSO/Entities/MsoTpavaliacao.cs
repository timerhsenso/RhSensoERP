// Src/Core/MSO/Entities/TpAvaliacao.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_tpavaliacao")]
public class TpAvaliacao
{
    [Key]
    [Column("cod_tpavaliacao")]
    public int CodTpAvaliacao { get; set; }

    [Column("desc_tpavaliacao")]
    [StringLength(100)]
    public string DescTpAvaliacao { get; set; } = string.Empty;

    [Column("periodicidade_meses")]
    public int? PeriodicidadeMeses { get; set; }

    [Column("obrigatoria")]
    public bool Obrigatoria { get; set; } = false;

    [Column("descricao_detalhada")]
    [StringLength(1000)]
    public string? DescricaoDetalhada { get; set; }

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
}