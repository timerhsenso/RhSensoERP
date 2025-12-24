using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// BasMotivosBloqueio - Motivos de Bloqueio Padronizados
/// Tabela: bas_motivos_bloqueio (Multi-tenant)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "bas_motivos_bloqueio",
    DisplayName = "BasMotivosBloqueio",
    CdSistema = "BAS",
    CdFuncao = "BAS_FM_MOTIVOSBLOQUEIO",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("bas_motivos_bloqueio")]
public class BasMotivosBloqueio
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
    [Column("Codigo", TypeName = "nvarchar(50)")]
    [StringLength(50)]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [Column("Descricao", TypeName = "nvarchar(500)")]
    [StringLength(500)]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = string.Empty;

    [Column("Categoria", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Categoria")]
    public string? Categoria { get; set; }

    [Column("Severidade", TypeName = "nvarchar(50)")]
    [StringLength(50)]
    [Display(Name = "Severidade")]
    public string? Severidade { get; set; }

    [Required]
    [Column("Ativo")]
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

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

    // Navigation Properties
  //  [ForeignKey(nameof(CreatedByUserId))]
  //  public virtual Usuario? CreatedByUser { get; set; }

 //   [ForeignKey(nameof(UpdatedByUserId))]
 //   public virtual Usuario? UpdatedByUser { get; set; }

    // Inverse Navigation
    [InverseProperty(nameof(BasHistoricoValidacoes.MotivoBloqueio))]
    public virtual ICollection<BasHistoricoValidacoes> HistoricoValidacoes { get; set; } = new List<BasHistoricoValidacoes>();
}