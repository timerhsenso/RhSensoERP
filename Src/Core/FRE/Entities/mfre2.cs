using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.FRE.Entities;

/// <summary>
/// Vincula motivos de ocorrência às filiais (escopo por empresa/filial).
/// Tabela: mfre2
/// </summary>
[Table("mfre2")]
public class Mfre2
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdmotoc"), StringLength(4)]
    public string CdMotoc { get; set; } = string.Empty;

    [Column("tpocorr")]
    public int TpOcorr { get; set; }

    [Column("cdempresa")]
    public int CdEmpresa { get; set; }

    [Column("cdfilial")]
    public int CdFilial { get; set; }

    [Column("idfilial")]
    public Guid? IdFilial { get; set; }

    [Column("idmotivosdeocorrenciafrequencia")]
    public Guid? IdMotivosDeOcorrenciaFrequencia { get; set; }
}
