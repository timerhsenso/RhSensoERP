using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.FRE.Entities;

/// <summary>Histórico de alterações de jornada do colaborador. Tabela: hjor1</summary>
[Table("hjor1")]
public class Hjor1
{
    [Column("nomatric"), StringLength(8)] public string NoMatric { get; set; } = string.Empty;
    [Column("cdempresa")] public int CdEmpresa { get; set; }
    [Column("cdfilial")] public int CdFilial { get; set; }
    [Column("dtmudanca")] public DateTime DtMudanca { get; set; }
    [Column("tpjornada"), StringLength(1)] public string? TpJornada { get; set; }
    [Column("cdcarghor"), StringLength(2)] public string? CdCargHor { get; set; }
    [Column("cdusuario"), StringLength(20)] public string? CdUsuario { get; set; }
    [Column("dtultmov")] public DateTime? DtUltMov { get; set; }
    [Column("dcdoc"), StringLength(20)] public string? DcDoc { get; set; }
    [Column("cdregime"), StringLength(10)] public string? CdRegime { get; set; }
    [Column("hespecial"), StringLength(1)] public string? HEspecial { get; set; }
    [Column("id")] public Guid Id { get; set; }
    [Column("idfuncionario")] public Guid? IdFuncionario { get; set; }
    [Column("tpjornadaesocial")] public int? TpJornadaEsocial { get; set; }
}
