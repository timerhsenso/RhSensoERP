// =================================================================================================
// RhSensoERP • Identity • Entity
// =================================================================================================
// Entidade:        Funcao
// Tabela:          dbo.fucn1
// Natureza:        Legada
// Chave primária:  (CdFuncao, CdSistema)
// Finalidade:      Representa uma função/tela do sistema
// Observações:     Estrutura baseada no banco bd_rhu_copenor
// Compatibilidade: SQL Server 2019+
// =================================================================================================

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.Seguranca.Core.Entities;

/// <summary>
/// Entidade que representa uma função/tela do sistema (tabela fucn1).
/// </summary>
[GenerateCrud(
    TableName = "fucn1",
    DisplayName = "Funções do Sistema",
    CdSistema = "SEG",
    CdFuncao = "SEG_FM_FUNCAO",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("fucn1", Schema = "dbo")]
[PrimaryKey(nameof(CdSistema), nameof(CdFuncao))]  // ✅ mesma ordem do banco
public class Funcao
{
    // =============================================================================================
    // CHAVE PRIMÁRIA (PK composta)
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.fucn1.cdfuncao (varchar(30)) - parte da PK composta.
    /// </summary>
    [Required, StringLength(30)]
    [Column("cdfuncao", TypeName = "varchar(30)")]
    public string CdFuncao { get; set; } = string.Empty;

    /// <summary>
    /// SQL: dbo.fucn1.cdsistema (char(10)) - parte da PK composta.
    /// </summary>
    [Required, StringLength(10)]
    [Column("cdsistema", TypeName = "char(10)")]
    public string CdSistema { get; set; } = string.Empty;

    // =============================================================================================
    // DADOS
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.fucn1.dcfuncao (varchar(80)) - descrição da função.
    /// </summary>
    [StringLength(80)]
    [Column("dcfuncao", TypeName = "varchar(80)")]
    public string? DcFuncao { get; set; }

    /// <summary>
    /// SQL: dbo.fucn1.dcmodulo (varchar(100)) - descrição do módulo.
    /// </summary>
    [StringLength(100)]
    [Column("dcmodulo", TypeName = "varchar(100)")]
    public string? DcModulo { get; set; }

    /// <summary>
    /// SQL: dbo.fucn1.descricaomodulo (varchar(100)) - campo adicional de módulo.
    /// </summary>
    [StringLength(100)]
    [Column("descricaomodulo", TypeName = "varchar(100)")]
    public string? DescricaoModulo { get; set; }

    // =============================================================================================
    // RELACIONAMENTOS
    // =============================================================================================

   // public virtual Tsistema Sistema { get; set; } = null!;
   // public virtual ICollection<BotaoFuncao> Botoes { get; set; } = new List<BotaoFuncao>();
   // public virtual ICollection<GrupoFuncao> GrupoFuncoes { get; set; } = new List<GrupoFuncao>();
}