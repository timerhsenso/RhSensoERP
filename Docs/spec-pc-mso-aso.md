Saúde Ocupacional — Especificação Funcional & Técnica

Módulo corporativo para gestão de saúde ocupacional integrado a Treinamentos, Portaria, EPI, Avaliação por Competências e Frequência.
Foco ASO completo, programas legais (PCMSO, LTCATLIP), gestão de exames, vigilância de saúde, e interoperabilidade com clínicasSESMSESocial, mantendo sigilo médico e LGPD.

Stack sugerida .NET 8  ASP.NET Core, SQL Server, AdminLTEBootstrap (Web) + MAUIFlutter (Mobile), RabbitMQKafka (eventos), Redis (cache), Serilog + OpenTelemetry (observabilidade).

1) Visão Geral

Objetivo Orquestrar todo o ciclo de aptidão ao trabalho (ASO), exames ocupacionais (admissionais, periódicos, mudança de função, retorno, demissionais), programas de saúde (PCMSO), imunizações e vigilância de agravos, com compliance legal e integração operacional (liberabloqueia treinamentos, EPI, acesso a áreas, escalas).

Personas

Médico do Trabalho  Enfermagem PCMSO, ASO, laudos, exames, atestados, vigilância.

SESMT  Segurança riscos, integração com PGRLTCAT, medidas, imunizações.

RHDP status de aptidão (consulta), agendamento e comunicação com colaboradores.

Gestores indicadores (cobertura, afastamentos, agravos).

Colaborador agenda de examesvacinas, resultados (quando permitido), documentos.

Princípios

Sigilo médico dados clínicos só para Saúde; demais módulos enxergam status (AptoRestriçãoNão Apto, validade).

Interoperabilidade APIsarquivos com clínicas e eSocial (quando aplicável).

Automação agendamentos, alertas, reciclagens e bloqueios automáticos.

2) Escopo Funcional
2.1 ASO — Atestado de Saúde Ocupacional

Tipos admissional, periódico, mudança de função, retorno ao trabalho, demissional.

Conteúdo aptidão (Apto  Apto com Restrição  Inapto), validade, restrições, médico responsável (CRM), exames associados.

Workflows

Geraçãorenovação automática por gatilhos (admissão, mudança de função, retorno pós-afastamento, demissão, periodicidade).

Integração com clínica solicitação, agendamento, retorno de resultadosASO.

Publicação de status (evento aso.updated) para TreinamentosEPIPortaria.

Por que ASO é a fonte da verdade de aptidão no ecossistema.

2.2 Programas & Riscos (PCMSO ↔ PGRLTCAT)

PCMSO plano anual, calendário de exames por riscoatividadecargo, responsabilidades, indicadores.

Matriz risco-ocupação agente (químico, físico, biológico, ergonômico), intensidadeexposição, EPIEPC requeridos, treinamentos vinculados.

LTCATLIP (quando aplicável) registro de exposições e condições.

Integração com Segurança importar PGRmapa de riscos para direcionar exames e periodicidades.

Por que alinha risco → exame → ASO → EPItreinamento.

2.3 Gestão de Exames

Catálogo de exames (audiometria, espirometria, exames laboratoriais, radiológicos etc.) com periodicidade e requisitos.

Ordem de exame gera pacote por colaborador conforme riscofunção.

Agendamento conflito de jornada (consulta a Frequênciaescala), lembretes (e-mailWhatsAppApp).

Recebimento de resultados via APISFTPportal da clínica; validação médica; vinculação ao ASO.

Por que automatiza e dá rastreabilidade ao ciclo ocupacional.

2.4 Atestados, Afastamentos & Retorno

Atestados registro (CID quando aplicável e permitido), período, recomendações, confidencialidade.

Afastamentos vínculo com INSSbenefícios, impactos em escala e treinamentos; gatilho para ASO de retorno.

CAT (Comunicação de Acidente do Trabalho) opcional abertura, vínculos com incidentes (módulo Segurança).

2.5 Imunizações & Campanhas

Vacinas por riscoárea (ex. tétano, hepatite B, influenza).

Carteira de vacinação digital (com validadereforços).

Campanhas (calendário, metas por unidadeárea, cobertura).

2.6 Integrações Operacionais

Treinamentos checar ASO válido para cursos práticosNRs; bloquear matrículapresença quando exigido.

EPI liberar entrega de EPIs críticos somente com ASO válido (e treinamentos quando necessários).

Portaria bloquear acesso a áreas de risco se ASO vencidoinapto.

Frequência ajustar escala em afastamentos; não marcar como falta injusta.

ERPDP comunicar afastamentosretornos; opcional eSocial (S-2220S-2240, conforme política).

2.7 Analytics & Vigilância em Saúde

Dashboards cobertura de ASO, exames em atraso, afastamentos por causaduração, incidência por árearisco, cobertura vacinal.

