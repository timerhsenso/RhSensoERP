using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.FRE.Entities;

/// <summary>Registra as batidas de ponto por colaborador. Tabela: BATIDAS</summary>
[Table("BATIDAS")]
public class Batidas
{
    [Column("CDEMPRESA")] public int CdEmpresa { get; set; }
    [Column("CDFILIAL")] public int CdFilial { get; set; }
    [Column("NOMATRIC"), StringLength(8)] public string NoMatric { get; set; } = string.Empty;
    [Column("DATA")] public DateTime Data { get; set; }
    [Column("HORA"), StringLength(5)] public string Hora { get; set; } = string.Empty;
    [Column("TIPO"), StringLength(2)] public string Tipo { get; set; } = string.Empty;
    [Column("ERRO"), StringLength(10)] public string Erro { get; set; } = "0000000000";
    [Column("IMPORTADO")] public int Importado { get; set; } = 0;
    [Column("MOTIVO"), StringLength(200)] public string? Motivo { get; set; }
    [Column("id_guid")] public Guid IdGuid { get; set; }
    [Column("idfuncionario")] public Guid? IdFuncionario { get; set; }
}
