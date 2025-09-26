// Src/Core/MSO/Entities/ParamHistorico.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_param_historico")]
public class ParamHistorico
{
    [Key]
    [Column("cod_param_hist")]
    public int CodParamHist { get; set; }

    [Column("tabela_origem")]
    [StringLength(50)]
    public string TabelaOrigem { get; set; } = string.Empty;

    [Column("chave_registro")]
    [StringLength(100)]
    public string ChaveRegistro { get; set; } = string.Empty;

    [Column("data_alteracao")]
    public DateTime DataAlteracao { get; set; }

    [Column("campo_alterado")]
    [StringLength(50)]
    public string CampoAlterado { get; set; } = string.Empty;

    [Column("valor_anterior")]
    [StringLength(2000)]
    public string? ValorAnterior { get; set; }

    [Column("valor_novo")]
    [StringLength(2000)]
    public string? ValorNovo { get; set; }

    [Column("cod_usuario")]
    [StringLength(20)]
    public string? CodUsuario { get; set; }

    [Column("ip_usuario")]
    [StringLength(50)]
    public string? IpUsuario { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
