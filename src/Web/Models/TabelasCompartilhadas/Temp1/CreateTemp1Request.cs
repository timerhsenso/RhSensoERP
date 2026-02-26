// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Temp1
// Module: TabelasCompartilhadas
// Data: 2026-02-25 19:38:48
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.TabelasCompartilhadas.Temp1;

/// <summary>
/// Request para criação de Empresa.
/// Compatível com backend: CreateTemp1Request
/// </summary>
public class CreateTemp1Request
{
    /// <summary>
    /// Código
    /// </summary>
    [Display(Name = "Código")]
    [Required(ErrorMessage = "Código é obrigatório")]
    public int Cdempresa { get; set; }

    /// <summary>
    /// Razão Social
    /// </summary>
    [Display(Name = "Razão Social")]
    [StringLength(100, ErrorMessage = "Razão Social deve ter no máximo {1} caracteres")]
    public string Nmempresa { get; set; } = string.Empty;

    /// <summary>
    /// Nome Fantasia
    /// </summary>
    [Display(Name = "Nome Fantasia")]
    [StringLength(30, ErrorMessage = "Nome Fantasia deve ter no máximo {1} caracteres")]
    public string Nmfantasia { get; set; } = string.Empty;

    /// <summary>
    /// Tipo Cálculo Cheque
    /// </summary>
    [Display(Name = "Tipo Cálculo Cheque")]
    [StringLength(2, ErrorMessage = "Tipo Cálculo Cheque deve ter no máximo {1} caracteres")]
    public string ChTpcche { get; set; } = string.Empty;

    /// <summary>
    /// Tipo DARF
    /// </summary>
    [Display(Name = "Tipo DARF")]
    [StringLength(2, ErrorMessage = "Tipo DARF deve ter no máximo {1} caracteres")]
    public string ChTpdarf { get; set; } = string.Empty;

    /// <summary>
    /// Tipo GRPS
    /// </summary>
    [Display(Name = "Tipo GRPS")]
    [StringLength(2, ErrorMessage = "Tipo GRPS deve ter no máximo {1} caracteres")]
    public string ChTpgrps { get; set; } = string.Empty;

    /// <summary>
    /// Tipo Três
    /// </summary>
    [Display(Name = "Tipo Três")]
    [StringLength(2, ErrorMessage = "Tipo Três deve ter no máximo {1} caracteres")]
    public string ChTptres { get; set; } = string.Empty;

    /// <summary>
    /// Browse Funcionário
    /// </summary>
    [Display(Name = "Browse Funcionário")]
    [StringLength(1, ErrorMessage = "Browse Funcionário deve ter no máximo {1} caracteres")]
    public string Chbrwfunc { get; set; } = string.Empty;

    /// <summary>
    /// Tipo Orçamento
    /// </summary>
    [Display(Name = "Tipo Orçamento")]
    [StringLength(1, ErrorMessage = "Tipo Orçamento deve ter no máximo {1} caracteres")]
    public string Chtorc1 { get; set; } = string.Empty;

    /// <summary>
    /// Configuração Férias
    /// </summary>
    [Display(Name = "Configuração Férias")]
    [StringLength(1, ErrorMessage = "Configuração Férias deve ter no máximo {1} caracteres")]
    public string Chferias { get; set; } = string.Empty;

    /// <summary>
    /// Arquivo Logo
    /// </summary>
    [Display(Name = "Arquivo Logo")]
    [StringLength(80, ErrorMessage = "Arquivo Logo deve ter no máximo {1} caracteres")]
    public string Nmarqlogo { get; set; } = string.Empty;

    /// <summary>
    /// Arquivo Logo Crachá
    /// </summary>
    [Display(Name = "Arquivo Logo Crachá")]
    [StringLength(80, ErrorMessage = "Arquivo Logo Crachá deve ter no máximo {1} caracteres")]
    public string Nmarqlogocra { get; set; } = string.Empty;

    /// <summary>
    /// Arquivo Logo (Path)
    /// </summary>
    [Display(Name = "Arquivo Logo (Path)")]
    [StringLength(42, ErrorMessage = "Arquivo Logo (Path) deve ter no máximo {1} caracteres")]
    public string Arquivologo { get; set; } = string.Empty;

    /// <summary>
    /// Logo (Base64)
    /// </summary>
    [Display(Name = "Logo (Base64)")]
    public string Logo { get; set; } = string.Empty;

    /// <summary>
    /// Arquivo Logo Crachá (Path)
    /// </summary>
    [Display(Name = "Arquivo Logo Crachá (Path)")]
    [StringLength(42, ErrorMessage = "Arquivo Logo Crachá (Path) deve ter no máximo {1} caracteres")]
    public string Arquivologocracha { get; set; } = string.Empty;

    /// <summary>
    /// Logo Crachá (Base64)
    /// </summary>
    [Display(Name = "Logo Crachá (Base64)")]
    public string Logocracha { get; set; } = string.Empty;

    /// <summary>
    /// Tipo Inscrição Empregador
    /// </summary>
    [Display(Name = "Tipo Inscrição Empregador")]
    [StringLength(1, ErrorMessage = "Tipo Inscrição Empregador deve ter no máximo {1} caracteres")]
    public string Tpinscempregador { get; set; } = string.Empty;

