using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.MultiTenant.Core.Entities;

/// <summary>
/// Catálogo de Planos do SaaS.
/// Tabela: dbo.SaasPlans
/// </summary>
[GenerateCrud(
    TableName = "SaasPlans",
    DisplayName = "Planos do SaaS",
    CdSistema = "SAS",
    CdFuncao = "SAS_FM_SAASPLANS",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("SaasPlans")]
public class SaasPlan
{
    /// <summary>
    /// Identificador único do Plano.
    /// SQL: Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID()
    /// </summary>
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    [Display(Name = "ID")]
    public Guid Id { get; set; }

    /// <summary>
    /// TenantId para planos customizados. NULL = plano global do catálogo SaaS.
    /// SQL: TenantId UNIQUEIDENTIFIER NULL
    /// </summary>
    [Column("TenantId", TypeName = "uniqueidentifier")]
    [Display(Name = "Tenant (NULL = Global)")]
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Código único do plano (ex: BASIC, PRO, ENTERPRISE).
    /// SQL: Code NVARCHAR(50) NOT NULL UNIQUE
    /// </summary>
    [Required]
    [StringLength(50)]
    [Column("Code")]
    [Display(Name = "Código")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Nome exibido do plano.
    /// SQL: Name NVARCHAR(100) NOT NULL
    /// </summary>
    [Required]
    [StringLength(100)]
    [Column("Name")]
    [Display(Name = "Nome")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada do plano.
    /// SQL: Description NVARCHAR(500) NULL
    /// </summary>
    [StringLength(500)]
    [Column("Description")]
    [Display(Name = "Descrição")]
    public string? Description { get; set; }

    /// <summary>
    /// Indica se o plano está disponível para contratação.
    /// SQL: IsActive BIT NOT NULL DEFAULT 1
    /// </summary>
    [Required]
    [Column("IsActive")]
    [Display(Name = "Ativo")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indica exclusão lógica (soft delete).
    /// SQL: IsDeleted BIT NOT NULL DEFAULT 0
    /// </summary>
    [Required]
    [Column("IsDeleted")]
    [Display(Name = "Excluído (soft delete)")]
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Data/hora de criação do registro (UTC).
    /// SQL: CreatedAtUtc DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()
    /// </summary>
    [Required]
    [Column("CreatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Criado em (UTC)")]
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Usuário que criou o registro (FK para dbo.tuse1.Id).
    /// SQL: CreatedByUserId UNIQUEIDENTIFIER NULL
    /// </summary>
    [Column("CreatedByUserId", TypeName = "uniqueidentifier")]
    [Display(Name = "Criado por (Usuário)")]
    public Guid? CreatedByUserId { get; set; }

    /// <summary>
    /// Data/hora da última atualização (UTC).
    /// SQL: UpdatedAtUtc DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()
    /// </summary>
    [Required]
    [Column("UpdatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Atualizado em (UTC)")]
    public DateTime UpdatedAtUtc { get; set; }

    /// <summary>
    /// Usuário que atualizou por último (FK para dbo.tuse1.Id).
    /// SQL: UpdatedByUserId UNIQUEIDENTIFIER NULL
    /// </summary>
    [Column("UpdatedByUserId", TypeName = "uniqueidentifier")]
    [Display(Name = "Atualizado por (Usuário)")]
    public Guid? UpdatedByUserId { get; set; }
}