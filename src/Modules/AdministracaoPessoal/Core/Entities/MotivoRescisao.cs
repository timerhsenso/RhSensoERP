using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// Motivo de Rescisão
/// Tabela: tcre1
/// </summary>
[GenerateCrud(
    TableName = "tcre1",
    DisplayName = "Motivo de Rescisão",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_MOTIVORESCISAO",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("tcre1")]
public class MotivoRescisao
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdrescisao")]
    [StringLength(2)]
    [Display(Name = "Código")]
    public string Cdrescisao { get; set; } = string.Empty;

    [Column("dcrescisao")]
    [StringLength(120)]
    [Display(Name = "Descrição")]
    public string Dcrescisao { get; set; } = string.Empty;

    [Column("cdfgts")]
    [StringLength(1)]
    [Display(Name = "Código FGTS")]
    public string? Cdfgts { get; set; }

    [Column("chaviso")]
    [StringLength(1)]
    [Display(Name = "Aviso Prévio")]
    public string? Chaviso { get; set; }

    [Column("cdrais")]
    [StringLength(2)]
    [Display(Name = "Código RAIS")]
    public string? Cdrais { get; set; }

    [Column("dcreduzida")]
    [StringLength(80)]
    [Display(Name = "Descrição Reduzida")]
    public string? Dcreduzida { get; set; }

    [Column("cdsaque")]
    [StringLength(2)]
    [Display(Name = "Código Saque")]
    public string? Cdsaque { get; set; }

    [Column("cdcaged")]
    [StringLength(2)]
    [Display(Name = "Código CAGED")]
    public string? Cdcaged { get; set; }

    [Column("cdsefip")]
    [StringLength(2)]
    [Display(Name = "Código SEFIP")]
    public string? Cdsefip { get; set; }

    [Column("chrecgrrf")]
    [StringLength(1)]
    [Display(Name = "Recolhe GRRF")]
    public string? Chrecgrrf { get; set; }

    [Column("cdafastrct")]
    [StringLength(5)]
    [Display(Name = "Afastamento RCT")]
    public string? Cdafastrct { get; set; }

    [Column("cod_esocial")]
    [StringLength(2)]
    [Display(Name = "Código eSocial")]
    public string? CodEsocial { get; set; }

    [Column("fltermcontr")]
    [StringLength(1)]
    [Display(Name = "Término Contrato")]
    public string? Fltermcontr { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Collections (lado inverso dos relacionamentos)
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Colaboradores demitidos por este motivo
    /// </summary>
    [InverseProperty(nameof(Colaborador.MotivoRescisao))]
    public virtual ICollection<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();
}
