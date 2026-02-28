// =================================================================================================
// RhSensoERP • Segurança • Entity
// =================================================================================================
// Entidade:        BotaoFuncao
// Tabela:          dbo.btfuncao
// Natureza:        Legada
// Chave primária:  (CdSistema, CdFuncao, NmBotao)
// Finalidade:      Botões/ações associados a uma função
// Compatibilidade: SQL Server 2019+
// =================================================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.Seguranca.Core.Entities;

/// <summary>
/// Entidade que representa botões/ações de uma função (tabela dbo.btfuncao).
/// PK composta: (CdSistema, CdFuncao, NmBotao)
/// </summary>
[GenerateCrud(
    TableName = "btfuncao",
    DisplayName = "Botões de Função",
    CdSistema = "SEG",
    CdFuncao = "SEG_FM_BOTAOFUNCAO",
    IsLegacyTable = true,
    GenerateApiController = true,
   // ApiRoute = "seguranca/botaofuncao",
    SupportsBatchDelete = false
)]
[Table("btfuncao")]
[PrimaryKey(nameof(CdSistema), nameof(CdFuncao), nameof(NmBotao))]
public class BotaoFuncao
{
    // =============================================================================================
    // CHAVE PRIMÁRIA (PK composta)
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.btfuncao.cdsistema (char(10)) - parte da PK composta.
    /// </summary>
    [Key]
    [Required, StringLength(10)]
    [Column("cdsistema", TypeName = "char(10)")]
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// SQL: dbo.btfuncao.cdfuncao (varchar(30)) - parte da PK composta.
    /// </summary>
    [Key]
    [Required, StringLength(30)]
    [Column("cdfuncao", TypeName = "varchar(30)")]
    public string CdFuncao { get; set; } = string.Empty;

    /// <summary>
    /// SQL: dbo.btfuncao.nmbotao (varchar(30)) - parte da PK composta.
    /// </summary>
    [Key]
    [Required, StringLength(30)]
    [Column("nmbotao", TypeName = "varchar(30)")]
    public string NmBotao { get; set; } = string.Empty;

    // =============================================================================================
    // DADOS
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.btfuncao.dcbotao (varchar(60)) - descrição do botão.
    /// </summary>
    [Required, StringLength(60)]
    [Column("dcbotao", TypeName = "varchar(60)")]
    public string DcBotao { get; set; } = string.Empty;

    /// <summary>
    /// SQL: dbo.btfuncao.cdacao (char(1)) - I/A/E/C.
    /// </summary>
    [Column("cdacao", TypeName = "char(1)")]
    public char CdAcao { get; set; }

    // =============================================================================================
    // RELACIONAMENTOS
    // =============================================================================================

    /// <summary>
    /// Função à qual este botão pertence.
    /// </summary>
    public virtual Funcao Funcao { get; set; } = null!;
}