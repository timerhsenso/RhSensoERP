// Src/Core/MSO/Entities/MsoProfsaude.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Core.MSO.Entities;

[Table("mso_profsaude")]
public class MsoProfsaude
{
    [Key]
    [Column("cod_profsaude")]
    public int CodProfSaude { get; set; }

    [Column("cod_especialidade")]
    public int? CodEspecialidade { get; set; }

    [Column("nome_profsaude")]
    [StringLength(100)]
    public string? NomeProfSaude { get; set; }

    [Column("end_profsaude")]
    [StringLength(200)]
    public string? EndProfSaude { get; set; }

    [Column("comp_end_profsaude")]
    [StringLength(100)]
    public string? CompEndProfSaude { get; set; }

    [Column("bai_profsaude")]
    [StringLength(20)]
    public string? BaiProfSaude { get; set; }

    [Column("muni_profsaude")]
    [StringLength(5)]
    public string? MuniProfSaude { get; set; }

    [Column("cep_profsaude")]
    [StringLength(18)]
    public string? CepProfSaude { get; set; }

    [Column("notel1_profsaude")]
    [StringLength(20)]
    public string? NoTel1ProfSaude { get; set; }

    [Column("notel2_profsaude")]
    [StringLength(20)]
    public string? NoTel2ProfSaude { get; set; }

    [Column("nocelular_profsaude")]
    [StringLength(20)]
    public string? NoCelularProfSaude { get; set; }

    [Column("uf_profsaude")]
    [StringLength(2)]
    public string? UfProfSaude { get; set; }

    [Column("num_registro")]
    [StringLength(10)]
    public string? NumRegistro { get; set; }

    [Column("cdusuario")]
    [StringLength(30)]
    public string? CdUsuario { get; set; }

    [Column("ch_ativo")]
    [StringLength(1)]
    public string ChAtivo { get; set; } = "S";

    [Column("flrespcmso")]
    [StringLength(1)]
    public string? FlRespCMSO { get; set; } = "N";

    [Column("numero_rqe")]
    [StringLength(20)]
    public string? NumeroRqe { get; set; }

    [Column("cpf_profsaude")]
    [StringLength(11)]
    public string? CpfProfSaude { get; set; }

