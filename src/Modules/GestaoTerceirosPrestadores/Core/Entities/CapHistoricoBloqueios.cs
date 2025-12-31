// =============================================================================
// RHSENSOERP - ENTITY CAPHISTORICOBLOQUEIOS
// =============================================================================
// Módulo: Gestão de Terceiros e Prestadores (CAP)
// Tabela: cap_historico_bloqueios
// Schema: dbo
// Multi-tenant: ✅ SIM (TenantId obrigatório)
// Tipo: Append-Only (sem updates/deletes)
// =============================================================================
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;

/// <summary>
/// CapHistoricoBloqueios - Histórico de Bloqueios de Pessoas
/// Tabela: cap_historico_bloqueios (Multi-tenant, Append-Only)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "cap_historico_bloqueios",
    DisplayName = "CapHistoricoBloqueios",
    CdSistema = "CAP",
    CdFuncao = "CAP_FM_HISTORICOBLOQUEIOS",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("cap_historico_bloqueios")]
public class CapHistoricoBloqueios
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "ID")]
    public long Id { get; set; }

    [Required]
    [Column("TenantId")]
    [Display(Name = "Tenant ID")]
    public Guid TenantId { get; set; }

    [Required]
    [Column("IdBloqueio")]
    [Display(Name = "ID Bloqueio")]
    public int IdBloqueio { get; set; }

    [Column("IdFuncionarioLegado")]
    [Display(Name = "ID Funcionário Legado")]
    public int? IdFuncionarioLegado { get; set; }

    [Column("IdColaboradorFornecedor")]
    [Display(Name = "ID Colaborador Fornecedor")]
    public int? IdColaboradorFornecedor { get; set; }

    [Column("IdVisitante")]
    [Display(Name = "ID Visitante")]
    public int? IdVisitante { get; set; }

    [Column("Motivo", TypeName = "nvarchar(500)")]
    [StringLength(500)]
    [Display(Name = "Motivo")]
    public string? Motivo { get; set; }

    [Column("DataBloqueio", TypeName = "datetime2(3)")]
    [Display(Name = "Data Bloqueio (UTC)")]
    public DateTime? DataBloqueio { get; set; }

    [Column("DataDesbloqueio", TypeName = "datetime2(3)")]
    [Display(Name = "Data Desbloqueio (UTC)")]
    public DateTime? DataDesbloqueio { get; set; }

    [Column("UsuarioBloqueio")]
    [Display(Name = "Usuário Bloqueio")]
    public Guid? UsuarioBloqueio { get; set; }

    [Column("UsuarioDesbloqueio")]
    [Display(Name = "Usuário Desbloqueio")]
    public Guid? UsuarioDesbloqueio { get; set; }

    // Default controlado pelo banco: SYSUTCDATETIME()
    [Required]
    [Column("DataRegistro", TypeName = "datetime2(3)")]
    [Display(Name = "Data Registro (UTC)")]
    public DateTime DataRegistro { get; set; }

    // =========================================================================
    // NAVIGATION PROPERTIES
    // =========================================================================
    [ForeignKey(nameof(IdBloqueio))]
    public virtual CapBloqueiosPessoa? Bloqueio { get; set; }

    [ForeignKey(nameof(IdColaboradorFornecedor))]
    public virtual CapColaboradoresFornecedor? ColaboradorFornecedor { get; set; }

    [ForeignKey(nameof(IdVisitante))]
    public virtual CapVisitantes? Visitante { get; set; }

    // Comentado: Aguardando implementação completa da entidade Usuario
    // [ForeignKey(nameof(UsuarioBloqueio))]
    // public virtual Usuario? UsuarioBloqueador { get; set; }

    // [ForeignKey(nameof(UsuarioDesbloqueio))]
    // public virtual Usuario? UsuarioDesbloqueador { get; set; }
}