// Src/Core/MSO/Entities/ParamExameEsp.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_param_exame_esp")]
public class ParamExameEsp
{
    [Key]
    [Column("cod_param")]
    public int CodParam { get; set; }

    [Column("cod_grupo")]
    public int CodGrupo { get; set; }

    [Column("nome_parametro")]
    [StringLength(100)]
    public string NomeParametro { get; set; } = string.Empty;

    [Column("valor_referencia_min")]
    public decimal? ValorReferenciaMin { get; set; }

    [Column("valor_referencia_max")]
    public decimal? ValorReferenciaMax { get; set; }

    [Column("unidade_medida")]
    [StringLength(20)]
    public string? UnidadeMedida { get; set; }

    [Column("sexo_aplicavel")]
    [StringLength(1)]
    public string? SexoAplicavel { get; set; } // M/F/A (Ambos)

    [Column("idade_min")]
    public int? IdadeMin { get; set; }

    [Column("idade_max")]
    public int? IdadeMax { get; set; }

    [Column("observacoes")]
    [StringLength(500)]
    public string? Observacoes { get; set; }

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ExameGrupo? ExameGrupo { get; set; }
}