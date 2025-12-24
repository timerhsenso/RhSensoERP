using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// BasHistoricoValidacoes - Histórico de Validações de Acesso
/// Tabela: bas_historico_validacoes (Multi-tenant, Append-Only)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "bas_historico_validacoes",
    DisplayName = "BasHistoricoValidacoes",
    CdSistema = "BAS",
    CdFuncao = "BAS_FM_HISTORICOVALIDACOES",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("bas_historico_validacoes")]
public class BasHistoricoValidacoes
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "ID")]
    public long Id { get; set; }

    [Required]
    [Column("TenantId")]
    [Display(Name = "Tenant ID")]
    public Guid TenantId { get; set; }

    [Required]
    [Column("IdAcessoPortaria")]
    [Display(Name = "ID Acesso Portaria")]
    public int IdAcessoPortaria { get; set; }

    [Column("IdMotivoBloqueio")]
    [Display(Name = "ID Motivo Bloqueio")]
    public int? IdMotivoBloqueio { get; set; }

    [Column("Validacao", TypeName = "nvarchar(50)")]
    [StringLength(50)]
    [Display(Name = "Validação")]
    public string? Validacao { get; set; }

    [Column("Resultado", TypeName = "nvarchar(50)")]
    [StringLength(50)]
    [Display(Name = "Resultado")]
    public string? Resultado { get; set; }

    [Column("Detalhes", TypeName = "nvarchar(max)")]
    [Display(Name = "Detalhes")]
    public string? Detalhes { get; set; }

    // Default controlado pelo banco: SYSUTCDATETIME()
    [Required]
    [Column("DataValidacao", TypeName = "datetime2(3)")]
    [Display(Name = "Data Validação (UTC)")]
    public DateTime DataValidacao { get; set; }

    // Navigation Properties
    // ⚠️ Descomentar quando existir a entidade/tabela gtc_acessos_portaria
    // [ForeignKey(nameof(IdAcessoPortaria))]
    // public virtual GtcAcessosPortaria? AcessoPortaria { get; set; }

    [ForeignKey(nameof(IdMotivoBloqueio))]
    public virtual BasMotivosBloqueio? MotivoBloqueio { get; set; }
}
