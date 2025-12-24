using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.TreinamentoDesenvolvimento.Core.Entities;

/// <summary>
/// Tipos de Treinamento do Sistema de Gestão de Treinamentos
/// Tabela: dbo.tre_tipos_treinamento
/// </summary>
[GenerateCrud(
    TableName = "tre_tipos_treinamento",
    DisplayName = "Tipos de Treinamento",
    CdSistema = "SGT",
    CdFuncao = "SGT_FM_TRETIPOSTREINAMENTO",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("tre_tipos_treinamento")]
public class TreTiposTreinamento
{
    /// <summary>
    /// Identificador interno (chave técnica).
    /// SQL: Id INT IDENTITY(1,1) PRIMARY KEY
    /// </summary>
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "ID")]
    public int Id { get; set; }

    /// <summary>
    /// Identificador do tenant (SaaS) - isola dados entre clientes.
    /// SQL: TenantId UNIQUEIDENTIFIER NOT NULL
    /// </summary>
    [Required]
    [Column("TenantId", TypeName = "uniqueidentifier")]
    [Display(Name = "Tenant")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// Nome do tipo de treinamento.
    /// SQL: Nome NVARCHAR(100) NOT NULL
    /// </summary>
    [Required]
    [StringLength(100)]
    [Column("Nome")]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada / observações do tipo.
    /// SQL: Descricao NVARCHAR(500) NULL
    /// </summary>
    [StringLength(500)]
    [Column("Descricao")]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    /// <summary>
    /// Código normativo/regulatório (ex.: NR-01, NR-10, NR-35). Pode ser NULL.
    /// SQL: CodigoNr NVARCHAR(20) NULL
    /// </summary>
    [StringLength(20)]
    [Column("CodigoNr")]
    [Display(Name = "Código NR")]
    public string? CodigoNr { get; set; }

    /// <summary>
    /// Validade em dias do treinamento (ex.: 365, 730). NULL = sem prazo definido.
    /// SQL: DiasPrazoValidade INT NULL (CHECK >= 0)
    /// </summary>
    [Column("DiasPrazoValidade")]
    [Display(Name = "Validade (dias)")]
    public int? DiasPrazoValidade { get; set; }

    /// <summary>
    /// Indica se este treinamento é obrigatório.
    /// SQL: Obrigatorio BIT NOT NULL DEFAULT 0
    /// </summary>
    [Required]
    [Column("Obrigatorio")]
    [Display(Name = "Obrigatório")]
    public bool Obrigatorio { get; set; }

    /// <summary>
    /// Texto livre com o público aplicável (ex.: "Todos", "Terceiros", "Operadores").
    /// SQL: AplicavelA NVARCHAR(100) NULL
    /// </summary>
    [StringLength(100)]
    [Column("AplicavelA")]
    [Display(Name = "Aplicável a")]
    public string? AplicavelA { get; set; }

    /// <summary>
    /// Carga horária em horas (ex.: 4, 8, 16, 40). NULL = não informado.
    /// SQL: CargaHoraria INT NULL (CHECK >= 0)
    /// </summary>
    [Column("CargaHoraria")]
    [Display(Name = "Carga horária (h)")]
    public int? CargaHoraria { get; set; }

    /// <summary>
    /// Indica se o tipo está ativo para uso no sistema.
    /// SQL: Ativo BIT NOT NULL DEFAULT 1
    /// </summary>
    [Required]
    [Column("Ativo")]
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    /// <summary>
    /// Data/hora (UTC) de criação do registro (gerada automaticamente pelo banco).
    /// SQL: CreatedAtUtc DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()
    /// </summary>
    [Required]
    [Column("CreatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Criado em (UTC)")]
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Usuário (GUID) que criou o registro (FK para dbo.tuse1.Id). Pode ser NULL.
    /// SQL: CreatedByUserId UNIQUEIDENTIFIER NULL
    /// </summary>
    [Column("CreatedByUserId", TypeName = "uniqueidentifier")]
    [Display(Name = "Criado por (Usuário)")]
    public Guid? CreatedByUserId { get; set; }

    /// <summary>
    /// Data/hora (UTC) da última atualização (gerada no INSERT e atualizada por trigger).
    /// SQL: UpdatedAtUtc DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()
    /// </summary>
    [Required]
    [Column("UpdatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Atualizado em (UTC)")]
    public DateTime UpdatedAtUtc { get; set; }

    /// <summary>
    /// Usuário (GUID) que atualizou por último (FK para dbo.tuse1.Id). Pode ser NULL.
    /// SQL: UpdatedByUserId UNIQUEIDENTIFIER NULL
    /// </summary>
    [Column("UpdatedByUserId", TypeName = "uniqueidentifier")]
    [Display(Name = "Atualizado por (Usuário)")]
    public Guid? UpdatedByUserId { get; set; }
}
