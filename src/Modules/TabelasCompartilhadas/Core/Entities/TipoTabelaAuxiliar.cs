using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.TabelasCompartilhadas.Core.Entities;

/// <summary>
/// Tipo de Tabela Auxiliar (taux1) — tabela de lookup para outras auxiliares.
/// </summary>
[Table("taux1")]
[GenerateCrud(
    TableName = "taux1",
    DisplayName = "Tipo de Tabela Auxiliar",
    CdSistema = "SEG",
    CdFuncao = "SEG_FM_TAUX1",
    IsLegacyTable = true,
    GenerateLookup = true,                    // ← Ativa endpoint /lookup + Select2
    SupportsBatchDelete = true,
    GenerateEfConfig = true
)]
public class TipoTabelaAuxiliar
{
    [Key]
    [Column("cdtptabela", TypeName = "varchar(2)")]
    [StringLength(2)]
    [Display(Name = "Código Tipo Tabela")]
    [LookupKey]                               // ← Value do Select2
    public string CdTpTabela { get; set; } = string.Empty;

    [Column("dctabela", TypeName = "varchar(60)")]
    [StringLength(60)]
    [Required]
    [Display(Name = "Descrição Tabela")]
    [LookupText]                              // ← Text do Select2
    public string DcTabela { get; set; } = string.Empty;

    /// <summary>
    /// Inverse navigation (One-to-Many) para TabelaAuxiliar.
    /// </summary>
    [InverseProperty(nameof(TabelaAuxiliar.TipoTabelaAuxiliar))]
    public virtual ICollection<TabelaAuxiliar> TabelasAuxiliares { get; set; }
        = new List<TabelaAuxiliar>();
}