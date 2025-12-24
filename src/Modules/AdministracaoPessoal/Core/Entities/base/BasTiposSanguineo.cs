using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// BasTiposSanguineo - Cadastro de Tipos Sanguíneos
/// Tabela: bas_tipos_sanguineo (Global - sem TenantId)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "bas_tipos_sanguineo",
    DisplayName = "BasTiposSanguineo",
    CdSistema = "BAS",
    CdFuncao = "BAS_FM_TIPOSSANGUINEO",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("bas_tipos_sanguineo")]
public class BasTiposSanguineo
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "ID")]
    public int Id { get; set; }

    [Required]
    [Column("Tipo", TypeName = "nvarchar(10)")]
    [StringLength(10)]
    [Display(Name = "Tipo")]
    public string Tipo { get; set; } = string.Empty;

    [Column("Descricao", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Required]
    [Column("Ativo")]
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; }

    // Auditoria — defaults controlados pelo banco (SYSUTCDATETIME())
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

    
 //   [ForeignKey(nameof(CreatedByUserId))]
  //  public virtual Usuario? CreatedByUser { get; set; }

  //  [ForeignKey(nameof(UpdatedByUserId))]
   //  public virtual Usuario? UpdatedByUser { get; set; }
}
