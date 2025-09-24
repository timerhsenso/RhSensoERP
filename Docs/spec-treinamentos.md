Sistema de Treinamentos Corporativos — Especificação Funcional & Técnica
1) Decisão: onde fica o ASO?

Ownership (fonte da verdade): Saúde Ocupacional
Treinamentos: leitura do status do ASO (Válido/Inválido/Data de vencimento/Restrição).
Por quê: ASO é documento médico/legal; exige governança de saúde e sigilo.
Como integrar: eventos/consulta síncrona:

Treinamentos só permitem matrícula/participação se ASO.Válido == true quando exigido (NRs, aulas práticas).

Portaria consulta ambos: Treinamento.Válido e ASO.Válido antes de liberar acesso a áreas críticas.

EPI só entrega se Treinamento + ASO estiverem em dia (dependendo do EPI).

Matriz de responsabilidade (RACI simplificado)
Dado/Regra	Dono	Consumidores	Observações
ASO (apto/inapto + vencimento)	Saúde	Treinamentos, Portaria, EPI	Sigilo médico; Treinamentos só lê status.
Catálogo de cursos (NRs, internos)	Treinamentos	Avaliação, Portaria, EPI, RH	Cursos com validade e pré-requisitos.
Vínculo curso⇄competência	Treinamentos	Avaliação	Gera PDIs e trilhas.
Liberação de acesso por curso	Portaria	Treinamentos	Portaria executa bloqueio físico.
2) Escopo Funcional (Treinamentos)
2.1 Catálogo & Currículos

Tipos: NRs (NR-20, NR-33, NR-35…), cursos internos (processos, qualidade), externos (fornecedores).

Atributos: carga horária, modalidade (EAD/presencial/prático), validade (meses), local, instrutores, requisitos (ASO, EPI, cursos prévios).

Currículos/Trilhas: conjuntos de cursos por cargo/área/risco.

2.2 Gestão de Turmas

Planejamento de turmas (datas, vagas, recursos, sala, equipamentos, EPIs exigidos).

Lista de espera, overbooking controlado, replanejamento rápido.

Chamada/Presença (QR por aula, biometria opcional).

Provas/avaliações (teórica/prática), banco de questões, gabarito, nota mínima.

2.3 Matrículas & Elegibilidade

Matrícula manual, por PDI (vindo da Avaliação por Competências) ou lote (por cargo/unidade).

Gate de elegibilidade: valida ASO, pré-requisitos, disponibilidade e conflito de escala (Frequência/Ponto).

Política de no-show, reagendamento automático.

2.4 Certificação, Validade e Reciclagem

Emissão de certificados com QR (verificação pública).

Controle de validade (ex.: NR-35 24 meses).

Reciclagem automática: lista de quem vencerá nos próximos X dias, criação de turmas sugeridas.

2.5 EAD/Conteúdo

Aulas EAD com SCORM/xAPI (quando possível) e tempo mínimo assistido.

Proctoring leve (random screenshot, checagem de presença), se necessário.

Pós-treinamento: avaliação de reação (Kirkpatrick N1).

2.6 Integrações

Avaliação/Competências: gaps geram trilhas e matrículas.

EPI: alguns cursos liberam entrega de EPI específico.

Portaria: bloqueia acesso a área se curso obrigatório vencido.

Saúde: consulta status do ASO para elegibilidade.

Frequência/Ponto: evita atrito com jornada (conflito de turnos).

2.7 Analytics & IA

KPIs de cobertura (quem está em dia), taxa de no-show, eficácia (nota, reprovação), impacto em acidentes/incidentes.

IA: sugerir turmas ótimas (lotação, agenda, instrutor), prever risco de vencimento crítico por área, recomendar trilhas por perfil/competência.

2.8 Gamificação (opcional)

Badges por trilhas concluídas, ranking por equipes, metas trimestrais de capacitação.

3) Regras de Negócio (exemplos)

Regra 1: Se curso exige ASO válido, matrícula e presença só liberadas se ASO.Válido == true na data do curso.

Regra 2: Se curso exige pré-requisito A, só conclui se A estiver válido (ex.: NR básica antes da específica).

Regra 3: Reciclagem: avisar 60/30/15 dias antes do vencimento; abrir turma automaticamente se grupo > N pessoas.

Regra 4: Portaria consulta CursoObrigatórioPorÁrea → se vencido, bloquear acesso.

Regra 5: EPI crítico (ex.: respirador) só entregue se curso + fit-test (se aplicável) em dia.

4) Arquitetura & Integração

Clean Architecture: Domain (Treinamento, Turma, Matrícula, Certificado, Currículo), Application (casos de uso), Infrastructure (EF Core, LMS/SCORM adapter, message broker), API/Web.

Integração por eventos (RabbitMQ/Kafka) + APIs:

competency.gap_detected (Avaliação) → Treinamentos sugere turma.

training.completed → EPI/Portaria atualizam permissões.

aso.updated (Saúde) → Revalida elegibilidades e bloqueios/liberações.

epi.policy_changed → Treinamentos recalcula requisitos de cursos.

5) Modelo de Dados (principal)

Curso(Id, Nome, Tipo: NR/Interno/Externo, CargaHoraria, Modalidade, ValidadeMeses, RequisitosJson, CompetenciasJson)

Turma(Id, CursoId, DataIni, DataFim, Vagas, Local/Sala, InstrutorId, RecursosJson, Status)

