using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.MultiTenant.Core.Entities;

/// <summary>
/// Perfil de cobrança (dados fiscais) de cada Tenant.
/// Tabela: dbo.SaasBillingProfiles
/// </summary>
[GenerateCrud(
    TableName = "SaasBillingProfiles",
    DisplayName = "Perfil de Cobrança",
    CdSistema = "SAS",
    CdFuncao = "SAS_FM_SAASBILLINGPROFILES",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("SaasBillingProfiles")]
public class SaasBillingProfile
{
    /// <summary>
    /// Identificador único do perfil.
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
    /// Razão social da empresa.
    /// SQL: LegalName NVARCHAR(255) NOT NULL
    /// </summary>
    [Required]
    [StringLength(255)]
    [Column("LegalName")]
    [Display(Name = "Razão Social")]
    public string LegalName { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ/CPF (sem formatação).
    /// SQL: TaxId NVARCHAR(30) NULL
    /// </summary>
    [StringLength(30)]
    [Column("TaxId")]
    [Display(Name = "CNPJ/CPF")]
    public string? TaxId { get; set; }

    /// <summary>
    /// E-mail para envio de faturas.
    /// SQL: BillingEmail NVARCHAR(255) NULL
    /// </summary>
    [EmailAddress]
    [StringLength(255)]
    [Column("BillingEmail")]
    [Display(Name = "E-mail de Cobrança")]
    public string? BillingEmail { get; set; }

    /// <summary>
    /// Telefone de contato.
    /// SQL: Phone NVARCHAR(30) NULL
    /// </summary>
    [StringLength(30)]
    [Column("Phone")]
    [Display(Name = "Telefone")]
    public string? Phone { get; set; }

    /// <summary>
    /// Endereço linha 1.
    /// SQL: AddressLine1 NVARCHAR(255) NULL
    /// </summary>
    [StringLength(255)]
    [Column("AddressLine1")]
    [Display(Name = "Endereço 1")]
    public string? AddressLine1 { get; set; }

    /// <summary>
    /// Endereço linha 2 (complemento).
    /// SQL: AddressLine2 NVARCHAR(255) NULL
    /// </summary>
    [StringLength(255)]
    [Column("AddressLine2")]
    [Display(Name = "Endereço 2")]
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// Cidade.
    /// SQL: City NVARCHAR(100) NULL
    /// </summary>
    [StringLength(100)]
    [Column("City")]
    [Display(Name = "Cidade")]
    public string? City { get; set; }

    /// <summary>
    /// Estado/UF.
    /// SQL: State NVARCHAR(50) NULL
    /// </summary>
    [StringLength(50)]
    [Column("State")]
    [Display(Name = "Estado")]
    public string? State { get; set; }

    /// <summary>
    /// CEP (sem formatação).
    /// SQL: PostalCode NVARCHAR(20) NULL
    /// </summary>
    [StringLength(20)]
    [Column("PostalCode")]
    [Display(Name = "CEP")]
    public string? PostalCode { get; set; }

    /// <summary>
    /// País (código ISO ou nome).
    /// SQL: Country NVARCHAR(50) NULL
    /// </summary>
    [StringLength(50)]
    [Column("Country")]
    [Display(Name = "País")]
    public string? Country { get; set; }

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