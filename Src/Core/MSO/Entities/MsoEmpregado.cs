// Src/Core/MSO/Entities/MsoEmpregado.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_empregado")]
public class MsoEmpregado
{
    [Key]
    [Column("cod_empregado")]
    public int CodEmpregado { get; set; }

    [Column("cod_tpempregado")]
    public int CodTpEmpregado { get; set; }

    [Column("nome_empregado")]
    [StringLength(60)]
    [Required]
    public string NomeEmpregado { get; set; } = string.Empty;

    [Column("cod_funcao")]
    public int CodFuncao { get; set; }

    [Column("cod_empresa")]
    public int CodEmpresa { get; set; }

    [Column("cdcolab")]
    [StringLength(6)]
    public string? CdColab { get; set; }

    [Column("data_nascimento")]
    public DateTime? DataNascimento { get; set; }

    [Column("local_nascimento")]
    [StringLength(60)]
    public string? LocalNascimento { get; set; }

    [Column("cod_sexo")]
    [StringLength(1)]
    public string? CodSexo { get; set; }

    [Column("cod_sangue")]
    [StringLength(2)]
    public string? CodSangue { get; set; }

    [Column("cod_raca")]
    [StringLength(2)]
    public string? CodRaca { get; set; }

    [Column("num_cpf")]
    [StringLength(11)]
    public string? NumCpf { get; set; }

    [Column("estado_civil")]
    [StringLength(1)]
    public string? EstadoCivil { get; set; }

    [Column("desc_endereco")]
    [StringLength(60)]
    public string? DescEndereco { get; set; }

    [Column("desc_compl")]
    [StringLength(60)]
    public string? DescComplemento { get; set; }

    [Column("cod_municipio")]
    [StringLength(5)]
    public string? CodMunicipio { get; set; }

    [Column("sigla_uf")]
    [StringLength(2)]
    public string? SiglaUf { get; set; }

    [Column("numero_cep")]
    [StringLength(8)]
    public string? NumeroCep { get; set; }

    [Column("num_telefone1")]
    [StringLength(30)]
    public string? NumTelefone1 { get; set; }

    [Column("num_telefone2")]
    [StringLength(15)]
    public string? NumTelefone2 { get; set; }

    [Column("desc_setor")]
    [StringLength(100)]
    public string? DescSetor { get; set; }

    [Column("ch_ativo")]
    [StringLength(1)]
    public string ChAtivo { get; set; } = "S";

    [Column("desc_email")]
    [StringLength(80)]
    public string? DescEmail { get; set; }

    [Column("numero_rg")]
    [StringLength(15)]
    public string? NumeroRg { get; set; }

    [Column("serie_rg")]
    [StringLength(20)]
    public string? SerieRg { get; set; }

    [Column("cod_matricula")]
    [StringLength(8)]
    public string? CodMatricula { get; set; }

    [Column("cod_tpjornada")]
    [StringLength(1)]
    public string? CodTpJornada { get; set; }

    [Column("desc_bairro")]
    [StringLength(40)]
    public string? DescBairro { get; set; }

    [Column("cod_nacionalidade")]
    [StringLength(2)]
    public string? CodNacionalidade { get; set; }

    [Column("cod_ccusto")]
    [StringLength(5)]
    public string? CodCCusto { get; set; }

    [Column("cod_instrucao")]
    [StringLength(2)]
    public string? CodInstrucao { get; set; }

    [Column("cod_convocacao")]
    public int? CodConvocacao { get; set; }

    [Column("cod_grupo")]
    public int? CodGrupo { get; set; }

    [Column("dtadmissao")]
    public DateTime? DataAdmissao { get; set; }

    [Column("nopis")]
    [StringLength(11)]
    public string? NumeroPis { get; set; }

    [Column("codcateg")]
    public int? CodCategoria { get; set; }

    [Column("cod_filial")]
    public int? CodFilial { get; set; }

    [Column("dtdemissao")]
    public DateTime? DataDemissao { get; set; }

    [Column("cod_empregadoSesi")]
    public int? CodEmpregadoSesi { get; set; }

    [Column("nmpaicolab")]
    [StringLength(60)]
    public string? NomePai { get; set; }

    [Column("nmmaecolab")]
    [StringLength(60)]
    public string? NomeMae { get; set; }

