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
    public string? CodSexo { get; set; } // M/F

    [Column("cod_sangue")]
    [StringLength(2)]
    public string? CodSangue { get; set; } // A+, B-, O+, etc.

    [Column("cod_raca")]
    [StringLength(2)]
    public string? CodRaca { get; set; }

    [Column("num_cpf")]
    [StringLength(11)]
    public string? NumCpf { get; set; }

    [Column("estado_civil")]
    [StringLength(1)]
    public string? EstadoCivil { get; set; } // S/C/D/V/U

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
    public string ChAtivo { get; set; } = "S"; // S/N

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

    [Column("salario_atual")]
    public decimal? SalarioAtual { get; set; }

    [Column("observacoes")]
    [StringLength(2000)]
    public string? Observacoes { get; set; }

    [Column("data_ultima_consulta")]
    public DateTime? DataUltimaConsulta { get; set; }

    [Column("status_medico")]
    [StringLength(20)]
    public string? StatusMedico { get; set; } // APTO, INAPTO, RESTRITO

    [Column("restricoes_medicas")]
    [StringLength(1000)]
    public string? RestricoesMedicas { get; set; }

    [Column("data_proxima_consulta")]
    public DateTime? DataProximaConsulta { get; set; }

    [Column("tipo_periodicidade")]
    [StringLength(20)]
    public string? TipoPeriodicidade { get; set; } // ANUAL, SEMESTRAL, BIENAL

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

    [Column("foto_empregado")]
    [StringLength(500)]
    public string? FotoEmpregado { get; set; }

    [Column("emergencia_contato")]
    [StringLength(100)]
    public string? EmergenciaContato { get; set; }

    [Column("emergencia_telefone")]
    [StringLength(20)]
    public string? EmergenciaTelefone { get; set; }

    [Column("emergencia_parentesco")]
    [StringLength(30)]
    public string? EmergenciaParentesco { get; set; }

    // Relacionamentos
    [ForeignKey(nameof(CodEmpresa))]
    public virtual MsoEmpresa? Empresa { get; set; }

    [ForeignKey(nameof(CodFuncao))]
    public virtual MsoFuncao? Funcao { get; set; }

    [ForeignKey(nameof(CodGrupo))]
    public virtual MsoGrupo? Grupo { get; set; }

    public virtual ICollection<MsoAgenda> Agendas { get; set; } = new List<MsoAgenda>();
    public virtual ICollection<MsoAtendimento> Atendimentos { get; set; } = new List<MsoAtendimento>();
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
    public bool EstaAfastado => Afastamentos?.Any(a => a.DataRetorno == null || a.DataRetorno > DateTime.Today) == true;

    [NotMapped]
    public bool TemRestricoes => !string.IsNullOrWhiteSpace(RestricoesMedicas);

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
    public int? TempoEmpresa
    {
        get
        {
            if (!DataAdmissao.HasValue) return null;
            var dataFim = DataDemissao ?? DateTime.Today;
            var anos = dataFim.Year - DataAdmissao.Value.Year;
            if (DataAdmissao.Value.Date > dataFim.AddYears(-anos)) anos--;
            return anos;
        }
    }

    [NotMapped]
    public string TempoEmpresaFormatado
    {
        get
        {
            if (!TempoEmpresa.HasValue) return "Não informado";
            var tempo = TempoEmpresa.Value;
            return tempo switch
            {
                0 => "Menos de 1 ano",
                1 => "1 ano",
                _ => $"{tempo} anos"
            };
        }
    }

    [NotMapped]
    public string SexoDescricao => CodSexo switch
    {
        "M" => "Masculino",
        "F" => "Feminino",
        _ => "Não informado"
    };

    [NotMapped]
    public string EstadoCivilDescricao => EstadoCivil switch
    {
        "S" => "Solteiro(a)",
        "C" => "Casado(a)",
        "D" => "Divorciado(a)",
        "V" => "Viúvo(a)",
        "U" => "União Estável",
        _ => "Não informado"
    };

    [NotMapped]
    public string StatusMedicoDescricao => StatusMedico switch
    {
        "APTO" => "Apto",
        "INAPTO" => "Inapto",
        "RESTRITO" => "Restrito",
        "PENDENTE" => "Pendente Avaliação",
        _ => "Não avaliado"
    };

    [NotMapped]
    public string StatusDescricao
    {
        get
        {
            if (!EstaAtivo) return "Inativo";
            if (DataDemissao.HasValue) return "Demitido";
            if (EstaAfastado) return "Afastado";
            if (StatusMedico == "INAPTO") return "Inapto";
            if (StatusMedico == "RESTRITO") return "Restrito";
            return "Ativo";
        }
    }

    [NotMapped]
    public bool PrecisaConsultaPeridodica
    {
        get
        {
            if (!DataProximaConsulta.HasValue) return false;
            return DataProximaConsulta.Value.Date <= DateTime.Today.AddDays(30);
        }
    }

    [NotMapped]
    public bool ConsultaVencida
    {
        get
        {
            if (!DataProximaConsulta.HasValue) return false;
            return DataProximaConsulta.Value.Date < DateTime.Today;
        }
    }

    [NotMapped]
    public int DiasParaConsulta
    {
        get
        {
            if (!DataProximaConsulta.HasValue) return int.MaxValue;
            return (DataProximaConsulta.Value.Date - DateTime.Today).Days;
        }
    }

    [NotMapped]
    public string EnderecoCompleto
    {
        get
        {
            var endereco = new List<string>();

            if (!string.IsNullOrWhiteSpace(DescEndereco))
                endereco.Add(DescEndereco);

            if (!string.IsNullOrWhiteSpace(EndNumero))
                endereco.Add($"nº {EndNumero}");

            if (!string.IsNullOrWhiteSpace(DescComplemento))
                endereco.Add(DescComplemento);

            if (!string.IsNullOrWhiteSpace(DescBairro))
                endereco.Add(DescBairro);

            var cidadeUf = new List<string>();
            if (!string.IsNullOrWhiteSpace(CodMunicipio))
                cidadeUf.Add(CodMunicipio);
            if (!string.IsNullOrWhiteSpace(SiglaUf))
                cidadeUf.Add(SiglaUf);

            if (cidadeUf.Any())
                endereco.Add(string.Join("/", cidadeUf));

            if (!string.IsNullOrWhiteSpace(NumeroCep))
                endereco.Add($"CEP: {NumeroCep}");

            return string.Join(", ", endereco);
        }
    }

    [NotMapped]
    public string TelefoneContato
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(NumTelefone1))
                return NumTelefone1;
            if (!string.IsNullOrWhiteSpace(NumTelefone2))
                return NumTelefone2;
            return "Não informado";
        }
    }

    [NotMapped]
    public bool TemFoto => !string.IsNullOrWhiteSpace(FotoEmpregado);

    [NotMapped]
    public bool TemContatoEmergencia => !string.IsNullOrWhiteSpace(EmergenciaContato) && !string.IsNullOrWhiteSpace(EmergenciaTelefone);

    [NotMapped]
    public string ContatoEmergenciaCompleto
    {
        get
        {
            if (!TemContatoEmergencia) return "Não informado";

            var contato = EmergenciaContato;
            if (!string.IsNullOrWhiteSpace(EmergenciaParentesco))
                contato += $" ({EmergenciaParentesco})";
            contato += $" - {EmergenciaTelefone}";

            return contato;
        }
    }

    // Métodos de negócio
    public void AtivarEmpregado(string usuario)
    {
        if (DataDemissao.HasValue)
            throw new InvalidOperationException("Não é possível ativar empregado demitido.");

        ChAtivo = "S";
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void InativarEmpregado(string usuario, string motivo = null)
    {
        ChAtivo = "N";

        if (!string.IsNullOrEmpty(motivo))
            Observacoes = $"{Observacoes}\n{DateTime.Now:dd/MM/yyyy}: Inativado - {motivo}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void RegistrarDemissao(DateTime dataDemissao, string usuario, string motivo = null)
    {
        DataDemissao = dataDemissao;
        ChAtivo = "N";

        if (!string.IsNullOrEmpty(motivo))
            Observacoes = $"{Observacoes}\n{DateTime.Now:dd/MM/yyyy}: Demitido - {motivo}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AtualizarStatusMedico(string novoStatus, string usuario, string restricoes = null)
    {
        var statusAnterior = StatusMedico;
        StatusMedico = novoStatus.ToUpper();
        RestricoesMedicas = restricoes;

        Observacoes = $"{Observacoes}\n{DateTime.Now:dd/MM/yyyy}: Status médico alterado de '{statusAnterior}' para '{novoStatus}' por {usuario}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AgendarProximaConsulta(DateTime dataConsulta, string tipoPeriodicidade, string usuario)
    {
        DataProximaConsulta = dataConsulta;
        TipoPeriodicidade = tipoPeriodicidade;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void RegistrarUltimaConsulta(DateTime dataConsulta, string usuario)
    {
        DataUltimaConsulta = dataConsulta;

        // Calcular próxima consulta baseada na periodicidade
        if (!string.IsNullOrWhiteSpace(TipoPeriodicidade))
        {
            DataProximaConsulta = TipoPeriodicidade.ToUpper() switch
            {
                "ANUAL" => dataConsulta.AddYears(1),
                "SEMESTRAL" => dataConsulta.AddMonths(6),
                "BIENAL" => dataConsulta.AddYears(2),
                "TRIMESTRAL" => dataConsulta.AddMonths(3),
                _ => dataConsulta.AddYears(1)
            };
        }

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AtualizarContato(string telefone1, string telefone2, string email, string usuario)
    {
        NumTelefone1 = telefone1;
        NumTelefone2 = telefone2;
        DescEmail = email;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AtualizarEndereco(string endereco, string numero, string complemento, string bairro,
                                  string cep, string municipio, string uf, string usuario)
    {
        DescEndereco = endereco;
        EndNumero = numero;
        DescComplemento = complemento;
        DescBairro = bairro;
        NumeroCep = cep;
        CodMunicipio = municipio;
        SiglaUf = uf;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void DefinirContatoEmergencia(string nome, string telefone, string parentesco, string usuario)
    {
        EmergenciaContato = nome;
        EmergenciaTelefone = telefone;
        EmergenciaParentesco = parentesco;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AnexarFoto(string caminhoFoto, string usuario)
    {
        FotoEmpregado = caminhoFoto;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AlterarFuncao(int novoCodFuncao, string usuario, string motivo = null)
    {
        var funcaoAnterior = CodFuncao;
        CodFuncao = novoCodFuncao;

        if (!string.IsNullOrEmpty(motivo))
            Observacoes = $"{Observacoes}\n{DateTime.Now:dd/MM/yyyy}: Função alterada de {funcaoAnterior} para {novoCodFuncao} - {motivo}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AlterarSetor(string novoSetor, string usuario, string motivo = null)
    {
        var setorAnterior = DescSetor;
        DescSetor = novoSetor;

        if (!string.IsNullOrEmpty(motivo))
            Observacoes = $"{Observacoes}\n{DateTime.Now:dd/MM/yyyy}: Setor alterado de '{setorAnterior}' para '{novoSetor}' - {motivo}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    // Validações
    public bool IsCpfValido()
    {
        if (string.IsNullOrWhiteSpace(NumCpf)) return true;
        return NumCpf.Length == 11 && NumCpf.All(char.IsDigit);
    }

    public bool IsEmailValido()
    {
        if (string.IsNullOrWhiteSpace(DescEmail)) return true;
        return DescEmail.Contains("@") && DescEmail.Contains(".");
    }

    public bool IsDataNascimentoValida()
    {
        if (!DataNascimento.HasValue) return true;
        return DataNascimento.Value.Date <= DateTime.Today.AddYears(-14) &&
               DataNascimento.Value.Date >= DateTime.Today.AddYears(-120);
    }

    public bool IsDataAdmissaoValida()
    {
        if (!DataAdmissao.HasValue) return true;
        if (DataNascimento.HasValue && DataAdmissao.Value.Date < DataNascimento.Value.AddYears(14))
            return false;
        return DataAdmissao.Value.Date <= DateTime.Today;
    }

    public bool IsDataDemissaoValida()
    {
        if (!DataDemissao.HasValue) return true;
        if (DataAdmissao.HasValue && DataDemissao.Value.Date < DataAdmissao.Value.Date)
            return false;
        return DataDemissao.Value.Date <= DateTime.Today;
    }

    public bool TemDadosObrigatorios()
    {
        return !string.IsNullOrWhiteSpace(NomeEmpregado) &&
               CodEmpresa > 0 &&
               CodFuncao > 0;
    }

    public bool TemDadosCompletos()
    {
        return TemDadosObrigatorios() &&
               DataNascimento.HasValue &&
               !string.IsNullOrWhiteSpace(CodSexo) &&
               !string.IsNullOrWhiteSpace(NumCpf) &&
               DataAdmissao.HasValue;
    }

    // Métodos de relatório
    public Dictionary<string, object> ToResumoEmpregado()
    {
        return new Dictionary<string, object>
        {
            ["Codigo"] = CodEmpregado,
            ["Nome"] = NomeEmpregado,
            ["Matricula"] = CodMatricula ?? "Não informado",
            ["CPF"] = NumCpf ?? "Não informado",
            ["Idade"] = IdadeFormatada,
            ["Sexo"] = SexoDescricao,
            ["Estado_Civil"] = EstadoCivilDescricao,
            ["Data_Nascimento"] = DataNascimento?.ToString("dd/MM/yyyy") ?? "Não informado",
            ["Data_Admissao"] = DataAdmissao?.ToString("dd/MM/yyyy") ?? "Não informado",
            ["Data_Demissao"] = DataDemissao?.ToString("dd/MM/yyyy") ?? "Não demitido",
            ["Tempo_Empresa"] = TempoEmpresaFormatado,
            ["Empresa"] = Empresa?.RzsocialEmpresa ?? "Não informado",
            ["Funcao"] = Funcao?.DescFuncao ?? "Não informado",
            ["Setor"] = DescSetor ?? "Não informado",
            ["Status"] = StatusDescricao,
            ["Status_Medico"] = StatusMedicoDescricao,
            ["Restricoes"] = TemRestricoes ? "Sim" : "Não",
            ["Ultima_Consulta"] = DataUltimaConsulta?.ToString("dd/MM/yyyy") ?? "Nunca",
            ["Proxima_Consulta"] = DataProximaConsulta?.ToString("dd/MM/yyyy") ?? "Não agendada",
            ["Dias_Para_Consulta"] = DiasParaConsulta == int.MaxValue ? "N/A" : DiasParaConsulta.ToString(),
            ["Telefone"] = TelefoneContato,
            ["Email"] = DescEmail ?? "Não informado",
            ["Endereco"] = EnderecoCompleto,
            ["Contato_Emergencia"] = ContatoEmergenciaCompleto
        };
    }

    public override string ToString()
    {
        return $"{CodEmpregado} - {NomeEmpregado} ({StatusDescricao})";
    }
}