Vigilância cluster de sintomasagravos, correlação com setoresagentes.

Indicadores legais prazos de ASOretorno, cumprimento de PCMSO.

2.8 LGPD & Governança de Sigilo

Compartilhamento mínimo outros módulos somente consomem AptoApto com RestriçãoInapto + validade.

Perfis de acesso médicoenfermagem (pleno), RHgestor (status), auditor (documentos com base legal).

Consentimento & base legal registro de finalidade, retenção e anonimização quando aplicável.

Logs imutáveis acesso a dados clínicos versionado e rastreado.

3) Requisitos Não-Funcionais
Categoria	Requisitos
Segurança	Criptografia “at rest” para dados sensíveis; TLS; RBACABAC; MFASSO; mascaramento de campos.
Confiabilidade	Idempotência em integrações com clínicas; reconciliação; reprocesso.
Escalabilidade	Filas para processamento de lotes (examesASO); cache de status para consumo cross-módulo.
Disponibilidade	Health checks; SLA ≥ 99,9% para consulta de status de ASO.
Privacidade	Trilhas de auditoria; políticas de retenção; segregação lógica por tenant.
Usabilidade	Telas clínicas objetivas; automações e assistentes (regras por risco).
4) Arquitetura (Macro)

DomainCore Agregados ASO, Exame, Programa (PCMSO), AfastamentoAtestado, Imunização.

Application casos de uso (gerar ordem de exame, consolidar ASO, notificar vencimentos, registrar afastamento).

Infrastructure EF Core; adaptadores de clínica (APISFTPPortal); eSocial (opcional); broker de eventos; storage de documentos (SDSASO PDF).

APIWeb endpoints REST; dashboards; controle de acesso por perfil.

Eventos aso.createdupdatedexpired, exam.order.sentresult.received, leave.openedclosed, immunization.updated.

Observabilidade Serilog + OpenTelemetry (tracesmetrics); painéis de falhas de integração e SLA de exames.

5) Modelo de Dados (Principais Entidades)

Multi-tenant adicionar IdSaas quando aplicável.

Colaborador(Id, Nomatric, CargoId, LotacaoId, …) — referência

RiscoOcupacional(Id, Nome, Agente, Severidade, PeriodicidadeMeses, EpIsRequeridosJson, CursosRequeridosJson)

ProgramaPCMSO(Id, VigenciaInicio, VigenciaFim, MedicoRespId, RegrasJson)

ExameCatalogo(Id, Nome, CodigoTUSS, Tipo, PrazoResultadoDias, RequisitosJson)

OrdemExame(Id, ColaboradorId, ItensJson[ExameId, Prioridade], ClinicaId, Status, AgendadoPara)

ResultadoExame(Id, OrdemExameId, ExameId, DataResultado, ArquivoUrl, IndicadoresJson, Conclusao)

ASO(Id, ColaboradorId, Tipo, DataEmissao, ValidoAte, Situacao AptoRestricaoInapto, RestricoesJson, MedicoRespId, ArquivoUrl, Hash)

Afastamento(Id, ColaboradorId, Motivo, DataInicio, DataFim, CID, DocumentoUrl, Status)

Atestado(Id, ColaboradorId, Data, Dias, CID, Recomendacoes, DocumentoUrl)

Imunizacao(Id, ColaboradorId, Vacina, Dose, DataAplicacao, ValidoAte, ComprovanteUrl)

ClinicaParceira(Id, Nome, Canal APISFTPPortal, CredenciaisConfigJson)

6) Endpoints (Exemplos)

Status & Integração Cross-Módulo

GET apihealthaso-status{colaboradorId} → { situacao, validoAte, restricoes[] } (sem dados clínicos)

POST apieventsaso (webhook de mudanças de ASO)

ASO & Exames

POST apiasogerar { colaboradorId, tipo } → cria OrdemExame + ASO pendente

POST apiexamesordem (batch por riscoárea)

POST apiexamesresultado (callback clínica)

POST apiasoconsolidar { ordemExameId } → gera ASO final

AfastamentosAtestados

POST apiafastamentos

PUT apiafastamentos{id}fechar

POST apiatestados

Imunizações

POST apiimunizacoes

GET apiimunizacoescoberturaarea=...

Relatórios

GET apireportsasocoberturaunidade&periodo

GET apireportsexamesatrasos

GET apireportsafastamentosmotivo&periodo

7) Regras de Negócio (Exemplos)

R1 – Periodicidade por riscofunção gerar OrdemExame automática antes do vencimento (ex. 30157 dias).

R2 – Consolidação do ASO somente após todos os ResultadosExame obrigatórios com conclusão válida e assinatura do médico responsável.

R3 – Publicação de status ao consolidarvencer ASO, emitir evento aso.updated → Treinamentos, EPI e Portaria reagem.

R4 – Afastamento cria bloqueios temporários (treinos práticos, acesso a áreas) e agenda ASO retorno na data estimada de volta.

