using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// Situação do Colaborador (Ativo, Afastado, Demitido, etc.)
/// Tabela: tsitu1
/// </summary>
[GenerateCrud(
    TableName = "tsitu1",
    DisplayName = "Situação do Colaborador",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_SITUACAO",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("tsitu1")]
public class SituacaoColaborador
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdsituacao")]
    [StringLength(2)]
    [Display(Name = "Código")]
    public string Cdsituacao { get; set; } = string.Empty;

    [Column("dcsituacao")]
    [StringLength(40)]
    [Display(Name = "Descrição")]
    public string Dcsituacao { get; set; } = string.Empty;

    [Column("fldemissao")]
    [StringLength(1)]
    [Display(Name = "É Demissão")]
    public string? Fldemissao { get; set; }

    [Column("flafastame")]
    [StringLength(1)]
    [Display(Name = "É Afastamento")]
    public string? Flafastame { get; set; }

    [Column("qtdiasbene")]
    [Display(Name = "Dias Benefício")]
    public int? Qtdiasbene { get; set; }

    [Column("qtdiasprev")]
    [Display(Name = "Dias Previdência")]
    public int? Qtdiasprev { get; set; }

    [Column("cdfgts")]
    [StringLength(1)]
    [Display(Name = "Código FGTS")]
    public string? Cdfgts { get; set; }

    [Column("cdsefip")]
    [StringLength(1)]
    [Display(Name = "Código SEFIP")]
    public string? Cdsefip { get; set; }

    [Column("cdsefip2")]
    [StringLength(2)]
    [Display(Name = "Código SEFIP 2")]
    public string? Cdsefip2 { get; set; }

    [Column("flpferias")]
    [StringLength(1)]
    [Display(Name = "Perde Férias")]
    public string? Flpferias { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Collections (lado inverso dos relacionamentos)
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Colaboradores nesta situação
    /// </summary>
    [InverseProperty(nameof(Colaborador.Situacao))]
    public virtual ICollection<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();
}
