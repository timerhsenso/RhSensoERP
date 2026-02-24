// =============================================================================
// RHSENSOERP - ENTITY GTPAREA
// =============================================================================
// Módulo: GestaoTerceiros (GTP)
// Tabela: GTP_Area
// Schema: dbo
// Multi-tenant: ✅ SIM (TenantId obrigatório)
//
// ✅ PADRÃO v2 — Consistente com TreTiposTreinamento:
//    - Coluna tenant: TenantId (não IdSaaS)
//    - Auditoria: CreatedAtUtc, UpdatedAtUtc, CreatedByUserId, UpdatedByUserId
//    - Concorrência otimista: RowVer (ROWVERSION)
// =============================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;

/// <summary>
/// GtpArea - Áreas / Setores
/// Tabela: GTP_Area (Multi-tenant)
/// Fonte da verdade: SQL Server
///
/// Áreas, setores ou locais de trabalho dentro de cada unidade.
/// Permite definir grau de risco (NR-4) e NRs exigidas.
/// </summary>
[GenerateCrud(
    TableName = "GTP_Area",
    DisplayName = "Áreas / Setores",
    CdSistema = "GTP",
    CdFuncao = "GTP_FM_GTPAREA",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("GTP_Area")]
[HasDatabaseTriggers("Auditoria automática de CreatedAt/UpdatedAt via triggers SQL Server")]
public class GtpArea
{
    // =========================================================================
    // CHAVE PRIMÁRIA
    // =========================================================================

    /// <summary>
    /// Identificador interno (chave técnica).
    /// SQL: Id INT IDENTITY(1,1) PRIMARY KEY
    /// </summary>
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "ID")]
    public int Id { get; set; }

    // =========================================================================
    // MULTI-TENANCY
    // =========================================================================

    /// <summary>
    /// Identificador do tenant (SaaS) - isola dados entre clientes.
    /// SQL: TenantId UNIQUEIDENTIFIER NOT NULL
    /// </summary>
    [Required]
    [Column("TenantId", TypeName = "uniqueidentifier")]
    [Display(Name = "Tenant")]
    public Guid TenantId { get; set; }

    // =========================================================================
    // DADOS PRINCIPAIS
    // =========================================================================

    /// <summary>
    /// FK para GTP_Unidade. Unidade operacional à qual a área pertence.
    /// SQL: IdUnidade INT NOT NULL
    /// </summary>
    [Required]
    [Column("IdUnidade")]
    [Display(Name = "Unidade")]
    public int IdUnidade { get; set; }

    /// <summary>
    /// Código da área (único por tenant + unidade).
    /// SQL: Codigo VARCHAR(20) NOT NULL
    /// </summary>
    [Required]
    [StringLength(20)]
    [Column("Codigo")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Nome da área.
    /// SQL: Nome NVARCHAR(150) NOT NULL
    /// </summary>
    [Required]
    [StringLength(150)]
    [Column("Nome")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Grau de risco conforme NR-4 (1 a 4). NULL = não classificado.
    /// SQL: GrauRisco INT NULL CHECK(1..4)
    /// </summary>
    [Column("GrauRisco")]
    [Display(Name = "Grau de Risco")]
    [Range(1, 4, ErrorMessage = "Grau de Risco deve ser entre 1 e 4")]
    public int? GrauRisco { get; set; }

    /// <summary>
    /// NRs exigidas para trabalho nesta área (separadas por vírgula, ex.: NR-10,NR-35).
    /// SQL: ExigeNR VARCHAR(100) NULL
    /// </summary>
    [StringLength(100)]
    [Column("ExigeNR")]
    [Display(Name = "NRs Exigidas")]
    public string? ExigeNR { get; set; }

    /// <summary>
    /// Nome do responsável pela área.
    /// SQL: ResponsavelArea NVARCHAR(150) NULL
    /// </summary>
    [StringLength(150)]
    [Column("ResponsavelArea")]
    [Display(Name = "Responsável")]
    public string? ResponsavelArea { get; set; }

    /// <summary>
    /// Indica se o registro está ativo (soft delete).
    /// SQL: Ativo BIT NOT NULL DEFAULT 1
    /// </summary>
    [Required]
    [Column("Ativo")]
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    // =========================================================================
    // AUDITORIA (defaults controlados pelo banco: SYSUTCDATETIME())
    // =========================================================================

    /// <summary>
    /// Data/hora (UTC) de criação do registro (gerada automaticamente pelo banco).
    /// SQL: CreatedAtUtc DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()
    /// ⚠️ PREENCHIDO POR TRIGGER - NÃO ENVIAR NO INSERT
    /// </summary>
    [Required]
    [Column("CreatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Criado em (UTC)")]
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Usuário (GUID) que criou o registro (FK para dbo.tuse1.Id).
    /// SQL: CreatedByUserId UNIQUEIDENTIFIER NULL
    /// ⚠️ PREENCHIDO POR TRIGGER - NÃO ENVIAR NO INSERT
    /// </summary>
    [Column("CreatedByUserId", TypeName = "uniqueidentifier")]
    [Display(Name = "Criado por (Usuário)")]
    public Guid? CreatedByUserId { get; set; }

    /// <summary>
    /// Data/hora (UTC) da última atualização (gerada no INSERT e atualizada por trigger).
    /// SQL: UpdatedAtUtc DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()
    /// ⚠️ PREENCHIDO POR TRIGGER - NÃO ENVIAR NO UPDATE
    /// </summary>
    [Required]
    [Column("UpdatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Atualizado em (UTC)")]
    public DateTime UpdatedAtUtc { get; set; }

    /// <summary>
    /// Usuário (GUID) que atualizou por último (FK para dbo.tuse1.Id).
    /// SQL: UpdatedByUserId UNIQUEIDENTIFIER NULL
    /// ⚠️ PREENCHIDO POR TRIGGER - NÃO ENVIAR NO UPDATE
    /// </summary>
    [Column("UpdatedByUserId", TypeName = "uniqueidentifier")]
    [Display(Name = "Atualizado por (Usuário)")]
    public Guid? UpdatedByUserId { get; set; }

    // =========================================================================
    // CONTROLE DE CONCORRÊNCIA
    // =========================================================================

    /// <summary>
    /// Token de concorrência otimista. Gerado/atualizado automaticamente pelo SQL Server.
    /// SQL: RowVer ROWVERSION NOT NULL
    /// ⚠️ NÃO ENVIAR — Controlado pelo banco
    /// </summary>
    [Column("RowVer")]
    [Timestamp]
    [Display(Name = "Versão do Registro")]
    public byte[] RowVer { get; set; } = null!;

    // =========================================================================
    // NAVIGATION PROPERTIES
    // =========================================================================

    // [ForeignKey(nameof(IdUnidade))]
    // public virtual GtpUnidade? Unidade { get; set; }

    // [ForeignKey(nameof(CreatedByUserId))]
    // public virtual Usuario? CreatedByUser { get; set; }

    // [ForeignKey(nameof(UpdatedByUserId))]
    // public virtual Usuario? UpdatedByUser { get; set; }
}