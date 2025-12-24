using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// Classificação Brasileira de Ocupações
/// Tabela: tcbo1
/// </summary>
[GenerateCrud(
    TableName = "tcbo1",
    DisplayName = "CBO",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_CBO",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("tcbo1")]
public class Cbo
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdcbo")]
    [StringLength(6)]
    [Display(Name = "Código CBO")]
    public string Cdcbo { get; set; } = string.Empty;

    [Column("dccbo")]
    [StringLength(80)]
    [Display(Name = "Descrição")]
    public string Dccbo { get; set; } = string.Empty;

    [Column("sinonimo")]
    [StringLength(4000)]
    [Display(Name = "Sinônimos")]
    public string? Sinonimo { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Collections (lado inverso dos relacionamentos)
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Cargos que usam este CBO
    /// </summary>
    [InverseProperty(nameof(Cargo.Cbo))]
    public virtual ICollection<Cargo> Cargos { get; set; } = new List<Cargo>();
}
