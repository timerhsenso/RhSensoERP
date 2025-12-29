// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.8
// Entity: TestEntityComplete
// Module: GestaoDePessoas
// Data: 2025-12-28 14:22:46
// =============================================================================

namespace RhSensoERP.Web.Models.GestaoDePessoas.TestEntityComplete;

/// <summary>
/// DTO de leitura para Test Entity Complete.
/// Compativel com backend: RhSensoERP.Modules.GestaoDePessoas.Application.DTOs.TestEntityCompleteDto
/// </summary>
public class TestEntityCompleteDto
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tenant
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Saa S
    /// </summary>
    public int? IdSaaS { get; set; }

    /// <summary>
    /// IdEmpresa
    /// </summary>
    public int? IdEmpresa { get; set; }

    /// <summary>
    /// IdFilial
    /// </summary>
    public int? IdFilial { get; set; }

    /// <summary>
    /// Codigo
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Nome
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Observações Ilimitadas
    /// </summary>
    public string ObservacoesMax { get; set; } = string.Empty;

    /// <summary>
    /// Texto Plano (text)
    /// </summary>
    public string TextoPlano { get; set; } = string.Empty;

    /// <summary>
    /// E-mail
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// URL
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Telefone
    /// </summary>
    public string Telefone { get; set; } = string.Empty;

    /// <summary>
    /// CPF
    /// </summary>
    public string Cpf { get; set; } = string.Empty;

    /// <summary>
    /// CNPJ
    /// </summary>
    public string Cnpj { get; set; } = string.Empty;

    /// <summary>
    /// Campo Int
    /// </summary>
    public int IntField { get; set; }

    /// <summary>
    /// Campo Int Nullable
    /// </summary>
    public int? IntNullable { get; set; }

    /// <summary>
    /// Campo Long
    /// </summary>
    public long LongField { get; set; }

    /// <summary>
    /// Campo Long Nullable
    /// </summary>
    public long? LongNullable { get; set; }

    /// <summary>
    /// Campo Short
    /// </summary>
    public short ShortField { get; set; }

    /// <summary>
    /// Campo Short Nullable
    /// </summary>
    public short? ShortNullable { get; set; }

    /// <summary>
    /// Campo Byte
    /// </summary>
    public byte ByteField { get; set; }

    /// <summary>
    /// Campo Byte Nullable
    /// </summary>
    public byte? ByteNullable { get; set; }

    /// <summary>
    /// Quantidade (com Range)
    /// </summary>
    public int QuantidadeComRange { get; set; }

    /// <summary>
    /// Decimal Padrão (18,2)
    /// </summary>
    public decimal DecimalPadrao { get; set; }

    /// <summary>
    /// Decimal Preciso (18,4)
    /// </summary>
    public decimal DecimalPreciso { get; set; }

    /// <summary>
    /// Valor Monetário
    /// </summary>
    public decimal DecimalMoeda { get; set; }

    /// <summary>
    /// Percentual
    /// </summary>
    public decimal DecimalPercentual { get; set; }

    /// <summary>
    /// Decimal Nullable
    /// </summary>
    public decimal? DecimalNullable { get; set; }

    /// <summary>
    /// Campo Double
    /// </summary>
    public double DoubleField { get; set; }

    /// <summary>
    /// Campo Double Nullable
    /// </summary>
    public double? DoubleNullable { get; set; }

    /// <summary>
    /// Campo Float
    /// </summary>
    public float FloatField { get; set; }

    /// <summary>
    /// Campo Float Nullable
    /// </summary>
    public float? FloatNullable { get; set; }

    /// <summary>
    /// Data/Hora Padrão
    /// </summary>
    public DateTime DataHoraPadrao { get; set; }

    /// <summary>
    /// Data/Hora Nullable
    /// </summary>
    public DateTime? DataHoraNullable { get; set; }

    /// <summary>
    /// Data/Hora sem Precisão
    /// </summary>
    public DateTime? DataHoraSemPrecisao { get; set; }

    /// <summary>
    /// Data/Hora Alta Precisão
    /// </summary>
    public DateTime? DataHoraAltaPrecisao { get; set; }

    /// <summary>
    /// Data Antiga (datetime legacy)
    /// </summary>
    public DateTime? DataAntiga { get; set; }

    /// <summary>
    /// SmallDateTime
    /// </summary>
    public DateTime? DataSmallDateTime { get; set; }

    /// <summary>
    /// Data de Nascimento
    /// </summary>
    public DateOnly? DataNascimento { get; set; }

    /// <summary>
    /// Hora de Início
    /// </summary>
    public TimeOnly? HoraInicio { get; set; }

    /// <summary>
    /// Duração (TimeSpan)
    /// </summary>
    public string DuracaoTimeSpan { get; set; } = string.Empty;

    /// <summary>
    /// Data com Offset
    /// </summary>
    public string DataComOffset { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Bool Nullable
    /// </summary>
    public bool? BoolNullable { get; set; }

    /// <summary>
    /// Bloqueado
    /// </summary>
    public bool Bloqueado { get; set; }

    /// <summary>
    /// GUID
    /// </summary>
    public Guid GuidField { get; set; }

    /// <summary>
    /// GUID Nullable
    /// </summary>
    public Guid? GuidNullable { get; set; }

    /// <summary>
    /// ID Externo
    /// </summary>
    public Guid? ExternalId { get; set; }

    /// <summary>
    /// Dados Binários
    /// </summary>
    public byte[] BinaryData { get; set; }

    /// <summary>
    /// Imagem Pequena
    /// </summary>
    public byte[] ImagemPequena { get; set; }

    /// <summary>
    /// Row Version
    /// </summary>
    public byte[] RowVersion { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Status Nullable
    /// </summary>
    public string StatusNullable { get; set; } = string.Empty;

    /// <summary>
    /// Prioridade
    /// </summary>
    public string Prioridade { get; set; } = string.Empty;

    /// <summary>
    /// Dados JSON (string)
    /// </summary>
    public string JsonData { get; set; } = string.Empty;

    /// <summary>
    /// Configuração JSON
    /// </summary>
    public string JsonConfig { get; set; } = string.Empty;

    /// <summary>
    /// Metadados JSON
    /// </summary>
    public string JsonMetadata { get; set; } = string.Empty;

    /// <summary>
    /// Conteúdo XML
    /// </summary>
    public string XmlContent { get; set; } = string.Empty;

    /// <summary>
    /// Configurações XML
    /// </summary>
    public string XmlSettings { get; set; } = string.Empty;

    /// <summary>
    /// ID Pai
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// ID Categoria
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// ID Proprietário
    /// </summary>
    public int? OwnerId { get; set; }

    /// <summary>
    /// Criado em
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Created By
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// ID Usuário Criador
    /// </summary>
    public int? CreatedByUserId { get; set; }

    /// <summary>
    /// Atualizado em
    /// </summary>
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>
    /// Updated By
    /// </summary>
    public string UpdatedBy { get; set; } = string.Empty;

    /// <summary>
    /// ID Usuário Atualizador
    /// </summary>
    public int? UpdatedByUserId { get; set; }

    /// <summary>
    /// Excluído em
    /// </summary>
    public DateTime? DeletedAtUtc { get; set; }

    /// <summary>
    /// Excluído por
    /// </summary>
    public string DeletedBy { get; set; } = string.Empty;

    /// <summary>
    /// ID Usuário que Excluiu
    /// </summary>
    public int? DeletedByUserId { get; set; }

    /// <summary>
    /// Está Excluído
    /// </summary>
    public bool IsDeleted { get; set; }
}
