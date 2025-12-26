using Microsoft.EntityFrameworkCore;
using RhSensoERP.Shared.Core.Attributes;
//using RhSensoERP.Shared.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace RhSensoERP.Modules.PeopleAnalyticsBI.Core.Entities;

/// <summary>
/// TestEntity - Entidade completa para validação do Source Generator
/// Tabela: test_entity_complete
/// Fonte da verdade: SQL Server
/// 
/// Esta entidade contém TODOS os tipos de campos possíveis:
/// - Tipos primitivos (int, long, short, byte, bool, string)
/// - Tipos numéricos com precisões variadas (decimal, double, float)
/// - Tipos de data/hora (DateTime, DateTimeOffset, DateOnly, TimeOnly, TimeSpan)
/// - Tipos nullable
/// - Tipos especiais (Guid, byte[], enum)
/// - Campos JSON (string, JsonElement, objeto tipado)
/// - Campos XML
/// - Campos calculados/NotMapped
/// - Relacionamentos (FK, navegação)
/// - Campos de auditoria
/// - Multi-tenancy
/// - Constraints variados (MaxLength, Required, Range, etc.)
/// </summary>
[GenerateCrud(
    TableName = "test_entity_complete",
    DisplayName = "Test Entity Complete",
    CdSistema = "TEST",
    CdFuncao = "TEST_FM_COMPLETE",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("test_entity_complete")]
[Index(nameof(TenantId), nameof(Codigo), IsUnique = true, Name = "IX_test_entity_TenantId_Codigo")]
[Index(nameof(Email), IsUnique = false, Name = "IX_test_entity_Email")]
[Index(nameof(CreatedAtUtc), Name = "IX_test_entity_CreatedAt")]
public class TestEntity
{
    // ═══════════════════════════════════════════════════════════════
    // 🔑 CHAVE PRIMÁRIA
    // ═══════════════════════════════════════════════════════════════

    [Key]
    [Column("Id", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "ID")]
    public int Id { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // 🏢 MULTI-TENANCY
    // ═══════════════════════════════════════════════════════════════

    [Required]
    [Column("TenantId")]
    [Display(Name = "Tenant")]
    public Guid TenantId { get; set; }

    [Column("IdSaaS")]
    [Display(Name = "ID SaaS")]
    public int? IdSaaS { get; set; }

    [Column("IdEmpresa")]
    [Display(Name = "ID Empresa")]
    public int? IdEmpresa { get; set; }

    [Column("IdFilial")]
    [Display(Name = "ID Filial")]
    public int? IdFilial { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // 📝 CAMPOS STRING - VARIAÇÕES
    // ═══════════════════════════════════════════════════════════════

    [Required]
    [Column("Codigo", TypeName = "varchar(20)")]
    [StringLength(20)]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [Column("Nome", TypeName = "nvarchar(200)")]
    [StringLength(200)]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [Column("Descricao", TypeName = "nvarchar(1000)")]
    [StringLength(1000)]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Column("ObservacoesMax", TypeName = "nvarchar(max)")]
    [Display(Name = "Observações Ilimitadas")]
    public string? ObservacoesMax { get; set; }

    [Column("TextoPlano", TypeName = "text")]
    [Display(Name = "Texto Plano (text)")]
    public string? TextoPlano { get; set; }

    [Column("Email", TypeName = "varchar(254)")]
    [StringLength(254)]
    [EmailAddress]
    [Display(Name = "E-mail")]
    public string? Email { get; set; }

    [Column("Url", TypeName = "nvarchar(500)")]
    [StringLength(500)]
    [Url]
    [Display(Name = "URL")]
    public string? Url { get; set; }

    [Column("Telefone", TypeName = "varchar(20)")]
    [StringLength(20)]
    [Phone]
    [Display(Name = "Telefone")]
    public string? Telefone { get; set; }

    [Column("CPF", TypeName = "char(11)")]
    [StringLength(11, MinimumLength = 11)]
    [Display(Name = "CPF")]
    public string? CPF { get; set; }

    [Column("CNPJ", TypeName = "char(14)")]
    [StringLength(14, MinimumLength = 14)]
    [Display(Name = "CNPJ")]
    public string? CNPJ { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // 🔢 CAMPOS NUMÉRICOS INTEIROS
    // ═══════════════════════════════════════════════════════════════

    [Column("IntField")]
    [Display(Name = "Campo Int")]
    public int IntField { get; set; }

    [Column("IntNullable")]
    [Display(Name = "Campo Int Nullable")]
    public int? IntNullable { get; set; }

    [Column("LongField")]
    [Display(Name = "Campo Long")]
    public long LongField { get; set; }

    [Column("LongNullable")]
    [Display(Name = "Campo Long Nullable")]
    public long? LongNullable { get; set; }

    [Column("ShortField")]
    [Display(Name = "Campo Short")]
    public short ShortField { get; set; }

    [Column("ShortNullable")]
    [Display(Name = "Campo Short Nullable")]
    public short? ShortNullable { get; set; }

    [Column("ByteField")]
    [Display(Name = "Campo Byte")]
    public byte ByteField { get; set; }

    [Column("ByteNullable")]
    [Display(Name = "Campo Byte Nullable")]
    public byte? ByteNullable { get; set; }

    [Column("QuantidadeComRange")]
    [Range(0, 9999)]
    [Display(Name = "Quantidade (com Range)")]
    public int QuantidadeComRange { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // 💰 CAMPOS NUMÉRICOS DECIMAIS
    // ═══════════════════════════════════════════════════════════════

    [Column("DecimalPadrao", TypeName = "decimal(18,2)")]
    [Precision(18, 2)]
    [Display(Name = "Decimal Padrão (18,2)")]
    public decimal DecimalPadrao { get; set; }

    [Column("DecimalPreciso", TypeName = "decimal(18,4)")]
    [Precision(18, 4)]
    [Display(Name = "Decimal Preciso (18,4)")]
    public decimal DecimalPreciso { get; set; }

    [Column("DecimalMoeda", TypeName = "decimal(15,2)")]
    [Precision(15, 2)]
    [DataType(DataType.Currency)]
    [Display(Name = "Valor Monetário")]
    public decimal DecimalMoeda { get; set; }

    [Column("DecimalPercentual", TypeName = "decimal(5,2)")]
    [Precision(5, 2)]
    [Range(0, 100)]
    [Display(Name = "Percentual")]
    public decimal DecimalPercentual { get; set; }

    [Column("DecimalNullable", TypeName = "decimal(18,2)")]
    [Precision(18, 2)]
    [Display(Name = "Decimal Nullable")]
    public decimal? DecimalNullable { get; set; }

    [Column("DoubleField")]
    [Display(Name = "Campo Double")]
    public double DoubleField { get; set; }

    [Column("DoubleNullable")]
    [Display(Name = "Campo Double Nullable")]
    public double? DoubleNullable { get; set; }

    [Column("FloatField", TypeName = "real")]
    [Display(Name = "Campo Float")]
    public float FloatField { get; set; }

    [Column("FloatNullable", TypeName = "real")]
    [Display(Name = "Campo Float Nullable")]
    public float? FloatNullable { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // 📅 CAMPOS DE DATA/HORA
    // ═══════════════════════════════════════════════════════════════

    [Required]
    [Column("DataHoraPadrao", TypeName = "datetime2(3)")]
    [Display(Name = "Data/Hora Padrão")]
    [DataType(DataType.DateTime)]
    public DateTime DataHoraPadrao { get; set; }

    [Column("DataHoraNullable", TypeName = "datetime2(3)")]
    [Display(Name = "Data/Hora Nullable")]
    [DataType(DataType.DateTime)]
    public DateTime? DataHoraNullable { get; set; }

    [Column("DataHoraSemPrecisao", TypeName = "datetime2(0)")]
    [Display(Name = "Data/Hora sem Precisão")]
    public DateTime? DataHoraSemPrecisao { get; set; }

    [Column("DataHoraAltaPrecisao", TypeName = "datetime2(7)")]
    [Display(Name = "Data/Hora Alta Precisão")]
    public DateTime? DataHoraAltaPrecisao { get; set; }

    [Column("DataAntiga", TypeName = "datetime")]
    [Display(Name = "Data Antiga (datetime legacy)")]
    public DateTime? DataAntiga { get; set; }

    [Column("DataSmallDateTime", TypeName = "smalldatetime")]
    [Display(Name = "SmallDateTime")]
    public DateTime? DataSmallDateTime { get; set; }

    [Column("DataNascimento", TypeName = "date")]
    [Display(Name = "Data de Nascimento")]
    [DataType(DataType.Date)]
    public DateOnly? DataNascimento { get; set; }

    [Column("HoraInicio", TypeName = "time(3)")]
    [Display(Name = "Hora de Início")]
    [DataType(DataType.Time)]
    public TimeOnly? HoraInicio { get; set; }

    [Column("DuracaoTimeSpan")]
    [Display(Name = "Duração (TimeSpan)")]
    public TimeSpan? DuracaoTimeSpan { get; set; }

    [Column("DataComOffset", TypeName = "datetimeoffset(3)")]
    [Display(Name = "Data com Offset")]
    public DateTimeOffset? DataComOffset { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // ✅ CAMPOS BOOLEANOS
    // ═══════════════════════════════════════════════════════════════

    [Required]
    [Column("Ativo", TypeName = "bit")]
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    [Column("BoolNullable", TypeName = "bit")]
    [Display(Name = "Bool Nullable")]
    public bool? BoolNullable { get; set; }

    [Column("Bloqueado", TypeName = "bit")]
    [Display(Name = "Bloqueado")]
    public bool Bloqueado { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // 🆔 CAMPOS GUID
    // ═══════════════════════════════════════════════════════════════

    [Column("GuidField")]
    [Display(Name = "GUID")]
    public Guid GuidField { get; set; }

    [Column("GuidNullable")]
    [Display(Name = "GUID Nullable")]
    public Guid? GuidNullable { get; set; }

    [Column("ExternalId")]
    [Display(Name = "ID Externo")]
    public Guid? ExternalId { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // 📦 CAMPOS BINÁRIOS
    // ═══════════════════════════════════════════════════════════════

    [Column("BinaryData", TypeName = "varbinary(max)")]
    [Display(Name = "Dados Binários")]
    public byte[]? BinaryData { get; set; }

    [Column("ImagemPequena", TypeName = "varbinary(8000)")]
    [MaxLength(8000)]
    [Display(Name = "Imagem Pequena")]
    public byte[]? ImagemPequena { get; set; }

    [Column("RowVersion", TypeName = "rowversion")]
    [Timestamp]
    [Display(Name = "Row Version")]
    public byte[]? RowVersion { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // 🎨 CAMPOS ENUM
    // ═══════════════════════════════════════════════════════════════

    [Required]
    [Column("Status")]
    [Display(Name = "Status")]
    public TestEntityStatus Status { get; set; } = TestEntityStatus.Ativo;

    [Column("StatusNullable")]
    [Display(Name = "Status Nullable")]
    public TestEntityStatus? StatusNullable { get; set; }

    [Column("Prioridade")]
    [Display(Name = "Prioridade")]
    public TestEntityPrioridade Prioridade { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // 📋 CAMPOS JSON
    // ═══════════════════════════════════════════════════════════════

    [Column("JsonData", TypeName = "nvarchar(max)")]
    [Display(Name = "Dados JSON (string)")]
    public string? JsonData { get; set; }

    [Column("JsonConfig", TypeName = "nvarchar(max)")]
    [Display(Name = "Configuração JSON")]
    public string? JsonConfig { get; set; }

    /// <summary>
    /// JSON fortemente tipado - exposto como objeto, armazenado como string
    /// </summary>
    [NotMapped]
    [Display(Name = "Configurações (objeto)")]
    public TestJsonSettings? Settings
    {
        get => string.IsNullOrWhiteSpace(JsonConfig)
            ? null
            : JsonSerializer.Deserialize<TestJsonSettings>(JsonConfig);
        set => JsonConfig = value != null
            ? JsonSerializer.Serialize(value)
            : null;
    }

    [Column("JsonMetadata", TypeName = "nvarchar(max)")]
    [Display(Name = "Metadados JSON")]
    public string? JsonMetadata { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // 📄 CAMPOS XML
    // ═══════════════════════════════════════════════════════════════

    [Column("XmlContent", TypeName = "xml")]
    [Display(Name = "Conteúdo XML")]
    public string? XmlContent { get; set; }

    [Column("XmlSettings", TypeName = "nvarchar(max)")]
    [Display(Name = "Configurações XML")]
    public string? XmlSettings { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // 🔗 RELACIONAMENTOS (FK)
    // ═══════════════════════════════════════════════════════════════

    [Column("ParentId")]
    [Display(Name = "ID Pai")]
    public int? ParentId { get; set; }

    [Column("CategoryId")]
    [Display(Name = "ID Categoria")]
    public int? CategoryId { get; set; }

    [Column("OwnerId")]
    [Display(Name = "ID Proprietário")]
    public int? OwnerId { get; set; }

    // Navegação (opcional - descomentar se houver entidades relacionadas)
    // [ForeignKey(nameof(ParentId))]
    // public virtual TestEntity? Parent { get; set; }
    // public virtual ICollection<TestEntity>? Children { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // 👤 CAMPOS DE AUDITORIA
    // ═══════════════════════════════════════════════════════════════

    [Required]
    [Column("CreatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Criado em")]
    public DateTime CreatedAtUtc { get; set; }

    [Column("CreatedBy", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Criado por")]
    public string? CreatedBy { get; set; }

    [Column("CreatedByUserId")]
    [Display(Name = "ID Usuário Criador")]
    public int? CreatedByUserId { get; set; }

    [Column("UpdatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Atualizado em")]
    public DateTime? UpdatedAtUtc { get; set; }

    [Column("UpdatedBy", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Atualizado por")]
    public string? UpdatedBy { get; set; }

    [Column("UpdatedByUserId")]
    [Display(Name = "ID Usuário Atualizador")]
    public int? UpdatedByUserId { get; set; }

    [Column("DeletedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Excluído em")]
    public DateTime? DeletedAtUtc { get; set; }

    [Column("DeletedBy", TypeName = "nvarchar(100)")]
    [StringLength(100)]
    [Display(Name = "Excluído por")]
    public string? DeletedBy { get; set; }

    [Column("DeletedByUserId")]
    [Display(Name = "ID Usuário que Excluiu")]
    public int? DeletedByUserId { get; set; }

    [Column("IsDeleted", TypeName = "bit")]
    [Display(Name = "Está Excluído")]
    public bool IsDeleted { get; set; }

    // ═══════════════════════════════════════════════════════════════
    // 🧮 CAMPOS CALCULADOS / NOT MAPPED
    // ═══════════════════════════════════════════════════════════════

    [NotMapped]
    [Display(Name = "Nome Completo")]
    public string NomeCompleto => $"{Codigo} - {Nome}";

    [NotMapped]
    [Display(Name = "Dias desde Criação")]
    public int DiasDesdeCriacao => (DateTime.UtcNow - CreatedAtUtc).Days;

    [NotMapped]
    [Display(Name = "É Recente")]
    public bool EhRecente => DiasDesdeCriacao <= 30;

    [NotMapped]
    [Display(Name = "Status Descrição")]
    public string StatusDescricao => Status switch
    {
        TestEntityStatus.Ativo => "Ativo",
        TestEntityStatus.Inativo => "Inativo",
        TestEntityStatus.Pendente => "Pendente",
        TestEntityStatus.Cancelado => "Cancelado",
        _ => "Desconhecido"
    };
}

// ═══════════════════════════════════════════════════════════════
// 📌 ENUMS
// ═══════════════════════════════════════════════════════════════

public enum TestEntityStatus
{
    Ativo = 1,
    Inativo = 2,
    Pendente = 3,
    Cancelado = 4
}

public enum TestEntityPrioridade
{
    Baixa = 1,
    Media = 2,
    Alta = 3,
    Urgente = 4
}

// ═══════════════════════════════════════════════════════════════
// 📌 CLASSES AUXILIARES (JSON)
// ═══════════════════════════════════════════════════════════════

public class TestJsonSettings
{
    public string? Theme { get; set; }
    public int TimeoutMinutes { get; set; }
    public List<string>? Features { get; set; }
    public Dictionary<string, bool>? Flags { get; set; }
    public TestNestedSettings? Advanced { get; set; }
}

public class TestNestedSettings
{
    public bool EnableCache { get; set; }
    public int MaxRetries { get; set; }
    public string? ApiKey { get; set; }
}