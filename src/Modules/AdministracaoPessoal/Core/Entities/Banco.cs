using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// Banco
/// Tabela: tban1
/// </summary>
[GenerateCrud(
    TableName = "tban1",
    DisplayName = "Banco",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_BANCO",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("tban1")]
public class Banco
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdbanco")]
    [StringLength(3)]
    [Display(Name = "Código")]
    public string Cdbanco { get; set; } = string.Empty;

    [Column("dcbanco")]
    [StringLength(40)]
    [Display(Name = "Descrição")]
    public string Dcbanco { get; set; } = string.Empty;

    // ═══════════════════════════════════════════════════════════════════
    // Collections (lado inverso dos relacionamentos)
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Agências deste banco
    /// </summary>
    [InverseProperty(nameof(AgenciaBancaria.Banco))]
    public virtual ICollection<AgenciaBancaria> Agencias { get; set; } = new List<AgenciaBancaria>();

    /// <summary>
    /// Colaboradores que recebem por este banco
    /// </summary>
    [InverseProperty(nameof(Colaborador.BancoRecebimento))]
    public virtual ICollection<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();
}
