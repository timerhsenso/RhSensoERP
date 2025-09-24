using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.FRE.Entities;

/// <summary>
/// Contém as faltas/saídas antecipadas e justificativas.
/// Tabela: FALTASANTECIPADAS
/// </summary>
[Table("FALTASANTECIPADAS")]
public class FaltasAntecipadas
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("CDEMPRESA")]
    public int CdEmpresa { get; set; }

    [Column("CDFILIAL")]
    public int CdFilial { get; set; }

    [Column("NOMATRIC"), StringLength(8)]
    public string NoMatric { get; set; } = string.Empty;

    [Column("DATAINICIO")]
    public DateTime DataInicio { get; set; }

    [Column("DATAFIM")]
    public DateTime DataFim { get; set; }

    [Column("TPOCORR")]
    public int TpOcorr { get; set; }

    [Column("CDMOTOC"), StringLength(4)]
    public string CdMotoc { get; set; } = string.Empty;

    [Column("TIPO"), StringLength(1)]
    public string Tipo { get; set; } = string.Empty;

    [Column("HORAINICIO"), StringLength(5)]
    public string? HoraInicio { get; set; }

    [Column("HORAFIM"), StringLength(5)]
    public string? HoraFim { get; set; }

    [Column("TEMPO")]
    public int? Tempo { get; set; }

    [Column("id_guid")]
    public Guid IdGuid { get; set; }

    [Column("idfuncionario")]
    public Guid? IdFuncionario { get; set; }

    [Column("idmotivosdeocorrenciafrequencia")]
    public Guid? IdMotivosDeOcorrenciaFrequencia { get; set; }
}
