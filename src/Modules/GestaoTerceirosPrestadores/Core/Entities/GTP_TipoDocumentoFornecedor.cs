using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;

/// <summary>
/// Catálogo configurável de tipos de documento exigidos de fornecedores.
/// IdSaas = 0 = template global; copiar por tenant na ativação do módulo.
/// Tabela: GTP__TipoDocumentoFornecedor
/// </summary>
[GenerateCrud(
    TableName = "GTP_TipoDocumentoFornecedor",
    DisplayName = "Tipo de Documento de Fornecedor",
    CdSistema = "GTP",
    CdFuncao = "GTP_WEB_TIPODOCFORNECEDOR",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("GTP_TipoDocumentoFornecedor")]
public class GTP_TipoDocumentoFornecedor
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
    /// Identificador do tenant. 0 = template global.
    /// SQL: IdSaas INT NOT NULL
    /// </summary>
    [Required]
    [Column("TenantId")]
    [Display(Name = "TenantId")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Chave de negócio do tipo. Ex: 'CERTIDAO_NEG_FEDERAL'.
    /// SQL: Codigo VARCHAR(40) NOT NULL
    /// </summary>
    [Required]
    [Column("Codigo")]
    [StringLength(40)]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = null!;

    /// <summary>
    /// Descrição exibida ao usuário.
    /// SQL: Descricao NVARCHAR(150) NOT NULL
    /// </summary>
    [Required]
    [Column("Descricao")]
    [StringLength(150)]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = null!;

    /// <summary>
    /// Categoria do documento: Juridico | Fiscal | Trabalhista | Contratual | Outros.
    /// SQL: Categoria VARCHAR(30) NULL
    /// </summary>
    [Column("Categoria")]
    [StringLength(30)]
    [Display(Name = "Categoria")]
    public string? Categoria { get; set; }

    /// <summary>
    /// Indica se o documento é obrigatório para homologar o fornecedor.
    /// SQL: Obrigatorio BIT NOT NULL DEFAULT 0
    /// </summary>
    [Column("Obrigatorio")]
    [Display(Name = "Obrigatório")]
    public bool Obrigatorio { get; set; } = false;

    /// <summary>
    /// Indica se este tipo de documento tem controle de vencimento.
    /// SQL: ControlVencimento BIT NOT NULL DEFAULT 0
    /// </summary>
    [Column("ControlVencimento")]
    [Display(Name = "Controla Vencimento")]
    public bool ControlVencimento { get; set; } = false;

    /// <summary>
    /// Antecedência padrão (dias) para alerta de vencimento.
    /// Pode ser sobrescrito por documento em GTP__FornecedorDocumento.AlertarDiasAntes.
    /// SQL: AlertarDiasAntes INT NULL
    /// </summary>
    [Column("AlertarDiasAntes")]
    [Display(Name = "Alertar (dias antes)")]
    public int? AlertarDiasAntes { get; set; }

    /// <summary>
    /// Indica se o tipo está ativo e disponível para seleção.
    /// SQL: Ativo BIT NOT NULL DEFAULT 1
    /// </summary>
    [Column("Ativo")]
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    /// <summary>
    /// Ordem de exibição na listagem de documentos do fornecedor.
    /// SQL: Ordem INT NOT NULL DEFAULT 0
    /// </summary>
    [Column("Ordem")]
    [Display(Name = "Ordem")]
    public int Ordem { get; set; } = 0;

    // ---- Auditoria -------------------------------------------------------

    /// <summary>SQL: CreatedAtUtc DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()</summary>
    [Column("CreatedAtUtc")]
    [Display(Name = "Criado em")]
    public DateTime? CreatedAtUtc { get; set; }

    /// <summary>SQL: UpdatedAtUtc DATETIME2(3) NOT NULL — mantido pelo trigger.</summary>
    [Column("UpdatedAtUtc")]
    [Display(Name = "Atualizado em")]
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>SQL: RowVer ROWVERSION NOT NULL</summary>
    [Timestamp]
    [Column("RowVer")]
    [Display(Name = "RowVer")]
    public byte[] RowVer { get; set; } = null!;

    // ---- Navegação -------------------------------------------------------

    /// <summary>Documentos vinculados a este tipo.</summary>
    public virtual ICollection<GTP_FornecedorDocumento> Documentos { get; set; } = new List<GTP_FornecedorDocumento>();
}

