using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.FRE.Entities;

/// <summary>
/// Controle de processamento diário de frequência por filial.
/// Tabela: freq3
/// </summary>
[Table("freq3")]
public class Freq3
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdempresa")]
    public int CdEmpresa { get; set; }

    [Column("cdfilial")]
    public int CdFilial { get; set; }

    [Column("dtfrequen")]
    public DateTime DtFrequen { get; set; }

    [Column("flfreq")]
    public int FlFreq { get; set; }

    [Column("idfilial")]
    public Guid? IdFilial { get; set; }
}
