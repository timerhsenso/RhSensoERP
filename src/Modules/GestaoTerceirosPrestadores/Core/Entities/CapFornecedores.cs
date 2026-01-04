// =============================================================================
// RHSENSOERP - ENTITY CAPFORNECEDORES
// =============================================================================
// Módulo: Gestão de Terceiros e Prestadores (CAP)
// Tabela: cap_fornecedores
// Schema: dbo
// Multi-tenant: ✅ SIM (TenantId obrigatório)
// 
// ✅ VALIDAÇÃO AUTOMÁTICA DE UNICIDADE:
// - CNPJ: Único por tenant
// =============================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;

/// <summary>
/// CapFornecedores - Cadastro de Fornecedores (Empresas Terceirizadas)
/// Tabela: cap_fornecedores (Multi-tenant)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "cap_fornecedores",
    DisplayName = "CapFornecedores",
    CdSistema = "SEG",
    CdFuncao = "SEG_FM_TSISTEMA",
    IsLegacyTable = false,
    GenerateApiController = true,
    GenerateLookup = true
)]
[Table("cap_fornecedores")]
[HasDatabaseTriggers("Auditoria automática de CreatedAt/UpdatedAt via triggers SQL Server")]
public class CapFornecedores
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "ID")]
    [LookupKey]
    public int Id { get; set; }

    [Required]
    [Column("TenantId")]
    [Display(Name = "Tenant ID")]
    public Guid TenantId { get; set; }

    [Required]
    [Column("RazaoSocial", TypeName = "nvarchar(255)")]
    [StringLength(255)]
    [Display(Name = "Razão Social")]
    [LookupText]
    public string RazaoSocial { get; set; } = string.Empty;

    [Column("NomeFantasia", TypeName = "nvarchar(255)")]
    [StringLength(255)]
    [Display(Name = "Nome Fantasia")]
    [LookupText]
    public string? NomeFantasia { get; set; }

    // =========================================================================
    // ✅ CNPJ - VALIDAÇÃO AUTOMÁTICA DE UNICIDADE POR TENANT
    // =========================================================================
    // [Unique] faz com que o Source Generator crie:
    // 1. Índice único: CREATE UNIQUE INDEX UX_CapFornecedores_Tenant_Cnpj 
    //    ON cap_fornecedores (TenantId, Cnpj) WHERE Cnpj IS NOT NULL
    // 2. Validação no UniqueValidationBehavior ANTES de SaveChanges
    // 3. Exception DuplicateEntityException → HTTP 409 Conflict
    // =========================================================================
    [Unique(UniqueScope.Tenant, "CNPJ")]
    [Column("Cnpj", TypeName = "nvarchar(18)")]
    [StringLength(18)]
    [Display(Name = "CNPJ")]
    public string? Cnpj { get; set; }

    [Column("Cpf", TypeName = "nvarchar(14)")]
    [StringLength(14)]
    [Display(Name = "CPF")]
    public string? Cpf { get; set; }

    [Column("Email", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "E-mail")]
    public string? Email { get; set; }

    [Column("Telefone", TypeName = "nvarchar(20)")]
    [StringLength(20)]
    [Display(Name = "Telefone")]
    public string? Telefone { get; set; }

    [Column("Endereco", TypeName = "nvarchar(500)")]
    [StringLength(500)]
    [Display(Name = "Endereço")]
    public string? Endereco { get; set; }

    [Column("Numero", TypeName = "nvarchar(20)")]
    [StringLength(20)]
    [Display(Name = "Número")]
    public string? Numero { get; set; }

    [Column("Complemento", TypeName = "nvarchar(200)")]
    [StringLength(200)]
    [Display(Name = "Complemento")]
    public string? Complemento { get; set; }

    [Column("Bairro", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Bairro")]
    public string? Bairro { get; set; }

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

    [Column("Contato", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Contato")]
    public string? Contato { get; set; }

    [Column("ContatoTelefone", TypeName = "nvarchar(20)")]
    [StringLength(20)]
    [Display(Name = "Telefone Contato")]
    public string? ContatoTelefone { get; set; }

    [Column("ContatoEmail", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "E-mail Contato")]
    public string? ContatoEmail { get; set; }

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
    [ForeignKey(nameof(IdUf))]
    public virtual BasUfs? Uf { get; set; }

    // Comentado: Aguardando implementação completa da entidade Usuario
    // [ForeignKey(nameof(CreatedByUserId))]
    // public virtual Usuario? CreatedByUser { get; set; }

    // [ForeignKey(nameof(UpdatedByUserId))]
    // public virtual Usuario? UpdatedByUser { get; set; }

    // =========================================================================
    // INVERSE NAVIGATION
    // =========================================================================
    [InverseProperty(nameof(CapColaboradoresFornecedor.Fornecedor))]
    public virtual ICollection<CapColaboradoresFornecedor> Colaboradores { get; set; } = new List<CapColaboradoresFornecedor>();

    [InverseProperty(nameof(CapContratosFornecedor.Fornecedor))]
    public virtual ICollection<CapContratosFornecedor> Contratos { get; set; } = new List<CapContratosFornecedor>();
}