// Src/Core/MSO/Entities/MsoConsultaGeral.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_consulta_geral")]
public class MsoConsultaGeral
{
    [Key]
    [Column("cod_consulta")]
    public int CodConsulta { get; set; }

    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("data_consulta")]
    public DateTime DataConsulta { get; set; }

    [Column("cod_profsaude")]
    public int CodProfSaude { get; set; }

    [Column("cod_tpconsulta")]
    public int? CodTpConsulta { get; set; }

    [Column("ch_nutricional")]
    [StringLength(1)]
    public string? ChNutricional { get; set; }

    [Column("ch_facies")]
    [StringLength(1)]
    public string? ChFacies { get; set; }

    [Column("ch_atitude")]
    [StringLength(1)]
    public string? ChAtitude { get; set; }

    [Column("ch_pele")]
    [StringLength(1)]
    public string? ChPele { get; set; }

    [Column("ch_mucosa")]
    [StringLength(1)]
    public string? ChMucosa { get; set; }

    [Column("ch_ganglios")]
    [StringLength(1)]
    public string? ChGanglios { get; set; }

    [Column("desc_obs")]
    [StringLength(255)]
    public string? DescObs { get; set; }

    [Column("status_consulta")]
    [StringLength(20)]
    public string StatusConsulta { get; set; } = "AGENDADA";

    [Column("tipo_consulta")]
    [StringLength(30)]
    public string? TipoConsulta { get; set; }

    [Column("resultado_geral")]
    [StringLength(50)]
    public string? ResultadoGeral { get; set; }

    [Column("apto_trabalho")]
    [StringLength(1)]
    public string? AptoTrabalho { get; set; }

    [Column("restricoes")]
    [StringLength(1000)]
    public string? Restricoes { get; set; }

    [Column("data_criacao")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    [Column("data_atualizacao")]
    public DateTime? DataAtualizacao { get; set; }

    [Column("usuario_criacao")]
    [StringLength(50)]
    public string? UsuarioCriacao { get; set; }

    [Column("usuario_atualizacao")]
    [StringLength(50)]
    public string? UsuarioAtualizacao { get; set; }

    [Column("ativo")]
    public bool Ativo { get; set; } = true;

    // Relacionamentos
    [ForeignKey(nameof(CodEmpregado))]
    public virtual MsoEmpregado? Empregado { get; set; }

    [ForeignKey(nameof(CodProfSaude))]
    public virtual MsoProfsaude? ProfSaude { get; set; }

    [ForeignKey(nameof(CodTpConsulta))]
    public virtual MsoTpconsulta? TpConsulta { get; set; }

    public virtual ICollection<MsoConsultaExame> ConsultaExames { get; set; } = new List<MsoConsultaExame>();
    public virtual ICollection<MsoConsultaBiometrico> ConsultaBiometricos { get; set; } = new List<MsoConsultaBiometrico>();
    public virtual ICollection<MsoConsultaHistorico> ConsultaHistoricos { get; set; } = new List<MsoConsultaHistorico>();
    public virtual ICollection<MsoConsultaHisPessoal> ConsultaHisPessoais { get; set; } = new List<MsoConsultaHisPessoal>();
    public virtual ICollection<MsoConsultaHmFamilia> ConsultaHmFamilias { get; set; } = new List<MsoConsultaHmFamilia>();
    public virtual ICollection<MsoConsultaRisco> ConsultaRiscos { get; set; } = new List<MsoConsultaRisco>();

    // Propriedades calculadas
    [NotMapped]
    public string StatusDescricao => StatusConsulta switch
    {
        "AGENDADA" => "Agendada",
        "EM_ANDAMENTO" => "Em Andamento",
        "CONCLUIDA" => "Concluída",
        "CANCELADA" => "Cancelada",
        _ => "Indefinido"
    };

    [NotMapped]
    public string AptoTrabalhoDescricao => AptoTrabalho switch
    {
        "S" => "Apto",
        "N" => "Inapto",
        "R" => "Restrito",
        _ => "Não avaliado"
    };

    [NotMapped]
    public bool TemRestricoes => !string.IsNullOrWhiteSpace(Restricoes);

    [NotMapped]
    public bool ConsultaConcluida => StatusConsulta == "CONCLUIDA";

    [NotMapped]
    public bool PodeEditar => StatusConsulta != "CONCLUIDA" && StatusConsulta != "CANCELADA";

    // Métodos de negócio
    public void IniciarConsulta(string usuario)
    {
        if (StatusConsulta != "AGENDADA")
            throw new InvalidOperationException("Só é possível iniciar consultas agendadas.");

        StatusConsulta = "EM_ANDAMENTO";
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void ConcluirConsulta(string resultadoGeral, string aptoTrabalho, string usuario, string restricoes = null)
    {
        if (StatusConsulta != "EM_ANDAMENTO")
            throw new InvalidOperationException("Só é possível concluir consultas em andamento.");

        StatusConsulta = "CONCLUIDA";
        ResultadoGeral = resultadoGeral;
        AptoTrabalho = aptoTrabalho;
        Restricoes = restricoes;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void CancelarConsulta(string usuario, string motivo = null)
    {
        if (StatusConsulta == "CONCLUIDA")
            throw new InvalidOperationException("Não é possível cancelar consultas concluídas.");

        StatusConsulta = "CANCELADA";

        if (!string.IsNullOrEmpty(motivo))
            DescObs = $"{DescObs}\nCancelada: {motivo}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AtualizarExameFisico(string nutricional, string facies, string atitude,
                                   string pele, string mucosa, string ganglios, string usuario)
    {
        ChNutricional = nutricional;
        ChFacies = facies;
        ChAtitude = atitude;
        ChPele = pele;
        ChMucosa = mucosa;
        ChGanglios = ganglios;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public override string ToString()
    {
        return $"Consulta {CodConsulta} - {DataConsulta:dd/MM/yyyy} - {StatusDescricao}";
    }
}