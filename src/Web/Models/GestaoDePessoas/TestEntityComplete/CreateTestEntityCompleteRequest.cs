// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: TestEntityComplete
// Module: GestaoDePessoas
// Data: 2025-12-28 14:22:46
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.GestaoDePessoas.TestEntityComplete;

/// <summary>
/// Request para criacao de Test Entity Complete.
/// Compativel com backend: CreateTestEntityCompleteRequest
/// </summary>
public class CreateTestEntityCompleteRequest
{
    /// <summary>
    /// IdEmpresa
    /// </summary>
    [Display(Name = "IdEmpresa")]
    public int? IdEmpresa { get; set; }

    /// <summary>
    /// IdFilial
    /// </summary>
    [Display(Name = "IdFilial")]
    public int? IdFilial { get; set; }

    /// <summary>
    /// Codigo
    /// </summary>
    [Display(Name = "Codigo")]
    [Required(ErrorMessage = "Codigo e obrigatorio")]
    [StringLength(20, ErrorMessage = "Codigo deve ter no maximo {1} caracteres")]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Nome
    /// </summary>
    [Display(Name = "Nome")]
    [Required(ErrorMessage = "Nome e obrigatorio")]
    [StringLength(200, ErrorMessage = "Nome deve ter no maximo {1} caracteres")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [StringLength(1000, ErrorMessage = "Descrição deve ter no maximo {1} caracteres")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Observações Ilimitadas
    /// </summary>
    [Display(Name = "Observações Ilimitadas")]
    public string ObservacoesMax { get; set; } = string.Empty;

    /// <summary>
    /// Texto Plano (text)
    /// </summary>
    [Display(Name = "Texto Plano (text)")]
    public string TextoPlano { get; set; } = string.Empty;

    /// <summary>
    /// E-mail
    /// </summary>
    [Display(Name = "E-mail")]
    [StringLength(254, ErrorMessage = "E-mail deve ter no maximo {1} caracteres")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// URL
    /// </summary>
    [Display(Name = "URL")]
    [StringLength(500, ErrorMessage = "URL deve ter no maximo {1} caracteres")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Telefone
    /// </summary>
    [Display(Name = "Telefone")]
    [StringLength(20, ErrorMessage = "Telefone deve ter no maximo {1} caracteres")]
    public string Telefone { get; set; } = string.Empty;

    /// <summary>
    /// CPF
    /// </summary>
    [Display(Name = "CPF")]
    [StringLength(11, ErrorMessage = "CPF deve ter no maximo {1} caracteres")]
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ
    /// </summary>
    [Display(Name = "CNPJ")]
    [StringLength(14, ErrorMessage = "CNPJ deve ter no maximo {1} caracteres")]
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>
    /// Campo Int
    /// </summary>
    [Display(Name = "Campo Int")]
    [Required(ErrorMessage = "Campo Int e obrigatorio")]
    public int IntField { get; set; }

    /// <summary>
    /// Campo Int Nullable
    /// </summary>
    [Display(Name = "Campo Int Nullable")]
    public int? IntNullable { get; set; }

    /// <summary>
    /// Campo Long
    /// </summary>
    [Display(Name = "Campo Long")]
    [Required(ErrorMessage = "Campo Long e obrigatorio")]
    public long LongField { get; set; }

    /// <summary>
    /// Campo Long Nullable
    /// </summary>
    [Display(Name = "Campo Long Nullable")]
    public long? LongNullable { get; set; }

    /// <summary>
    /// Campo Short
    /// </summary>
    [Display(Name = "Campo Short")]
    [Required(ErrorMessage = "Campo Short e obrigatorio")]
    public short ShortField { get; set; }

    /// <summary>
    /// Campo Short Nullable
    /// </summary>
    [Display(Name = "Campo Short Nullable")]
    public short? ShortNullable { get; set; }

    /// <summary>
    /// Campo Byte
    /// </summary>
    [Display(Name = "Campo Byte")]
    [Required(ErrorMessage = "Campo Byte e obrigatorio")]
    public byte ByteField { get; set; }

    /// <summary>
    /// Campo Byte Nullable
    /// </summary>
    [Display(Name = "Campo Byte Nullable")]
    public byte? ByteNullable { get; set; }

    /// <summary>
    /// Quantidade (com Range)
    /// </summary>
    [Display(Name = "Quantidade (com Range)")]
    [Required(ErrorMessage = "Quantidade (com Range) e obrigatorio")]
    public int QuantidadeComRange { get; set; }

    /// <summary>
    /// Decimal Padrão (18,2)
    /// </summary>
    [Display(Name = "Decimal Padrão (18,2)")]
    [Required(ErrorMessage = "Decimal Padrão (18,2) e obrigatorio")]
    public decimal DecimalPadrao { get; set; }

    /// <summary>
    /// Decimal Preciso (18,4)
    /// </summary>
    [Display(Name = "Decimal Preciso (18,4)")]
    [Required(ErrorMessage = "Decimal Preciso (18,4) e obrigatorio")]
    public decimal DecimalPreciso { get; set; }

    /// <summary>
    /// Valor Monetário
    /// </summary>
    [Display(Name = "Valor Monetário")]
    [Required(ErrorMessage = "Valor Monetário e obrigatorio")]
    public decimal DecimalMoeda { get; set; }

    /// <summary>
    /// Percentual
    /// </summary>
    [Display(Name = "Percentual")]
    [Required(ErrorMessage = "Percentual e obrigatorio")]
    public decimal DecimalPercentual { get; set; }

    /// <summary>
    /// Decimal Nullable
    /// </summary>
    [Display(Name = "Decimal Nullable")]
    public decimal? DecimalNullable { get; set; }

    /// <summary>
    /// Campo Double
    /// </summary>
    [Display(Name = "Campo Double")]
    [Required(ErrorMessage = "Campo Double e obrigatorio")]
    public double DoubleField { get; set; }

    /// <summary>
    /// Campo Double Nullable
    /// </summary>
    [Display(Name = "Campo Double Nullable")]
    public double? DoubleNullable { get; set; }

    /// <summary>
    /// Campo Float
    /// </summary>
    [Display(Name = "Campo Float")]
    [Required(ErrorMessage = "Campo Float e obrigatorio")]
    public float FloatField { get; set; }

    /// <summary>
    /// Campo Float Nullable
    /// </summary>
    [Display(Name = "Campo Float Nullable")]
    public float? FloatNullable { get; set; }

    /// <summary>
    /// Data/Hora Padrão
    /// </summary>
    [Display(Name = "Data/Hora Padrão")]
    [Required(ErrorMessage = "Data/Hora Padrão e obrigatorio")]
    public DateTime DataHoraPadrao { get; set; }

    /// <summary>
    /// Data/Hora Nullable
    /// </summary>
    [Display(Name = "Data/Hora Nullable")]
    public DateTime? DataHoraNullable { get; set; }

    /// <summary>
    /// Data/Hora sem Precisão
    /// </summary>
    [Display(Name = "Data/Hora sem Precisão")]
    public DateTime? DataHoraSemPrecisao { get; set; }

    /// <summary>
    /// Data/Hora Alta Precisão
    /// </summary>
    [Display(Name = "Data/Hora Alta Precisão")]
    public DateTime? DataHoraAltaPrecisao { get; set; }

    /// <summary>
    /// Data Antiga (datetime legacy)
    /// </summary>
    [Display(Name = "Data Antiga (datetime legacy)")]
    public DateTime? DataAntiga { get; set; }

    /// <summary>
    /// SmallDateTime
    /// </summary>
    [Display(Name = "SmallDateTime")]
    public DateTime? DataSmallDateTime { get; set; }

    /// <summary>
    /// Data de Nascimento
    /// </summary>
    [Display(Name = "Data de Nascimento")]
    public DateOnly? DataNascimento { get; set; }

    /// <summary>
    /// Hora de Início
    /// </summary>
    [Display(Name = "Hora de Início")]
    public TimeOnly? HoraInicio { get; set; }

    /// <summary>
    /// Duração (TimeSpan)
    /// </summary>
    [Display(Name = "Duração (TimeSpan)")]
    public string DuracaoTimeSpan { get; set; } = string.Empty;

    /// <summary>
    /// Data com Offset
    /// </summary>
    [Display(Name = "Data com Offset")]
    public string DataComOffset { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo e obrigatorio")]
    public bool Ativo { get; set; }

    /// <summary>
    /// Bool Nullable
    /// </summary>
    [Display(Name = "Bool Nullable")]
    public bool? BoolNullable { get; set; }

    /// <summary>
    /// Bloqueado
    /// </summary>
    [Display(Name = "Bloqueado")]
    [Required(ErrorMessage = "Bloqueado e obrigatorio")]
    public bool Bloqueado { get; set; }

    /// <summary>
    /// GUID
    /// </summary>
    [Display(Name = "GUID")]
    [Required(ErrorMessage = "GUID e obrigatorio")]
    public Guid GuidField { get; set; }

    /// <summary>
    /// GUID Nullable
    /// </summary>
    [Display(Name = "GUID Nullable")]
    public Guid? GuidNullable { get; set; }

    /// <summary>
    /// ID Externo
    /// </summary>
    [Display(Name = "ID Externo")]
    public Guid? ExternalId { get; set; }

    /// <summary>
    /// Dados Binários
    /// </summary>
    [Display(Name = "Dados Binários")]
    [Required(ErrorMessage = "Dados Binários e obrigatorio")]
    public byte[] BinaryData { get; set; }

    /// <summary>
    /// Imagem Pequena
    /// </summary>
    [Display(Name = "Imagem Pequena")]
    [Required(ErrorMessage = "Imagem Pequena e obrigatorio")]
    public byte[] ImagemPequena { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    [Display(Name = "Status")]
    [Required(ErrorMessage = "Status e obrigatorio")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Status Nullable
    /// </summary>
    [Display(Name = "Status Nullable")]
    public string StatusNullable { get; set; } = string.Empty;

    /// <summary>
    /// Prioridade
    /// </summary>
    [Display(Name = "Prioridade")]
    [Required(ErrorMessage = "Prioridade e obrigatorio")]
    public string Prioridade { get; set; } = string.Empty;

    /// <summary>
    /// Dados JSON (string)
    /// </summary>
    [Display(Name = "Dados JSON (string)")]
    public string JsonData { get; set; } = string.Empty;

    /// <summary>
    /// Configuração JSON
    /// </summary>
    [Display(Name = "Configuração JSON")]
    public string JsonConfig { get; set; } = string.Empty;

    /// <summary>
    /// Metadados JSON
    /// </summary>
    [Display(Name = "Metadados JSON")]
    public string JsonMetadata { get; set; } = string.Empty;

    /// <summary>
    /// Conteúdo XML
    /// </summary>
    [Display(Name = "Conteúdo XML")]
    public string XmlContent { get; set; } = string.Empty;

    /// <summary>
    /// Configurações XML
    /// </summary>
    [Display(Name = "Configurações XML")]
    public string XmlSettings { get; set; } = string.Empty;

    /// <summary>
    /// ID Pai
    /// </summary>
    [Display(Name = "ID Pai")]
    public int? ParentId { get; set; }

    /// <summary>
    /// ID Categoria
    /// </summary>
    [Display(Name = "ID Categoria")]
    public int? CategoryId { get; set; }

    /// <summary>
    /// ID Proprietário
    /// </summary>
    [Display(Name = "ID Proprietário")]
    public int? OwnerId { get; set; }

    /// <summary>
    /// Excluído em
    /// </summary>
    [Display(Name = "Excluído em")]
    public DateTime? DeletedAtUtc { get; set; }
}
