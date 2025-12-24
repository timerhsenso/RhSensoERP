using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// Vínculo Empregatício
/// Tabela: tvin1
/// </summary>
[GenerateCrud(
    TableName = "tvin1",
    DisplayName = "Vínculo Empregatício",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_VINCULO",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("tvin1")]
public class VinculoEmpregaticio
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdvincul")]
    [StringLength(2)]
    [Display(Name = "Código")]
    public string Cdvincul { get; set; } = string.Empty;

    [Column("dcvincul")]
    [StringLength(120)]
    [Display(Name = "Descrição")]
    public string Dcvincul { get; set; } = string.Empty;

    [Column("cdsefip")]
    [StringLength(2)]
    [Display(Name = "Código SEFIP")]
    public string? Cdsefip { get; set; }

    [Column("cdclasse")]
    [StringLength(2)]
    [Display(Name = "Classe")]
    public string? Cdclasse { get; set; }

    [Column("flrais")]
    [Display(Name = "RAIS")]
    public int Flrais { get; set; }

    [Column("natatividade")]
    [Display(Name = "Natureza Atividade")]
    public int Natatividade { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Collections (lado inverso dos relacionamentos)
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Colaboradores com este vínculo
    /// </summary>
    [InverseProperty(nameof(Colaborador.VinculoEmpregaticio))]
    public virtual ICollection<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();
}
