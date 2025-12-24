using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// BasConfiguracoes - Configurações do Sistema
/// Tabela: bas_configuracoes (Multi-tenant)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "bas_configuracoes",
    DisplayName = "BasConfiguracoes",
    CdSistema = "BAS",
    CdFuncao = "BAS_FM_CONFIGURACOES",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("bas_configuracoes")]
public class BasConfiguracoes
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "ID")]
    public int Id { get; set; }

    [Required]
    [Column("TenantId")]
    [Display(Name = "Tenant ID")]
    public Guid TenantId { get; set; }

    [Required]
    [Column("Chave", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Chave")]
    public string Chave { get; set; } = string.Empty;

    [Required]
    [Column("Valor", TypeName = "nvarchar(max)")]
    [Display(Name = "Valor")]
    public string Valor { get; set; } = string.Empty;

    [Column("Descricao", TypeName = "nvarchar(500)")]
    [StringLength(500)]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Column("TipoDado", TypeName = "nvarchar(50)")]
    [StringLength(50)]
    [Display(Name = "Tipo de Dado")]
    public string? TipoDado { get; set; }

    // Auditoria — defaults controlados pelo banco (SYSUTCDATETIME())
    [Required]
    [Column("CreatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Data Criação (UTC)")]
    public DateTime CreatedAtUtc { get; set; }

    [Column("CreatedByUserId")]
    [Display(Name = "Criado Por")]
    public Guid? CreatedByUserId { get; set; }

    [Required]
    [Column("UpdatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Data Atualização (UTC)")]
    public DateTime UpdatedAtUtc { get; set; }

    [Column("UpdatedByUserId")]
    [Display(Name = "Atualizado Por")]
    public Guid? UpdatedByUserId { get; set; }

    // Navigation Properties (ativar quando a entidade Tuse1 existir no Core)
    // [ForeignKey(nameof(CreatedByUserId))]
    // public virtual Tuse1? CreatedByUser { get; set; }

    // [ForeignKey(nameof(UpdatedByUserId))]
    // public virtual Tuse1? UpdatedByUser { get; set; }
}
