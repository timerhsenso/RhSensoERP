using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.FRE.Entities;

/// <summary>Jornada mensal (horas previstas). Tabela: jornada</summary>
[Table("jornada")]
public class Jornada
{
    [Column("cdempresa")] public int CdEmpresa { get; set; }
    [Column("cdfilial")] public int CdFilial { get; set; }
    [Column("tpjornada"), StringLength(1)] public string TpJornada { get; set; } = string.Empty;
    [Column("ano")] public int Ano { get; set; }
    [Column("mes")] public int Mes { get; set; }
    [Column("qthoras")] public double? QtHoras { get; set; }
    [Column("dtref")] public DateTime? DtRef { get; set; }
}
