using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.FRE.Entities;

/// <summary>Períodos de compensação vinculados ao cabeçalho. Tabela: COMP2</summary>
[Table("COMP2")]
public class Comp2
{
    [Column("IDCOMP")] public int IdComp { get; set; }
    [Column("INICIO")] public DateTime Inicio { get; set; }
    [Column("FIM")] public DateTime Fim { get; set; }
    [Column("TPOCORR")] public int TpOcorr { get; set; }
    [Column("CDMOTOC"), StringLength(4)] public string CdMotoc { get; set; } = string.Empty;
}
