// =============================================================================
// RHSENSOERP - ENTITY CAPCONTRATOSFORNECEDOR
// =============================================================================
// Módulo: Gestão de Terceiros e Prestadores (CAP)
// Tabela: cap_contratos_fornecedor
// Schema: dbo
// Multi-tenant: ✅ SIM (TenantId obrigatório)
// 
// ✅ VALIDAÇÃO AUTOMÁTICA DE UNICIDADE:
// - NumeroContrato: Único por tenant
// =============================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;

/// <summary>
/// CapContratosFornecedor - Contratos com Fornecedores
/// Tabela: cap_contratos_fornecedor (Multi-tenant)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "cap_contratos_fornecedor",
    DisplayName = "CapContratosFornecedor",
    CdSistema = "CAP",
    CdFuncao = "CAP_FM_CONTRATOS",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("cap_contratos_fornecedor")]
[HasDatabaseTriggers("Auditoria automática de CreatedAt/UpdatedAt via triggers SQL Server")]
public class CapContratosFornecedor
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

    [Required]
    [Column("IdFornecedor")]
    [Display(Name = "ID Fornecedor")]
    public int IdFornecedor { get; set; }

    // =========================================================================
    // ✅ NÚMERO CONTRATO - VALIDAÇÃO AUTOMÁTICA DE UNICIDADE POR TENANT
    // =========================================================================
    // [Unique] faz com que o Source Generator crie:
    // 1. Índice único: CREATE UNIQUE INDEX UX_CapContratosFornecedor_Tenant_NumeroContrato 
    //    ON cap_contratos_fornecedor (TenantId, NumeroContrato)
    // 2. Validação no UniqueValidationBehavior ANTES de SaveChanges
    // 3. Exception DuplicateEntityException → HTTP 409 Conflict
    // =========================================================================
    [Required]
    [Unique(UniqueScope.Tenant, "Número do Contrato")]
    [Column("NumeroContrato", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Número Contrato")]
    public string NumeroContrato { get; set; } = string.Empty;

    [Required]
    [Column("DataInicio", TypeName = "date")]
    [Display(Name = "Data Início")]
    public DateOnly DataInicio { get; set; }

    [Column("DataFim", TypeName = "date")]
    [Display(Name = "Data Fim")]
    public DateOnly? DataFim { get; set; }

    [Column("Valor", TypeName = "decimal(15,2)")]
    [Display(Name = "Valor")]
    public decimal? Valor { get; set; }

    [Column("Descricao", TypeName = "nvarchar(500)")]
    [StringLength(500)]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Column("Status", TypeName = "nvarchar(50)")]
    [StringLength(50)]
    [Display(Name = "Status")]
    public string? Status { get; set; }

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
    [ForeignKey(nameof(IdFornecedor))]
    public virtual CapFornecedores? Fornecedor { get; set; }

    // Comentado: Aguardando implementação completa da entidade Usuario
    // [ForeignKey(nameof(CreatedByUserId))]
    // public virtual Usuario? CreatedByUser { get; set; }

    // [ForeignKey(nameof(UpdatedByUserId))]
    // public virtual Usuario? UpdatedByUser { get; set; }

    // =========================================================================
    // INVERSE NAVIGATION
    // =========================================================================
    [InverseProperty(nameof(CapResponsaveisContrato.Contrato))]
    public virtual ICollection<CapResponsaveisContrato> Responsaveis { get; set; } = new List<CapResponsaveisContrato>();
}