using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// Grau de Instrução / Escolaridade
/// Tabela: tgin1
/// </summary>
[GenerateCrud(
    TableName = "tgin1",
    DisplayName = "Grau de Instrução",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_GRAUINSTRUCAO",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("tgin1")]
public class GrauInstrucao
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdinstruc")]
    [StringLength(2)]
    [Display(Name = "Código")]
    public string Cdinstruc { get; set; } = string.Empty;

    [Column("dcinstruc")]
    [StringLength(40)]
    [Display(Name = "Descrição")]
    public string Dcinstruc { get; set; } = string.Empty;

    [Column("cdrais")]
    [StringLength(2)]
    [Display(Name = "Código RAIS")]
    public string? Cdrais { get; set; }

    [Column("cdcaged")]
    [StringLength(2)]
    [Display(Name = "Código CAGED")]
    public string? Cdcaged { get; set; }

    [Column("cdesocial")]
    [StringLength(2)]
    [Display(Name = "Código eSocial")]
    public string? Cdesocial { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Collections (lado inverso dos relacionamentos)
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Colaboradores com este grau de instrução
    /// </summary>
    [InverseProperty(nameof(Colaborador.GrauInstrucao))]
    public virtual ICollection<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();

    /// <summary>
    /// Cargos que exigem este grau de instrução
    /// </summary>
    [InverseProperty(nameof(Cargo.GrauInstrucao))]
    public virtual ICollection<Cargo> Cargos { get; set; } = new List<Cargo>();
}
