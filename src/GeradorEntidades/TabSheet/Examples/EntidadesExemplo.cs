// =============================================================================
// EXEMPLO: ENTIDADES GERADAS PELO TABSHEET GENERATOR
// 
// Este arquivo demonstra como as entidades serão geradas para um relacionamento
// mestre/detalhe com tabsheets.
// 
// Cenário: Agenda de Eventos com Convidados e Lembretes
// - Mestre: agenda_notes (PK: nomatric)
// - Detalhe 1: agenda_rh_convidado (FK: nomatric)
// - Detalhe 2: agenda_rh_lembrete (FK: nomatric)
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GeradorEntidades.TabSheet.Attributes;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

#region Entidade MESTRE

/// <summary>
/// Agenda de eventos/notas.
/// </summary>
/// <remarks>
/// Tabela legada: agenda_notes
/// Relacionamentos:
/// - 1:N → AgendaRhConvidado (Convidados)
/// - 1:N → AgendaRhLembrete (Lembretes)
/// </remarks>
[MasterEntity("Evento")]
[Table("agenda_notes")]
public class AgendaNotes
{
    #region Chave Primária

    /// <summary>
    /// Código do evento (PK).
    /// </summary>
    [Key]
    [Column("nomatric")]
    [FieldDisplayName("Código")]
    public int NoMatric { get; set; }

    #endregion

    #region Propriedades

    /// <summary>
    /// Código do projeto relacionado.
    /// </summary>
    [Column("cdprojeto")]
    [FieldDisplayName("Projeto")]
    public int? CdProjeto { get; set; }

    /// <summary>
    /// Código da turma.
    /// </summary>
    [Column("cdturma")]
    [FieldDisplayName("Turma")]
    public int? CdTurma { get; set; }

    /// <summary>
    /// Descrição/título do evento.
    /// </summary>
    [Required]
    [Column("dcevento")]
    [StringLength(255)]
    [FieldDisplayName("Evento")]
    public string DcEvento { get; set; } = string.Empty;

    /// <summary>
    /// E-mail de contato.
    /// </summary>
    [Column("dcemail")]
    [StringLength(100)]
    [FieldDisplayName("E-mail")]
    public string? DcEmail { get; set; }

    /// <summary>
    /// Data/hora de início.
    /// </summary>
    [Required]
    [Column("dtinicio")]
    [FieldDisplayName("Início")]
    public DateTime DtInicio { get; set; }

    /// <summary>
    /// Data/hora de término.
    /// </summary>
    [Column("dtfinal")]
    [FieldDisplayName("Término")]
    public DateTime? DtFinal { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Lista de convidados do evento.
    /// </summary>
    [DetailCollection(typeof(AgendaRhConvidado), "nomatric", 
        TabTitle = "Convidados", 
        TabIcon = "fas fa-users",
        TabOrder = 1)]
    public virtual ICollection<AgendaRhConvidado> Convidados { get; set; } = new List<AgendaRhConvidado>();

    /// <summary>
    /// Lista de lembretes do evento.
    /// </summary>
    [DetailCollection(typeof(AgendaRhLembrete), "nomatric", 
        TabTitle = "Lembretes", 
        TabIcon = "fas fa-bell",
        TabOrder = 2)]
    public virtual ICollection<AgendaRhLembrete> Lembretes { get; set; } = new List<AgendaRhLembrete>();

    #endregion
}

#endregion

#region Entidade DETALHE 1 - Convidados

/// <summary>
/// Convidado de um evento da agenda.
/// </summary>
/// <remarks>
/// Tabela legada: agenda_rh_convidado
/// FK: nomatric → agenda_notes.nomatric
/// </remarks>
[DetailEntity(typeof(AgendaNotes), "nomatric", 
    DisplayName = "Convidado",
    OnDelete = DeleteBehavior.Cascade)]
[Table("agenda_rh_convidado")]
public class AgendaRhConvidado
{
    #region Chave Primária

    /// <summary>
    /// ID do registro (PK).
    /// </summary>
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [FieldDisplayName("ID")]
    public int Id { get; set; }

