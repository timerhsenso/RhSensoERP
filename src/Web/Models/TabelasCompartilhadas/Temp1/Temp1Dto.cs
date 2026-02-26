// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v4.0
// Entity: Temp1
// Module: TabelasCompartilhadas
// Data: 2026-02-25 19:38:48
// =============================================================================

namespace RhSensoERP.Web.Models.TabelasCompartilhadas.Temp1;

/// <summary>
/// DTO de leitura para Empresa.
/// Compatível com backend: RhSensoERP.Modules.TabelasCompartilhadas.Application.DTOs.Temp1Dto
/// </summary>
public class Temp1Dto
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Código
    /// </summary>
    public int Cdempresa { get; set; }

    /// <summary>
    /// Razão Social
    /// </summary>
    public string Nmempresa { get; set; } = string.Empty;

    /// <summary>
    /// Nome Fantasia
    /// </summary>
    public string Nmfantasia { get; set; } = string.Empty;

    /// <summary>
    /// Tipo Cálculo Cheque
    /// </summary>
    public string ChTpcche { get; set; } = string.Empty;

    /// <summary>
    /// Tipo DARF
    /// </summary>
    public string ChTpdarf { get; set; } = string.Empty;

    /// <summary>
    /// Tipo GRPS
    /// </summary>
    public string ChTpgrps { get; set; } = string.Empty;

    /// <summary>
    /// Tipo Três
    /// </summary>
    public string ChTptres { get; set; } = string.Empty;

    /// <summary>
    /// Browse Funcionário
    /// </summary>
    public string Chbrwfunc { get; set; } = string.Empty;

    /// <summary>
    /// Tipo Orçamento
    /// </summary>
    public string Chtorc1 { get; set; } = string.Empty;

    /// <summary>
    /// Configuração Férias
    /// </summary>
    public string Chferias { get; set; } = string.Empty;

    /// <summary>
    /// Arquivo Logo
    /// </summary>
    public string Nmarqlogo { get; set; } = string.Empty;

    /// <summary>
    /// Arquivo Logo Crachá
    /// </summary>
    public string Nmarqlogocra { get; set; } = string.Empty;

    /// <summary>
    /// Arquivo Logo (Path)
    /// </summary>
    public string Arquivologo { get; set; } = string.Empty;

    /// <summary>
    /// Logo (Base64)
    /// </summary>
    public string Logo { get; set; } = string.Empty;

    /// <summary>
    /// Arquivo Logo Crachá (Path)
    /// </summary>
    public string Arquivologocracha { get; set; } = string.Empty;

    /// <summary>
    /// Logo Crachá (Base64)
    /// </summary>
    public string Logocracha { get; set; } = string.Empty;

    /// <summary>
    /// Tipo Inscrição Empregador
    /// </summary>
    public string Tpinscempregador { get; set; } = string.Empty;

    /// <summary>
    /// Nº Inscrição Empregador
    /// </summary>
    public string Nrinscempregador { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    public string Flativo { get; set; } = string.Empty;

    /// <summary>
    /// Flag FAP eSocial
    /// </summary>
    public int? Flfapesocial { get; set; }

    /// <summary>
    /// CNPJ EFR
    /// </summary>
    public string Cnpjefr { get; set; } = string.Empty;

    /// <summary>
    /// Indicador Acordo Isenção Multa
    /// </summary>
    public int? IndacordoiseNmulta { get; set; }

    /// <summary>
    /// Indicador Construtora
    /// </summary>
    public int? InDconStrutora { get; set; }

    /// <summary>
    /// Indicador Cooperativa
    /// </summary>
    public int? InDcooperativa { get; set; }

    /// <summary>
    /// Indicador Desoneração Folha
    /// </summary>
    public int? Inddesfolha { get; set; }

    /// <summary>
    /// Indicador Opção CP
    /// </summary>
    public int? IndoPccp { get; set; }

    /// <summary>
    /// Indicador Porte
    /// </summary>
    public string Indporte { get; set; } = string.Empty;

    /// <summary>
    /// Indicador Opção Registro Eletrônico
    /// </summary>
    public int? Indoptregeletronico { get; set; }

    /// <summary>
    /// Nº Certificado
    /// </summary>
    public string Nrcertificado { get; set; } = string.Empty;

    /// <summary>
    /// Data Emissão Certificado
    /// </summary>
    public DateTime? Dtemissaocertificado { get; set; }

    /// <summary>
    /// Data Vencimento Certificado
    /// </summary>
    public DateTime? Dtvenctocertificado { get; set; }

    /// <summary>
    /// Nº Protocolo Renovação
    /// </summary>
    public string NrprotreNovacao { get; set; } = string.Empty;

    /// <summary>
    /// Data Protocolo Renovação
    /// </summary>
    public DateTime? DtprotreNovacao { get; set; }

    /// <summary>
    /// Nº Registro ETT
    /// </summary>
    public string Nrregett { get; set; } = string.Empty;

    /// <summary>
    /// Data DOU
    /// </summary>
    public DateTime? Dtdou { get; set; }

    /// <summary>
    /// Página DOU
    /// </summary>
    public string Paginadou { get; set; } = string.Empty;

    /// <summary>
    /// Identificação Lei
    /// </summary>
    public string Ideminlei { get; set; } = string.Empty;

    /// <summary>
    /// Classificação Tributária
    /// </summary>
    public string ClasStrib { get; set; } = string.Empty;

    /// <summary>
    /// Natureza Jurídica
    /// </summary>
    public string NatjurIdica { get; set; } = string.Empty;
}
