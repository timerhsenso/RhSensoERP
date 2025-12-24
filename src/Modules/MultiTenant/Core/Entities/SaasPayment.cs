using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.MultiTenant.Core.Entities;

/// <summary>
/// Pagamentos realizados para faturas SaaS.
/// Tabela: dbo.SaasPayments
/// </summary>
[GenerateCrud(
    TableName = "SaasPayments",
    DisplayName = "Pagamentos",
    CdSistema = "SAS",
    CdFuncao = "SAS_FM_SAASPAYMENTS",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("SaasPayments")]
public class SaasPayment
{
    /// <summary>
    /// Identificador único do pagamento.
    /// SQL: Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID()
    /// </summary>
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    [Display(Name = "ID")]
    public Guid Id { get; set; }

    /// <summary>
    /// Identificador do Tenant (FK -> SaasTenants).
    /// SQL: TenantId UNIQUEIDENTIFIER NOT NULL
    /// </summary>
    [Required]
    [Column("TenantId", TypeName = "uniqueidentifier")]
    [Display(Name = "Tenant")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Identificador da fatura (FK -> SaasInvoices).
    /// SQL: InvoiceId UNIQUEIDENTIFIER NOT NULL
    /// </summary>
    [Required]
    [Column("InvoiceId", TypeName = "uniqueidentifier")]
    [Display(Name = "Fatura")]
    public Guid InvoiceId { get; set; }

    /// <summary>
    /// Provedor de pagamento (ex: Stripe, PayPal, Manual).
    /// SQL: Provider NVARCHAR(30) NOT NULL DEFAULT 'Manual'
    /// </summary>
    [Required]
    [StringLength(30)]
    [Column("Provider")]
    [Display(Name = "Provedor")]
    public string Provider { get; set; } = "Manual";

    /// <summary>
    /// ID do pagamento no sistema do provedor.
    /// SQL: ProviderPaymentId NVARCHAR(100) NULL
    /// </summary>
    [StringLength(100)]
    [Column("ProviderPaymentId")]
    [Display(Name = "ID Provedor")]
    public string? ProviderPaymentId { get; set; }

    /// <summary>
    /// Status do pagamento: Pending, Succeeded, Failed, Refunded.
    /// SQL: Status NVARCHAR(20) NOT NULL DEFAULT 'Pending'
    ///      CHECK (Status IN ('Pending','Succeeded','Failed','Refunded'))
    /// </summary>
    [Required]
    [StringLength(20)]
    [RegularExpression("^(Pending|Succeeded|Failed|Refunded)$",
        ErrorMessage = "Status deve ser: Pending, Succeeded, Failed ou Refunded")]
    [Column("Status")]
    [Display(Name = "Status")]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Valor do pagamento.
    /// SQL: Amount DECIMAL(18,2) NOT NULL CHECK (Amount >= 0)
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    [Column("Amount", TypeName = "decimal(18,2)")]
    [Display(Name = "Valor")]
    public decimal Amount { get; set; }

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
    /// Data/hora da confirmação do pagamento (UTC).
    /// SQL: PaidAt DATETIME2(3) NULL
    /// </summary>
    [Column("PaidAt", TypeName = "datetime2(3)")]
    [Display(Name = "Pago em (UTC)")]
    public DateTime? PaidAt { get; set; }

    /// <summary>
    /// Payload completo retornado pelo provedor (JSON).
    /// SQL: RawPayload NVARCHAR(MAX) NULL
    /// </summary>
    [Column("RawPayload")]
    [Display(Name = "Payload Bruto")]
    public string? RawPayload { get; set; }

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