    #endregion

    #region Chave Estrangeira

    /// <summary>
    /// Código do evento (FK).
    /// </summary>
    [Required]
    [Column("nomatric")]
    [FieldDisplayName("Evento")]
    public int NoMatric { get; set; }

    #endregion

    #region Propriedades

    /// <summary>
    /// Nome do convidado.
    /// </summary>
    [Required]
    [Column("nome")]
    [StringLength(200)]
    [FieldDisplayName("Nome")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// E-mail do convidado.
    /// </summary>
    [Column("email")]
    [StringLength(100)]
    [FieldDisplayName("E-mail")]
    public string? Email { get; set; }

    /// <summary>
    /// Telefone do convidado.
    /// </summary>
    [Column("telefone")]
    [StringLength(20)]
    [FieldDisplayName("Telefone")]
    public string? Telefone { get; set; }

    /// <summary>
    /// Se confirmou presença.
    /// </summary>
    [Column("confirmado")]
    [FieldDisplayName("Confirmado")]
    public bool Confirmado { get; set; } = false;

    /// <summary>
    /// Data da confirmação.
    /// </summary>
    [Column("data_confirmacao")]
    [FieldDisplayName("Data Confirmação")]
    public DateTime? DataConfirmacao { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Evento relacionado.
    /// </summary>
    [MasterReference("NoMatric")]
    [ForeignKey("NoMatric")]
    public virtual AgendaNotes AgendaNotes { get; set; } = null!;

    #endregion
}

#endregion

#region Entidade DETALHE 2 - Lembretes

/// <summary>
/// Lembrete de um evento da agenda.
/// </summary>
/// <remarks>
/// Tabela legada: agenda_rh_lembrete
/// FK: nomatric → agenda_notes.nomatric
/// </remarks>
[DetailEntity(typeof(AgendaNotes), "nomatric",
    DisplayName = "Lembrete",
    OnDelete = DeleteBehavior.Cascade)]
[Table("agenda_rh_lembrete")]
public class AgendaRhLembrete
{
    #region Chave Primária

    /// <summary>
    /// ID do registro (PK).
    /// </summary>
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [FieldDisplayName("ID")]
    public int Id { get; set; }

    #endregion

    #region Chave Estrangeira

    /// <summary>
    /// Código do evento (FK).
    /// </summary>
    [Required]
    [Column("nomatric")]
    [FieldDisplayName("Evento")]
    public int NoMatric { get; set; }

    #endregion

    #region Propriedades

    /// <summary>
    /// Descrição do lembrete.
    /// </summary>
    [Required]
    [Column("descricao")]
    [StringLength(500)]
    [FieldDisplayName("Descrição")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Data/hora do lembrete.
    /// </summary>
    [Required]
    [Column("data_lembrete")]
    [FieldDisplayName("Data")]
    public DateTime DataLembrete { get; set; }

    /// <summary>
    /// Tipo do lembrete.
    /// </summary>
    [Column("tipo")]
    [StringLength(50)]
    [FieldDisplayName("Tipo")]
    public string? Tipo { get; set; }

    /// <summary>
    /// Se o lembrete foi enviado.
    /// </summary>
    [Column("enviado")]
    [FieldDisplayName("Enviado")]
    public bool Enviado { get; set; } = false;

    /// <summary>
    /// Data/hora do envio.
    /// </summary>
    [Column("data_envio")]
    [FieldDisplayName("Data Envio")]
    public DateTime? DataEnvio { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Evento relacionado.
    /// </summary>
    [MasterReference("NoMatric")]
    [ForeignKey("NoMatric")]
    public virtual AgendaNotes AgendaNotes { get; set; } = null!;

    #endregion
}

#endregion

#region Atributo Auxiliar (já existente no projeto)

/// <summary>
/// Atributo para nome de exibição de campo.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FieldDisplayNameAttribute : Attribute
{
    public string Name { get; }
    
    public FieldDisplayNameAttribute(string name)
    {
        Name = name;
    }
}

#endregion
