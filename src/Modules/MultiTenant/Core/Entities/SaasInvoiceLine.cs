using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.MultiTenant.Core.Entities;

/// <summary>
/// Itens/linhas de uma fatura SaaS.
/// Tabela: dbo.SaasInvoiceLines
/// </summary>
[GenerateCrud(
    TableName = "SaasInvoiceLines",
    DisplayName = "Itens de Fatura",
    CdSistema = "SAS",
    CdFuncao = "SAS_FM_SAASINVOICELINES",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("SaasInvoiceLines")]
public class SaasInvoiceLine
{
    /// <summary>
    /// Identificador único do item.
    /// SQL: Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID()
    /// </summary>
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    [Display(Name = "ID")]
    public Guid Id { get; set; }

    /// <summary>
    /// TenantId herdado da fatura (desnormalizado para compatibilidade com gerador CRUD).
    /// SQL: TenantId UNIQUEIDENTIFIER NULL
    /// </summary>
    [Column("TenantId", TypeName = "uniqueidentifier")]
    [Display(Name = "Tenant")]
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Identificador da fatura (FK -> SaasInvoices).
    /// SQL: InvoiceId UNIQUEIDENTIFIER NOT NULL
    /// </summary>
    [Required]
    [Column("InvoiceId", TypeName = "uniqueidentifier")]
    [Display(Name = "Fatura")]
    public Guid InvoiceId { get; set; }

    /// <summary>
    /// Descrição do item/serviço faturado.
    /// SQL: Description NVARCHAR(255) NOT NULL
    /// </summary>
    [Required]
    [StringLength(255)]
    [Column("Description")]
    [Display(Name = "Descrição")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Quantidade (deve ser > 0).
    /// SQL: Qty INT NOT NULL CHECK (Qty > 0)
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    [Column("Qty")]
    [Display(Name = "Quantidade")]
    public int Qty { get; set; } = 1;

    /// <summary>
    /// Valor unitário do item.
    /// SQL: UnitAmount DECIMAL(18,2) NOT NULL CHECK (UnitAmount >= 0)
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    [Column("UnitAmount", TypeName = "decimal(18,2)")]
    [Display(Name = "Valor Unitário")]
    public decimal UnitAmount { get; set; }

    /// <summary>
    /// Valor total do item (Qty x UnitAmount).
    /// SQL: Amount DECIMAL(18,2) NOT NULL CHECK (Amount >= 0)
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    [Column("Amount", TypeName = "decimal(18,2)")]
    [Display(Name = "Valor Total")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Tipo de referência (ex: Subscription, Addon, SetupFee).
    /// SQL: ReferenceType NVARCHAR(30) NULL
    /// </summary>
    [StringLength(30)]
    [Column("ReferenceType")]
    [Display(Name = "Tipo de Referência")]
    public string? ReferenceType { get; set; }

    /// <summary>
    /// ID da entidade referenciada (ex: SubscriptionId, AddonId).
    /// SQL: ReferenceId NVARCHAR(100) NULL
    /// </summary>
    [StringLength(100)]
    [Column("ReferenceId")]
    [Display(Name = "ID de Referência")]
    public string? ReferenceId { get; set; }
}