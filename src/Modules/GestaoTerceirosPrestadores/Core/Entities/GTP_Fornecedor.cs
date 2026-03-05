using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;


// =============================================================================
// GTP_Fornecedor
// =============================================================================

/// <summary>
/// Cadastro principal de fornecedores PJ e PF.
/// Flags rápidas Ativo/Bloqueado/Homologado evitam JOIN nas listagens.
/// Histórico de bloqueio em GTP_FornecedorBloqueio.
/// Tabela: GTP__Fornecedor
/// </summary>
[GenerateCrud(
    TableName = "GTP_Fornecedor",
    DisplayName = "Fornecedor",
    CdSistema = "GTP",
    CdFuncao = "GTP_WEB_FORNECEDOR",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("GTP_Fornecedor")]
public class GTP_Fornecedor
{
    /// <summary>
    /// Chave primária identity.
    /// SQL: Id INT IDENTITY(1,1) NOT NULL
    /// </summary>
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "ID")]
    public int Id { get; set; }

    /// <summary>
    /// Identificador do tenant (multi-tenant).
    /// SQL: IdSaas INT NOT NULL
    /// </summary>
    [Required]
    [Column("TenantId")]
    [Display(Name = "TenantId")]
    public Guid TenantId { get; set; }

    // ---- Chaves corporativas ERP legado ----------------------------------

    /// <summary>SQL: nomatric CHAR(8) NULL</summary>
    [Column("nomatric")]
    [StringLength(8)]
    [Display(Name = "Matrícula")]
    public string? Nomatric { get; set; }

    /// <summary>SQL: cdempresa INT NULL</summary>
    [Column("cdempresa")]
    [Display(Name = "Empresa")]
    public int? Cdempresa { get; set; }

    /// <summary>SQL: cdfilial INT NULL</summary>
    [Column("cdfilial")]
    [Display(Name = "Filial")]
    public int? Cdfilial { get; set; }

    /// <summary>
    /// Código do fornecedor no ERP de origem (opcional, para integração).
    /// SQL: CodigoInterno VARCHAR(20) NULL
    /// </summary>
    [Column("CodigoInterno")]
    [StringLength(20)]
    [Display(Name = "Código Interno")]
    public string? CodigoInterno { get; set; }

    // ---- Identificação ---------------------------------------------------

    /// <summary>
    /// Tipo de pessoa: 'J' = Jurídica | 'F' = Física.
    /// SQL: TipoPessoa CHAR(1) NOT NULL — CHECK IN ('J','F')
    /// </summary>
    [Required]
    [Column("TipoPessoa")]
    [StringLength(1)]
    [Display(Name = "Tipo Pessoa")]
    public string TipoPessoa { get; set; } = null!;

    /// <summary>
    /// CNPJ (somente dígitos, 14 chars). Obrigatório quando TipoPessoa = 'J'.
    /// SQL: Cnpj CHAR(14) NULL
    /// </summary>
    [Column("Cnpj")]
    [StringLength(14)]
    [Display(Name = "CNPJ")]
    public string? Cnpj { get; set; }

    /// <summary>
    /// CPF (somente dígitos, 11 chars). Obrigatório quando TipoPessoa = 'F'.
    /// SQL: Cpf CHAR(11) NULL
    /// </summary>
    [Column("Cpf")]
    [StringLength(11)]
    [Display(Name = "CPF")]
    public string? Cpf { get; set; }

    // ---- Pessoa Jurídica -------------------------------------------------

    /// <summary>SQL: RazaoSocial NVARCHAR(200) NULL</summary>
    [Column("RazaoSocial")]
    [StringLength(200)]
    [Display(Name = "Razão Social")]
    public string? RazaoSocial { get; set; }

    /// <summary>SQL: NomeFantasia NVARCHAR(200) NULL</summary>
    [Column("NomeFantasia")]
    [StringLength(200)]
    [Display(Name = "Nome Fantasia")]
    public string? NomeFantasia { get; set; }

    /// <summary>
    /// Porte da empresa: ME | EPP | MED | GRD.
    /// SQL: PorteEmpresa CHAR(3) NULL — CHECK IN ('ME','EPP','MED','GRD')
    /// </summary>
    [Column("PorteEmpresa")]
    [StringLength(3)]
    [Display(Name = "Porte")]
    public string? PorteEmpresa { get; set; }

    /// <summary>
    /// Regime tributário: 1=Simples Nacional | 2=Lucro Presumido | 3=Lucro Real | 4=MEI.
    /// SQL: RegimeTributario TINYINT NULL — CHECK IN (1,2,3,4)
    /// </summary>
    [Column("RegimeTributario")]
    [Display(Name = "Regime Tributário")]
    public byte? RegimeTributario { get; set; }

    /// <summary>SQL: OptanteSimples BIT NULL</summary>
    [Column("OptanteSimples")]
    [Display(Name = "Optante Simples")]
    public bool? OptanteSimples { get; set; }

    /// <summary>SQL: ContribuinteICMS BIT NULL</summary>
    [Column("ContribuinteICMS")]
    [Display(Name = "Contribuinte ICMS")]
    public bool? ContribuinteICMS { get; set; }

    /// <summary>
    /// CNAE principal (7 dígitos sem ponto/barra). Ex: '6201500'.
    /// SQL: CnaeCode CHAR(7) NULL
    /// </summary>
    [Column("CnaeCode")]
    [StringLength(7)]
    [Display(Name = "CNAE")]
    public string? CnaeCode { get; set; }

    /// <summary>
    /// Código da Natureza Jurídica (tabela IBGE, 4 dígitos). Ex: '2062'.
    /// SQL: NaturezaJuridica CHAR(4) NULL
    /// </summary>
    [Column("NaturezaJuridica")]
    [StringLength(4)]
    [Display(Name = "Natureza Jurídica")]
    public string? NaturezaJuridica { get; set; }

    // ---- Pessoa Física ---------------------------------------------------

    /// <summary>SQL: Nome NVARCHAR(200) NULL</summary>
    [Column("Nome")]
    [StringLength(200)]
    [Display(Name = "Nome")]
    public string? Nome { get; set; }

    /// <summary>SQL: DataNascimento DATE NULL</summary>
    [Column("DataNascimento")]
    [Display(Name = "Data Nascimento")]
    public DateOnly? DataNascimento { get; set; }

    /// <summary>
    /// Gênero: 'M' = Masculino | 'F' = Feminino | 'N' = Não informado.
    /// SQL: Genero CHAR(1) NULL — CHECK IN ('M','F','N')
    /// </summary>
    [Column("Genero")]
    [StringLength(1)]
    [Display(Name = "Gênero")]
    public string? Genero { get; set; }

    // ---- Classificação ---------------------------------------------------

    /// <summary>
    /// Segmento de atuação. Ex: 'Tecnologia', 'Construção Civil'.
    /// SQL: Segmento NVARCHAR(80) NULL
    /// </summary>
    [Column("Segmento")]
    [StringLength(80)]
    [Display(Name = "Segmento")]
    public string? Segmento { get; set; }

    /// <summary>
    /// Origem do cadastro: MANUAL | API | IMPORTACAO | CNPJ_WS.
    /// SQL: OrigemCadastro VARCHAR(20) NULL
    /// </summary>
    [Column("OrigemCadastro")]
    [StringLength(20)]
    [Display(Name = "Origem Cadastro")]
    public string? OrigemCadastro { get; set; }

    // ---- Status (flags rápidas — sem JOIN nas listagens) -----------------

    /// <summary>SQL: Ativo BIT NOT NULL DEFAULT 1</summary>
    [Column("Ativo")]
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    /// <summary>
    /// Flag de bloqueio rápido. Detalhe do bloqueio em GTP_FornecedorBloqueio.
    /// SQL: Bloqueado BIT NOT NULL DEFAULT 0
    /// </summary>
    [Column("Bloqueado")]
    [Display(Name = "Bloqueado")]
    public bool Bloqueado { get; set; } = false;

    /// <summary>
    /// Indica se o fornecedor passou pelo processo de homologação.
    /// SQL: Homologado BIT NOT NULL DEFAULT 0
    /// </summary>
    [Column("Homologado")]
    [Display(Name = "Homologado")]
    public bool Homologado { get; set; } = false;

    /// <summary>SQL: HomologadoEmUtc DATETIME2(3) NULL</summary>
    [Column("HomologadoEmUtc")]
    [Display(Name = "Homologado em")]
    public DateTime? HomologadoEmUtc { get; set; }

    /// <summary>SQL: HomologadoPorUserId UNIQUEIDENTIFIER NULL</summary>
    [Column("HomologadoPorUserId")]
    [Display(Name = "Homologado por")]
    public Guid? HomologadoPorUserId { get; set; }

    /// <summary>SQL: Observacoes NVARCHAR(MAX) NULL</summary>
    [Column("Observacoes")]
    [Display(Name = "Observações")]
    public string? Observacoes { get; set; }

    // ---- Auditoria -------------------------------------------------------

    /// <summary>SQL: CreatedAtUtc DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()</summary>
    [Column("CreatedAtUtc")]
    [Display(Name = "Criado em")]
    public DateTime? CreatedAtUtc { get; set; }

    /// <summary>SQL: UpdatedAtUtc DATETIME2(3) NOT NULL — mantido pelo trigger.</summary>
    [Column("UpdatedAtUtc")]
    [Display(Name = "Atualizado em")]
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>SQL: CreatedByUserId UNIQUEIDENTIFIER NULL</summary>
    [Column("CreatedByUserId")]
    [Display(Name = "Criado por")]
    public Guid? CreatedByUserId { get; set; }

    /// <summary>SQL: UpdatedByUserId UNIQUEIDENTIFIER NULL</summary>
    [Column("UpdatedByUserId")]
    [Display(Name = "Atualizado por")]
    public Guid? UpdatedByUserId { get; set; }

    /// <summary>SQL: RowVer ROWVERSION NOT NULL</summary>
    [Timestamp]
    [Column("RowVer")]
    [Display(Name = "RowVer")]
    public byte[] RowVer { get; set; } = null!;

    // ---- Navegação -------------------------------------------------------

    /// <summary>Dados fiscais e financeiros (1:1).</summary>
    public virtual GTP_FornecedorFiscalFinanceiro? FiscalFinanceiro { get; set; }

    /// <summary>Contas bancárias do fornecedor (1:N).</summary>
    public virtual ICollection<GTP_FornecedorContaBancaria> ContasBancarias { get; set; } = new List<GTP_FornecedorContaBancaria>();

    /// <summary>Contatos do fornecedor (1:N).</summary>
    public virtual ICollection<GTP_FornecedorContato> Contatos { get; set; } = new List<GTP_FornecedorContato>();

    /// <summary>Endereços do fornecedor (1:N).</summary>
    public virtual ICollection<GTP_FornecedorEndereco> Enderecos { get; set; } = new List<GTP_FornecedorEndereco>();

    /// <summary>Documentos de compliance do fornecedor (1:N).</summary>
    public virtual ICollection<GTP_FornecedorDocumento> Documentos { get; set; } = new List<GTP_FornecedorDocumento>();

    /// <summary>Histórico de bloqueios do fornecedor (1:N).</summary>
    public virtual ICollection<GTP_FornecedorBloqueio> Bloqueios { get; set; } = new List<GTP_FornecedorBloqueio>();
}

