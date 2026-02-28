// =================================================================================================
// RhSensoERP • Segurança • Entity
// =================================================================================================
// Entidade:        Tsistema
// Tabela:          dbo.tsistema
// Natureza:        Legada
// Chave primária:  (CdsiStema)
// Finalidade:      Representa um sistema cadastrado
// Compatibilidade: SQL Server 2019+
// =================================================================================================

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.Seguranca.Core.Entities;

/// <summary>
/// Entidade que representa um sistema cadastrado (tabela tsistema).
/// PK simples: CdsiStema (string)
/// </summary>
[GenerateCrud(
    TableName = "tsistema",
    DisplayName = "Tabela de Sistemas",
    CdSistema = "SEG",
    CdFuncao = "SEG_FM_TSISTEMA",
    IsLegacyTable = true,
    GenerateApiController = true,
    UsePluralRoute = false
   // ApiRoute = "seguranca/tsistema"
)]
[Table("tsistema")]
public class Tsistema
{
    // =============================================================================================
    // CHAVE PRIMÁRIA
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.tsistema.cdsistema (char(10)) - PK.
    /// </summary>
    [Key]
    [Required, StringLength(10)]
    [Column("cdsistema", TypeName = "char(10)")]
    public string CdsiStema { get; set; } = string.Empty;

    // =============================================================================================
    // DADOS
    // =============================================================================================

    /// <summary>
    /// SQL: dbo.tsistema.dcsistema (varchar(60)) - descrição do sistema.
    /// </summary>
    [Required, StringLength(60)]
    [Column("dcsistema", TypeName = "varchar(60)")]
    public string DcsiStema { get; set; } = string.Empty;

    // =============================================================================================
    // RELACIONAMENTOS
    // =============================================================================================

    /// <summary>
    /// Funções deste sistema.
    /// </summary>
    public virtual ICollection<Funcao> Funcoes { get; set; } = new List<Funcao>();
}