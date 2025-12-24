using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// BasFeriados - Feriados Corporativos
/// Tabela: bas_feriados (Multi-tenant)
/// </summary>
[GenerateCrud(
    TableName = "bas_feriados",
    DisplayName = "BasFeriados",
    CdSistema = "BAS",
    CdFuncao = "BAS_FM_FERIADOS",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("bas_feriados")]
public class BasFeriados
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
    [Column("Data", TypeName = "date")]
    [Display(Name = "Data")]
    public DateOnly Data { get; set; }

    [Required]
    [Column("Descricao", TypeName = "nvarchar(255)")]
    [StringLength(255)]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = string.Empty;

    [Column("Tipo", TypeName = "nvarchar(50)")]
    [StringLength(50)]
    [Display(Name = "Tipo")]
    public string? Tipo { get; set; }

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

    // Navigation Properties (habilitar quando a entidade Tuse1 existir no Core)
    // [ForeignKey(nameof(CreatedByUserId))]
    // public virtual Tuse1? CreatedByUser { get; set; }

    // [ForeignKey(nameof(UpdatedByUserId))]
    // public virtual Tuse1? UpdatedByUser { get; set; }
}
