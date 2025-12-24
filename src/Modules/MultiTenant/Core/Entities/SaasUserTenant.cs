using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.MultiTenant.Core.Entities;

/// <summary>
/// Relacionamento N:N entre Usuários e Tenants, com perfil de acesso.
/// Tabela: dbo.SaasUserTenants
/// </summary>
[GenerateCrud(
    TableName = "SaasUserTenants",
    DisplayName = "Usuário x Tenant",
    CdSistema = "SAS",
    CdFuncao = "SAS_FM_SAASUSERTENANTS",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("SaasUserTenants")]
public class SaasUserTenant
{
    /// <summary>
    /// Identificador único do relacionamento.
    /// SQL: Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID()
    /// </summary>
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    [Display(Name = "ID")]
    public Guid Id { get; set; }

    /// <summary>
    /// Identificador do usuário (FK -> SaasUsers).
    /// SQL: UserId UNIQUEIDENTIFIER NOT NULL
    /// </summary>
    [Required]
    [Column("UserId", TypeName = "uniqueidentifier")]
    [Display(Name = "Usuário")]
    public Guid UserId { get; set; }

    /// <summary>
    /// Identificador do Tenant (FK -> SaasTenants).
    /// SQL: TenantId UNIQUEIDENTIFIER NOT NULL
    /// </summary>
    [Required]
    [Column("TenantId", TypeName = "uniqueidentifier")]
    [Display(Name = "Tenant")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Perfil de acesso do usuário no Tenant: Owner, Admin, Billing, Viewer.
    /// SQL: Role NVARCHAR(30) NOT NULL DEFAULT 'Viewer'
    ///      CHECK (Role IN ('Owner','Admin','Billing','Viewer'))
    /// </summary>
    [Required]
    [StringLength(30)]
    [RegularExpression("^(Owner|Admin|Billing|Viewer)$",
        ErrorMessage = "Role deve ser: Owner, Admin, Billing ou Viewer")]
    [Column("Role")]
    [Display(Name = "Perfil")]
    public string Role { get; set; } = "Viewer";

    /// <summary>
    /// Indica se este é o Tenant principal do usuário.
    /// SQL: IsPrimary BIT NOT NULL DEFAULT 0
    /// </summary>
    [Required]
    [Column("IsPrimary")]
    [Display(Name = "Principal")]
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Status do vínculo: Active, Invited, Disabled.
    /// SQL: Status NVARCHAR(20) NOT NULL DEFAULT 'Active'
    ///      CHECK (Status IN ('Active','Invited','Disabled'))
    /// </summary>
    [Required]
    [StringLength(20)]
    [RegularExpression("^(Active|Invited|Disabled)$",
        ErrorMessage = "Status deve ser: Active, Invited ou Disabled")]
    [Column("Status")]
    [Display(Name = "Status")]
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Data/hora de ingresso do usuário no Tenant (UTC).
    /// SQL: JoinedAt DATETIME2(3) NULL
    /// </summary>
    [Column("JoinedAt", TypeName = "datetime2(3)")]
    [Display(Name = "Ingresso (UTC)")]
    public DateTime? JoinedAt { get; set; }

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