// =============================================================================
// RHSENSOERP - ENTITY SHRCNAE
// =============================================================================
// Módulo: Shared (SHR)
// Tabela: SHR_CNAE
// Schema: dbo
// Multi-tenant: ❌ NÃO (dados compartilhados)
// =============================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Shared.Core.Domain.Entities;

/// <summary>
/// ShrCnae - CNAE
/// Tabela: SHR_CNAE (Compartilhada)
/// Fonte da verdade: SQL Server
/// 
/// Classificação Nacional de Atividades Econômicas (CNAE). Dados compartilhados.
/// </summary>
[GenerateCrud(
    TableName = "SHR_CNAE",
    DisplayName = "CNAE",
    CdSistema = "SHR",
    CdFuncao = "SHR_FM_SHRCNAE",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("SHR_CNAE")]
[HasDatabaseTriggers("Auditoria automática de CreatedAt/UpdatedAt via triggers SQL Server")]
public class ShrCnae
{

    // =========================================================================
    // CHAVE PRIMÁRIA
    // =========================================================================

    /// <summary>
    /// Identificador interno (chave técnica).
    /// SQL: Id INT IDENTITY(1,1) PRIMARY KEY
    /// </summary>
    [Required]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("Id")]
    [Display(Name = "ID")]
    public int Id { get; set; }

    // =========================================================================
    // DADOS PRINCIPAIS
    // =========================================================================

    /// <summary>
    /// Código CNAE com máscara (ex.: 62.01-5-01).
    /// SQL: Codigo VARCHAR(10) NOT NULL
    /// </summary>
    [Required]
    [StringLength(10)]
    [Column("Codigo")]
    [Display(Name = "Código CNAE")]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Código CNAE sem formatação (7 dígitos, ex.: 6201501).
    /// SQL: CodigoLimpo CHAR(7) NOT NULL UNIQUE
    /// </summary>
    [Required]
    [StringLength(7)]
    [Column("CodigoLimpo")]
    [Display(Name = "Código Limpo")]
    public string CodigoLimpo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição da atividade econômica.
    /// SQL: Descricao NVARCHAR(300) NOT NULL
    /// </summary>
    [Required]
    [StringLength(300)]
    [Column("Descricao")]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Letra da seção CNAE.
    /// SQL: Secao CHAR(1) NULL
    /// </summary>
    [StringLength(1)]
    [Column("Secao")]
    [Display(Name = "Seção")]
    public string? Secao { get; set; }

    /// <summary>
    /// Código da divisão (2 dígitos).
    /// SQL: Divisao CHAR(2) NULL
    /// </summary>
    [StringLength(2)]
    [Column("Divisao")]
    [Display(Name = "Divisão")]
    public string? Divisao { get; set; }

    /// <summary>
    /// Código do grupo (3 dígitos).
    /// SQL: Grupo CHAR(3) NULL
    /// </summary>
    [StringLength(3)]
    [Column("Grupo")]
    [Display(Name = "Grupo")]
    public string? Grupo { get; set; }

    /// <summary>
    /// Código da classe (5 dígitos).
    /// SQL: Classe CHAR(5) NULL
    /// </summary>
    [StringLength(5)]
    [Column("Classe")]
    [Display(Name = "Classe")]
    public string? Classe { get; set; }

    /// <summary>
    /// Grau de risco da atividade (NR-4).
    /// SQL: GrauRisco INT NULL
    /// </summary>
    [Column("GrauRisco")]
    [Display(Name = "Grau de Risco")]
    public int? GrauRisco { get; set; }

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
    public byte[] RowVer { get; set; }

    // =========================================================================
    // NAVIGATION PROPERTIES
    // =========================================================================
    // TODO: Adicionar navigation properties conforme implementação avançar.
}