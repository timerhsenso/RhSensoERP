// Src/Core/MSO/Entities/Risco.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_risco")]
public class Risco
{
    [Key]
    [Column("cod_risco")]
    public int CodRisco { get; set; }

    [Column("nome_risco")]
    [StringLength(200)]
    public string NomeRisco { get; set; } = string.Empty;

    [Column("descricao")]
    [StringLength(1000)]
    public string? Descricao { get; set; }

    [Column("cod_tprisco")]
    public int CodTpRisco { get; set; }

    [Column("agente_risco")]
    [StringLength(200)]
    public string? AgenteRisco { get; set; }

    [Column("fonte_geradora")]
    [StringLength(500)]
    public string? FonteGeradora { get; set; }

    [Column("via_penetracao")]
    [StringLength(200)]
    public string? ViaPenetracao { get; set; }

    [Column("efeitos_saude")]
    [StringLength(1000)]
    public string? EfeitosSaude { get; set; }

    [Column("medidas_controle")]
    [StringLength(2000)]
    public string? MedidasControle { get; set; }

    [Column("epi_recomendado")]
    [StringLength(1000)]
    public string? EpiRecomendado { get; set; }

    [Column("limite_tolerancia")]
    [StringLength(100)]
    public string? LimiteTolerancia { get; set; }

    [Column("metodologia_avaliacao")]
    [StringLength(500)]
    public string? MetodologiaAvaliacao { get; set; }

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Relacionamentos
    public virtual AuxTpRisco? TpRisco { get; set; }
    public virtual ICollection<FuncaoRisco> FuncaoRiscos { get; set; } = new List<FuncaoRisco>();
    public virtual ICollection<GrupoRisco> GrupoRiscos { get; set; } = new List<GrupoRisco>();
    public virtual ICollection<ConsultaRisco> ConsultaRiscos { get; set; } = new List<ConsultaRisco>();
}