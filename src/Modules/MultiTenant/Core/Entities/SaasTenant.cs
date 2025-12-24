using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.MultiTenant.Core.Entities;

/// <summary>
/// Cadastro de Tenants (clientes) do SaaS.
/// Tabela: dbo.SaasTenants
/// </summary>
[GenerateCrud(
    TableName = "SaasTenants",
    DisplayName = "Tenants do SaaS",
    CdSistema = "SAS",
    CdFuncao = "SAS_FM_SAASTENANTS",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("SaasTenants")]
public class SaasTenant
{
    /// <summary>
    /// Identificador único do Tenant.
    /// SQL: Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID()
    /// </summary>
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    [Display(Name = "ID")]
    public Guid Id { get; set; }

    /// <summary>
    /// TenantId para compatibilidade com gerador CRUD (sempre NULL para SaasTenant).
    /// SQL: TenantId UNIQUEIDENTIFIER NULL
    /// </summary>
    [Column("TenantId", TypeName = "uniqueidentifier")]
    [Display(Name = "Tenant (sempre NULL)")]
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Nome da empresa/organização do Tenant.
    /// SQL: CompanyName NVARCHAR(255) NOT NULL
    /// </summary>
    [Required]
    [StringLength(255)]
    [Column("CompanyName")]
    [Display(Name = "Empresa")]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Domínio personalizado do Tenant (ex: cliente.rhsenso.com.br).
    /// SQL: Domain NVARCHAR(100) NULL
    /// </summary>
    [StringLength(100)]
    [Column("Domain")]
    [Display(Name = "Domínio")]
    public string? Domain { get; set; }

    /// <summary>
    /// Indica se o Tenant está ativo no sistema.
    /// SQL: IsActive BIT NOT NULL DEFAULT 1
    /// </summary>
    [Required]
    [Column("IsActive")]
    [Display(Name = "Ativo")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Limite máximo de usuários permitidos para este Tenant.
    /// SQL: MaxUsers INT NOT NULL DEFAULT 10
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    [Column("MaxUsers")]
    [Display(Name = "Máx. Usuários")]
    public int MaxUsers { get; set; } = 10;

    /// <summary>
    /// Tipo de plano (campo legado). A fonte de verdade é SaasSubscriptions.
    /// SQL: PlanType NVARCHAR(50) NOT NULL DEFAULT 'Basic'
    /// </summary>
    [Required]
    [StringLength(50)]
    [Column("PlanType")]
    [Display(Name = "Plano (texto)")]
    public string PlanType { get; set; } = "Basic";

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