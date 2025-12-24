using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// BasUfs - Unidades Federativas do Brasil
/// Tabela: bas_ufs (Global - sem TenantId)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "bas_ufs",
    DisplayName = "BasUfs",
    CdSistema = "BAS",
    CdFuncao = "BAS_FM_UFS",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("bas_ufs")]
public class BasUfs
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "ID")]
    public int Id { get; set; }

    [Required]
    [Column("Sigla", TypeName = "nvarchar(2)")]
    [StringLength(2)]
    [Display(Name = "Sigla")]
    public string Sigla { get; set; } = string.Empty;

    [Required]
    [Column("Nome", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

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
 //   [ForeignKey(nameof(CreatedByUserId))]
//    public virtual Usuario? CreatedByUser { get; set; }

//    [ForeignKey(nameof(UpdatedByUserId))]
 //   public virtual Usuario? UpdatedByUser { get; set; }
}