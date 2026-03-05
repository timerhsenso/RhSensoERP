// =================================================================================================
// RhSensoERP • TabelasCompartilhadas • Entity
// =================================================================================================
// Entidade:        TabelaAuxiliar
// Tabela:          dbo.taux2
// Natureza:        Legada
// Chave primária:  (CdTpTabela, CdSituacao)
// Finalidade:      Itens de cada tipo de tabela auxiliar
// Compatibilidade: SQL Server 2019+
// =================================================================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.TabelasCompartilhadas.Core.Entities;

/// <summary>
/// Entidade que representa itens de tabela auxiliar (tabela dbo.taux2).
/// PK composta: (CdTpTabela, CdSituacao)
/// </summary>
[GenerateCrud(
    TableName = "taux2",
    DisplayName = "Tabela Auxiliar",
    CdSistema = "SEG",
    CdFuncao = "SEG_FM_TAUX2",
    IsLegacyTable = true,
    GenerateApiController = true,
    SupportsBatchDelete = false
)]
[Table("taux2")]
[PrimaryKey(nameof(CdTpTabela), nameof(CdSituacao))]
public class TabelaAuxiliar
{
    // =========================================================================================
    // CHAVE PRIMÁRIA (PK composta)
    // =========================================================================================

    /// <summary>
    /// SQL: dbo.taux2.cdtptabela (varchar(2)) - parte da PK composta.
    /// </summary>
    [Key]
    [Required, StringLength(2)]
    [Column("cdtptabela", TypeName = "varchar(2)")]
    [Display(Name = "Tabela")]
    public string CdTpTabela { get; set; } = string.Empty;

    /// <summary>
    /// SQL: dbo.taux2.cdsituacao (varchar(2)) - parte da PK composta.
    /// </summary>
    [Key]
    [Required, StringLength(2)]
    [Column("cdsituacao", TypeName = "varchar(2)")]
    [Display(Name = "Código")]
    public string CdSituacao { get; set; } = string.Empty;

    // =========================================================================================
    // DADOS
    // =========================================================================================

    /// <summary>
    /// SQL: dbo.taux2.dcsituacao (varchar(60)) - descrição da situação.
    /// </summary>
    [Required, StringLength(60)]
    [Column("dcsituacao", TypeName = "varchar(60)")]
    [Display(Name = "Descrição")]
    public string DcSituacao { get; set; } = string.Empty;

    /// <summary>
    /// SQL: dbo.taux2.noordem (int) - ordem de exibição.
    /// </summary>
    [Column("noordem")]
    [Display(Name = "Ordem")]
    public int? NoOrdem { get; set; }

    /// <summary>
    /// SQL: dbo.taux2.flativoaux (char(1)) - flag ativo.
    /// </summary>
    [StringLength(1)]
    [Column("flativoaux", TypeName = "char(1)")]
    [Display(Name = "Ativo")]
    public string? FlAtivoAux { get; set; }

    // =========================================================================================
    // RELACIONAMENTOS
    // =========================================================================================

    /// <summary>
    /// FK → TipoTabelaAuxiliar (taux1).
    /// </summary>
    [ForeignKey(nameof(CdTpTabela))]
    [Display(Name = "Tipo Tabela Auxiliar")]
    public virtual TipoTabelaAuxiliar? TipoTabelaAuxiliar { get; set; }
}