    [Column("end_numero")]
    [StringLength(10)]
    public string? EndNumero { get; set; }

    [Column("cod_cdcarreira")]
    public int? CodCarreira { get; set; }

    [Column("desc_localtrab")]
    [StringLength(60)]
    public string? DescLocalTrabalho { get; set; }

    [Column("idcodltrab")]
    public decimal? IdCodLocalTrabalho { get; set; }

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

    // Relacionamentos
    [ForeignKey(nameof(CodEmpresa))]
    public virtual MsoEmpresa? Empresa { get; set; }

    [ForeignKey(nameof(CodFuncao))]
    public virtual MsoFuncao? Funcao { get; set; }

    [ForeignKey(nameof(CodGrupo))]
    public virtual MsoGrupo? Grupo { get; set; }

    public virtual ICollection<MsoAgenda> Agendas { get; set; } = new List<MsoAgenda>();
    public virtual ICollection<MsoAtendimento> Atendimentos { get; set; } = new List<MsoAtendimento>();
    public virtual ICollection<MsoConsultaGeral> ConsultasGerais { get; set; } = new List<MsoConsultaGeral>();
    public virtual ICollection<MsoConsultaExame> ConsultaExames { get; set; } = new List<MsoConsultaExame>();
    public virtual ICollection<MsoSolicitacao> Solicitacoes { get; set; } = new List<MsoSolicitacao>();
    public virtual ICollection<MsoAvaliacao> Avaliacoes { get; set; } = new List<MsoAvaliacao>();
    public virtual ICollection<MsoAfastamento> Afastamentos { get; set; } = new List<MsoAfastamento>();
    public virtual ICollection<MsoConduta> Condutas { get; set; } = new List<MsoConduta>();
    public virtual ICollection<MsoNotas> Notas { get; set; } = new List<MsoNotas>();
    public virtual ICollection<MsoGrupoEmpregado> GrupoEmpregados { get; set; } = new List<MsoGrupoEmpregado>();
    public virtual ICollection<MsoExameEmpregado> ExameEmpregados { get; set; } = new List<MsoExameEmpregado>();
    public virtual ICollection<MsoResultadoExame> ResultadosExame { get; set; } = new List<MsoResultadoExame>();
    public virtual ICollection<MsoSeriesAudiometricas> SeriesAudiometricas { get; set; } = new List<MsoSeriesAudiometricas>();
    public virtual ICollection<MsoSeriesHematologicas> SeriesHematologicas { get; set; } = new List<MsoSeriesHematologicas>();
    public virtual ICollection<MsoSeriesHistoricas> SeriesHistoricas { get; set; } = new List<MsoSeriesHistoricas>();
    public virtual ICollection<MsoExameExtra1> ExamesExtra1 { get; set; } = new List<MsoExameExtra1>();
    public virtual ICollection<MsoExameExtra2> ExamesExtra2 { get; set; } = new List<MsoExameExtra2>();

    // Propriedades calculadas
    [NotMapped]
    public bool EstaAtivo => ChAtivo == "S";

    [NotMapped]
    public int? Idade
    {
        get
        {
            if (!DataNascimento.HasValue) return null;
            var hoje = DateTime.Today;
            var idade = hoje.Year - DataNascimento.Value.Year;
            if (DataNascimento.Value.Date > hoje.AddYears(-idade)) idade--;
            return idade;
        }
    }

    [NotMapped]
    public string IdadeFormatada => Idade?.ToString() ?? "Não informado";

    [NotMapped]
    public string SexoDescricao => CodSexo switch
    {
        "M" => "Masculino",
        "F" => "Feminino",
        _ => "Não informado"
    };

    [NotMapped]
    public string StatusDescricao
    {
        get
        {
            if (!EstaAtivo) return "Inativo";
            if (DataDemissao.HasValue) return "Demitido";
            return "Ativo";
        }
    }

    // Métodos de negócio
    public void AtivarEmpregado(string usuario)
    {
        ChAtivo = "S";
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void InativarEmpregado(string usuario, string motivo = null)
    {
        ChAtivo = "N";
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void RegistrarDemissao(DateTime dataDemissao, string usuario)
    {
        DataDemissao = dataDemissao;
        ChAtivo = "N";
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public override string ToString()
    {
        return $"{CodEmpregado} - {NomeEmpregado} ({StatusDescricao})";
    }
}