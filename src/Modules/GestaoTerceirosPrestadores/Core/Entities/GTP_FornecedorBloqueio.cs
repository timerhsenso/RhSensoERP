using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;


// =============================================================================
// SgcFornecedorBloqueio
// =============================================================================

/// <summary>
/// Histórico completo de bloqueios e desbloqueios do fornecedor (1:N).
/// A flag SgcFornecedor.Bloqueado é atualizada pela camada Application (FornecedorService).
/// UNIQUE INDEX filtrado no banco garante no máximo 1 bloqueio aberto por fornecedor/tenant.
/// Tabela: SGC_FornecedorBloqueio
/// </summary>
[GenerateCrud(
    TableName = "GTP_FornecedorBloqueio",
    DisplayName = "Bloqueio de Fornecedor",
    CdSistema = "GTP",
    CdFuncao = "GTP_WEB_FORNECBLOQUEIO",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("GTP_FornecedorBloqueio")]
public class GTP_FornecedorBloqueio
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

    // ---- Bloqueio --------------------------------------------------------

    /// <summary>SQL: BloqueadoEmUtc DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()</summary>
    [Column("BloqueadoEmUtc")]
    [Display(Name = "Bloqueado em")]
    public DateTime? BloqueadoEmUtc { get; set; }

    /// <summary>SQL: BloqueadoPorUserId UNIQUEIDENTIFIER NULL</summary>
    [Column("BloqueadoPorUserId")]
    [Display(Name = "Bloqueado por")]
    public Guid? BloqueadoPorUserId { get; set; }

    /// <summary>
    /// FINANCEIRO | JURIDICO | QUALIDADE | OUTROS.
    /// SQL: TipoBloqueio VARCHAR(20) NULL
    /// </summary>
    [Column("TipoBloqueio")]
    [StringLength(20)]
    [Display(Name = "Tipo Bloqueio")]
    public string? TipoBloqueio { get; set; }

    /// <summary>SQL: Motivo NVARCHAR(400) NOT NULL</summary>
    [Required]
    [Column("Motivo")]
    [StringLength(400)]
    [Display(Name = "Motivo")]
    public string Motivo { get; set; } = null!;

    /// <summary>SQL: Observacao NVARCHAR(1000) NULL</summary>
    [Column("Observacao")]
    [StringLength(1000)]
    [Display(Name = "Observação")]
    public string? Observacao { get; set; }

    // ---- Desbloqueio (NULL = bloqueio ainda ativo) -----------------------

    /// <summary>SQL: DesbloqueadoEmUtc DATETIME2(3) NULL</summary>
    [Column("DesbloqueadoEmUtc")]
    [Display(Name = "Desbloqueado em")]
    public DateTime? DesbloqueadoEmUtc { get; set; }

    /// <summary>SQL: DesbloqueadoPorUserId UNIQUEIDENTIFIER NULL</summary>
    [Column("DesbloqueadoPorUserId")]
    [Display(Name = "Desbloqueado por")]
    public Guid? DesbloqueadoPorUserId { get; set; }

    /// <summary>SQL: MotivoDesbloqueio NVARCHAR(400) NULL</summary>
    [Column("MotivoDesbloqueio")]
    [StringLength(400)]
    [Display(Name = "Motivo Desbloqueio")]
    public string? MotivoDesbloqueio { get; set; }

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

    [ForeignKey(nameof(FornecedorId))]
    public virtual GTP_Fornecedor? Fornecedor { get; set; }
}
