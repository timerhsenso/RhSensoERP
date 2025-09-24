Sistema de Avaliação por Competências — Especificação Funcional & Técnica

Documento de referência para um sistema de avaliação por competências moderno, escalável e estratégico.
Stack sugerida: .NET 8 (ASP.NET Core) + SQL Server + AdminLTE/Bootstrap (Web) + MAUI/Flutter (Mobile) + IA/ML para análises preditivas.

1) Visão Geral

Objetivo: Avaliar colaboradores com base em competências técnicas e comportamentais, gerar planos de desenvolvimento, promover engajamento contínuo e fornecer inteligência estratégica ao RH e à liderança.

Público-alvo (personas):

Colaborador: realiza autoavaliações, recebe feedbacks, acompanha evolução.

Gestor: avalia equipe, aprova PDIs, compara gaps vs. perfil ideal.

RH: parametriza competências, aplica ciclos, gera relatórios estratégicos.

Diretoria: visualiza insights globais, talentos, riscos e benchmarks.

Benefícios-chave:

Cultura de feedback contínuo.

Planos de desenvolvimento automáticos e rastreáveis.

Identificação de talentos ocultos e riscos de turnover.

Engajamento e gamificação no processo de avaliação.

2) Escopo Funcional
2.1 Cadastro e Matriz de Competências

Definição de competências técnicas, comportamentais e de liderança.

Associação a cargos, funções e projetos.

Peso configurável por competência.

Por que: cria o “perfil ideal” que servirá como referência.

2.2 Ciclos de Avaliação

Configuração de ciclos (ex.: anual, semestral, sob demanda).

Avaliação 90º (gestor), 180º (gestor + colaborador), 360º (inclui pares e subordinados).

Autoavaliação obrigatória/opcional.

Por que: permite diferentes modelos, adaptáveis à cultura da empresa.

2.3 Feedback Contínuo

Microfeedbacks (elogios, críticas construtivas).

“Kudos” vinculados a competências específicas.

Integração com notificações (e-mail, Teams, Slack).

Por que: avaliação deixa de ser apenas um evento anual.

2.4 Planos de Desenvolvimento Individual (PDI)

Geração automática de PDIs com base nos gaps detectados.

Sugestão de treinamentos/cursos/mentorias.

Acompanhamento de progresso do colaborador.

Por que: conecta diagnóstico → ação → resultado.

2.5 Gamificação

Sistema de pontos, badges e ranking interno.

Conquistas atreladas a evolução de competências ou conclusão de treinamentos.

Por que: aumenta engajamento e adesão dos colaboradores.

2.6 Relatórios e Dashboards

Relatórios individuais (espelho de competências).

Comparativo esperado vs. real (gráfico radar).

Relatórios por time, unidade e empresa.

KPIs de evolução de competências.

Por que: dá visibilidade estratégica e permite benchmarking interno.

2.7 Benchmarking e Insights

Comparação entre equipes/unidades.

Comparação externa (se houver base de mercado).

Heatmaps de competências críticas.

Por que: gestores enxergam gaps e oportunidades de desenvolvimento.

2.8 IA e Análises Preditivas

Identificação de talentos com alto potencial.

Previsão de risco de turnover.

Sugestão de movimentações internas (promoções, trocas de projeto).

Por que: transforma o RH em área estratégica, antecipa problemas.

2.9 Integrações

Integração com sistemas de RH, folha, ERP.

Integração com LMS (plataformas de treinamento).

SSO/AD/Azure AD (autenticação).

Por que: evita retrabalho e conecta todo o ecossistema.

2.10 Auditoria e Compliance

Registro de todas as avaliações e feedbacks (quem, quando, o quê).

Logs de alterações de notas.

Conformidade com LGPD (consentimento, minimização, anonimização).

Por que: avaliações impactam carreira, precisam ser auditáveis e seguras.

3) Requisitos Não-Funcionais
Categoria	Requisitos
Segurança	MFA opcional, SSO, roles, criptografia de dados sensíveis, LGPD.
Escalabilidade	Suporte a milhares de colaboradores em ciclos simultâneos.
Performance	Relatórios de até 1.000 colaboradores em < 2s (P95).
Disponibilidade	SLA ≥ 99,9% para o módulo de avaliação.
Confiabilidade	Logs de auditoria imutáveis.
Usabilidade	Mobile-first, interface intuitiva, responsiva e gamificada.
4) Arquitetura (Visão Macro)

