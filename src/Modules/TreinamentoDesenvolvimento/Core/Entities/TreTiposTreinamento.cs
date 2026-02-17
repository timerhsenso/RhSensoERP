// =============================================================================
// RHSENSOERP - ENTITY TRETIPOSTREINAMENTO
// =============================================================================
// Módulo: Treinamento e Desenvolvimento (TRE/SGT)
// Tabela: tre_tipos_treinamento
// Schema: dbo
// Multi-tenant: ✅ SIM (TenantId obrigatório)
// 
// ✅ VALIDAÇÃO AUTOMÁTICA DE UNICIDADE:
// - CodigoNr: Único por tenant (quando não for NULL)
// 
// O Source Generator irá criar automaticamente:
// 1. Índices únicos no banco de dados
// 2. Validação de duplicatas no pipeline MediatR (ANTES do SaveChanges)
// 3. Retorno HTTP 409 Conflict em caso de duplicata
// =============================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.TreinamentoDesenvolvimento.Core.Entities;

/// <summary>
/// TreTiposTreinamento - Cadastro de Tipos de Treinamento
/// Tabela: tre_tipos_treinamento (Multi-tenant)
/// Fonte da verdade: SQL Server
/// 
/// Representa um "tipo" de treinamento (ex.: NR-35, Integração, LOTO) que
/// poderá ser associado a pessoas/colaboradores/terceiros como treinamentos
/// realizados, com regras de validade, obrigatoriedade e aplicabilidade.
/// </summary>
[GenerateCrud(
    TableName = "tre_tipos_treinamento",
    DisplayName = "Tipos de Treinamento",
    CdSistema = "TRE",
    CdFuncao = "TRE_FM_TRETIPOSTREINAMENTO",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("tre_tipos_treinamento")]
[HasDatabaseTriggers("Auditoria automática de CreatedAt/UpdatedAt via triggers SQL Server")]
public class TreTiposTreinamento
{
    // =========================================================================
    // CHAVE PRIMÁRIA
    // =========================================================================

    /// <summary>
    /// Identificador interno (chave técnica).
    /// SQL: Id INT IDENTITY(1,1) PRIMARY KEY
    /// </summary>
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "ID")]
    public int Id { get; set; }

    // =========================================================================
    // MULTI-TENANCY
    // =========================================================================

    /// <summary>
    /// Identificador do tenant (SaaS) - isola dados entre clientes.
    /// SQL: TenantId UNIQUEIDENTIFIER NOT NULL
    /// </summary>
    [Required]
    [Column("TenantId", TypeName = "uniqueidentifier")]
    [Display(Name = "Tenant")]
    public Guid TenantId { get; set; }

    // =========================================================================
    // DADOS PRINCIPAIS
    // =========================================================================

    /// <summary>
    /// Nome do tipo de treinamento.
    /// Exemplos: "NR 35 - Trabalho em Altura", "LOTO", "Integração"
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

    // =========================================================================
    // ✅ CODIGO NR - VALIDAÇÃO AUTOMÁTICA DE UNICIDADE POR TENANT
    // =========================================================================
    // [Unique] faz com que o Source Generator crie:
    // 1. Índice único: CREATE UNIQUE INDEX UX_TreTiposTreinamento_Tenant_CodigoNr 
    //    ON tre_tipos_treinamento (TenantId, CodigoNr) WHERE CodigoNr IS NOT NULL
    // 2. Validação no UniqueValidationBehavior ANTES de SaveChanges
    // 3. Exception DuplicateEntityException → HTTP 409 Conflict
    // =========================================================================

    /// <summary>
    /// Código normativo/regulatório (ex.: NR-01, NR-10, NR-35).
    /// ⚠️ ÚNICO POR TENANT quando preenchido.
    /// SQL: CodigoNr NVARCHAR(20) NULL
    /// </summary>
    [Unique(UniqueScope.Tenant, DisplayName = "Código NR", ErrorMessage = "Código NR '{PropertyValue}' já está cadastrado neste tenant.")]
    [StringLength(20)]
    [Column("CodigoNr")]
    [Display(Name = "Código NR")]
    public string? CodigoNr { get; set; }

    // =========================================================================
    // REGRAS DE NEGÓCIO
    // =========================================================================

    /// <summary>
    /// Validade em dias do treinamento (ex.: 365, 730).
    /// NULL = sem prazo definido.
    /// SQL: DiasPrazoValidade INT NULL (CHECK >= 0)
    /// </summary>
    [Column("DiasPrazoValidade")]
    [Display(Name = "Validade (dias)")]
    [Range(0, int.MaxValue, ErrorMessage = "Validade deve ser maior ou igual a zero")]
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
    /// Texto livre com o público aplicável.
    /// Exemplos: "Todos", "Terceiros", "Operadores", "Eletricistas"
    /// SQL: AplicavelA NVARCHAR(100) NULL
    /// </summary>
    [StringLength(100)]
    [Column("AplicavelA")]
    [Display(Name = "Aplicável a")]
    public string? AplicavelA { get; set; }

    /// <summary>
    /// Carga horária em horas (ex.: 4, 8, 16, 40).
    /// NULL = não informado.
    /// SQL: CargaHoraria INT NULL (CHECK >= 0)
    /// </summary>
    [Column("CargaHoraria")]
    [Display(Name = "Carga horária (h)")]
    [Range(0, int.MaxValue, ErrorMessage = "Carga horária deve ser maior ou igual a zero")]
    public int? CargaHoraria { get; set; }

    /// <summary>
    /// Indica se o tipo está ativo para uso no sistema (soft delete).
    /// SQL: Ativo BIT NOT NULL DEFAULT 1
    /// </summary>
    [Required]
    [Column("Ativo")]
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    // =========================================================================
    // AUDITORIA (defaults controlados pelo banco: SYSUTCDATETIME())
    // =========================================================================

    /// <summary>
    /// Data/hora (UTC) de criação do registro (gerada automaticamente pelo banco).
    /// SQL: CreatedAtUtc DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()
    /// ⚠️ PREENCHIDO POR TRIGGER - NÃO ENVIAR NO INSERT
    /// </summary>
    [Required]
    [Column("CreatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Criado em (UTC)")]
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Usuário (GUID) que criou o registro (FK para dbo.tuse1.Id).
    /// SQL: CreatedByUserId UNIQUEIDENTIFIER NULL
    /// ⚠️ PREENCHIDO POR TRIGGER - NÃO ENVIAR NO INSERT
    /// </summary>
    [Column("CreatedByUserId", TypeName = "uniqueidentifier")]
    [Display(Name = "Criado por (Usuário)")]
    public Guid? CreatedByUserId { get; set; }

    /// <summary>
    /// Data/hora (UTC) da última atualização (gerada no INSERT e atualizada por trigger).
    /// SQL: UpdatedAtUtc DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()
    /// ⚠️ PREENCHIDO POR TRIGGER - NÃO ENVIAR NO UPDATE
    /// </summary>
    [Required]
    [Column("UpdatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Atualizado em (UTC)")]
    public DateTime UpdatedAtUtc { get; set; }

    /// <summary>
    /// Usuário (GUID) que atualizou por último (FK para dbo.tuse1.Id).
    /// SQL: UpdatedByUserId UNIQUEIDENTIFIER NULL
    /// ⚠️ PREENCHIDO POR TRIGGER - NÃO ENVIAR NO UPDATE
    /// </summary>
    [Column("UpdatedByUserId", TypeName = "uniqueidentifier")]
    [Display(Name = "Atualizado por (Usuário)")]
    public Guid? UpdatedByUserId { get; set; }

    // =========================================================================
    // NAVIGATION PROPERTIES
    // =========================================================================
    // Comentado: Aguardando implementação completa da entidade Usuario
    // [ForeignKey(nameof(CreatedByUserId))]
    // public virtual Usuario? CreatedByUser { get; set; }

    // [ForeignKey(nameof(UpdatedByUserId))]
    // public virtual Usuario? UpdatedByUser { get; set; }

    // =========================================================================
    // INVERSE NAVIGATION
    // =========================================================================
    // Quando houver relacionamentos com outras entidades (ex.: TreTreinamentos),
    // adicione as navigation properties aqui:

    // Exemplo:
    // [InverseProperty(nameof(TreTreinamento.TipoTreinamento))]
    // public virtual ICollection<TreTreinamento> Treinamentos { get; set; } = new List<TreTreinamento>();
}