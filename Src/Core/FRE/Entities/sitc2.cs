using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.FRE.Entities;

/// <summary>
/// Situação de processamento da frequência por colaborador/dia.
/// Tabela: sitc2
/// </summary>
[Table("sitc2")]
public class Sitc2
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdempresa")]
    public int CdEmpresa { get; set; }

    [Column("cdfilial")]
    public int CdFilial { get; set; }

    [Column("nomatric"), StringLength(8)]
    public string NoMatric { get; set; } = string.Empty;

    [Column("dtfrequen")]
    public DateTime DtFrequen { get; set; }

    [Column("flsituacao")]
    public int FlSituacao { get; set; }

    [Column("cdusuario"), StringLength(20)]
    public string? CdUsuario { get; set; }

    [Column("dtultmov")]
    public DateTime DtUltMov { get; set; }

    [Column("FLPROCESSADO")]
    public int FlProcessado { get; set; }

    [Column("FLIMPORTADO")]
    public int FlImportado { get; set; }

    [Column("DTIMPORTACAO")]
    public DateTime? DtImportacao { get; set; }

    [Column("DTPROCESSAMENTO")]
    public DateTime? DtProcessamento { get; set; }

    [Column("idfuncionario")]
    public Guid? IdFuncionario { get; set; }
}
