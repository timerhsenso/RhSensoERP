using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;


/// <summary>
/// Contatos do fornecedor (1:N).
/// UNIQUE INDEX garante apenas 1 contato com Principal = true por fornecedor/tenant.
/// Tabela: GTP__FornecedorContato
/// </summary>
[GenerateCrud(
    TableName = "GTP_FornecedorContato",
    DisplayName = "Contato do Fornecedor",
    CdSistema = "GTP",
    CdFuncao = "GTP_WEB_FORNECCONTATO",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("GTP_FornecedorContato")]
public class GTP_FornecedorContato
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
    /// Contato preferencial. UNIQUE INDEX no banco garante apenas 1 por fornecedor/tenant.
    /// SQL: Principal BIT NOT NULL DEFAULT 0
    /// </summary>
    [Column("Principal")]
    [Display(Name = "Principal")]
    public bool Principal { get; set; } = false;

    /// <summary>SQL: ContatoNome NVARCHAR(150) NULL</summary>
    [Column("ContatoNome")]
    [StringLength(150)]
    [Display(Name = "Nome")]
    public string? ContatoNome { get; set; }

    /// <summary>SQL: Cargo NVARCHAR(100) NULL</summary>
    [Column("Cargo")]
    [StringLength(100)]
    [Display(Name = "Cargo")]
    public string? Cargo { get; set; }

    /// <summary>
    /// Financeiro | Comercial | Operacional | Fiscal | Juridico | Logistica | Outro.
    /// SQL: TipoContato VARCHAR(20) NULL
    /// </summary>
    [Column("TipoContato")]
    [StringLength(20)]
    [Display(Name = "Tipo Contato")]
    public string? TipoContato { get; set; }

    /// <summary>SQL: Email NVARCHAR(150) NULL</summary>
    [Column("Email")]
    [StringLength(150)]
    [Display(Name = "E-mail")]
    public string? Email { get; set; }

    /// <summary>Telefone fixo/comercial. SQL: Telefone VARCHAR(20) NULL</summary>
    [Column("Telefone")]
    [StringLength(20)]
    [Display(Name = "Telefone")]
    public string? Telefone { get; set; }

    /// <summary>SQL: Celular VARCHAR(20) NULL</summary>
    [Column("Celular")]
    [StringLength(20)]
    [Display(Name = "Celular")]
    public string? Celular { get; set; }

    /// <summary>SQL: WhatsApp VARCHAR(20) NULL</summary>
    [Column("WhatsApp")]
    [StringLength(20)]
    [Display(Name = "WhatsApp")]
    public string? WhatsApp { get; set; }

    /// <summary>SQL: Site NVARCHAR(200) NULL</summary>
    [Column("Site")]
    [StringLength(200)]
    [Display(Name = "Site")]
    public string? Site { get; set; }

    /// <summary>SQL: Ativo BIT NOT NULL DEFAULT 1</summary>
    [Column("Ativo")]
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

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