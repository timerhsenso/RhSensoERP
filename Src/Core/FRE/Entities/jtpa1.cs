using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.FRE.Entities;

/// <summary>Tabela de jornadas por ano (qtd de horas por mês). Tabela: jtpa1</summary>
[Table("jtpa1")]
public class Jtpa1
{
    [Column("cdempresa")] public int CdEmpresa { get; set; }
    [Column("cdfilial")] public int CdFilial { get; set; }
    [Column("tpjornada"), StringLength(1)] public string TpJornada { get; set; } = string.Empty;
    [Column("aajornada"), StringLength(4)] public string AaJornada { get; set; } = string.Empty;

    [Column("janeiro")] public double? Janeiro { get; set; }
    [Column("fevereiro")] public double? Fevereiro { get; set; }
    [Column("marco")] public double? Marco { get; set; }
    [Column("abril")] public double? Abril { get; set; }
    [Column("maio")] public double? Maio { get; set; }
    [Column("junho")] public double? Junho { get; set; }
    [Column("julho")] public double? Julho { get; set; }
    [Column("agosto")] public double? Agosto { get; set; }
    [Column("setembro")] public double? Setembro { get; set; }
    [Column("outubro")] public double? Outubro { get; set; }
    [Column("novembro")] public double? Novembro { get; set; }
    [Column("dezembro")] public double? Dezembro { get; set; }

    [Column("id")] public Guid Id { get; set; }
    [Column("idfilial")] public Guid? IdFilial { get; set; }
}
