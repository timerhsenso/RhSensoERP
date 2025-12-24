using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// Cargo
/// Tabela: cargo1
/// </summary>
[GenerateCrud(
    TableName = "cargo1",
    DisplayName = "Cargo",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_CARGO",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("cargo1")]
public class Cargo
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdcargo")]
    [StringLength(5)]
    [Display(Name = "Código")]
    public string Cdcargo { get; set; } = string.Empty;

    [Column("dccargo")]
    [StringLength(50)]
    [Display(Name = "Descrição")]
    public string Dccargo { get; set; } = string.Empty;

    [Column("flativo")]
    [Display(Name = "Ativo")]
    public int Flativo { get; set; }

    [Column("dtinival")]
    [Display(Name = "Data Início Validade")]
    public DateTime? Dtinival { get; set; }

    [Column("dtfimval")]
    [Display(Name = "Data Fim Validade")]
    public DateTime? Dtfimval { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Níveis Salariais
    // ═══════════════════════════════════════════════════════════════════

    [Column("cdtabela")]
    [StringLength(3)]
    [Display(Name = "Código Tabela Salarial")]
    public string? Cdtabela { get; set; }

    [Column("cdniveini")]
    [StringLength(5)]
    [Display(Name = "Nível Inicial")]
    public string? Cdniveini { get; set; }

    [Column("cdnivefim")]
    [StringLength(5)]
    [Display(Name = "Nível Final")]
    public string? Cdnivefim { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Códigos Legados
    // ═══════════════════════════════════════════════════════════════════

    [Column("cdinstruc")]
    [StringLength(2)]
    [Display(Name = "Código Instrução")]
    public string? Cdinstruc { get; set; }

    [Column("cdcbo")]
    [StringLength(5)]
    [Display(Name = "Código CBO (5 dígitos)")]
    public string? Cdcbo { get; set; }

    [Column("cdcbo6")]
    [StringLength(6)]
    [Display(Name = "Código CBO (6 dígitos)")]
    public string? Cdcbo6 { get; set; }

    [Column("cdgrprof")]
    [StringLength(2)]
    [Display(Name = "Grupo Profissional")]
    public string? Cdgrprof { get; set; }

    [Column("Tenant")]
    [Display(Name = "Tenant")]
    public int Tenant { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // FKs (GUIDs)
    // ═══════════════════════════════════════════════════════════════════

    [Column("idcbo")]
    public Guid? Idcbo { get; set; }

    [Column("idgraudeinstrucao")]
    public Guid? Idgraudeinstrucao { get; set; }

    [Column("idtabelasalarial")]
    public Guid? Idtabelasalarial { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Navigation Properties (apenas 1 por FK!)
    // ═══════════════════════════════════════════════════════════════════

    [ForeignKey(nameof(Idcbo))]
    public virtual Cbo? Cbo { get; set; }

    [ForeignKey(nameof(Idgraudeinstrucao))]
    public virtual GrauInstrucao? GrauInstrucao { get; set; }

    [ForeignKey(nameof(Idtabelasalarial))]
    public virtual TabelaSalarial? TabelaSalarial { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Collections (lado inverso dos relacionamentos)
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Colaboradores neste cargo
    /// </summary>
    [InverseProperty(nameof(Colaborador.Cargo))]
    public virtual ICollection<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();
}
