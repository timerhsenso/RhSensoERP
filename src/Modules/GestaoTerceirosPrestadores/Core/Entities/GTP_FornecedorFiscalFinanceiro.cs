using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;



/// <summary>
/// Dados fiscais e financeiros do fornecedor (1:1).
/// Retenções IR/PIS/COFINS/CSLL/ISS/INSS, regime, condição de pagamento.
/// Tabela: GTP__FornecedorFiscalFinanceiro
/// </summary>
[GenerateCrud(
    TableName = "GTP_FornecedorFiscalFinanceiro",
    DisplayName = "Dados Fiscais/Financeiros do Fornecedor",
    CdSistema = "GTP",
    CdFuncao = "GTP_WEB_FORNECFISCALFINANCEIRO",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("GTP_FornecedorFiscalFinanceiro")]
public class GTP_FornecedorFiscalFinanceiro
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

    // ---- Fiscal ----------------------------------------------------------

    /// <summary>SQL: InscricaoEstadual VARCHAR(20) NULL</summary>
    [Column("InscricaoEstadual")]
    [StringLength(20)]
    [Display(Name = "Inscrição Estadual")]
    public string? InscricaoEstadual { get; set; }

    /// <summary>SQL: InscricaoMunicipal VARCHAR(20) NULL</summary>
    [Column("InscricaoMunicipal")]
    [StringLength(20)]
    [Display(Name = "Inscrição Municipal")]
    public string? InscricaoMunicipal { get; set; }

    /// <summary>Código SUFRAMA (Zona Franca / ALC). SQL: SuframaCode VARCHAR(20) NULL</summary>
    [Column("SuframaCode")]
    [StringLength(20)]
    [Display(Name = "SUFRAMA")]
    public string? SuframaCode { get; set; }

    /// <summary>Código NBS para serviços internacionais. SQL: CodigoNbs VARCHAR(10) NULL</summary>
    [Column("CodigoNbs")]
    [StringLength(10)]
    [Display(Name = "Código NBS")]
    public string? CodigoNbs { get; set; }

    /// <summary>SQL: NrCertNegDebitos VARCHAR(60) NULL</summary>
    [Column("NrCertNegDebitos")]
    [StringLength(60)]
    [Display(Name = "Nº Certidão Negativa")]
    public string? NrCertNegDebitos { get; set; }

    /// <summary>SQL: VencCertNegDebitos DATE NULL</summary>
    [Column("VencCertNegDebitos")]
    [Display(Name = "Venc. Certidão Negativa")]
    public DateOnly? VencCertNegDebitos { get; set; }

    // ---- Classificação financeira ----------------------------------------

    /// <summary>
    /// 1 = Produto | 2 = Serviço | 3 = Ambos.
    /// SQL: TipoFornecedor TINYINT NULL — CHECK IN (1,2,3)
    /// </summary>
    [Column("TipoFornecedor")]
    [Display(Name = "Tipo Fornecedor")]
    public byte? TipoFornecedor { get; set; }

    /// <summary>Moeda padrão (ISO 4217). SQL: MoedaPadrao CHAR(3) NOT NULL DEFAULT 'BRL'</summary>
    [Required]
    [Column("MoedaPadrao")]
    [StringLength(3)]
    [Display(Name = "Moeda Padrão")]
    public string MoedaPadrao { get; set; } = "BRL";

    // ---- Condições de pagamento ------------------------------------------

    /// <summary>Ex: '30/60/90', 'À Vista'. SQL: CondicaoPagamento NVARCHAR(80) NULL</summary>
    [Column("CondicaoPagamento")]
    [StringLength(80)]
    [Display(Name = "Condição de Pagamento")]
    public string? CondicaoPagamento { get; set; }

    /// <summary>SQL: PrazoPagamentoDias INT NULL</summary>
    [Column("PrazoPagamentoDias")]
    [Display(Name = "Prazo Pagamento (dias)")]
    public int? PrazoPagamentoDias { get; set; }

    /// <summary>SQL: LimiteCredito DECIMAL(18,2) NULL</summary>
    [Column("LimiteCredito")]
    [Display(Name = "Limite de Crédito")]
    public decimal? LimiteCredito { get; set; }

    /// <summary>
    /// BOLETO | PIX | TED | DOC | CHEQUE.
    /// SQL: FormaPagamentoPadrao VARCHAR(20) NULL
    /// </summary>
    [Column("FormaPagamentoPadrao")]
    [StringLength(20)]
    [Display(Name = "Forma de Pagamento Padrão")]
    public string? FormaPagamentoPadrao { get; set; }

    // ---- Retenções -------------------------------------------------------

    /// <summary>SQL: RetIR BIT NOT NULL DEFAULT 0</summary>
    [Column("RetIR")]
    [Display(Name = "Retenção IR")]
    public bool RetIR { get; set; } = false;

    /// <summary>SQL: RetPIS BIT NOT NULL DEFAULT 0</summary>
    [Column("RetPIS")]
    [Display(Name = "Retenção PIS")]
    public bool RetPIS { get; set; } = false;

    /// <summary>SQL: RetCOFINS BIT NOT NULL DEFAULT 0</summary>
    [Column("RetCOFINS")]
    [Display(Name = "Retenção COFINS")]
    public bool RetCOFINS { get; set; } = false;

    /// <summary>SQL: RetCSLL BIT NOT NULL DEFAULT 0</summary>
    [Column("RetCSLL")]
    [Display(Name = "Retenção CSLL")]
    public bool RetCSLL { get; set; } = false;

    /// <summary>SQL: RetISS BIT NOT NULL DEFAULT 0</summary>
    [Column("RetISS")]
    [Display(Name = "Retenção ISS")]
    public bool RetISS { get; set; } = false;

    /// <summary>SQL: RetINSS BIT NOT NULL DEFAULT 0</summary>
    [Column("RetINSS")]
    [Display(Name = "Retenção INSS")]
    public bool RetINSS { get; set; } = false;

    /// <summary>SQL: AliquotaIR DECIMAL(5,2) NULL</summary>
    [Column("AliquotaIR")]
    [Display(Name = "Alíquota IR (%)")]
    public decimal? AliquotaIR { get; set; }

    /// <summary>SQL: AliquotaISS DECIMAL(5,2) NULL</summary>
    [Column("AliquotaISS")]
    [Display(Name = "Alíquota ISS (%)")]
    public decimal? AliquotaISS { get; set; }

    /// <summary>SQL: AliquotaINSS DECIMAL(5,2) NULL</summary>
    [Column("AliquotaINSS")]
    [Display(Name = "Alíquota INSS (%)")]
    public decimal? AliquotaINSS { get; set; }

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

    /// <summary>Fornecedor pai.</summary>
    [ForeignKey(nameof(FornecedorId))]
    public virtual GTP_Fornecedor? Fornecedor { get; set; }
}