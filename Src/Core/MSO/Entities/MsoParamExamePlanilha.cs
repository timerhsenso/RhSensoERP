// Src/Core/MSO/Entities/ParamExamePlanilha.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_param_exame_planilha")]
public class ParamExamePlanilha
{
    [Key]
    [Column("cod_planilha")]
    public int CodPlanilha { get; set; }

    [Column("nome_planilha")]
    [StringLength(100)]
    public string NomePlanilha { get; set; } = string.Empty;

    [Column("cod_grupo")]
    public int CodGrupo { get; set; }

    [Column("layout_colunas")]
    [StringLength(4000)]
    public string? LayoutColunas { get; set; } // JSON com configuração das colunas

    [Column("cabecalho_personalizado")]
    [StringLength(2000)]
    public string? CabecalhoPersonalizado { get; set; }

    [Column("rodape_personalizado")]
    [StringLength(1000)]
    public string? RodapePersonalizado { get; set; }

    [Column("formato_saida")]
    [StringLength(20)]
    public string? FormatoSaida { get; set; } // PDF, EXCEL, CSV

    [Column("situacao")]
    [StringLength(1)]
    public string Situacao { get; set; } = "A";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ExameGrupo? ExameGrupo { get; set; }
}