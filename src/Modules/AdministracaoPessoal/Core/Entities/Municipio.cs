using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.AdministracaoPessoal.Core.Entities;

/// <summary>
/// Município
/// Tabela: muni1
/// </summary>
[GenerateCrud(
    TableName = "muni1",
    DisplayName = "Município",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_MUNICIPIO",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("muni1")]
public class Municipio
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdmunicip")]
    [StringLength(5)]
    [Display(Name = "Código")]
    public string Cdmunicip { get; set; } = string.Empty;

    [Column("sgestado")]
    [StringLength(2)]
    [Display(Name = "UF")]
    public string Sgestado { get; set; } = string.Empty;

    [Column("nmmunicip")]
    [StringLength(60)]
    [Display(Name = "Nome")]
    public string Nmmunicip { get; set; } = string.Empty;

    [Column("cod_ibge")]
    [Display(Name = "Código IBGE")]
    public int? CodIbge { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Collections (lado inverso dos relacionamentos)
    // Colaborador tem DUAS FKs para Município (naturalidade e endereço)
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Colaboradores nascidos neste município
    /// </summary>
    [InverseProperty(nameof(Colaborador.MunicipioNaturalidade))]
    public virtual ICollection<Colaborador> ColaboradoresNaturais { get; set; } = new List<Colaborador>();

    /// <summary>
    /// Colaboradores que residem neste município
    /// </summary>
    [InverseProperty(nameof(Colaborador.MunicipioEndereco))]
    public virtual ICollection<Colaborador> ColaboradoresResidentes { get; set; } = new List<Colaborador>();

    /// <summary>
    /// Filiais localizadas neste município
    /// </summary>
  //  [InverseProperty(nameof(Filial.Municipio))]
  //  public virtual ICollection<Filial> Filiais { get; set; } = new List<Filial>();
}
