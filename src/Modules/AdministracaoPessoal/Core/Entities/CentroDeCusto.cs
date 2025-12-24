using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// Centro de Custo
/// Tabela: tcus1
/// </summary>
[GenerateCrud(
    TableName = "tcus1",
    DisplayName = "Centro de Custo",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_CENTRODECUSTO",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("tcus1")]
public class CentroDeCusto
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdccusto")]
    [StringLength(5)]
    [Display(Name = "Código")]
    public string Cdccusto { get; set; } = string.Empty;

    [Column("dcccusto")]
    [StringLength(100)]
    [Display(Name = "Descrição")]
    public string Dcccusto { get; set; } = string.Empty;

    [Column("sgccusto")]
    [StringLength(20)]
    [Display(Name = "Sigla")]
    public string? Sgccusto { get; set; }

    [Column("noccusto")]
    [StringLength(20)]
    [Display(Name = "Nome")]
    public string? Noccusto { get; set; }

    [Column("flativo")]
    [Display(Name = "Ativo")]
    public int? Flativo { get; set; }

    [Column("dcarea_cracha")]
    [StringLength(25)]
    [Display(Name = "Área Crachá")]
    public string? DcareaCracha { get; set; }

    [Column("dtbloqueio")]
    [Display(Name = "Data Bloqueio")]
    public DateTime? Dtbloqueio { get; set; }

    [Column("cdccresp")]
    [StringLength(20)]
    [Display(Name = "Responsável")]
    public string? Cdccresp { get; set; }

    [Column("flccusto")]
    [StringLength(1)]
    [Display(Name = "Flag Centro Custo")]
    public string? Flccusto { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Auto-referência (Centro de Custo Pai - Hierarquia)
    // ═══════════════════════════════════════════════════════════════════

    [Column("cdccusto_pai")]
    [StringLength(5)]
    [Display(Name = "Centro Custo Pai")]
    public string? CdccustoPai { get; set; }

    // NOTA: Não há FK por GUID para o pai no banco legado
    // Se precisar navegar, use serviço ou crie FK manual

    // ═══════════════════════════════════════════════════════════════════
    // Collections (lado inverso dos relacionamentos)
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Colaboradores alocados neste centro de custo
    /// </summary>
    [InverseProperty(nameof(Colaborador.CentroDeCusto))]
    public virtual ICollection<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();
}