Domain/Core: Competências, Avaliações, Feedbacks, PDIs.

Application: casos de uso (aplicar avaliação, gerar PDI, aprovar feedback).

Infrastructure: EF Core, serviços de IA/ML, integrações LMS/ERP.

API/Web: controllers REST/GraphQL, dashboards, relatórios, mobile app.

Complementos:

Cache (Redis): métricas agregadas.

Mensageria (RabbitMQ/Kafka): notificações, relatórios em lote.

Observabilidade: Serilog, OpenTelemetry, dashboards (Grafana/Seq).

5) Modelo de Dados (Entidades Principais)

Competencia (Id, Nome, Tipo [Técnica/Comportamental/Liderança], Descrição).

CargoCompetencia (Id, CargoId, CompetenciaId, Peso).

Avaliacao (Id, CicloId, AvaliadoId, AvaliadorId, CompetenciaId, Nota, Comentario, Data).

CicloAvaliacao (Id, Nome, PeriodoIni, PeriodoFim, Tipo [90/180/360], Status).

Feedback (Id, DeId, ParaId, CompetenciaId?, Texto, Data, Tipo [Kudo/Construtivo]).

PDI (Id, FuncionarioId, CompetenciaId, Objetivo, Acao, Prazo, Status).

Gamificacao (Id, FuncionarioId, Pontos, BadgesJson).

6) Endpoints (Exemplos)

Competências

POST /api/competencias

GET /api/competencias/{id}

Ciclos

POST /api/ciclos

POST /api/ciclos/{id}/abrir

POST /api/ciclos/{id}/fechar

Avaliações

POST /api/avaliacoes

GET /api/avaliacoes/espelho?userId=...&ciclo=...

Feedbacks

POST /api/feedbacks

GET /api/feedbacks?userId=...

PDI

POST /api/pdi

PUT /api/pdi/{id}/atualizar

GET /api/pdi?userId=...

Relatórios

GET /api/reports/dashboard?empresaId=...

GET /api/reports/radar?userId=...

7) Experiência do Usuário

Colaborador: autoavaliação fácil, feedbacks rápidos, visualização clara do PDI.

Gestor: dashboard da equipe, aprova PDIs, acompanha evolução.

RH: parametrização, relatórios avançados, comparativos globais.

Diretoria: insights de talentos, riscos, sucessão.

8) Diferenciais Modernos

Gamificação integrada (badges, ranking).

Feedback contínuo estilo rede social interna.

IA/ML para predição de talentos e turnover.

Dashboards BI embutidos (gráficos dinâmicos, heatmaps, radar).

Mobile-first com notificações push.

Integração com LMS (sugestão automática de trilhas de aprendizado).

9) Roadmap

MVP (0–3 meses)

Cadastro de competências e cargos.

Ciclo simples de avaliação (auto + gestor).

Relatório radar básico.

V2 (3–6 meses)

Avaliação 360º.

Feedback contínuo.

Planos de Desenvolvimento.

Dashboards de equipe/unidade.

V3 (6–12 meses)

Gamificação.

Integração com LMS.

IA preditiva (talentos/turnover).

Benchmarking interno e externo.

10) KPIs de Sucesso

Adesão ao ciclo de avaliação: ≥ 95%.

Feedbacks contínuos/mês: ≥ 3 por colaborador.

PDIs criados vs. concluídos: ≥ 80%.

Satisfação dos colaboradores: NPS ≥ 8.

Aderência a prazos de ciclo: ≥ 90%.

11) Riscos & Mitigações
Risco	Mitigação
Resistência cultural	Treinamentos e gamificação.
Feedback enviesado	Avaliação 360º + anonimato parcial.
Baixa adesão mobile	Mobile-first + UX simples + notificações.
Vazamento de dados sensíveis	Criptografia + LGPD + controles de acesso.
Carga alta em ciclos	Scale-out + filas para processamento em lote.
12) Padrões & Boas Práticas

Clean Architecture (separação de responsabilidades).

FluentValidation em DTOs.

Feature Flags (ativar 360º, gamificação etc.).

CI/CD automatizado.

Documentação viva (Swagger, Markdown).

Testes unitários, integração e e2e.