    [Column("nis_profsaude")]
    [StringLength(11)]
    public string? NisProfSaude { get; set; }

    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("flativo")]
    [StringLength(1)]
    public string? FlAtivo { get; set; } = "S";

    [Column("flprioridadeatend")]
    public int? FlPrioridadeAtendimento { get; set; } = 1;

    [Column("email_profsaude")]
    [StringLength(100)]
    public string? EmailProfSaude { get; set; }

    [Column("data_nascimento")]
    public DateTime? DataNascimento { get; set; }

    [Column("sexo")]
    [StringLength(1)]
    public string? Sexo { get; set; }

    [Column("conselho_regional")]
    [StringLength(10)]
    public string? ConselhoRegional { get; set; } // CRM, CRF, CRO, etc.

    [Column("numero_conselho")]
    [StringLength(20)]
    public string? NumeroConselho { get; set; }

    [Column("uf_conselho")]
    [StringLength(2)]
    public string? UfConselho { get; set; }

    [Column("data_inscricao")]
    public DateTime? DataInscricao { get; set; }

    [Column("situacao_conselho")]
    [StringLength(20)]
    public string? SituacaoConselho { get; set; } // ATIVO, SUSPENSO, CANCELADO

    [Column("titulo_especialista")]
    [StringLength(200)]
    public string? TituloEspecialista { get; set; }

    [Column("instituicao_formacao")]
    [StringLength(200)]
    public string? InstituicaoFormacao { get; set; }

    [Column("ano_formacao")]
    public int? AnoFormacao { get; set; }

    [Column("observacoes")]
    [StringLength(2000)]
    public string? Observacoes { get; set; }

    [Column("valor_consulta")]
    public decimal? ValorConsulta { get; set; }

    [Column("tempo_consulta_minutos")]
    public int? TempoConsultaMinutos { get; set; } = 30;

    [Column("horario_inicio")]
    [StringLength(5)]
    public string? HorarioInicio { get; set; }

    [Column("horario_fim")]
    [StringLength(5)]
    public string? HorarioFim { get; set; }

    [Column("dias_atendimento")]
    [StringLength(50)]
    public string? DiasAtendimento { get; set; } // SEG,TER,QUA,QUI,SEX,SAB,DOM

    [Column("atende_emergencia")]
    public bool AtendeEmergencia { get; set; } = false;

    [Column("atende_domicilio")]
    public bool AtendeDomicilio { get; set; } = false;

    [Column("valor_domicilio")]
    public decimal? ValorDomicilio { get; set; }

    [Column("raio_atendimento_km")]
    public int? RaioAtendimentoKm { get; set; }

    [Column("banco")]
    [StringLength(10)]
    public string? Banco { get; set; }

    [Column("agencia")]
    [StringLength(10)]
    public string? Agencia { get; set; }

    [Column("conta_corrente")]
    [StringLength(20)]
    public string? ContaCorrente { get; set; }

    [Column("pix")]
    [StringLength(100)]
    public string? Pix { get; set; }

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

    [Column("foto_profissional")]
    [StringLength(500)]
    public string? FotoProfissional { get; set; }

    [Column("assinatura_digital")]
    [StringLength(500)]
    public string? AssinaturaDigital { get; set; }

    [Column("numero_contrato")]
    [StringLength(30)]
    public string? NumeroContrato { get; set; }

    [Column("data_inicio_contrato")]
    public DateTime? DataInicioContrato { get; set; }

    [Column("data_fim_contrato")]
    public DateTime? DataFimContrato { get; set; }

    [Column("tipo_contrato")]
    [StringLength(20)]
    public string? TipoContrato { get; set; } // CLT, PJ, COOPERADO, TERCEIRIZADO

    [Column("status_profissional")]
    [StringLength(20)]
    public string? StatusProfissional { get; set; } = "DISPONIVEL";

    [Column("data_ultima_atividade")]
    public DateTime? DataUltimaAtividade { get; set; }

    // Relacionamentos
    [ForeignKey(nameof(CodEspecialidade))]
    public virtual MsoEspecialidade? Especialidade { get; set; }

    public virtual ICollection<MsoAgenda> Agendas { get; set; } = new List<MsoAgenda>();
    public virtual ICollection<MsoAtendimento> Atendimentos { get; set; } = new List<MsoAtendimento>();
    public virtual ICollection<MsoSolicitacao> Solicitacoes { get; set; } = new List<MsoSolicitacao>();
    public virtual ICollection<MsoAvaliacao> Avaliacoes { get; set; } = new List<MsoAvaliacao>();
    public virtual ICollection<MsoConduta> Condutas { get; set; } = new List<MsoConduta>();
    public virtual ICollection<MsoAtendimentoItens> AtendimentoItensExecutados { get; set; } = new List<MsoAtendimentoItens>();
    public virtual ICollection<MsoResultadoExame> ResultadosExame { get; set; } = new List<MsoResultadoExame>();
    public virtual ICollection<MsoExameEmpregado> ExameEmpregados { get; set; } = new List<MsoExameEmpregado>();
    public virtual ICollection<MsoExameExtra1> ExamesExtra1 { get; set; } = new List<MsoExameExtra1>();
    public virtual ICollection<MsoExameExtra2> ExamesExtra2 { get; set; } = new List<MsoExameExtra2>();

    // Propriedades calculadas
    [NotMapped]
    public bool EstaAtivo => ChAtivo == "S" && FlAtivo == "S";

    [NotMapped]
    public bool EhResponsavelCMSO => FlRespCMSO == "S";

    [NotMapped]
    public bool TemRegistroConselho => !string.IsNullOrWhiteSpace(NumeroConselho) && !string.IsNullOrWhiteSpace(ConselhoRegional);

    [NotMapped]
    public bool ConselhoAtivo => SituacaoConselho == "ATIVO";

    [NotMapped]
    public bool ContratoVigente
    {
        get
        {
            if (!DataInicioContrato.HasValue) return true;

            var hoje = DateTime.Today;
            var inicioOk = DataInicioContrato.Value.Date <= hoje;
            var fimOk = !DataFimContrato.HasValue || DataFimContrato.Value.Date >= hoje;

            return inicioOk && fimOk;
        }
    }

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
    public int? TempoFormacao
    {
        get
        {
            if (!AnoFormacao.HasValue) return null;
            return DateTime.Now.Year - AnoFormacao.Value;
        }
    }

    [NotMapped]
    public string TempoFormacaoFormatado
    {
        get
        {
            if (!TempoFormacao.HasValue) return "Não informado";
            var tempo = TempoFormacao.Value;
            return tempo switch
            {
                0 => "Recém-formado",
                1 => "1 ano",
                _ => $"{tempo} anos"
            };
        }
    }

    [NotMapped]
    public string SexoDescricao => Sexo switch
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
            if (!ContratoVigente) return "Contrato Vencido";
            if (!ConselhoAtivo) return "Conselho Irregular";
            return StatusProfissional switch
            {
                "DISPONIVEL" => "Disponível",
                "OCUPADO" => "Ocupado",
                "LICENCA" => "Em Licença",
                "FERIAS" => "Em Férias",
                "AFASTADO" => "Afastado",
                _ => "Indefinido"
            };
        }
    }

    [NotMapped]
    public string EnderecoCompleto
    {
        get
        {
            var endereco = new List<string>();

            if (!string.IsNullOrWhiteSpace(EndProfSaude))
                endereco.Add(EndProfSaude);

            if (!string.IsNullOrWhiteSpace(CompEndProfSaude))
                endereco.Add(CompEndProfSaude);

            if (!string.IsNullOrWhiteSpace(BaiProfSaude))
                endereco.Add(BaiProfSaude);

            var cidadeUf = new List<string>();
            if (!string.IsNullOrWhiteSpace(MuniProfSaude))
                cidadeUf.Add(MuniProfSaude);
            if (!string.IsNullOrWhiteSpace(UfProfSaude))
                cidadeUf.Add(UfProfSaude);

            if (cidadeUf.Any())
                endereco.Add(string.Join("/", cidadeUf));

            if (!string.IsNullOrWhiteSpace(CepProfSaude))
                endereco.Add($"CEP: {CepProfSaude}");

            return string.Join(", ", endereco);
        }
    }

    [NotMapped]
    public string TelefoneContato
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(NoCelularProfSaude))
                return NoCelularProfSaude;
            if (!string.IsNullOrWhiteSpace(NoTel1ProfSaude))
                return NoTel1ProfSaude;
            if (!string.IsNullOrWhiteSpace(NoTel2ProfSaude))
                return NoTel2ProfSaude;
            return "Não informado";
        }
    }

    [NotMapped]
    public string RegistroCompleto
    {
        get
        {
            if (!TemRegistroConselho) return "Não informado";
            return $"{ConselhoRegional} {NumeroConselho}/{UfConselho}";
        }
    }

    [NotMapped]
    public string PrioridadeDescricao => FlPrioridadeAtendimento switch
    {
        1 => "Alta",
        2 => "Média",
        3 => "Baixa",
        _ => "Não definida"
    };

    [NotMapped]
    public bool TemFoto => !string.IsNullOrWhiteSpace(FotoProfissional);

    [NotMapped]
    public bool TemAssinatura => !string.IsNullOrWhiteSpace(AssinaturaDigital);

    [NotMapped]
    public bool PodeAtender => EstaAtivo && ContratoVigente && ConselhoAtivo && StatusProfissional == "DISPONIVEL";

    [NotMapped]
    public bool AtendeFimSemana => DiasAtendimento?.Contains("SAB") == true || DiasAtendimento?.Contains("DOM") == true;

    [NotMapped]
    public string DiasAtendimentoFormatado
    {
        get
        {
            if (string.IsNullOrWhiteSpace(DiasAtendimento)) return "Não informado";

            return DiasAtendimento
                .Replace("SEG", "Segunda")
                .Replace("TER", "Terça")
                .Replace("QUA", "Quarta")
                .Replace("QUI", "Quinta")
                .Replace("SEX", "Sexta")
                .Replace("SAB", "Sábado")
                .Replace("DOM", "Domingo")
                .Replace(",", ", ");
        }
    }

    [NotMapped]
    public string HorarioAtendimento
    {
        get
        {
            if (string.IsNullOrWhiteSpace(HorarioInicio) || string.IsNullOrWhiteSpace(HorarioFim))
                return "Não informado";
            return $"{HorarioInicio} às {HorarioFim}";
        }
    }

    [NotMapped]
    public int DiasParaVencimentoContrato
    {
        get
        {
            if (!DataFimContrato.HasValue) return int.MaxValue;
            return (DataFimContrato.Value.Date - DateTime.Today).Days;
        }
    }

    [NotMapped]
    public bool ContratoProximoVencimento => DiasParaVencimentoContrato <= 30 && DiasParaVencimentoContrato > 0;

    [NotMapped]
    public decimal ValorTotalComDomicilio
    {
        get
        {
            var valor = ValorConsulta ?? 0;
            if (AtendeDomicilio && ValorDomicilio.HasValue)
                valor += ValorDomicilio.Value;
            return valor;
        }
    }

    // Métodos de negócio
    public void AtivarProfissional(string usuario)
    {
        ChAtivo = "S";
        FlAtivo = "S";
        StatusProfissional = "DISPONIVEL";
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void InativarProfissional(string usuario, string motivo = null)
    {
        ChAtivo = "N";
        FlAtivo = "N";
        StatusProfissional = "INDISPONIVEL";

        if (!string.IsNullOrEmpty(motivo))
            Observacoes = $"{Observacoes}\n{DateTime.Now:dd/MM/yyyy}: Inativado - {motivo}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void DefinirComoResponsavelCMSO(string usuario)
    {
        FlRespCMSO = "S";
        FlPrioridadeAtendimento = 1;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void RemoverResponsabilidadeCMSO(string usuario)
    {
        FlRespCMSO = "N";
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AlterarStatus(string novoStatus, string usuario, string motivo = null)
    {
        var statusAnterior = StatusProfissional;
        StatusProfissional = novoStatus.ToUpper();

        if (!string.IsNullOrEmpty(motivo))
            Observacoes = $"{Observacoes}\n{DateTime.Now:dd/MM/yyyy}: Status alterado de '{statusAnterior}' para '{novoStatus}' - {motivo}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AtualizarValores(decimal? valorConsulta, decimal? valorDomicilio, string usuario)
    {
        ValorConsulta = valorConsulta;
        ValorDomicilio = valorDomicilio;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void DefinirHorarioAtendimento(string inicio, string fim, string dias, string usuario)
    {
        HorarioInicio = inicio;
        HorarioFim = fim;
        DiasAtendimento = dias;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AtualizarContato(string telefone1, string telefone2, string celular, string email, string usuario)
    {
        NoTel1ProfSaude = telefone1;
        NoTel2ProfSaude = telefone2;
        NoCelularProfSaude = celular;
        EmailProfSaude = email;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AtualizarEndereco(string endereco, string complemento, string bairro,
                                  string cep, string municipio, string uf, string usuario)
    {
        EndProfSaude = endereco;
        CompEndProfSaude = complemento;
        BaiProfSaude = bairro;
        CepProfSaude = cep;
        MuniProfSaude = municipio;
        UfProfSaude = uf;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AtualizarRegistroConselho(string conselho, string numero, string uf,
                                         DateTime? dataInscricao, string situacao, string usuario)
    {
        ConselhoRegional = conselho;
        NumeroConselho = numero;
        UfConselho = uf;
        DataInscricao = dataInscricao;
        SituacaoConselho = situacao;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void RenovarContrato(DateTime novaDataInicio, DateTime novaDataFim,
                               string novoNumeroContrato, string tipoContrato, string usuario)
    {
        var contratoAnterior = NumeroContrato;
        var vencimentoAnterior = DataFimContrato;

        NumeroContrato = novoNumeroContrato;
        DataInicioContrato = novaDataInicio;
        DataFimContrato = novaDataFim;
        TipoContrato = tipoContrato;

        Observacoes = $"{Observacoes}\n{DateTime.Now:dd/MM/yyyy}: Contrato renovado. Anterior: {contratoAnterior} (venc: {vencimentoAnterior:dd/MM/yyyy}) → Novo: {novoNumeroContrato} (venc: {novaDataFim:dd/MM/yyyy})".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AlterarPrioridade(int novaPrioridade, string usuario, string motivo = null)
    {
        if (novaPrioridade < 1 || novaPrioridade > 5)
            throw new ArgumentException("Prioridade deve estar entre 1 e 5.");

        var prioridadeAnterior = FlPrioridadeAtendimento;
        FlPrioridadeAtendimento = novaPrioridade;

        if (!string.IsNullOrEmpty(motivo))
            Observacoes = $"{Observacoes}\n{DateTime.Now:dd/MM/yyyy}: Prioridade alterada de {prioridadeAnterior} para {novaPrioridade} - {motivo}".Trim();

        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void RegistrarAtividade(string usuario)
    {
        DataUltimaAtividade = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AnexarFoto(string caminhoFoto, string usuario)
    {
        FotoProfissional = caminhoFoto;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    public void AnexarAssinatura(string caminhoAssinatura, string usuario)
    {
        AssinaturaDigital = caminhoAssinatura;
        DataAtualizacao = DateTime.UtcNow;
        UsuarioAtualizacao = usuario;
    }

    // Validações
    public bool IsCpfValido()
    {
        if (string.IsNullOrWhiteSpace(CpfProfSaude)) return true;
        return CpfProfSaude.Length == 11 && CpfProfSaude.All(char.IsDigit);
    }

    public bool IsEmailValido()
    {
        if (string.IsNullOrWhiteSpace(EmailProfSaude)) return true;
        return EmailProfSaude.Contains("@") && EmailProfSaude.Contains(".");
    }

    public bool IsRegistroConselhoValido()
    {
        return !string.IsNullOrWhiteSpace(NumeroConselho) &&
               !string.IsNullOrWhiteSpace(ConselhoRegional) &&
               !string.IsNullOrWhiteSpace(UfConselho);
    }

    public bool IsContratoValido()
    {
        if (!DataInicioContrato.HasValue) return true;
        if (!DataFimContrato.HasValue) return true;

        return DataFimContrato.Value >= DataInicioContrato.Value;
    }

    public bool IsHorarioValido()
    {
        if (string.IsNullOrEmpty(HorarioInicio) || string.IsNullOrEmpty(HorarioFim))
            return true;

        if (!TimeSpan.TryParse(HorarioInicio, out var inicio) ||
            !TimeSpan.TryParse(HorarioFim, out var fim))
            return false;

        return fim > inicio;
    }

    public bool TemDadosObrigatorios()
    {
        return !string.IsNullOrWhiteSpace(NomeProfSaude) &&
               CodEspecialidade.HasValue &&
               IsRegistroConselhoValido();
    }

    public bool TemDadosCompletos()
    {
        return TemDadosObrigatorios() &&
               !string.IsNullOrWhiteSpace(CpfProfSaude) &&
               !string.IsNullOrWhiteSpace(EmailProfSaude) &&
               !string.IsNullOrWhiteSpace(TelefoneContato);
    }

    // Métodos utilitários
    public bool PodeAtenderDia(DayOfWeek dia)
    {
        if (string.IsNullOrWhiteSpace(DiasAtendimento)) return false;

        var diaAbrev = dia switch
        {
            DayOfWeek.Monday => "SEG",
            DayOfWeek.Tuesday => "TER",
            DayOfWeek.Wednesday => "QUA",
            DayOfWeek.Thursday => "QUI",
            DayOfWeek.Friday => "SEX",
            DayOfWeek.Saturday => "SAB",
            DayOfWeek.Sunday => "DOM",
            _ => ""
        };

        return DiasAtendimento.Contains(diaAbrev);
    }

    public bool PodeAtenderHorario(TimeSpan horario)
    {
        if (!IsHorarioValido()) return false;

        var inicio = TimeSpan.Parse(HorarioInicio!);
        var fim = TimeSpan.Parse(HorarioFim!);

        return horario >= inicio && horario <= fim;
    }

    // Métodos de relatório
    public Dictionary<string, object> ToRelatorioProfissional()
    {
        return new Dictionary<string, object>
        {
            ["Codigo"] = CodProfSaude,
            ["Nome"] = NomeProfSaude ?? "Não informado",
            ["Especialidade"] = Especialidade?.DescEspecialidade ?? "Não informado",
            ["Registro"] = RegistroCompleto,
            ["CPF"] = CpfProfSaude ?? "Não informado",
            ["Idade"] = IdadeFormatada,
            ["Sexo"] = SexoDescricao,
            ["Tempo_Formacao"] = TempoFormacaoFormatado,
            ["Instituicao"] = InstituicaoFormacao ?? "Não informado",
            ["Status"] = StatusDescricao,
            ["Responsavel_CMSO"] = EhResponsavelCMSO ? "Sim" : "Não",
            ["Prioridade"] = PrioridadeDescricao,
            ["Valor_Consulta"] = ValorConsulta?.ToString("C") ?? "Não informado",
            ["Atende_Domicilio"] = AtendeDomicilio ? "Sim" : "Não",
            ["Valor_Domicilio"] = ValorDomicilio?.ToString("C") ?? "N/A",
            ["Atende_Emergencia"] = AtendeEmergencia ? "Sim" : "Não",
            ["Dias_Atendimento"] = DiasAtendimentoFormatado,
            ["Horario_Atendimento"] = HorarioAtendimento,
            ["Telefone"] = TelefoneContato,
            ["Email"] = EmailProfSaude ?? "Não informado",
            ["Endereco"] = EnderecoCompleto,
            ["Tipo_Contrato"] = TipoContrato ?? "Não informado",
            ["Contrato_Vigente"] = ContratoVigente ? "Sim" : "Não",
            ["Vencimento_Contrato"] = DataFimContrato?.ToString("dd/MM/yyyy") ?? "Não informado",
            ["Dias_Para_Vencimento"] = DiasParaVencimentoContrato == int.MaxValue ? "Sem vencimento" : $"{DiasParaVencimentoContrato} dias",
            ["Ultima_Atividade"] = DataUltimaAtividade?.ToString("dd/MM/yyyy HH:mm") ?? "Nunca"
        };
    }

    public override string ToString()
    {
        var especialidade = Especialidade?.DescEspecialidade ?? "Sem especialidade";
        var status = EstaAtivo ? "" : " [INATIVO]";
        var cmso = EhResponsavelCMSO ? " (CMSO)" : "";
        return $"{CodProfSaude} - {NomeProfSaude} - {especialidade}{cmso}{status}";
    }
}