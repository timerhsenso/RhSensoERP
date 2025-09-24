using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.FRE.Entities;

/// <summary>Motivos de ocorrência de frequência. Tabela: mfre1</summary>
[Table("mfre1")]
public class Mfre1
{
    [Column("cdmotoc"), StringLength(4)] public string CdMotoc { get; set; } = string.Empty;
    [Column("tpocorr")] public int TpOcorr { get; set; }
    [Column("dcmotoc"), StringLength(40)] public string? DcMotoc { get; set; }
    [Column("flmovimen")] public int? FlMovimen { get; set; }
    [Column("cdconta"), StringLength(4)] public string? CdConta { get; set; }
    [Column("fltpfal")] public int? FlTpFal { get; set; }
    [Column("flextra")] public int? FlExtra { get; set; }
    [Column("flflanj")] public int? FlFlAnj { get; set; }
    [Column("FLTROCA")] public int? FlTroca { get; set; }
    [Column("FLREGRAHE")] public int? FlRegraHe { get; set; }
    [Column("FLBANCOHORAS")] public int FlBancoHoras { get; set; }
    [Column("TPOCORRLINK")] public int? TpOcorrLink { get; set; }
    [Column("CDMOTOCLINK"), StringLength(4)] public string? CdMotocLink { get; set; }
    [Column("id")] public Guid Id { get; set; }
    [Column("idmotivosdeocorrenciafrequenciapai")] public Guid? IdMotivosDeOcorrenciaFrequenciaPai { get; set; }
    [Column("idverba")] public Guid? IdVerba { get; set; }
}
