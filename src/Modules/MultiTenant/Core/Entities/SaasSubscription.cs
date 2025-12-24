using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.MultiTenant.Core.Entities;

/// <summary>
/// Assinaturas de Planos SaaS vinculadas a Tenants.
/// Tabela: dbo.SaasSubscriptions
/// </summary>
[GenerateCrud(
    TableName = "SaasSubscriptions",
    DisplayName = "Assinaturas",
    CdSistema = "SAS",
    CdFuncao = "SAS_FM_SAASSUBSCRIPTIONS",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("SaasSubscriptions")]
public class SaasSubscription
{
    /// <summary>
    /// Identificador único da assinatura.
    /// SQL: Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID()
    /// </summary>
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    [Display(Name = "ID")]
    public Guid Id { get; set; }

    /// <summary>
    /// Identificador do Tenant (cliente SaaS).
    /// SQL: TenantId UNIQUEIDENTIFIER NOT NULL FK -> SaasTenants(Id)
    /// </summary>
    [Required]
    [Column("TenantId", TypeName = "uniqueidentifier")]
    [Display(Name = "Tenant")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Identificador do preço do plano contratado.
    /// SQL: PlanPriceId UNIQUEIDENTIFIER NOT NULL FK -> SaasPlanPrices(Id)
    /// </summary>
    [Required]
    [Column("PlanPriceId", TypeName = "uniqueidentifier")]
    [Display(Name = "Plano/Preço")]
    public Guid PlanPriceId { get; set; }

    /// <summary>
    /// Status da assinatura: Trialing, Active, PastDue, Canceled.
    /// SQL: Status NVARCHAR(20) NOT NULL DEFAULT 'Active'
    ///      CHECK (Status IN ('Trialing','Active','PastDue','Canceled'))
    /// </summary>
    [Required]
    [StringLength(20)]
    [RegularExpression("^(Trialing|Active|PastDue|Canceled)$",
        ErrorMessage = "Status deve ser: Trialing, Active, PastDue ou Canceled")]
    [Column("Status")]
    [Display(Name = "Status")]
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Data/hora de início da assinatura (UTC).
    /// SQL: StartAt DATETIME2(3) NOT NULL
    /// </summary>
    [Required]
    [Column("StartAt", TypeName = "datetime2(3)")]
    [Display(Name = "Início (UTC)")]
    public DateTime StartAt { get; set; }

    /// <summary>
    /// Data/hora de término da assinatura (NULL = ativa).
    /// SQL: EndAt DATETIME2(3) NULL
    /// </summary>
    [Column("EndAt", TypeName = "datetime2(3)")]
    [Display(Name = "Término (UTC)")]
    public DateTime? EndAt { get; set; }

    /// <summary>
    /// Próxima data de cobrança (UTC).
    /// SQL: NextBillingAt DATETIME2(3) NULL
    /// </summary>
    [Column("NextBillingAt", TypeName = "datetime2(3)")]
    [Display(Name = "Próxima Cobrança (UTC)")]
    public DateTime? NextBillingAt { get; set; }

    /// <summary>
    /// Indica se a assinatura renova automaticamente.
    /// SQL: AutoRenew BIT NOT NULL DEFAULT 1
    /// </summary>
    [Required]
    [Column("AutoRenew")]
    [Display(Name = "Renovação Automática")]
    public bool AutoRenew { get; set; } = true;

    /// <summary>
    /// Data/hora do cancelamento (UTC). NULL = não cancelada.
    /// SQL: CancelAt DATETIME2(3) NULL
    /// </summary>
    [Column("CancelAt", TypeName = "datetime2(3)")]
    [Display(Name = "Cancelada em (UTC)")]
    public DateTime? CancelAt { get; set; }

    /// <summary>
    /// Motivo do cancelamento (texto livre).
    /// SQL: CancelReason NVARCHAR(255) NULL
    /// </summary>
    [StringLength(255)]
    [Column("CancelReason")]
    [Display(Name = "Motivo do Cancelamento")]
    public string? CancelReason { get; set; }

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