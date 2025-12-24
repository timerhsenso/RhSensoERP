using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// Agência Bancária
/// Tabela: tage1
/// </summary>
[GenerateCrud(
    TableName = "tage1",
    DisplayName = "Agência Bancária",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_AGENCIA",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("tage1")]
public class AgenciaBancaria
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdbanco")]
    [StringLength(3)]
    [Display(Name = "Código Banco")]
    public string Cdbanco { get; set; } = string.Empty;

    [Column("cdagencia")]
    [StringLength(4)]
    [Display(Name = "Código Agência")]
    public string Cdagencia { get; set; } = string.Empty;

    [Column("dvagencia")]
    [StringLength(1)]
    [Display(Name = "Dígito")]
    public string? Dvagencia { get; set; }

    [Column("nmagencia")]
    [StringLength(40)]
    [Display(Name = "Nome")]
    public string Nmagencia { get; set; } = string.Empty;

    [Column("cdmunicip")]
    [StringLength(5)]
    [Display(Name = "Código Município")]
    public string? Cdmunicip { get; set; }

    [Column("noconta")]
    [StringLength(15)]
    [Display(Name = "Conta")]
    public string? Noconta { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // FK para Banco
    // ═══════════════════════════════════════════════════════════════════

    [Column("idbanco")]
    public Guid? Idbanco { get; set; }

    [ForeignKey(nameof(Idbanco))]
    public virtual Banco? Banco { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Collections (lado inverso dos relacionamentos)
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Colaboradores que recebem por esta agência
    /// </summary>
    [InverseProperty(nameof(Colaborador.AgenciaRecebimento))]
    public virtual ICollection<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();
}
