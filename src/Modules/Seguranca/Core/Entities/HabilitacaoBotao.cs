// =================================================================================================
// RhSensoERP • Segurança • Entity
// =================================================================================================
// Entidade:        HabilitacaoBotao
// Tabela:          dbo.hbrh2
// Natureza:        Modernizada (Guid PK)
// Chave primária:  Id (uniqueidentifier, newsequentialid)
// Finalidade:      Habilitações de botões por grupo de usuário e função
// Compatibilidade: SQL Server 2019+
// =================================================================================================
// FK: (cdsistema, cdfuncao, nmbotao) → dbo.btfuncao(cdsistema, cdfuncao, nmbotao) (BotaoFuncao)
// FK: idgrupodeusuariofuncao          → dbo.hbrh1(id) ON DELETE CASCADE           (HabilitacaoGrupo)
// =================================================================================================

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.Seguranca.Core.Entities;

/// <summary>
/// Habilitação de botões por grupo de usuário e função (tabela hbrh2).
/// PK simples: Id (Guid, newsequentialid).
/// Vincula um botão específico a uma habilitação de grupo (hbrh1).
/// </summary>
[GenerateCrud(
    TableName = "hbrh2",
    DisplayName = "Habilitação de Botão",
    CdSistema = "SEG",
    CdFuncao = "SEG_FM_HABILITACAOBOTAO",
    IsLegacyTable = true,
    GenerateApiController = true
  //  ApiRoute = "seguranca/habilitacaobotao"
)]
[Table("hbrh2", Schema = "dbo")]
public class HabilitacaoBotao
{
    // =============================================================================================
    // CHAVE PRIMÁRIA
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.hbrh2.id (uniqueidentifier, default newsequentialid()) - PK.
    /// </summary>
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    // =============================================================================================
    // DADOS LEGADOS
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.hbrh2.cdgruser (varchar(30)) - código do grupo de usuário.
    /// </summary>
    [Required, StringLength(30)]
    [Column("cdgruser", TypeName = "varchar(30)")]
    public string CdGrUser { get; set; } = string.Empty;

    /// <summary>
    /// SQL: dbo.hbrh2.cdfuncao (varchar(30)) - código da função.
    /// Parte da FK composta → btfuncao(cdsistema, cdfuncao, nmbotao).
    /// </summary>
    [Required, StringLength(30)]
    [Column("cdfuncao", TypeName = "varchar(30)")]
    public string CdFuncao { get; set; } = string.Empty;

    /// <summary>
    /// SQL: dbo.hbrh2.cdsistema (char(10)) - código do sistema.
    /// Parte da FK composta → btfuncao(cdsistema, cdfuncao, nmbotao).
    /// </summary>
    [Required, StringLength(10)]
    [Column("cdsistema", TypeName = "char(10)")]
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// SQL: dbo.hbrh2.nmbotao (varchar(30)) - nome do botão.
    /// Parte da FK composta → btfuncao(cdsistema, cdfuncao, nmbotao).
    /// </summary>
    [Required, StringLength(30)]
    [Column("nmbotao", TypeName = "varchar(30)")]
    public string NmBotao { get; set; } = string.Empty;

    // =============================================================================================
    // FK (Guid)
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.hbrh2.idgrupodeusuariofuncao (uniqueidentifier) - FK → dbo.hbrh1(id) ON DELETE CASCADE.
    /// </summary>
    [Column("idgrupodeusuariofuncao")]
    public Guid? IdGrupoDeUsuarioFuncao { get; set; }

    // =============================================================================================
    // RELACIONAMENTOS
    // =============================================================================================

    /// <summary>
    /// Habilitação de grupo/função pai (FK Guid → hbrh1, CASCADE).
    /// </summary>
  //  [ForeignKey(nameof(IdGrupoDeUsuarioFuncao))]
  //  public virtual HabilitacaoGrupo? HabilitacaoGrupo { get; set; }

    /// <summary>
    /// Botão vinculado (FK composta: cdsistema + cdfuncao + nmbotao → btfuncao).
    /// Configurar no EfConfig manual com HasForeignKey(e => new { e.CdSistema, e.CdFuncao, e.NmBotao }).
    /// </summary>
    public virtual BotaoFuncao? BotaoFuncao { get; set; }
}