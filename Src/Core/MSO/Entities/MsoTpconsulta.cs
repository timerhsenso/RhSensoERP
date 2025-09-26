// Src/Core/MSO/Entities/TpConsulta.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_tpconsulta")]
public class TpConsulta
{
    [Key]
    [Column("cod_tpconsulta")]
    public int CodTpConsulta { get; set; }

    [Column("desc_tpconsulta")]
    [StringLength(100)]
    public string DescTpConsulta { get; set; } = string.Empty;

    [Column("categoria")]
    [StringLength(50)]
    public string? Categoria { get; set; }

    [Column("periodicidade_meses")]
    public int? PeriodicidadeMeses { get; set; }

    [Column("obrigatoria")]
    public bool Obrigatoria { get; set; } = false;

    [Column("duracao_minutos")]
    public int? DuracaoMinutos { get; set; }

    [Column("descricao_detalhada")]
    [StringLength(1000)]
    public string? DescricaoDetalhada { get; set; }

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ICollection<Agenda> Agendas { get; set; } = new List<Agenda>();
}