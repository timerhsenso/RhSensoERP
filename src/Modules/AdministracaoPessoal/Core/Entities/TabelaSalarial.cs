using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// Tabela Salarial
/// Tabela: tsal1
/// </summary>
[GenerateCrud(
    TableName = "tsal1",
    DisplayName = "Tabela Salarial",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_TABELASALARIAL",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("tsal1")]
public class TabelaSalarial
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdtabela")]
    [StringLength(3)]
    [Display(Name = "Código")]
    public string Cdtabela { get; set; } = string.Empty;

    [Column("dctabela")]
    [StringLength(220)]
    [Display(Name = "Descrição")]
    public string? Dctabela { get; set; }

    [Column("flseq")]
    [StringLength(1)]
    [Display(Name = "Sequencial")]
    public string? Flseq { get; set; }

    [Column("vlsalinicial")]
    [Display(Name = "Salário Inicial")]
    public decimal? Vlsalinicial { get; set; }

    [Column("vlsalmediana")]
    [Display(Name = "Salário Mediana")]
    public decimal? Vlsalmediana { get; set; }

    [Column("vlsalmaximo")]
    [Display(Name = "Salário Máximo")]
    public decimal? Vlsalmaximo { get; set; }

    [Column("dtvalidade")]
    [Display(Name = "Data Validade")]
    public DateTime? Dtvalidade { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Auto-referência (Tabela de Validade)
    // ═══════════════════════════════════════════════════════════════════

    [Column("idtsalvalidade")]
    public Guid? Idtsalvalidade { get; set; }

    [ForeignKey(nameof(Idtsalvalidade))]
    public virtual TabelaSalarial? TabelaValidade { get; set; }

    // Campo legado - não usado
    [Column("tsalvalidade_id")]
    public decimal? TsalvalidadeId { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Collections (lado inverso dos relacionamentos)
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Cargos que usam esta tabela salarial
    /// </summary>
    [InverseProperty(nameof(Cargo.TabelaSalarial))]
    public virtual ICollection<Cargo> Cargos { get; set; } = new List<Cargo>();
}
