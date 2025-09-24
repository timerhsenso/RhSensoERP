using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.FRE.Entities;

/// <summary>
/// Contém lançamentos do banco de horas (débito/crédito).
/// Tabela: BancoHoras
/// </summary>
[Table("BancoHoras")]
public class BancoHoras
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

    [Column("DATA")]
    public DateTime Data { get; set; }

    [Column("ORDEM")]
    public int Ordem { get; set; }

    [Column("TEMPO")]
    public int Tempo { get; set; }

    [Column("DEBCRED"), StringLength(1)]
    public string DebCred { get; set; } = string.Empty;

    [Column("TIPO"), StringLength(1)]
    public string Tipo { get; set; } = string.Empty;

    [Column("DESCRICAO"), StringLength(100)]
    public string? Descricao { get; set; }

    [Column("CDCONTA"), StringLength(4)]
    public string? CdConta { get; set; }

    [Column("SALDOANTERIOR")]
    public int SaldoAnterior { get; set; }

    [Column("DATA_FREQ1")]
    public DateTime? DataFreq1 { get; set; }

    [Column("INICIO_FREQ1")]
    public DateTime? InicioFreq1 { get; set; }
}
