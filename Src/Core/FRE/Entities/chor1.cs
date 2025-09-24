using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.FRE.Entities;

/// <summary>
/// Horário administrativo (carga horária base).
/// Tabela: chor1
/// </summary>
[Table("chor1")]
public class Chor1
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdcarghor"), StringLength(2)]
    public string CdCargHor { get; set; } = string.Empty;

    [Column("hhentrada"), StringLength(5)]
    public string HhEntrada { get; set; } = string.Empty;

    [Column("hhsaida"), StringLength(5)]
    public string HhSaida { get; set; } = string.Empty;

    [Column("hhinicint"), StringLength(5)]
    public string HhIniInt { get; set; } = string.Empty;

    [Column("hhfimint"), StringLength(5)]
    public string HhFimInt { get; set; } = string.Empty;

    [Column("MMTOLERANCIA")]
    public int? MmTolerancia { get; set; }

    [Column("FLINTERVALO"), StringLength(1)]
    public string? FlIntervalo { get; set; }

    [Column("MMTOLERANCIA2")]
    public int? MmTolerancia2 { get; set; }

    [Column("DCCARGHOR"), StringLength(100)]
    public string DcCargHor { get; set; } = string.Empty;

    [Column("codhors1050"), StringLength(30)]
    public string? CodHors1050 { get; set; }
}
