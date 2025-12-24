using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// Sindicato
/// Tabela: sind1
/// </summary>
[GenerateCrud(
    TableName = "sind1",
    DisplayName = "Sindicato",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_SINDICATO",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("sind1")]
public class Sindicato
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdsindicat")]
    [StringLength(2)]
    [Display(Name = "Código")]
    public string Cdsindicat { get; set; } = string.Empty;

    [Column("dcsindicat")]
    [StringLength(40)]
    [Display(Name = "Descrição")]
    public string Dcsindicat { get; set; } = string.Empty;

    [Column("dcendereco")]
    [StringLength(30)]
    [Display(Name = "Endereço")]
    public string? Dcendereco { get; set; }

    [Column("cgcsindicat")]
    [StringLength(14)]
    [Display(Name = "CNPJ")]
    public string? Cgcsindicat { get; set; }

    [Column("cdentidade")]
    [StringLength(20)]
    [Display(Name = "Código Entidade")]
    public string? Cdentidade { get; set; }

    [Column("data_base")]
    [StringLength(2)]
    [Display(Name = "Data Base")]
    public string? DataBase { get; set; }

    [Column("fltipo")]
    [Display(Name = "Tipo")]
    public int? Fltipo { get; set; }

    [Column("cdtabbase")]
    [StringLength(3)]
    [Display(Name = "Tabela Base")]
    public string? Cdtabbase { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Collections (lado inverso dos relacionamentos)
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Colaboradores filiados a este sindicato
    /// </summary>
    [InverseProperty(nameof(Colaborador.Sindicato))]
    public virtual ICollection<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();

    /// <summary>
    /// Filiais vinculadas a este sindicato
    /// </summary>
    [InverseProperty(nameof(Filial.Sindicato))]
    public virtual ICollection<Filial> Filiais { get; set; } = new List<Filial>();
}
