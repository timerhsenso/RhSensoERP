using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.MultiTenant.Core.Entities;

/// <summary>
/// Usuários do portal/admin do SaaS.
/// Tabela: dbo.SaasUsers
/// </summary>
[GenerateCrud(
    TableName = "SaasUsers",
    DisplayName = "Usuários do SaaS",
    CdSistema = "SAS",
    CdFuncao = "SAS_FM_SAASUSERS",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("SaasUsers")]
public class SaasUser
{
    /// <summary>
    /// Identificador único do usuário.
    /// SQL: Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID()
    /// </summary>
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    [Display(Name = "ID")]
    public Guid Id { get; set; }

    /// <summary>
    /// TenantId para usuários específicos de um tenant. NULL = usuário global/administrador do SaaS.
    /// SQL: TenantId UNIQUEIDENTIFIER NULL
    /// </summary>
    [Column("TenantId", TypeName = "uniqueidentifier")]
    [Display(Name = "Tenant (NULL = Global)")]
    public Guid? TenantId { get; set; }

    /// <summary>
    /// E-mail do usuário (usado para login).
    /// SQL: Email NVARCHAR(255) NOT NULL UNIQUE
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(255)]
    [Column("Email")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash da senha (BCrypt ou similar).
    /// SQL: PasswordHash NVARCHAR(255) NOT NULL
    /// </summary>
    [Required]
    [StringLength(255)]
    [Column("PasswordHash")]
    [Display(Name = "Senha (hash)")]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo do usuário.
    /// SQL: FullName NVARCHAR(255) NULL
    /// </summary>
    [StringLength(255)]
    [Column("FullName")]
    [Display(Name = "Nome")]
    public string? FullName { get; set; }

    /// <summary>
    /// Indica se o usuário está ativo no sistema.
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
    /// Data/hora do último login (UTC).
    /// SQL: LastLoginAt DATETIME2(3) NULL
    /// </summary>
    [Column("LastLoginAt", TypeName = "datetime2(3)")]
    [Display(Name = "Último login (UTC)")]
    public DateTime? LastLoginAt { get; set; }

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