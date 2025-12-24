using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.MultiTenant.Core.Entities;

/// <summary>
/// Log de auditoria de ações no sistema SaaS.
/// Tabela: dbo.SaasAuditLogs
/// </summary>
[GenerateCrud(
    TableName = "SaasAuditLogs",
    DisplayName = "Auditoria SaaS",
    CdSistema = "SAS",
    CdFuncao = "SAS_FM_SAASAUDITLOGS",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("SaasAuditLogs")]
public class SaasAuditLog
{
    /// <summary>
    /// Identificador único do log.
    /// SQL: Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID()
    /// </summary>
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    [Display(Name = "ID")]
    public Guid Id { get; set; }

    /// <summary>
    /// Identificador do Tenant (NULL = ação global/sistema).
    /// SQL: TenantId UNIQUEIDENTIFIER NULL
    /// </summary>
    [Column("TenantId", TypeName = "uniqueidentifier")]
    [Display(Name = "Tenant")]
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Identificador do usuário que executou a ação.
    /// SQL: ActorUserId UNIQUEIDENTIFIER NULL
    /// </summary>
    [Column("ActorUserId", TypeName = "uniqueidentifier")]
    [Display(Name = "Usuário Ator")]
    public Guid? ActorUserId { get; set; }

    /// <summary>
    /// Ação executada (ex: Create, Update, Delete, Login).
    /// SQL: Action NVARCHAR(100) NOT NULL
    /// </summary>
    [Required]
    [StringLength(100)]
    [Column("Action")]
    [Display(Name = "Ação")]
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Nome da entidade afetada (ex: SaasTenant, SaasUser).
    /// SQL: EntityName NVARCHAR(100) NULL
    /// </summary>
    [StringLength(100)]
    [Column("EntityName")]
    [Display(Name = "Entidade")]
    public string? EntityName { get; set; }

    /// <summary>
    /// ID da entidade afetada (chave primária como string).
    /// SQL: EntityId NVARCHAR(100) NULL
    /// </summary>
    [StringLength(100)]
    [Column("EntityId")]
    [Display(Name = "ID da Entidade")]
    public string? EntityId { get; set; }

    /// <summary>
    /// Estado anterior da entidade (JSON).
    /// SQL: BeforeJson NVARCHAR(MAX) NULL
    /// </summary>
    [Column("BeforeJson")]
    [Display(Name = "Estado Anterior (JSON)")]
    public string? BeforeJson { get; set; }

    /// <summary>
    /// Estado posterior da entidade (JSON).
    /// SQL: AfterJson NVARCHAR(MAX) NULL
    /// </summary>
    [Column("AfterJson")]
    [Display(Name = "Estado Posterior (JSON)")]
    public string? AfterJson { get; set; }

    /// <summary>
    /// Endereço IP de origem da requisição.
    /// SQL: IpAddress NVARCHAR(50) NULL
    /// </summary>
    [StringLength(50)]
    [Column("IpAddress")]
    [Display(Name = "IP")]
    public string? IpAddress { get; set; }

    /// <summary>
    /// User-Agent do navegador/cliente.
    /// SQL: UserAgent NVARCHAR(255) NULL
    /// </summary>
    [StringLength(255)]
    [Column("UserAgent")]
    [Display(Name = "User Agent")]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Data/hora de criação do log (UTC).
    /// SQL: CreatedAtUtc DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()
    /// </summary>
    [Required]
    [Column("CreatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Criado em (UTC)")]
    public DateTime CreatedAtUtc { get; set; }
}