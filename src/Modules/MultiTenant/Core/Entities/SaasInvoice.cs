using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.MultiTenant.Core.Entities;

/// <summary>
/// Faturas geradas pelas assinaturas SaaS.
/// Tabela: dbo.SaasInvoices
/// </summary>
[GenerateCrud(
    TableName = "SaasInvoices",
    DisplayName = "Faturas",
    CdSistema = "SAS",
    CdFuncao = "SAS_FM_SAASINVOICES",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("SaasInvoices")]
public class SaasInvoice
{
    /// <summary>
    /// Identificador único da fatura.
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
    /// Identificador da assinatura (FK -> SaasSubscriptions).
    /// SQL: SubscriptionId UNIQUEIDENTIFIER NOT NULL
    /// </summary>
    [Required]
    [Column("SubscriptionId", TypeName = "uniqueidentifier")]
    [Display(Name = "Assinatura")]
    public Guid SubscriptionId { get; set; }

    /// <summary>
    /// Número sequencial/único da fatura.
    /// SQL: Number NVARCHAR(50) NOT NULL UNIQUE
    /// </summary>
    [Required]
    [StringLength(50)]
    [Column("Number")]
    [Display(Name = "Número")]
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Status da fatura: Draft, Open, Paid, Void, Uncollectible.
    /// SQL: Status NVARCHAR(20) NOT NULL DEFAULT 'Open'
    ///      CHECK (Status IN ('Draft','Open','Paid','Void','Uncollectible'))
    /// </summary>
    [Required]
    [StringLength(20)]
    [RegularExpression("^(Draft|Open|Paid|Void|Uncollectible)$",
        ErrorMessage = "Status deve ser: Draft, Open, Paid, Void ou Uncollectible")]
    [Column("Status")]
    [Display(Name = "Status")]
    public string Status { get; set; } = "Open";

    /// <summary>
    /// Data de emissão da fatura (UTC).
    /// SQL: IssueAt DATETIME2(3) NOT NULL
    /// </summary>
    [Required]
    [Column("IssueAt", TypeName = "datetime2(3)")]
    [Display(Name = "Emitida em (UTC)")]
    public DateTime IssueAt { get; set; }

    /// <summary>
    /// Data de vencimento (UTC).
    /// SQL: DueAt DATETIME2(3) NULL
    /// </summary>
    [Column("DueAt", TypeName = "datetime2(3)")]
    [Display(Name = "Vencimento (UTC)")]
    public DateTime? DueAt { get; set; }

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
    /// Valor total da fatura (soma dos itens).
    /// SQL: TotalAmount DECIMAL(18,2) NOT NULL CHECK (TotalAmount >= 0)
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    [Column("TotalAmount", TypeName = "decimal(18,2)")]
    [Display(Name = "Valor Total")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Data/hora do pagamento (UTC). NULL = não pago.
    /// SQL: PaidAt DATETIME2(3) NULL
    /// </summary>
    [Column("PaidAt", TypeName = "datetime2(3)")]
    [Display(Name = "Pago em (UTC)")]
    public DateTime? PaidAt { get; set; }

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