using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.FRE.Entities;

/// <summary>Lançamentos de frequência (ocorrências) diárias. Tabela: freq1</summary>
[Table("freq1")]
public class Freq1
{
    [Column("nomatric"), StringLength(8)] public string NoMatric { get; set; } = string.Empty;
    [Column("cdempresa")] public int CdEmpresa { get; set; }
    [Column("cdfilial")] public int CdFilial { get; set; }
    [Column("dtocorr")] public DateTime DtOcorr { get; set; }
    [Column("tpocorr")] public int TpOcorr { get; set; }
    [Column("cdmotoc"), StringLength(4)] public string CdMotoc { get; set; } = string.Empty;
    [Column("hhiniocor")] public DateTime HhIniOcor { get; set; }
    [Column("hhfimocor")] public DateTime HhFimOcor { get; set; }
    [Column("dtfrequen")] public DateTime DtFrequen { get; set; }
    [Column("qthorocor")] public int? QtHorOcor { get; set; }
    [Column("qtabonada")] public int? QtAbonada { get; set; }
    [Column("flaprovhe")] public int? FlAprovHe { get; set; }
    [Column("flexporta")] public int? FlExporta { get; set; }
    [Column("qtadicion")] public int? QtAdicion { get; set; }
    [Column("txocorr"), StringLength(80)] public string? TxOcorr { get; set; }
    [Column("cdccusres"), StringLength(5)] public string? CdCcUsRes { get; set; }
    [Column("cdusuario"), StringLength(20)] public string? CdUsuario { get; set; }
    [Column("dtultmov")] public DateTime? DtUltMov { get; set; }
    [Column("dttroca")] public DateTime? DtTroca { get; set; }
    [Column("nomattroc"), StringLength(8)] public string? NoMatTroc { get; set; }
    [Column("cdusaprhe"), StringLength(20)] public string? CdUsAprHe { get; set; }
    [Column("noprocesso"), StringLength(6)] public string? NoProcesso { get; set; }
    [Column("IMPORTADO")] public int Importado { get; set; }
    [Column("CONTRAPESOTROCA")] public int ContraPesoTroca { get; set; }
    [Column("FALTOUDIA")] public int FaltouDia { get; set; }
    [Column("FLBANCOHORAS")] public int FlBancoHoras { get; set; }
    [Column("QTMINDESCFDS")] public int QtMinDescFds { get; set; }
    [Column("HHINIOCOR_OLD"), StringLength(4)] public string? HhIniOcorOld { get; set; }
    [Column("HHFIMOCOR_OLD"), StringLength(4)] public string? HhFimOcorOld { get; set; }
    [Column("CDMOTOCDEFAULT"), StringLength(4)] public string? CdMotocDefault { get; set; }
    [Column("IDBANCOHORAS")] public int? IdBancoHoras { get; set; }
    [Column("FLHEPJ")] public int FlHePj { get; set; }
    [Column("HHINITROCA")] public DateTime? HhIniTroca { get; set; }
    [Column("id")] public Guid Id { get; set; }
    [Column("cdusuarioaceito"), StringLength(20)] public string? CdUsuarioAceito { get; set; }
    [Column("cdusuarioautoriza"), StringLength(30)] public string? CdUsuarioAutoriza { get; set; }
    [Column("cod_justific"), StringLength(4)] public string? CodJustific { get; set; }
    [Column("dtaceito")] public DateTime? DtAceito { get; set; }
    [Column("DTautoriza")] public DateTime? DtAutoriza { get; set; }
    [Column("flaceito")] public int? FlAceito { get; set; }
    [Column("flalmoco")] public int? FlAlmoco { get; set; }
    [Column("flautorizado")] public int? FlAutorizado { get; set; }
    [Column("idmotivosdeocorrenciafrequencia")] public Guid? IdMotivosDeOcorrenciaFrequencia { get; set; }
}
