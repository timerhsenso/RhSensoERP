using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoTerceirosPrestadores.Core.Entities;

// =============================================================================
// GtpParametros
// =============================================================================

/// <summary>
/// Parâmetros globais e por tenant do sistema RhSenso ERP.
/// IdSaas = 0 = template global. Leia sempre via VW_GTP_Parametros.
/// Tabela: GTP_Parametros
/// </summary>
[GenerateCrud(
    TableName = "GTP_Parametros",
    DisplayName = "Parâmetros do Sistema",
    CdSistema = "GTP",
    CdFuncao = "GTP_WEB_GTPPARAMETROS",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("GTP_Parametros")]
public class GTP_Parametros
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
    /// Grupo lógico do parâmetro. Ex: 'GTP__FORNECEDOR', 'RH_PONTO'.
    /// SQL: Grupo VARCHAR(60) NOT NULL
    /// </summary>
    [Required]
    [Column("Grupo")]
    [StringLength(60)]
    [Display(Name = "Grupo")]
    public string Grupo { get; set; } = null!;

    /// <summary>
    /// Chave única do parâmetro dentro do grupo. Ex: 'GTP__FOR_ABA_FISCAL'.
    /// SQL: Chave VARCHAR(100) NOT NULL
    /// </summary>
    [Required]
    [Column("Chave")]
    [StringLength(100)]
    [Display(Name = "Chave")]
    public string Chave { get; set; } = null!;

    /// <summary>
    /// Valor atual do parâmetro (sempre string; conversão na camada Application).
    /// SQL: Valor NVARCHAR(1000) NOT NULL
    /// </summary>
    [Required]
    [Column("Valor")]
    [StringLength(1000)]
    [Display(Name = "Valor")]
    public string Valor { get; set; } = null!;

    /// <summary>
    /// Valor padrão de fábrica (fallback se Valor for apagado/corrompido).
    /// SQL: ValorPadrao NVARCHAR(1000) NULL
    /// </summary>
    [Column("ValorPadrao")]
    [StringLength(1000)]
    [Display(Name = "Valor Padrão")]
    public string? ValorPadrao { get; set; }

    /// <summary>
    /// Descrição legível do parâmetro para exibição na tela de configuração.
    /// SQL: Descricao NVARCHAR(300) NOT NULL
    /// </summary>
    [Required]
    [Column("Descricao")]
    [StringLength(300)]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = null!;

    /// <summary>
    /// Tipo do valor: INT | BOOL | STRING | DECIMAL | JSON.
    /// SQL: Tipo VARCHAR(20) NOT NULL DEFAULT 'STRING'
    /// </summary>
    [Required]
    [Column("Tipo")]
    [StringLength(20)]
    [Display(Name = "Tipo")]
    public string Tipo { get; set; } = "STRING";

    /// <summary>
    /// Indica se o parâmetro pode ser editado pelo usuário na tela de configuração.
    /// 0 = somente sistema altera.
    /// SQL: Editavel BIT NOT NULL DEFAULT 1
    /// </summary>
    [Column("Editavel")]
    [Display(Name = "Editável")]
    public bool Editavel { get; set; } = true;

    /// <summary>
    /// Indica se o parâmetro é visível na tela de configuração.
    /// SQL: Visivel BIT NOT NULL DEFAULT 1
    /// </summary>
    [Column("Visivel")]
    [Display(Name = "Visível")]
    public bool Visivel { get; set; } = true;

    /// <summary>
    /// Ordem de exibição na tela de configuração.
    /// SQL: Ordem INT NOT NULL DEFAULT 0
    /// </summary>
    [Column("Ordem")]
    [Display(Name = "Ordem")]
    public int Ordem { get; set; } = 0;

    // ---- Auditoria -------------------------------------------------------

    /// <summary>
    /// Data/hora de criação em UTC.
    /// SQL: CreatedAtUtc DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()
    /// </summary>
    [Column("CreatedAtUtc")]
    [Display(Name = "Criado em")]
    public DateTime? CreatedAtUtc { get; set; }

    /// <summary>
    /// Data/hora da última atualização em UTC. Mantido pelo trigger TR_GTP_Parametros_UpdatedAt.
    /// SQL: UpdatedAtUtc DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()
    /// </summary>
    [Column("UpdatedAtUtc")]
    [Display(Name = "Atualizado em")]
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>
    /// Usuário que criou o registro.
    /// SQL: CreatedByUserId UNIQUEIDENTIFIER NULL
    /// </summary>
    [Column("CreatedByUserId")]
    [Display(Name = "Criado por")]
    public Guid? CreatedByUserId { get; set; }

    /// <summary>
    /// Usuário que atualizou o registro por último.
    /// SQL: UpdatedByUserId UNIQUEIDENTIFIER NULL
    /// </summary>
    [Column("UpdatedByUserId")]
    [Display(Name = "Atualizado por")]
    public Guid? UpdatedByUserId { get; set; }

    /// <summary>
    /// Controle de concorrência otimista.
    /// SQL: RowVer ROWVERSION NOT NULL
    /// </summary>
    [Timestamp]
    [Column("RowVer")]
    [Display(Name = "RowVer")]
    public byte[] RowVer { get; set; } = null!;
}
