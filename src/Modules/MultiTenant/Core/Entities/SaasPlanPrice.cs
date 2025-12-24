using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.MultiTenant.Core.Entities;

/// <summary>
/// Preços e ciclos de cobrança dos Planos SaaS.
/// Tabela: dbo.SaasPlanPrices
/// </summary>
[GenerateCrud(
    TableName = "SaasPlanPrices",
    DisplayName = "Preços de Planos",
    CdSistema = "SAS",
    CdFuncao = "SAS_FM_SAASPLANPRICES",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("SaasPlanPrices")]
public class SaasPlanPrice
{
    /// <summary>
    /// Identificador único do preço.
    /// SQL: Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID()
    /// </summary>
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    [Display(Name = "ID")]
    public Guid Id { get; set; }

    /// <summary>
    /// TenantId para preços customizados. NULL = preço global do catálogo SaaS.
    /// SQL: TenantId UNIQUEIDENTIFIER NULL
    /// </summary>
    [Column("TenantId", TypeName = "uniqueidentifier")]
    [Display(Name = "Tenant (NULL = Global)")]
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Identificador do Plano (FK -> SaasPlans).
    /// SQL: PlanId UNIQUEIDENTIFIER NOT NULL
    /// </summary>
    [Required]
    [Column("PlanId", TypeName = "uniqueidentifier")]
    [Display(Name = "Plano")]
    public Guid PlanId { get; set; }

    /// <summary>
    /// Ciclo de cobrança: Monthly, Yearly.
    /// SQL: BillingCycle NVARCHAR(20) NOT NULL DEFAULT 'Monthly'
    ///      CHECK (BillingCycle IN ('Monthly','Yearly'))
    /// </summary>
    [Required]
    [StringLength(20)]
    [RegularExpression("^(Monthly|Yearly)$",
        ErrorMessage = "BillingCycle deve ser: Monthly ou Yearly")]
    [Column("BillingCycle")]
    [Display(Name = "Ciclo")]
    public string BillingCycle { get; set; } = "Monthly";

    /// <summary>
    /// Moeda (ISO 4217, 3 caracteres).
    /// SQL: Currency CHAR(3) NOT NULL DEFAULT 'BRL'
    /// </summary>
    [Required]
    [Column("Currency", TypeName = "char(3)")]
    [StringLength(3)]
    [Display(Name = "Moeda")]
    public string Currency { get; set; } = "BRL";

    /// <summary>
    /// Valor do plano no ciclo especificado.
    /// SQL: Amount DECIMAL(18,2) NOT NULL CHECK (Amount >= 0)
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    [Column("Amount", TypeName = "decimal(18,2)")]
    [Display(Name = "Valor")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Taxa de adesão/setup (cobrada uma única vez).
    /// SQL: SetupFee DECIMAL(18,2) NULL CHECK (SetupFee >= 0)
    /// </summary>
    [Range(0, double.MaxValue)]
    [Column("SetupFee", TypeName = "decimal(18,2)")]
    [Display(Name = "Taxa de adesão")]
    public decimal? SetupFee { get; set; }

    /// <summary>
    /// Período de trial em dias (NULL = sem trial).
    /// SQL: TrialDays INT NULL CHECK (TrialDays >= 0)
    /// </summary>
    [Range(0, int.MaxValue)]
    [Column("TrialDays")]
    [Display(Name = "Trial (dias)")]
    public int? TrialDays { get; set; }

    /// <summary>
    /// Indica se este preço está disponível para contratação.
    /// SQL: IsActive BIT NOT NULL DEFAULT 1
    /// </summary>
    [Required]
    [Column("IsActive")]
    [Display(Name = "Ativo")]
    public bool IsActive { get; set; } = true;

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