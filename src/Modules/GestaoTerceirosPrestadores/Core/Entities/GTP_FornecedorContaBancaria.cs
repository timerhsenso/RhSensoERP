using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;


// =============================================================================
// GTP_FornecedorContaBancaria
// =============================================================================

/// <summary>
/// Contas bancárias do fornecedor (1:N).
/// Suporte a CC/CP/CI, PIX (todos os tipos) e contas internacionais (IBAN/SWIFT).
/// UNIQUE INDEX garante apenas 1 conta com Principal = true por fornecedor/tenant.
/// Tabela: GTP__FornecedorContaBancaria
/// </summary>
[GenerateCrud(
    TableName = "GTP_FornecedorContaBancaria",
    DisplayName = "Conta Bancária do Fornecedor",
    CdSistema = "GTP",
    CdFuncao = "GTP_WEB_FORNECCONTA",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("GTP_FornecedorContaBancaria")]
public class GTP_FornecedorContaBancaria
{
    /// <summary>SQL: Id INT IDENTITY(1,1) NOT NULL</summary>
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "ID")]
    public int Id { get; set; }

    /// <summary>SQL: IdSaas INT NOT NULL</summary>
    [Required]
    [Column("TenantId")]
    [Display(Name = "TenantId")]
    public Guid TenantId { get; set; }

    /// <summary>FK para GTP__Fornecedor. SQL: FornecedorId INT NOT NULL</summary>
    [Required]
    [Column("FornecedorId")]
    [Display(Name = "Fornecedor")]
    public int FornecedorId { get; set; }

    /// <summary>
    /// Conta preferencial para pagamentos.
    /// UNIQUE INDEX no banco garante apenas 1 por fornecedor/tenant.
    /// SQL: Principal BIT NOT NULL DEFAULT 0
    /// </summary>
    [Column("Principal")]
    [Display(Name = "Principal")]
    public bool Principal { get; set; } = false;

    // ---- Banco -----------------------------------------------------------

    /// <summary>Código Febraban. Ex: '001'=BB, '341'=Itaú. SQL: BancoCodigo CHAR(3) NULL</summary>
    [Column("BancoCodigo")]
    [StringLength(3)]
    [Display(Name = "Código Banco")]
    public string? BancoCodigo { get; set; }

    /// <summary>SQL: BancoNome NVARCHAR(80) NULL</summary>
    [Column("BancoNome")]
    [StringLength(80)]
    [Display(Name = "Banco")]
    public string? BancoNome { get; set; }

    /// <summary>ISPB Bacen (8 dígitos sem pontos). SQL: Ispb CHAR(8) NULL</summary>
    [Column("Ispb")]
    [StringLength(8)]
    [Display(Name = "ISPB")]
    public string? Ispb { get; set; }

    // ---- Agência / Conta ------------------------------------------------

    /// <summary>SQL: Agencia VARCHAR(10) NULL</summary>
    [Column("Agencia")]
    [StringLength(10)]
    [Display(Name = "Agência")]
    public string? Agencia { get; set; }

    /// <summary>SQL: AgenciaDigito CHAR(1) NULL</summary>
    [Column("AgenciaDigito")]
    [StringLength(1)]
    [Display(Name = "Dígito Agência")]
    public string? AgenciaDigito { get; set; }

    /// <summary>SQL: Conta VARCHAR(20) NULL</summary>
    [Column("Conta")]
    [StringLength(20)]
    [Display(Name = "Conta")]
    public string? Conta { get; set; }

    /// <summary>SQL: ContaDigito CHAR(2) NULL</summary>
    [Column("ContaDigito")]
    [StringLength(2)]
    [Display(Name = "Dígito Conta")]
    public string? ContaDigito { get; set; }

    /// <summary>
    /// CC = Corrente | CP = Poupança | CI = Investimento.
    /// SQL: ContaTipo CHAR(2) NULL — CHECK IN ('CC','CP','CI')
    /// </summary>
    [Column("ContaTipo")]
    [StringLength(2)]
    [Display(Name = "Tipo Conta")]
    public string? ContaTipo { get; set; }

    // ---- Titular --------------------------------------------------------

    /// <summary>Nome do titular (pode diferir do fornecedor). SQL: TitularNome NVARCHAR(200) NULL</summary>
    [Column("TitularNome")]
    [StringLength(200)]
    [Display(Name = "Titular")]
    public string? TitularNome { get; set; }

    /// <summary>CNPJ(14) ou CPF(11) do titular. SQL: TitularDocumento CHAR(14) NULL</summary>
    [Column("TitularDocumento")]
    [StringLength(14)]
    [Display(Name = "Documento Titular")]
    public string? TitularDocumento { get; set; }

    // ---- PIX ------------------------------------------------------------

    /// <summary>SQL: PixChave NVARCHAR(150) NULL</summary>
    [Column("PixChave")]
    [StringLength(150)]
    [Display(Name = "Chave PIX")]
    public string? PixChave { get; set; }

    /// <summary>
    /// CPF | CNPJ | EMAIL | TELEFONE | ALEATORIA.
    /// SQL: PixTipo VARCHAR(20) NULL
    /// </summary>
    [Column("PixTipo")]
    [StringLength(20)]
    [Display(Name = "Tipo Chave PIX")]
    public string? PixTipo { get; set; }

    // ---- Internacional --------------------------------------------------

    /// <summary>ISO 13616. SQL: Iban VARCHAR(34) NULL</summary>
    [Column("Iban")]
    [StringLength(34)]
    [Display(Name = "IBAN")]
    public string? Iban { get; set; }

    /// <summary>ISO 9362. SQL: Swift VARCHAR(11) NULL</summary>
    [Column("Swift")]
    [StringLength(11)]
    [Display(Name = "SWIFT")]
    public string? Swift { get; set; }

    /// <summary>SQL: BancoCorrespondente NVARCHAR(100) NULL</summary>
    [Column("BancoCorrespondente")]
    [StringLength(100)]
    [Display(Name = "Banco Correspondente")]
    public string? BancoCorrespondente { get; set; }

    // ---- Controle -------------------------------------------------------

    /// <summary>SQL: Ativa BIT NOT NULL DEFAULT 1</summary>
    [Column("Ativa")]
    [Display(Name = "Ativa")]
    public bool Ativa { get; set; } = true;

    /// <summary>SQL: ValidadaEmUtc DATETIME2(3) NULL</summary>
    [Column("ValidadaEmUtc")]
    [Display(Name = "Validada em")]
    public DateTime? ValidadaEmUtc { get; set; }

    /// <summary>SQL: ValidadaPorUserId UNIQUEIDENTIFIER NULL</summary>
    [Column("ValidadaPorUserId")]
    [Display(Name = "Validada por")]
    public Guid? ValidadaPorUserId { get; set; }

    /// <summary>SQL: Observacao NVARCHAR(400) NULL</summary>
    [Column("Observacao")]
    [StringLength(400)]
    [Display(Name = "Observação")]
    public string? Observacao { get; set; }

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

    [ForeignKey(nameof(FornecedorId))]
    public virtual GTP_Fornecedor? Fornecedor { get; set; }
}