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

    [Column("Cpf", TypeName = "nvarchar(14)")]
    [StringLength(14)]
    [Display(Name = "CPF")]
    public string? Cpf { get; set; }

    [Column("Rg", TypeName = "nvarchar(20)")]
    [StringLength(20)]
    [Display(Name = "RG")]
    public string? Rg { get; set; }

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

    // Auditoria (defaults controlados pelo banco: SYSUTCDATETIME())
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

    // Navigation Properties
  //  [ForeignKey(nameof(CreatedByUserId))]
 //   public virtual Usuario? CreatedByUser { get; set; }

  //  [ForeignKey(nameof(UpdatedByUserId))]
  //  public virtual Usuario? UpdatedByUser { get; set; }

    // Inverse Navigation
    [InverseProperty(nameof(CapBloqueiosPessoa.Visitante))]
    public virtual ICollection<CapBloqueiosPessoa> Bloqueios { get; set; } = new List<CapBloqueiosPessoa>();

    [InverseProperty(nameof(CapHistoricoBloqueios.Visitante))]
    public virtual ICollection<CapHistoricoBloqueios> HistoricoBloqueios { get; set; } = new List<CapHistoricoBloqueios>();
}