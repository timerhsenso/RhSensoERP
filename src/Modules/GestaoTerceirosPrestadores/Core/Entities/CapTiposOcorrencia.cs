// =============================================================================
// RHSENSOERP - ENTITY CAPTIPOSOCORRENCIA
// =============================================================================
// Módulo: Gestão de Terceiros e Prestadores (CAP)
// Tabela: cap_tipos_ocorrencia
// Schema: dbo
// Multi-tenant: ✅ SIM (TenantId obrigatório)
// 
// ✅ VALIDAÇÃO AUTOMÁTICA DE UNICIDADE:
// - Nome: Único por tenant
// =============================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;

/// <summary>
/// CapTiposOcorrencia - Tipos de Ocorrência
/// Tabela: cap_tipos_ocorrencia (Multi-tenant)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "cap_tipos_ocorrencia",
    DisplayName = "CapTiposOcorrencia",
    CdSistema = "CAP",
    CdFuncao = "CAP_FM_TIPOSOCORRENCIA",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("cap_tipos_ocorrencia")]
[HasDatabaseTriggers("Auditoria automática de CreatedAt/UpdatedAt via triggers SQL Server")]
public class CapTiposOcorrencia
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "ID")]
    public int Id { get; set; }

    [Required]
    [Column("TenantId")]
    [Display(Name = "Tenant ID")]
    public Guid TenantId { get; set; }

    // =========================================================================
    // ✅ NOME - VALIDAÇÃO AUTOMÁTICA DE UNICIDADE POR TENANT
    // =========================================================================
    // [Unique] faz com que o Source Generator crie:
    // 1. Índice único: CREATE UNIQUE INDEX UX_CapTiposOcorrencia_Tenant_Nome 
    //    ON cap_tipos_ocorrencia (TenantId, Nome)
    // 2. Validação no UniqueValidationBehavior ANTES de SaveChanges
    // 3. Exception DuplicateEntityException → HTTP 409 Conflict
    // =========================================================================
    [Required]
    [Unique(UniqueScope.Tenant, "Nome do Tipo de Ocorrência")]
    [Column("Nome", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("Descricao", TypeName = "nvarchar(500)")]
    [StringLength(500)]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Column("Severidade", TypeName = "nvarchar(50)")]
    [StringLength(50)]
    [Display(Name = "Severidade")]
    public string? Severidade { get; set; }

    [Required]
    [Column("Ativo")]
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    // =========================================================================
    // AUDITORIA (defaults controlados pelo banco: SYSUTCDATETIME())
    // =========================================================================
    [Required]
    [Column("CreatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Data Criação (UTC)")]
    public DateTime CreatedAtUtc { get; set; }

    [Column("CreatedByUserId")]
    [Display(Name = "Criado Por")]
    public Guid? CreatedByUserId { get; set; }

    [Required]
    [Column("UpdatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Data Atualização (UTC)")]
    public DateTime UpdatedAtUtc { get; set; }

    [Column("UpdatedByUserId")]
    [Display(Name = "Atualizado Por")]
    public Guid? UpdatedByUserId { get; set; }

    // =========================================================================
    // NAVIGATION PROPERTIES
    // =========================================================================
    // Comentado: Aguardando implementação completa da entidade Usuario
    // [ForeignKey(nameof(CreatedByUserId))]
    // public virtual Usuario? CreatedByUser { get; set; }

    // [ForeignKey(nameof(UpdatedByUserId))]
    // public virtual Usuario? UpdatedByUser { get; set; }

    // =========================================================================
    // INVERSE NAVIGATION
    // =========================================================================
    [InverseProperty(nameof(CapOcorrencias.TipoOcorrencia))]
    public virtual ICollection<CapOcorrencias> Ocorrencias { get; set; } = new List<CapOcorrencias>();
}