    /// <summary>
    /// Nº Inscrição Empregador
    /// </summary>
    [Display(Name = "Nº Inscrição Empregador")]
    [StringLength(15, ErrorMessage = "Nº Inscrição Empregador deve ter no máximo {1} caracteres")]
    public string Nrinscempregador { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [StringLength(1, ErrorMessage = "Ativo deve ter no máximo {1} caracteres")]
    public string Flativo { get; set; } = string.Empty;

    /// <summary>
    /// Flag FAP eSocial
    /// </summary>
    [Display(Name = "Flag FAP eSocial")]
    public int? Flfapesocial { get; set; }

    /// <summary>
    /// CNPJ EFR
    /// </summary>
    [Display(Name = "CNPJ EFR")]
    [StringLength(14, ErrorMessage = "CNPJ EFR deve ter no máximo {1} caracteres")]
    public string Cnpjefr { get; set; } = string.Empty;

    /// <summary>
    /// Indicador Acordo Isenção Multa
    /// </summary>
    [Display(Name = "Indicador Acordo Isenção Multa")]
    public int? IndacordoiseNmulta { get; set; }

    /// <summary>
    /// Indicador Construtora
    /// </summary>
    [Display(Name = "Indicador Construtora")]
    public int? InDconStrutora { get; set; }

    /// <summary>
    /// Indicador Cooperativa
    /// </summary>
    [Display(Name = "Indicador Cooperativa")]
    public int? InDcooperativa { get; set; }

    /// <summary>
    /// Indicador Desoneração Folha
    /// </summary>
    [Display(Name = "Indicador Desoneração Folha")]
    public int? Inddesfolha { get; set; }

    /// <summary>
    /// Indicador Opção CP
    /// </summary>
    [Display(Name = "Indicador Opção CP")]
    public int? IndoPccp { get; set; }

    /// <summary>
    /// Indicador Porte
    /// </summary>
    [Display(Name = "Indicador Porte")]
    [StringLength(1, ErrorMessage = "Indicador Porte deve ter no máximo {1} caracteres")]
    public string Indporte { get; set; } = string.Empty;

    /// <summary>
    /// Indicador Opção Registro Eletrônico
    /// </summary>
    [Display(Name = "Indicador Opção Registro Eletrônico")]
    public int? Indoptregeletronico { get; set; }

    /// <summary>
    /// Nº Certificado
    /// </summary>
    [Display(Name = "Nº Certificado")]
    [StringLength(40, ErrorMessage = "Nº Certificado deve ter no máximo {1} caracteres")]
    public string Nrcertificado { get; set; } = string.Empty;

    /// <summary>
    /// Data Emissão Certificado
    /// </summary>
    [Display(Name = "Data Emissão Certificado")]
    public DateTime? Dtemissaocertificado { get; set; }

    /// <summary>
    /// Data Vencimento Certificado
    /// </summary>
    [Display(Name = "Data Vencimento Certificado")]
    public DateTime? Dtvenctocertificado { get; set; }

    /// <summary>
    /// Nº Protocolo Renovação
    /// </summary>
    [Display(Name = "Nº Protocolo Renovação")]
    [StringLength(40, ErrorMessage = "Nº Protocolo Renovação deve ter no máximo {1} caracteres")]
    public string NrprotreNovacao { get; set; } = string.Empty;

    /// <summary>
    /// Data Protocolo Renovação
    /// </summary>
    [Display(Name = "Data Protocolo Renovação")]
    public DateTime? DtprotreNovacao { get; set; }

    /// <summary>
    /// Nº Registro ETT
    /// </summary>
    [Display(Name = "Nº Registro ETT")]
    [StringLength(30, ErrorMessage = "Nº Registro ETT deve ter no máximo {1} caracteres")]
    public string Nrregett { get; set; } = string.Empty;

    /// <summary>
    /// Data DOU
    /// </summary>
    [Display(Name = "Data DOU")]
    public DateTime? Dtdou { get; set; }

    /// <summary>
    /// Página DOU
    /// </summary>
    [Display(Name = "Página DOU")]
    [StringLength(5, ErrorMessage = "Página DOU deve ter no máximo {1} caracteres")]
    public string Paginadou { get; set; } = string.Empty;

    /// <summary>
    /// Identificação Lei
    /// </summary>
    [Display(Name = "Identificação Lei")]
    [StringLength(70, ErrorMessage = "Identificação Lei deve ter no máximo {1} caracteres")]
    public string Ideminlei { get; set; } = string.Empty;

    /// <summary>
    /// Classificação Tributária
    /// </summary>
    [Display(Name = "Classificação Tributária")]
    [StringLength(2, ErrorMessage = "Classificação Tributária deve ter no máximo {1} caracteres")]
    public string ClasStrib { get; set; } = string.Empty;

    /// <summary>
    /// Natureza Jurídica
    /// </summary>
    [Display(Name = "Natureza Jurídica")]
    [StringLength(4, ErrorMessage = "Natureza Jurídica deve ter no máximo {1} caracteres")]
    public string NatjurIdica { get; set; } = string.Empty;
}
