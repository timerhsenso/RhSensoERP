using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;

/// <summary>
/// CapResponsaveisContrato - Responsáveis por Contrato com Fornecedor
/// Tabela: cap_responsaveis_contrato (Multi-tenant)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "cap_responsaveis_contrato",
    DisplayName = "CapResponsaveisContrato",
    CdSistema = "CAP",
    CdFuncao = "CAP_FM_RESPONSAVEIS",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("cap_responsaveis_contrato")]
[HasDatabaseTriggers("Auditoria automática de CreatedAt/UpdatedAt via triggers SQL Server")]

public class CapResponsaveisContrato
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
    [Column("IdContrato")]
    [Display(Name = "ID Contrato")]
    public int IdContrato { get; set; }

    [Column("IdFuncionarioLegado")]
    [Display(Name = "ID Funcionário Legado")]
    public int? IdFuncionarioLegado { get; set; }

    [Column("TipoResponsabilidade", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Tipo Responsabilidade")]
    public string? TipoResponsabilidade { get; set; }

    [Column("DataInicio", TypeName = "date")]
    [Display(Name = "Data Início")]
    public DateOnly? DataInicio { get; set; }

    [Column("DataFim", TypeName = "date")]
    [Display(Name = "Data Fim")]
    public DateOnly? DataFim { get; set; }

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
    [ForeignKey(nameof(IdContrato))]
    public virtual CapContratosFornecedor? Contrato { get; set; }

 //   [ForeignKey(nameof(CreatedByUserId))]
//    public virtual Usuario? CreatedByUser { get; set; }

//    [ForeignKey(nameof(UpdatedByUserId))]
//    public virtual Usuario? UpdatedByUser { get; set; }
}