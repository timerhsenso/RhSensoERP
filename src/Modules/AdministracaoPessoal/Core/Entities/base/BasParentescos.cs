using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// BasParentescos - Cadastro de Parentescos
/// Tabela: bas_parentescos (Global - sem TenantId)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "bas_parentescos",
    DisplayName = "BasParentescos",
    CdSistema = "BAS",
    CdFuncao = "BAS_FM_PARENTESCOS",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("bas_parentescos")]
public class BasParentescos
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "ID")]
    public int Id { get; set; }

    [Required]
    [Column("Nome", TypeName = "nvarchar(50)")]
    [StringLength(50)]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("Descricao", TypeName = "nvarchar(200)")]
    [StringLength(200)]
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
 //   public virtual Usuario? CreatedByUser { get; set; }
    
 //   [ForeignKey(nameof(UpdatedByUserId))]
 //   public virtual Usuario? UpdatedByUser { get; set; }
}
