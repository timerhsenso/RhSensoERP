// Src/Core/MSO/Entities/ConsultaHistorico.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_consulta_historico")]
public class ConsultaHistorico
{
    [Key]
    [Column("cod_historico")]
    public int CodHistorico { get; set; }

    [Column("cod_consulta")]
    public int CodConsulta { get; set; }

    [Column("cod_tphisto")]
    public int CodTpHisto { get; set; }

    [Column("descricao")]
    [StringLength(2000)]
    public string Descricao { get; set; } = string.Empty;

    [Column("data_ocorrencia")]
    public DateTime? DataOcorrencia { get; set; }

    [Column("observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamentos
    public virtual ConsultaGeral? Consulta { get; set; }
    public virtual AuxTpHisto? TpHisto { get; set; }
}