using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;

/// <summary>
/// CapContatosEmergencia - Contatos de Emergência de Funcionários e Colaboradores
/// Tabela: cap_contatos_emergencia (Multi-tenant)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "cap_contatos_emergencia",
    DisplayName = "CapContatosEmergencia",
    CdSistema = "CAP",
    CdFuncao = "CAP_FM_CONTATOSEMERGENCIA",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("cap_contatos_emergencia")]
[HasDatabaseTriggers("Auditoria automática de CreatedAt/UpdatedAt via triggers SQL Server")]

public class CapContatosEmergencia
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

    [Required]
    [Column("Nome", TypeName = "nvarchar(255)")]
    [StringLength(255)]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("IdParentesco")]
    [Display(Name = "ID Parentesco")]
    public int? IdParentesco { get; set; }

    [Required]
    [Column("TelefonePrincipal", TypeName = "nvarchar(20)")]
    [StringLength(20)]
    [Display(Name = "Telefone Principal")]
    public string TelefonePrincipal { get; set; } = string.Empty;

    [Column("TelefoneSecundario", TypeName = "nvarchar(20)")]
    [StringLength(20)]
    [Display(Name = "Telefone Secundário")]
    public string? TelefoneSecundario { get; set; }

    [Column("Email", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "E-mail")]
    public string? Email { get; set; }

    [Column("Endereco", TypeName = "nvarchar(500)")]
    [StringLength(500)]
    [Display(Name = "Endereço")]
    public string? Endereco { get; set; }

    [Column("Cidade", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Cidade")]
    public string? Cidade { get; set; }

    [Column("IdUf")]
    [Display(Name = "ID UF")]
    public int? IdUf { get; set; }

    [Column("Cep", TypeName = "nvarchar(10)")]
    [StringLength(10)]
    [Display(Name = "CEP")]
    public string? Cep { get; set; }

    [Required]
    [Column("OrdemPrioridade")]
    [Display(Name = "Ordem Prioridade")]
    public int OrdemPrioridade { get; set; } = 1;

    [Column("Observacoes", TypeName = "nvarchar(500)")]
    [StringLength(500)]
    [Display(Name = "Observações")]
    public string? Observacoes { get; set; }

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
    [ForeignKey(nameof(IdColaboradorFornecedor))]
    public virtual CapColaboradoresFornecedor? ColaboradorFornecedor { get; set; }

    [ForeignKey(nameof(IdParentesco))]
    public virtual BasParentescos? Parentesco { get; set; }

    [ForeignKey(nameof(IdUf))]
    public virtual BasUfs? Uf { get; set; }

  //  [ForeignKey(nameof(CreatedByUserId))]
  //  public virtual Usuario? CreatedByUser { get; set; }

  //  [ForeignKey(nameof(UpdatedByUserId))]
   // public virtual Usuario? UpdatedByUser { get; set; }
}