Matricula(Id, TurmaId, ColaboradorId, Status: Pendente/Confirmada/Concluída/Reprovada/NoShow, Motivo)

PresencaAula(Id, TurmaId, ColaboradorId, AulaId, Presente[bool], Registro[QR/Bio/IP])

AvaliacaoProva(Id, TurmaId, ColaboradorId, Nota, Resultado, Tentativas)

Certificado(Id, ColaboradorId, CursoId, Emissao, ValidoAte, Numero, QrHash, UrlVerificacao)

Curriculo(Id, Nome, RegrasJson) — mapeia cargos/áreas ⇄ cursos obrigatórios

RequisitoASO(Id, CursoId, Obrigatorio[bool], TipoExame?) — só referência; status vem de Saúde

TreinamentoAreaObrig(Id, AreaId, CursoId, Criticidade) — para Portaria/EPI

Multi-tenant: incluir IdSaas quando aplicável.

6) Endpoints (exemplos)

Catálogo

POST /api/cursos / GET /api/cursos/{id}

POST /api/curriculos (vincular cargos/áreas a cursos)

Turmas & Matrículas

POST /api/turmas

POST /api/turmas/{id}/matriculas

POST /api/turmas/{id}/presenca (scan QR/bio por aula)

POST /api/turmas/{id}/prova (nota/resultado)

Certificação

POST /api/certificados/emitir

GET /public/certificado/{qr} (verificação pública)

Relatórios

GET /api/reports/cobertura?cursoId|area|unidade&periodo

GET /api/reports/vencimentos?dias=60

GET /api/reports/eficacia?cursoId&periodo

Integração

GET /api/eligibility/{colaboradorId}?cursoId= → responde { elegivel: true/false, motivos[] }

Webhooks: training.completed, training.expiring

7) Políticas & Config (exemplo)
{
  "Training": {
    "Reciclagem": { "AvisoDias": [60,30,15], "AutoTurmaMinPessoas": 8 },
    "Elegibilidade": { "ExigirASO": true, "ValidarPreRequisitos": true },
    "Presenca": { "MinPercentual": 75, "AceitaEAD": true },
    "Prova": { "NotaMinima": 7.0, "MaxTentativas": 2 }
  }
}

8) UX (telas-chave)

Painel RH/SESMT: cobertura por curso/área, vencimentos próximos, turmas sugeridas, taxa de no-show, eficácia.

Planejamento de Turmas: calendário, colisões de agenda (escala/ponto), recursos (sala/EPI).

Chamada/Presença: tela “operacional”, scan QR grande, contador de presentes/faltantes.

Certificados: listagem, reemissão, verificação QR pública.

Matriz Curricular: por cargo/área/risco (verde = ok, vermelho = pendente).

9) IA & Diferenciais

Turmas ótimas: algoritmo que sugere data/local/instrutor para maximizar presença e reduzir custo.

Risco por área: score que cruza vencimentos de cursos críticos + incidentes + rotatividade.

Recomendação personalizada: baseada em competências (do módulo de Avaliação) e trajetória do colaborador.

10) KPIs

Cobertura de cursos obrigatórios por área (≥ 95%).

Redução de vencimentos críticos (≥ 80% YoY).

No-show ≤ 10%.

Tempo médio de emissão de certificado < 1 min.

Satisfação do participante (NPS) ≥ 8.

11) Roadmap

MVP (0–3 meses):
Catálogo de cursos + turmas + matrículas + presença (QR) + certificados + vencimentos & alertas + integração ASO (consulta).

V2 (3–6 meses):
EAD/SCORM/xAPI, banco de questões, prova online, currículos por cargo/área, integração Portaria/EPI.

V3 (6–12 meses):
IA (turmas ótimas, risco), gamificação, proctoring, verificação pública de certificados e APIs abertas.

12) Pseudocódigo — Elegibilidade de Matrícula
public async Task<ElegibilidadeDto> VerificarElegibilidade(Guid colaboradorId, Guid cursoId, DateTime dataTurma)
{
    var curso = await _cursoRepo.Get(cursoId);
    var requisitos = _requisitoService.ObterRequisitos(cursoId);

    var asoOk = !requisitos.ExigeASO 
                || await _saudeApi.AsoValido(colaboradorId, dataTurma);

    var preReqsOk = await _cursoReqService.TodosPreRequisitosValidos(colaboradorId, cursoId, dataTurma);

    var escalaOk = await _frequenciaApi.SemConflitoDeJornada(colaboradorId, dataTurma);

    var motivos = new List<string>();
    if (!asoOk) motivos.Add("ASO inválido para a data da turma");
    if (!preReqsOk) motivos.Add("Pré-requisitos não atendidos");
    if (!escalaOk) motivos.Add("Conflito com jornada/turno");

    return new ElegibilidadeDto { Elegivel = asoOk && preReqsOk && escalaOk, Motivos = motivos };
}

Resposta direta à sua pergunta

O ASO fica no módulo de Saúde (proprietário).

O sistema de Treinamentos consome o status do ASO para elegibilidade e conclusão de cursos que exigem aptidão (NRs, aulas práticas).

Treinamentos não se restringe a ASO: ele orquestra catálogo, turmas, presença, provas, certificação, validade, reciclagem, integra Avaliação por Competências (gaps → PDIs/trilhas), EPI (liberações) e Portaria (acesso por área de risco).