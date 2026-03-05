using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;



// =============================================================================
// GTP_FornecedorEndereco
// =============================================================================

/// <summary>
/// Endereços do fornecedor (1:N).
/// CodigoIBGE (7 dígitos) é OBRIGATÓRIO para emissão de NF-e e NFS-e no Brasil.
/// UNIQUE INDEX garante apenas 1 endereço com Principal = true por fornecedor/tenant.
/// Tabela: GTP__FornecedorEndereco
/// </summary>
[GenerateCrud(
    TableName = "GTP_FornecedorEndereco",
    DisplayName = "Endereço do Fornecedor",
    CdSistema = "GTP_",
    CdFuncao = "GTP_WEB_FORNECENDERECO",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("GTP_FornecedorEndereco")]
public class GTP_FornecedorEndereco
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
    /// Endereço padrão para NF-e/correspondência.
    /// UNIQUE INDEX no banco garante apenas 1 por fornecedor/tenant.
    /// SQL: Principal BIT NOT NULL DEFAULT 0
    /// </summary>
    [Column("Principal")]
    [Display(Name = "Principal")]
    public bool Principal { get; set; } = false;

    /// <summary>
    /// Sede | Cobranca | Entrega | Correspondencia | Outro.
    /// SQL: TipoEndereco VARCHAR(20) NULL
    /// </summary>
    [Column("TipoEndereco")]
    [StringLength(20)]
    [Display(Name = "Tipo Endereço")]
    public string? TipoEndereco { get; set; }

    /// <summary>CEP (somente dígitos, 8 chars). SQL: Cep CHAR(8) NULL</summary>
    [Column("Cep")]
    [StringLength(8)]
    [Display(Name = "CEP")]
    public string? Cep { get; set; }

    /// <summary>SQL: Logradouro NVARCHAR(200) NULL</summary>
    [Column("Logradouro")]
    [StringLength(200)]
    [Display(Name = "Logradouro")]
    public string? Logradouro { get; set; }

    /// <summary>SQL: Numero NVARCHAR(20) NULL</summary>
    [Column("Numero")]
    [StringLength(20)]
    [Display(Name = "Número")]
    public string? Numero { get; set; }

    /// <summary>SQL: Complemento NVARCHAR(100) NULL</summary>
    [Column("Complemento")]
    [StringLength(100)]
    [Display(Name = "Complemento")]
    public string? Complemento { get; set; }

    /// <summary>SQL: Bairro NVARCHAR(100) NULL</summary>
    [Column("Bairro")]
    [StringLength(100)]
    [Display(Name = "Bairro")]
    public string? Bairro { get; set; }

    /// <summary>SQL: Cidade NVARCHAR(100) NULL</summary>
    [Column("Cidade")]
    [StringLength(100)]
    [Display(Name = "Cidade")]
    public string? Cidade { get; set; }

    /// <summary>
    /// Código IBGE do município (7 dígitos). ESSENCIAL para NF-e e NFS-e.
    /// SQL: CodigoIBGE CHAR(7) NULL
    /// </summary>
    [Column("CodigoIBGE")]
    [StringLength(7)]
    [Display(Name = "Código IBGE")]
    public string? CodigoIBGE { get; set; }

    /// <summary>UF (2 letras maiúsculas). SQL: Uf CHAR(2) NULL</summary>
    [Column("Uf")]
    [StringLength(2)]
    [Display(Name = "UF")]
    public string? Uf { get; set; }

    /// <summary>SQL: Pais NVARCHAR(60) NULL DEFAULT 'Brasil'</summary>
    [Column("Pais")]
    [StringLength(60)]
    [Display(Name = "País")]
    public string? Pais { get; set; } = "Brasil";

    /// <summary>Código do país na tabela Bacen. Ex: '1058' = Brasil. SQL: CodigoPaisBacen CHAR(4) NULL</summary>
    [Column("CodigoPaisBacen")]
    [StringLength(4)]
    [Display(Name = "Código País Bacen")]
    public string? CodigoPaisBacen { get; set; }

    /// <summary>SQL: Latitude DECIMAL(9,6) NULL</summary>
    [Column("Latitude")]
    [Display(Name = "Latitude")]
    public decimal? Latitude { get; set; }

    /// <summary>SQL: Longitude DECIMAL(9,6) NULL</summary>
    [Column("Longitude")]
    [Display(Name = "Longitude")]
    public decimal? Longitude { get; set; }

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