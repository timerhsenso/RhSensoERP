using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// BasAuditoriaAlteracoes - Auditoria de Alterações em Registros
/// Tabela: bas_auditoria_alteracoes (Multi-tenant, Append-Only)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "bas_auditoria_alteracoes",
    DisplayName = "BasAuditoriaAlteracoes",
    CdSistema = "BAS",
    CdFuncao = "BAS_FM_AUDITORIAALTERACOES",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("bas_auditoria_alteracoes")]
public class BasAuditoriaAlteracoes
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
    [Column("TabelaAfetada", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Tabela Afetada")]
    public string TabelaAfetada { get; set; } = string.Empty;

    [Required]
    [Column("IdRegistro", TypeName = "nvarchar(50)")]
    [StringLength(50)]
    [Display(Name = "ID Registro")]
    public string IdRegistro { get; set; } = string.Empty;

    [Required]
    [Column("TipoOperacao", TypeName = "nvarchar(20)")]
    [StringLength(20)]
    [Display(Name = "Tipo Operação")]
    public string TipoOperacao { get; set; } = string.Empty;

    [Column("CampoAlterado", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Campo Alterado")]
    public string? CampoAlterado { get; set; }

    [Column("ValorAnterior", TypeName = "nvarchar(max)")]
    [Display(Name = "Valor Anterior")]
    public string? ValorAnterior { get; set; }

    [Column("ValorNovo", TypeName = "nvarchar(max)")]
    [Display(Name = "Valor Novo")]
    public string? ValorNovo { get; set; }

    // Default controlado pelo banco: SYSUTCDATETIME()
    [Required]
    [Column("DataAlteracao", TypeName = "datetime2(3)")]
    [Display(Name = "Data Alteração (UTC)")]
    public DateTime DataAlteracao { get; set; }

    [Column("UsuarioAlteracao")]
    [Display(Name = "Usuário Alteração")]
    public Guid? UsuarioAlteracao { get; set; }

    [Column("EnderecoIp", TypeName = "nvarchar(50)")]
    [StringLength(50)]
    [Display(Name = "Endereço IP")]
    public string? EnderecoIp { get; set; }

    
   // [ForeignKey(nameof(UsuarioAlteracao))]
 //   public virtual Usuario? Usuario { get; set; }
}
