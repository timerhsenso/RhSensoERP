// =================================================================================================
// RhSensoERP • Segurança • Entity
// =================================================================================================
// Entidade:        HabilitacaoGrupo
// Tabela:          dbo.hbrh1
// Natureza:        Modernizada (Guid PK + TenantId)
// Chave primária:  Id (uniqueidentifier, newsequentialid)
// Finalidade:      Habilitações/permissões de ações por grupo e função
// Compatibilidade: SQL Server 2019+
// =================================================================================================
// FK: (cdsistema, cdfuncao) → dbo.fucn1(cdsistema, cdfuncao)  (Funcao - FK composta)
// FK: idgrupodeusuario      → dbo.gurh1(id)                   (GrupoDeUsuario)
// =================================================================================================

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.Seguranca.Core.Entities;

/// <summary>
/// Habilitações de ações por grupo de usuário e função (tabela hbrh1).
/// PK simples: Id (Guid, newsequentialid).
/// Contém CdAcoes (ações habilitadas) e CdRestric (tipo de restrição).
/// </summary>
[GenerateCrud(
    TableName = "hbrh1",
    DisplayName = "Habilitação de Grupo",
    CdSistema = "SEG",
    CdFuncao = "SEG_FM_HABILITACAOGRUPO",
    IsLegacyTable = false,
    GenerateApiController = true
   // ApiRoute = "seguranca/habilitacaogrupo"
)]
[Table("hbrh1", Schema = "dbo")]
public class HabilitacaoGrupo
{
    // =============================================================================================
    // CHAVE PRIMÁRIA
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.hbrh1.id (uniqueidentifier, default newsequentialid()) - PK.
    /// </summary>
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    // =============================================================================================
    // MULTI-TENANT
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.hbrh1.TenantId (uniqueidentifier, nullable).
    /// </summary>
    [Column("TenantId")]
    public Guid? TenantId { get; set; }

    // =============================================================================================
    // DADOS LEGADOS
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.hbrh1.cdgruser (varchar(30)) - código do grupo de usuário.
    /// </summary>
    [Required, StringLength(30)]
    [Column("cdgruser", TypeName = "varchar(30)")]
    public string CdGrUser { get; set; } = string.Empty;

    /// <summary>
    /// SQL: dbo.hbrh1.cdfuncao (varchar(30)) - código da função.
    /// Parte da FK composta → fucn1(cdsistema, cdfuncao).
    /// </summary>
    [Required, StringLength(30)]
    [Column("cdfuncao", TypeName = "varchar(30)")]
    public string CdFuncao { get; set; } = string.Empty;

    /// <summary>
    /// SQL: dbo.hbrh1.cdsistema (char(10)) - código do sistema.
    /// Parte da FK composta → fucn1(cdsistema, cdfuncao).
    /// </summary>
    [StringLength(10)]
    [Column("cdsistema", TypeName = "char(10)")]
    public string? CdSistema { get; set; }

    /// <summary>
    /// SQL: dbo.hbrh1.cdacoes (char(20)) - ações habilitadas (ex: IAEC).
    /// Usado pelo PermissaoRepository para verificar permissões.
    /// </summary>
    [Required, StringLength(20)]
    [Column("cdacoes", TypeName = "char(20)")]
    public string CdAcoes { get; set; } = string.Empty;

    /// <summary>
    /// SQL: dbo.hbrh1.cdrestric (char(1)) - tipo de restrição (S/N).
    /// </summary>
    [Required]
    [Column("cdrestric", TypeName = "char(1)")]
    public char CdRestric { get; set; }

    // =============================================================================================
    // FK's (Guid) - MODERNIZADAS
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.hbrh1.idgrupodeusuario (uniqueidentifier) - FK → dbo.gurh1(id).
    /// </summary>
    [Column("idgrupodeusuario")]
    public Guid? IdGrupoDeUsuario { get; set; }

    // =============================================================================================
    // RELACIONAMENTOS
    // =============================================================================================

    /// <summary>
    /// Grupo de usuário (FK Guid → gurh1).
    /// </summary>
  //  [ForeignKey(nameof(IdGrupoDeUsuario))]
  //  public virtual GrupoDeUsuario? GrupoDeUsuario { get; set; }

    /// <summary>
    /// Função vinculada (FK composta: cdsistema + cdfuncao → fucn1).
    /// Configurar no EfConfig manual com HasForeignKey(e => new { e.CdSistema, e.CdFuncao }).
    /// </summary>
    public virtual Funcao? Funcao { get; set; }
}