using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.TabelasCompartilhadas.Core.Entities;

/// <summary>
/// Empresa/Empregador
/// Tabela: temp1
/// </summary>
[GenerateCrud(
    TableName = "temp1",
    DisplayName = "Empresa",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_EMPRESA",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("temp1")]
public class Empresa
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdempresa")]
    [Display(Name = "Código")]
    public int CdEmpresa { get; set; }

    [Column("nmempresa")]
    [StringLength(100)]
    [Display(Name = "Razão Social")]
    public string NmEmpresa { get; set; } = string.Empty;

    [Column("nmfantasia")]
    [StringLength(30)]
    [Display(Name = "Nome Fantasia")]
    public string? NmFantasia { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Configurações de Cálculo
    // ═══════════════════════════════════════════════════════════════════

    [Column("chtpcche")]
    [StringLength(2)]
    [Display(Name = "Tipo Cálculo Cheque")]
    public string? ChTpcChe { get; set; }

    [Column("chtpdarf")]
    [StringLength(2)]
    [Display(Name = "Tipo DARF")]
    public string? ChTpDarf { get; set; }

    [Column("chtpgrps")]
    [StringLength(2)]
    [Display(Name = "Tipo GRPS")]
    public string? ChTpGrps { get; set; }

    [Column("chtptres")]
    [StringLength(2)]
    [Display(Name = "Tipo Três")]
    public string? ChTpTres { get; set; }

    [Column("chbrwfunc")]
    [StringLength(1)]
    [Display(Name = "Browse Funcionário")]
    public string? ChBrwFunc { get; set; }

    [Column("chtorc1")]
    [StringLength(1)]
    [Display(Name = "Tipo Orçamento")]
    public string? ChTorc1 { get; set; }

    [Column("chferias")]
    [StringLength(1)]
    [Display(Name = "Configuração Férias")]
    public string? ChFerias { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Logos
    // ═══════════════════════════════════════════════════════════════════

    [Column("nmarqlogo")]
    [StringLength(80)]
    [Display(Name = "Arquivo Logo")]
    public string? NmArqLogo { get; set; }

    [Column("nmarqlogocra")]
    [StringLength(80)]
    [Display(Name = "Arquivo Logo Crachá")]
    public string? NmArqLogoCra { get; set; }

    [Column("arquivologo")]
    [StringLength(42)]
    [Display(Name = "Arquivo Logo (Path)")]
    public string? ArquivoLogo { get; set; }

    [Column("logo")]
    [Display(Name = "Logo (Base64)")]
    public string? Logo { get; set; }

    [Column("arquivologocracha")]
    [StringLength(42)]
    [Display(Name = "Arquivo Logo Crachá (Path)")]
    public string? ArquivoLogoCracha { get; set; }

    [Column("logocracha")]
    [Display(Name = "Logo Crachá (Base64)")]
    public string? LogoCracha { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Dados do Empregador (eSocial S-1000)
    // ═══════════════════════════════════════════════════════════════════

    [Column("tpinscempregador")]
    [StringLength(1)]
    [Display(Name = "Tipo Inscrição Empregador")]
    public string? TpInscEmpregador { get; set; }

    [Column("nrinscempregador")]
    [StringLength(15)]
    [Display(Name = "Nº Inscrição Empregador")]
    public string? NrInscEmpregador { get; set; }

    [Column("flativo")]
    [StringLength(1)]
    [Display(Name = "Ativo")]
    public string? FlAtivo { get; set; }

    [Column("flfapesocial")]
    [Display(Name = "Flag FAP eSocial")]
    public int? FlFapEsocial { get; set; }

    [Column("cnpjefr")]
    [StringLength(14)]
    [Display(Name = "CNPJ EFR")]
    public string? CnpjEfr { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Indicadores eSocial
    // ═══════════════════════════════════════════════════════════════════

    [Column("indacordoisenmulta")]
    [Display(Name = "Indicador Acordo Isenção Multa")]
    public int? IndAcordoIsenmulta { get; set; }

    [Column("indconstrutora")]
    [Display(Name = "Indicador Construtora")]
    public int? IndConstrutora { get; set; }

    [Column("indcooperativa")]
    [Display(Name = "Indicador Cooperativa")]
    public int? IndCooperativa { get; set; }

    [Column("inddesfolha")]
    [Display(Name = "Indicador Desoneração Folha")]
    public int? IndDesFolha { get; set; }

    [Column("indopccp")]
    [Display(Name = "Indicador Opção CP")]
    public int? IndOpcCp { get; set; }

    [Column("indporte")]
    [StringLength(1)]
    [Display(Name = "Indicador Porte")]
    public string? IndPorte { get; set; }

    [Column("indoptregeletronico")]
    [Display(Name = "Indicador Opção Registro Eletrônico")]
    public int? IndOptRegEletronico { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Certificados
    // ═══════════════════════════════════════════════════════════════════

    [Column("nrcertificado")]
    [StringLength(40)]
    [Display(Name = "Nº Certificado")]
    public string? NrCertificado { get; set; }

    [Column("dtemissaocertificado")]
    [Display(Name = "Data Emissão Certificado")]
    public DateTime? DtEmissaoCertificado { get; set; }

    [Column("dtvenctocertificado")]
    [Display(Name = "Data Vencimento Certificado")]
    public DateTime? DtVenctoCertificado { get; set; }

    [Column("nrprotrenovacao")]
    [StringLength(40)]
    [Display(Name = "Nº Protocolo Renovação")]
    public string? NrProtRenovacao { get; set; }

    [Column("dtprotrenovacao")]
    [Display(Name = "Data Protocolo Renovação")]
    public DateTime? DtProtRenovacao { get; set; }

    [Column("nrregett")]
    [StringLength(30)]
    [Display(Name = "Nº Registro ETT")]
    public string? NrRegEtt { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // DOU (Diário Oficial da União)
    // ═══════════════════════════════════════════════════════════════════

    [Column("dtdou")]
    [Display(Name = "Data DOU")]
    public DateTime? DtDou { get; set; }

    [Column("paginadou")]
    [StringLength(5)]
    [Display(Name = "Página DOU")]
    public string? PaginaDou { get; set; }

    [Column("ideminlei")]
    [StringLength(70)]
    [Display(Name = "Identificação Lei")]
    public string? IdeMinLei { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // FKs para entidades de OUTRO MÓDULO (Esocial) ⚠️
    // APENAS o código - SEM navigation property
    // Para buscar dados, usar IEsocialLookupService
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Classificação Tributária - Tabela 8 eSocial (Módulo Esocial)
    /// </summary>
    [Column("classtrib")]
    [StringLength(2)]
    [Display(Name = "Classificação Tributária")]
    public string? ClassTrib { get; set; }

    /// <summary>
    /// Natureza Jurídica - Tabela 21 eSocial (Módulo Esocial)
    /// </summary>
    [Column("natjuridica")]
    [StringLength(4)]
    [Display(Name = "Natureza Jurídica")]
    public string? NatJuridica { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Collections do MESMO MÓDULO (GestaoDePessoas) ✅
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Filiais desta empresa
    /// </summary>
    [InverseProperty(nameof(Filial.Empresa))]
    public virtual ICollection<Filial> Filiais { get; set; } = new List<Filial>();

    // NOTA: Colaborador não tem FK idempresa (GUID) no banco!
    // A relação é por cdempresa (código legado), não por GUID
    // Se precisar, configure manualmente no DbContext
}
