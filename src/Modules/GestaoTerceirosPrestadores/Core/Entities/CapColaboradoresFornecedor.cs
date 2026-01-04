// =============================================================================
// RHSENSOERP - ENTITY CAPCOLABORADORESFORNECEDOR
// =============================================================================
// Módulo: Gestão de Terceiros e Prestadores (CAP)
// Tabela: cap_colaboradores_fornecedor
// Schema: dbo
// Multi-tenant: ✅ SIM (TenantId obrigatório)
// 
// ✅ VALIDAÇÃO AUTOMÁTICA DE UNICIDADE:
// - CPF: Único por tenant
// =============================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;

/// <summary>
/// CapColaboradoresFornecedor - Cadastro de Colaboradores de Fornecedores (Funcionários Terceirizados)
/// Tabela: cap_colaboradores_fornecedor (Multi-tenant)
/// Fonte da verdade: SQL Server
/// </summary>
[GenerateCrud(
    TableName = "cap_colaboradores_fornecedor",
    DisplayName = "CapColaboradoresFornecedor",
    CdSistema = "CAP",
    CdFuncao = "CAP_FM_COLABORADORES",
    IsLegacyTable = false,
    GenerateApiController = true

)]
[Table("cap_colaboradores_fornecedor")]
[HasDatabaseTriggers("Auditoria automática de CreatedAt/UpdatedAt via triggers SQL Server")]
public class CapColaboradoresFornecedor
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
    [Column("IdFornecedor")]
    [Display(Name = "ID Fornecedor")]
    public int IdFornecedor { get; set; }

    [Required]
    [Column("Nome", TypeName = "nvarchar(255)")]
    [StringLength(255)]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    // =========================================================================
    // ✅ CPF - VALIDAÇÃO AUTOMÁTICA DE UNICIDADE POR TENANT
    // =========================================================================
    // [Unique] faz com que o Source Generator crie:
    // 1. Índice único: CREATE UNIQUE INDEX UX_CapColaboradoresFornecedor_Tenant_Cpf 
    //    ON cap_colaboradores_fornecedor (TenantId, Cpf)
    // 2. Validação no UniqueValidationBehavior ANTES de SaveChanges
    // 3. Exception DuplicateEntityException → HTTP 409 Conflict
    // =========================================================================
    [Required]
    [Unique(UniqueScope.Tenant, "CPF")]
    [Column("Cpf", TypeName = "nvarchar(14)")]
    [StringLength(14)]
    [Display(Name = "CPF")]
    public string Cpf { get; set; } = string.Empty;

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

    [Column("DataNascimento", TypeName = "date")]
    [Display(Name = "Data Nascimento")]
    public DateOnly? DataNascimento { get; set; }

    [Column("Genero", TypeName = "nvarchar(20)")]
    [StringLength(20)]
    [Display(Name = "Gênero")]
    public string? Genero { get; set; }

    [Column("EstadoCivil", TypeName = "nvarchar(50)")]
    [StringLength(50)]
    [Display(Name = "Estado Civil")]
    public string? EstadoCivil { get; set; }

    [Column("IdTipoSanguineo")]
    [Display(Name = "ID Tipo Sanguíneo")]
    public int? IdTipoSanguineo { get; set; }

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

    [Required]
    [Column("DataAdmissao", TypeName = "date")]
    [Display(Name = "Data Admissão")]
    public DateOnly DataAdmissao { get; set; }

    [Column("DataDemissao", TypeName = "date")]
    [Display(Name = "Data Demissão")]
    public DateOnly? DataDemissao { get; set; }

    [Column("Cargo", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Cargo")]
    public string? Cargo { get; set; }

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
    [ForeignKey(nameof(IdFornecedor))]
    [NavigationDisplay(
        Property = "RazaoSocial",
        GridColumn = true,
        GridHeader = "Fornecedor"
    )]
    public virtual CapFornecedores? Fornecedor { get; set; }

    [ForeignKey(nameof(IdTipoSanguineo))]
    [NavigationDisplay(
        Property = "Descricao",
        Module = "AdministracaoPessoal",
        EntityRoute = "tiposanguineo",
        GridColumn = true,
        GridHeader = "Tipo Sanguíneo"
    )]
    public virtual BasTiposSanguineo? TipoSanguineo { get; set; }

    [ForeignKey(nameof(IdUf))]
    [NavigationDisplay(
        Property = "Sigla",
        Module = "AdministracaoPessoal",
        EntityRoute = "ufs",
        GridColumn = true,
        GridHeader = "UF"
    )]
    public virtual BasUfs? Uf { get; set; }

    // Comentado: Aguardando implementação completa da entidade Usuario
    // [ForeignKey(nameof(CreatedByUserId))]
    // public virtual Usuario? CreatedByUser { get; set; }

    // [ForeignKey(nameof(UpdatedByUserId))]
    // public virtual Usuario? UpdatedByUser { get; set; }

    // =========================================================================
    // INVERSE NAVIGATION
    // =========================================================================
    [InverseProperty(nameof(CapBloqueiosPessoa.ColaboradorFornecedor))]
    public virtual ICollection<CapBloqueiosPessoa> Bloqueios { get; set; } = new List<CapBloqueiosPessoa>();

    [InverseProperty(nameof(CapHistoricoBloqueios.ColaboradorFornecedor))]
    public virtual ICollection<CapHistoricoBloqueios> HistoricoBloqueios { get; set; } = new List<CapHistoricoBloqueios>();

    [InverseProperty(nameof(CapContatosEmergencia.ColaboradorFornecedor))]
    public virtual ICollection<CapContatosEmergencia> ContatosEmergencia { get; set; } = new List<CapContatosEmergencia>();

    [InverseProperty(nameof(CapOcorrencias.ColaboradorResponsavel))]
    public virtual ICollection<CapOcorrencias> OcorrenciasComoResponsavel { get; set; } = new List<CapOcorrencias>();
}