R5 – Restrições Apto com Restrição (ex. sem trabalho em altura) deve refletir em Portaria (bloquear área) e Escala (evitar alocação em tarefa incompatível).

8) UX (Telas-chave)

Painel Clínico (MédicoEnfermagem) fila de ordens, exames pendentes, ASOs a consolidar, afastamentos recentes, alertas críticos.

Agenda Integrada disponibilidade da clínica, conflitos de jornada, reagendamento em massa.

Colaborador (PortalApp) próximos examesvacinas, recibosASO (quando permitido), orientações.

RHGestor status de aptidão por equipe (verdeâmbarvermelho), vencimentos próximos, restrições ativas (sem detalhe clínico).

Relatórios cobertura ASO, KPIs de PCMSO, afastamentos por causa, campanhas de vacinação.

9) LGPD, Sigilo & Perfis

Dados Clínicos visíveis somente a saúde (perfil MédicoEnfermagem).

Demais Módulos consomem API de status (AptoRestriçãoInapto + validade), sem laudos.

Trilhas de Acesso log com quem, quando, de onde, qual campo.

MascaramentoCriptografia CID, resultados e anexos críticos com controle fino de acesso.

Retenção & Anonimização políticas por leicontrato; scripts de limpezaanonimização.

10) KPIs & SLAs

Cobertura ASO vigente (empresaárea) ≥ 98%

Tempo médio ASO (ordem → consolidado) ≤ 7 dias

Exames em atraso ≤ 2%

Afastamentos sem ASO de retorno no prazo 0%

Falhas de integração com clínica resolvidas em ≤ 24h (P95)

11) Roadmap

MVP (0–3 meses)

Catálogo de exames, geração de ordens, integração simples com clínica (arquivoAPI), consolidação de ASO, API de status para módulos.

Dashboards de cobertura de ASO e vencimentos; notificações.

V2 (3–6 meses)

PCMSO completo, matriz risco-ocupação, campanhas de vacinação, afastamentosatestados, integração com TreinamentosEPIPortariaEscola.

Portal do colaborador; relatórios avançados; eSocial (opcional, feature flag).

V3 (6–12 meses)

IA para previsão de demanda de exames e risco por área; vigilância de agravos (detecção de clusters), análises de correlação (acidentes x ASO x treinamento x EPI).

Assinatura digital avançada (ICP-Brasil) para ASOlaudos (quando aplicável).

12) Pseudocódigo — Publicação de Status de ASO
public async Task ConsolidarAsoAsync(Guid ordemExameId)
{
    var ordem = await _ordens.GetAsync(ordemExameId);
    var pendentes = await _exames.ObterPendentes(ordemExameId);

    if (pendentes.Any())
        throw new DomainException(Existem exames pendentes.);

    var conclusoes = await _exames.ObterConclusoes(ordemExameId);
    var situacao = _medico.DefinirSituacao(conclusoes);  Apto  Restricao  Inapto

    var aso = new Aso
    {
        ColaboradorId = ordem.ColaboradorId,
        Tipo = ordem.TipoAso,
        DataEmissao = DateTime.UtcNow,
        ValidoAte = _regras.CalcularValidade(ordem),
        Situacao = situacao,
        MedicoRespId = _ctx.UserId,
        ArquivoUrl = await _docs.GerarPdfAsync(ordemExameId, situacao),
        Hash = _hash.Criar(  dados essenciais  )
    };

    await _asos.SalvarAsync(aso);

     Publica somente STATUS (sem dados clínicos)
    await _broker.PublishAsync(aso.updated, new {
        ColaboradorId = aso.ColaboradorId,
        Situacao = aso.Situacao,
        ValidoAte = aso.ValidoAte
    });
}

13) Políticas (exemplo de appsettings)
{
  Health {
    ASO {
      AvisoVencimentoDias [60, 30, 15, 7],
      BloquearTreinamentoSeInvalido true,
      BloquearEpiCriticoSeInvalido true,
      BloquearAcessoAreaRiscoSeInvalido true
    },
    Exames {
      ReprocessarResultadosFalhosEmHoras 6,
      PrioridadePorRisco [Quimico,Fisico,Biologico,Ergonomico]
    },
    IntegracaoClinica {
      Modo API,  API  SFTP  Portal
      RetryPolicy { Max 5, BackoffSeconds 30 }
    }
  }
}

Resumo das decisões-chave

ASO é do módulo de Saúde (sigilo e governança médica).

Os demais módulos consomem status (aptorestriçãoinapto + validade) e reagem

Treinamentos elegibilidadematrículapresença.

EPI entrega de EPIs críticos.

Portaria acesso a áreas de risco.

Frequência ajustes por afastamento.

PCMSO direciona exames por riscos ocupacionais integrados ao PGRLTCAT.

Automação e observabilidade são cruciais para escala e compliance.