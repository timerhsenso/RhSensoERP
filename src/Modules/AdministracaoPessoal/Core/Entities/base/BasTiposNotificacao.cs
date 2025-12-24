using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// BasTiposNotificacao - Tipos de Notificação do Sistema
/// Tabela: bas_tipos_notificacao (Multi-tenant)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "bas_tipos_notificacao",
    DisplayName = "BasTiposNotificacao",
    CdSistema = "BAS",
    CdFuncao = "BAS_FM_TIPOSNOTIFICACAO",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("bas_tipos_notificacao")]
public class BasTiposNotificacao
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
    [Column("Nome", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("Descricao", TypeName = "nvarchar(500)")]
    [StringLength(500)]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Column("TemplateTitulo", TypeName = "nvarchar(255)")]
    [StringLength(255)]
    [Display(Name = "Template Título")]
    public string? TemplateTitulo { get; set; }

    [Column("TemplateMensagem", TypeName = "nvarchar(max)")]
    [Display(Name = "Template Mensagem")]
    public string? TemplateMensagem { get; set; }

    [Column("Categoria", TypeName = "nvarchar(50)")]
    [StringLength(50)]
    [Display(Name = "Categoria")]
    public string? Categoria { get; set; }

    [Column("Severidade", TypeName = "nvarchar(20)")]
    [StringLength(20)]
    [Display(Name = "Severidade")]
    public string? Severidade { get; set; }

    [Required]
    [Column("EnviaEmail")]
    [Display(Name = "Envia E-mail")]
    public bool EnviaEmail { get; set; } = false;

    [Required]
    [Column("EnviaPush")]
    [Display(Name = "Envia Push")]
    public bool EnviaPush { get; set; } = false;

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

  //  [ForeignKey(nameof(UpdatedByUserId))]
  //  public virtual Usuario? UpdatedByUser { get; set; }

    // Inverse Navigation
 //   [InverseProperty(nameof(BasNotificacoes.TipoNotificacao))]
 //   public virtual ICollection<BasNotificacoes> Notificacoes { get; set; } = new List<BasNotificacoes>();
}