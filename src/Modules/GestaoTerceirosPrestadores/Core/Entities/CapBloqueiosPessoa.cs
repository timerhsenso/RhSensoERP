// =============================================================================
// RHSENSOERP - ENTITY CAPBLOQUEIOSPESSOA
// =============================================================================
// Módulo: Gestão de Terceiros e Prestadores (CAP)
// Tabela: cap_bloqueios_pessoa
// Schema: dbo
// Multi-tenant: ✅ SIM (TenantId obrigatório)
// =============================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;

/// <summary>
/// CapBloqueiosPessoa - Bloqueios de Pessoas (Funcionários, Colaboradores e Visitantes)
/// Tabela: cap_bloqueios_pessoa (Multi-tenant)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "cap_bloqueios_pessoa",
    DisplayName = "CapBloqueiosPessoa",
    CdSistema = "CAP",
    CdFuncao = "CAP_FM_BLOQUEIOS",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("cap_bloqueios_pessoa")]
[HasDatabaseTriggers("Auditoria automática de CreatedAt/UpdatedAt via triggers SQL Server")]
public class CapBloqueiosPessoa
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

    [Column("IdFuncionarioLegado")]
    [Display(Name = "ID Funcionário Legado")]
    public int? IdFuncionarioLegado { get; set; }

    [Column("IdColaboradorFornecedor")]
    [Display(Name = "ID Colaborador Fornecedor")]
    public int? IdColaboradorFornecedor { get; set; }

    [Column("IdVisitante")]
    [Display(Name = "ID Visitante")]
    public int? IdVisitante { get; set; }

    [Required]
    [Column("Motivo", TypeName = "nvarchar(500)")]
    [StringLength(500)]
    [Display(Name = "Motivo")]
    public string Motivo { get; set; } = string.Empty;

    // Default controlado pelo banco: SYSUTCDATETIME()
    [Required]
    [Column("DataBloqueio", TypeName = "datetime2(3)")]
    [Display(Name = "Data Bloqueio (UTC)")]
    public DateTime DataBloqueio { get; set; }

    [Column("DataDesbloqueio", TypeName = "datetime2(3)")]
    [Display(Name = "Data Desbloqueio (UTC)")]
    public DateTime? DataDesbloqueio { get; set; }

    [Column("UsuarioBloqueio")]
    [Display(Name = "Usuário Bloqueio")]
    public Guid? UsuarioBloqueio { get; set; }

    [Column("UsuarioDesbloqueio")]
    [Display(Name = "Usuário Desbloqueio")]
    public Guid? UsuarioDesbloqueio { get; set; }

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
    [ForeignKey(nameof(IdColaboradorFornecedor))]
    public virtual CapColaboradoresFornecedor? ColaboradorFornecedor { get; set; }

    [ForeignKey(nameof(IdVisitante))]
    public virtual CapVisitantes? Visitante { get; set; }

    // Comentado: Aguardando implementação completa da entidade Usuario
    // [ForeignKey(nameof(UsuarioBloqueio))]
    // public virtual Usuario? UsuarioBloqueador { get; set; }

    // [ForeignKey(nameof(UsuarioDesbloqueio))]
    // public virtual Usuario? UsuarioDesbloqueador { get; set; }

    // [ForeignKey(nameof(CreatedByUserId))]
    // public virtual Usuario? CreatedByUser { get; set; }

    // [ForeignKey(nameof(UpdatedByUserId))]
    // public virtual Usuario? UpdatedByUser { get; set; }

    // =========================================================================
    // INVERSE NAVIGATION
    // =========================================================================
    [InverseProperty(nameof(CapHistoricoBloqueios.Bloqueio))]
    public virtual ICollection<CapHistoricoBloqueios> Historico { get; set; } = new List<CapHistoricoBloqueios>();
}