using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;

/// <summary>
/// Documentos de compliance e homologação do fornecedor (1:N).
/// TipoDocumentoId referencia o catálogo configurável SgcTipoDocumentoFornecedor.
/// StoragePath aponta para o arquivo no provider externo (Azure Blob / S3 / local).
/// Tabela: SGC_FornecedorDocumento
/// </summary>
[GenerateCrud(
    TableName = "GTP_FornecedorDocumento",
    DisplayName = "Documento do Fornecedor",
    CdSistema = "GTP",
    CdFuncao = "GTP_WEB_FORNECDOCUMENTO",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("GTP_FornecedorDocumento")]
public class GTP_FornecedorDocumento
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

    /// <summary>FK para SGC_Fornecedor. SQL: FornecedorId INT NOT NULL</summary>
    [Required]
    [Column("FornecedorId")]
    [Display(Name = "Fornecedor")]
    public int FornecedorId { get; set; }

    /// <summary>FK para SGC_TipoDocumentoFornecedor. SQL: TipoDocumentoId INT NOT NULL</summary>
    [Required]
    [Column("TipoDocumentoId")]
    [Display(Name = "Tipo de Documento")]
    public int TipoDocumentoId { get; set; }

    // ---- Identificação ---------------------------------------------------

    /// <summary>SQL: Descricao NVARCHAR(200) NULL</summary>
    [Column("Descricao")]
    [StringLength(200)]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    /// <summary>Número ou protocolo oficial do documento. SQL: Numero VARCHAR(80) NULL</summary>
    [Column("Numero")]
    [StringLength(80)]
    [Display(Name = "Número")]
    public string? Numero { get; set; }

    /// <summary>Órgão emissor. SQL: Orgao NVARCHAR(100) NULL</summary>
    [Column("Orgao")]
    [StringLength(100)]
    [Display(Name = "Órgão Emissor")]
    public string? Orgao { get; set; }

    // ---- Validade --------------------------------------------------------

    /// <summary>SQL: EmissaoData DATE NULL</summary>
    [Column("EmissaoData")]
    [Display(Name = "Data Emissão")]
    public DateOnly? EmissaoData { get; set; }

    /// <summary>SQL: VencimentoData DATE NULL</summary>
    [Column("VencimentoData")]
    [Display(Name = "Data Vencimento")]
    public DateOnly? VencimentoData { get; set; }

    // ---- Alerta ----------------------------------------------------------

    /// <summary>
    /// Indica se deve gerar alerta antes do vencimento.
    /// SQL: Alertar BIT NOT NULL DEFAULT 0
    /// </summary>
    [Column("Alertar")]
    [Display(Name = "Alertar")]
    public bool Alertar { get; set; } = false;

    /// <summary>
    /// Sobrescreve o padrão definido em SgcTipoDocumentoFornecedor.AlertarDiasAntes.
    /// SQL: AlertarDiasAntes INT NULL
    /// </summary>
    [Column("AlertarDiasAntes")]
    [Display(Name = "Alertar (dias antes)")]
    public int? AlertarDiasAntes { get; set; }

    // ---- Storage ---------------------------------------------------------

    /// <summary>LOCAL | AZUREBLOB | S3 | GDRIVE. SQL: StorageProvider VARCHAR(20) NULL</summary>
    [Column("StorageProvider")]
    [StringLength(20)]
    [Display(Name = "Storage Provider")]
    public string? StorageProvider { get; set; }

    /// <summary>Path relativo ou URL do blob. SQL: StoragePath NVARCHAR(500) NULL</summary>
    [Column("StoragePath")]
    [StringLength(500)]
    [Display(Name = "Storage Path")]
    public string? StoragePath { get; set; }

    /// <summary>Nome original do arquivo. SQL: StorageFileName NVARCHAR(260) NULL</summary>
    [Column("StorageFileName")]
    [StringLength(260)]
    [Display(Name = "Nome do Arquivo")]
    public string? StorageFileName { get; set; }

    /// <summary>Ex: 'application/pdf', 'image/jpeg'. SQL: StorageContentType VARCHAR(100) NULL</summary>
    [Column("StorageContentType")]
    [StringLength(100)]
    [Display(Name = "Tipo do Arquivo")]
    public string? StorageContentType { get; set; }

    /// <summary>SQL: StorageSizeBytes BIGINT NULL</summary>
    [Column("StorageSizeBytes")]
    [Display(Name = "Tamanho (bytes)")]
    public long? StorageSizeBytes { get; set; }

    // ---- Workflow de análise --------------------------------------------

    /// <summary>
    /// 1=Pendente | 2=Recebido | 3=Aprovado | 4=Rejeitado | 5=Vencido.
    /// SQL: StatusDocumento TINYINT NOT NULL DEFAULT 1
    /// </summary>
    [Required]
    [Column("StatusDocumento")]
    [Display(Name = "Status")]
    public byte StatusDocumento { get; set; } = 1;

    /// <summary>SQL: AnalisadoPorUserId UNIQUEIDENTIFIER NULL</summary>
    [Column("AnalisadoPorUserId")]
    [Display(Name = "Analisado por")]
    public Guid? AnalisadoPorUserId { get; set; }

    /// <summary>SQL: AnalisadoEmUtc DATETIME2(3) NULL</summary>
    [Column("AnalisadoEmUtc")]
    [Display(Name = "Analisado em")]
    public DateTime? AnalisadoEmUtc { get; set; }

    /// <summary>SQL: ObservacaoAnalise NVARCHAR(500) NULL</summary>
    [Column("ObservacaoAnalise")]
    [StringLength(500)]
    [Display(Name = "Observação Análise")]
    public string? ObservacaoAnalise { get; set; }

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

    [ForeignKey(nameof(TipoDocumentoId))]
    public virtual GTP_TipoDocumentoFornecedor? TipoDocumento { get; set; }
}
