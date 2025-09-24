using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.FRE.Entities;

/// <summary>Batidas com intervalos intrajornada por dia. Tabela: FREQ4</summary>
[Table("FREQ4")]
public class Freq4
{
    [Column("NOMATRIC"), StringLength(8)] public string NoMatric { get; set; } = string.Empty;
    [Column("CDEMPRESA")] public int CdEmpresa { get; set; }
    [Column("CDFILIAL")] public int CdFilial { get; set; }
    [Column("DATA")] public DateTime Data { get; set; }
    [Column("INICIO")] public DateTime Inicio { get; set; }
    [Column("FIM")] public DateTime? Fim { get; set; }
    [Column("INICIOINTERVALO")] public DateTime? InicioIntervalo { get; set; }
    [Column("FIMINTERVALO")] public DateTime? FimIntervalo { get; set; }
    [Column("FLINTERVALO"), StringLength(1)] public string? FlIntervalo { get; set; }
    [Column("INICIO_OLD"), StringLength(5)] public string? InicioOld { get; set; }
    [Column("FIM_OLD"), StringLength(5)] public string? FimOld { get; set; }
    [Column("INICIOINTERVALO_OLD"), StringLength(5)] public string? InicioIntervaloOld { get; set; }
    [Column("FIMINTERVALO_OLD"), StringLength(5)] public string? FimIntervaloOld { get; set; }
}
