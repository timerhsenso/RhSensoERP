using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// BasNumeracoes - Gerenciamento de Numerações Sequenciais
/// Tabela: bas_numeracoes (Multi-tenant)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "bas_numeracoes",
    DisplayName = "BasNumeracoes",
    CdSistema = "BAS",
    CdFuncao = "BAS_FM_NUMERACOES",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("bas_numeracoes")]
public class BasNumeracoes
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
    [Column("Tipo", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Tipo")]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    [Column("ProximoNumero")]
    [Display(Name = "Próximo Número")]
    public long ProximoNumero { get; set; }

    [Column("Prefixo", TypeName = "nvarchar(20)")]
    [StringLength(20)]
    [Display(Name = "Prefixo")]
    public string? Prefixo { get; set; }

    [Column("Sufixo", TypeName = "nvarchar(20)")]
    [StringLength(20)]
    [Display(Name = "Sufixo")]
    public string? Sufixo { get; set; }

    [Column("Formato", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Formato")]
    public string? Formato { get; set; }

    // Auditoria (defaults controlados pelo banco: SYSUTCDATETIME())
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

    // Navigation Properties (mantidas, mas dependem de Usuario.Id ser Guid e ser a tabela correta do banco)
 //   [ForeignKey(nameof(CreatedByUserId))]
 //   public virtual Usuario? CreatedByUser { get; set; }

  //  [ForeignKey(nameof(UpdatedByUserId))]
  //  public virtual Usuario? UpdatedByUser { get; set; }
}
