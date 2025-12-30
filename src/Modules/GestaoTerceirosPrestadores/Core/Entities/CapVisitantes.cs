// =============================================================================
// RHSENSOERP - ENTITY CAPVISITANTES
// =============================================================================
// Módulo: Gestão de Terceiros e Prestadores (CAP)
// Tabela: cap_visitantes
// Schema: dbo
// Multi-tenant: ✅ SIM (TenantId obrigatório)
// 
// ✅ VALIDAÇÃO AUTOMÁTICA DE UNICIDADE:
// - CPF: Único por tenant
// - Email: Único por tenant
// 
// O Source Generator irá criar automaticamente:
// 1. Índices únicos no banco de dados
// 2. Validação de duplicatas no pipeline MediatR (ANTES do SaveChanges)
// 3. Retorno HTTP 409 Conflict em caso de duplicata
// =============================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;

/// <summary>
/// CapVisitantes - Cadastro de Visitantes
/// Tabela: cap_visitantes (Multi-tenant)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "cap_visitantes",
    DisplayName = "CapVisitantes",
    CdSistema = "CAP",
    CdFuncao = "CAP_FM_VISITANTES",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("cap_visitantes")]
[HasDatabaseTriggers("Auditoria automática de CreatedAt/UpdatedAt via triggers SQL Server")]
public class CapVisitantes
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
    [Column("Nome", TypeName = "nvarchar(255)")]
    [StringLength(255)]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    // =========================================================================
    // ✅ CPF - VALIDAÇÃO AUTOMÁTICA DE UNICIDADE POR TENANT
    // =========================================================================
    // [Unique] faz com que o Source Generator crie:
    // 1. Índice único: CREATE UNIQUE INDEX UX_CapVisitantes_Tenant_Cpf 
    //    ON cap_visitantes (TenantId, Cpf) WHERE Cpf IS NOT NULL
    // 2. Validação no UniqueValidationBehavior ANTES de SaveChanges
    // 3. Exception DuplicateEntityException → HTTP 409 Conflict
    // =========================================================================
    [Unique(UniqueScope.Tenant, "CPF")]
    [Column("Cpf", TypeName = "nvarchar(14)")]
    [StringLength(14)]
    [Display(Name = "CPF")]
    public string? Cpf { get; set; }

    [Column("Rg", TypeName = "nvarchar(20)")]
    [StringLength(20)]
    [Display(Name = "RG")]
    public string? Rg { get; set; }

    // =========================================================================
    // ✅ EMAIL - VALIDAÇÃO AUTOMÁTICA DE UNICIDADE POR TENANT
    // =========================================================================
    // Mensagem de erro customizada: "E-mail '{valor}' já está em uso."
    // =========================================================================
    [Unique(UniqueScope.Tenant, DisplayName = "E-mail", ErrorMessage = "E-mail '{PropertyValue}' já está em uso.")]
    [Column("Email", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "E-mail")]
    public string? Email { get; set; }

    [Column("Telefone", TypeName = "nvarchar(20)")]
    [StringLength(20)]
    [Display(Name = "Telefone")]
    public string? Telefone { get; set; }

    [Column("Empresa", TypeName = "nvarchar(255)")]
    [StringLength(255)]
    [Display(Name = "Empresa")]
    public string? Empresa { get; set; }

    [Column("IdFuncionarioResponsavel")]
    [Display(Name = "ID Funcionário Responsável")]
    public int? IdFuncionarioResponsavel { get; set; }

    [Required]
    [Column("RequerResponsavel")]
    [Display(Name = "Requer Responsável")]
    public bool RequerResponsavel { get; set; } = false;

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
    [InverseProperty(nameof(CapBloqueiosPessoa.Visitante))]
    public virtual ICollection<CapBloqueiosPessoa> Bloqueios { get; set; } = new List<CapBloqueiosPessoa>();

    [InverseProperty(nameof(CapHistoricoBloqueios.Visitante))]
    public virtual ICollection<CapHistoricoBloqueios> HistoricoBloqueios { get; set; } = new List<CapHistoricoBloqueios>();
}