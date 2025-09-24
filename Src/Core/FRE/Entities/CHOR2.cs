using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.FRE.Entities;

/// <summary>Grade semanal do horário administrativo. Tabela: CHOR2</summary>
[Table("CHOR2")]
public class Chor2
{
    [Column("CDCARGHOR"), StringLength(2)] public string CdCargHor { get; set; } = string.Empty;
    [Column("DIADASEMANA")] public int DiaDaSemana { get; set; }
    [Column("HHENTRADA"), StringLength(5)] public string HhEntrada { get; set; } = string.Empty;
    [Column("HHSAIDA"), StringLength(5)] public string HhSaida { get; set; } = string.Empty;
    [Column("HHINIINT"), StringLength(5)] public string HhIniInt { get; set; } = string.Empty;
    [Column("HHFIMINT"), StringLength(5)] public string HhFimInt { get; set; } = string.Empty;
    [Column("FLHABILITADO")] public int FlHabilitado { get; set; }
    [Column("codhors1050"), StringLength(30)] public string? CodHors1050 { get; set; }
    [Column("id")] public Guid Id { get; set; }
    [Column("idhorarioadministrativo")] public Guid? IdHorarioAdministrativo { get; set; }
}
