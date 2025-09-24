using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.FRE.Entities;

/// <summary>
/// Cabeçalho de compensação de horas.
/// Tabela: COMP1
/// </summary>
[Table("COMP1")]
public class Comp1
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("CDEMPRESA")]
    public int? CdEmpresa { get; set; }

    [Column("CDFILIAL")]
    public int? CdFilial { get; set; }

    [Column("CDCCUSTO"), StringLength(5)]
    public string? CdCcusto { get; set; }

    [Column("MOTIVO"), StringLength(150)]
    public string Motivo { get; set; } = string.Empty;

    [Column("TPJORNADA"), StringLength(1)]
    public string? TpJornada { get; set; }

    [Column("CDTURMA"), StringLength(2)]
    public string? CdTurma { get; set; }

    [Column("CDCARGHOR"), StringLength(2)]
    public string? CdCargHor { get; set; }

    [Column("DTULTALT")]
    public DateTime DtUltAlt { get; set; }
}
