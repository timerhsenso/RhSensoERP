 Sistema de Controle de EPI — Especificação Funcional & Técnica

Documento de referência para um sistema de controle de EPI (Equipamentos de Proteção Individual) moderno, escalável e superior às soluções tradicionais.
Stack sugerida: .NET 8 (ASP.NET Core) + SQL Server + AdminLTE/Bootstrap (Web) + MAUI/Flutter (Mobile) + IA preditiva para consumo.

1) Visão Geral

Objetivo: Garantir a entrega, uso correto e rastreabilidade dos EPIs de forma digital, inteligente e auditável, reduzindo riscos legais, acidentes de trabalho e desperdício de recursos.

Público-alvo:

Colaborador: recebe, consulta e solicita EPI via app.

Gestor/SESMT: controla estoque, validade, entrega e conformidade.

RH/DP: integra com treinamentos, ASO e folha.

Diretoria: monitora custos, riscos e indicadores de segurança.

Benefícios-chave:

Eliminação de papel (assinatura digital/biometria).

Redução de perdas e EPIs vencidos.

Compliance automático com NR-6.

Insights de custo e segurança para gestão estratégica.

2) Escopo Funcional
2.1 Gestão de Estoque

Controle de entradas/saídas por lote.

Alertas de estoque mínimo e validade.

Integração com códigos de barras, QR ou RFID.

2.2 Entrega Digital

Registro de entrega com assinatura eletrônica ou biometria.

Comprovante digital enviado ao colaborador.

Bloqueio de retirada sem ASO/treinamento em dia.

2.3 Histórico Individual

“Carteira digital de EPIs” por colaborador.

Consulta rápida de entregas anteriores.

Alertas de prazos de troca.

2.4 Condições & Checklists

Checklist de uso correto antes da atividade.

Registro de não conformidade com foto/evidência.

Substituição imediata em caso de dano.

2.5 Integração Saúde & Treinamentos

Só libera EPI se ASO válido.

Vinculação com treinamentos (NR-35, NR-33 etc.).

Bloqueio automático caso treinamento vença.

2.6 Indicadores & Dashboards

KPIs: custo por setor, EPI mais consumido, perdas, validade.

Relatório de EPI x acidente.

Heatmap de consumo por área.

2.7 Mobile First

App para retirada, checklist e alertas.

Solicitação de reposição com foto.

Offline mode (sincroniza depois).

2.8 Gamificação

Pontos/badges por uso correto e checklist em dia.

Ranking de equipes mais seguras.

2.9 Compliance & Auditoria

Relatórios NR-6 automáticos.

Logs imutáveis (quem entregou, quem recebeu, quando).

Trilhas digitais para auditoria.

2.10 IA Preditiva

Previsão de consumo futuro por setor.

Sugestão automática de compras.

Análise de anomalias (setor consumindo acima da média).

3) Requisitos Não-Funcionais
Categoria	Requisitos
Segurança	Assinatura digital, criptografia, LGPD, logs imutáveis.
Escalabilidade	Suporte a milhares de colaboradores e estoques distribuídos.
Performance	Registro de entrega em < 2s (P95).
Disponibilidade	SLA ≥ 99,9% em ambientes produtivos.
Usabilidade	Mobile-first, UX simples, integração com QR/barcode.
4) Arquitetura (Visão Macro)

Domain/Core: Epi, LoteEpi, EntregaEpi, ChecklistEpi.

Application: casos de uso (entregar EPI, registrar checklist, prever consumo).

Infrastructure: EF Core, serviços de IA, integrações (portaria, LMS, ASO).

API/Web: endpoints REST, dashboards, app mobile.

Complementos:

Cache: Redis (estoque e alertas).

Mensageria: RabbitMQ/Kafka (alertas de validade, sincronização offline).

Observabilidade: Serilog, OpenTelemetry, dashboards (Grafana/Seq).

5) Modelo de Dados (Principais Entidades)

Epi (Id, Nome, Categoria, ValidadeMeses, EstoqueMinimo).

LoteEpi (Id, EpiId, NumeroLote, Validade, Quantidade).

EntregaEpi (Id, ColaboradorId, EpiId, DataEntrega, Validade, AssinaturaDigital, DeviceId).

HistoricoEpiColaborador (Id, ColaboradorId, EpiId, DataEntrega, DataDevolucao, Motivo).

ChecklistEpi (Id, ColaboradorId, EpiId, Data, StatusUso, EvidenciaFoto).

6) Endpoints (Exemplos)

EPIs & Estoque

POST /api/epis

GET /api/epis/{id}

GET /api/estoque/alertas

Entregas

POST /api/entregas { colaboradorId, epiId, loteId, assinatura }

GET /api/entregas?colaboradorId=...

Checklists

POST /api/checklists { colaboradorId, epiId, status, foto }

Relatórios

GET /api/reports/estoque

GET /api/reports/consumo

GET /api/reports/nr6

7) Experiência do Usuário

Colaborador: app simples, carteira digital de EPIs, notificações de validade.

Gestor: dashboards em tempo real, alertas de estoque baixo, consumo por setor.

RH/SESMT: relatórios de conformidade, integração com treinamentos/ASO.

Auditor: relatórios imutáveis, trilhas digitais com validade jurídica.

8) Diferenciais Modernos

Mobile + offline para áreas sem internet.

Assinatura digital/biometria no recebimento.

IA preditiva para compras assertivas.

Gamificação para engajar cultura de segurança.

Integração com portaria/acesso (bloqueio de entrada sem EPI válido).

9) Roadmap

MVP (0–3 meses)

Cadastro de EPIs/estoque.

Entrega digital com assinatura.

Histórico individual.

V2 (3–6 meses)

Controle de validade.

Integração com treinamentos/ASO.

Dashboards e relatórios.

V3 (6–12 meses)

Mobile App com offline mode.

IA preditiva de consumo.

Integração com portaria/acesso.

Gamificação.

10) KPIs de Sucesso

Redução de perdas por vencimento: ≥ 40%.

Entrega registrada 100% digital: ≥ 95%.

Tempo médio de entrega: ≤ 2 min.

Aderência a NR-6: 100%.

Engajamento em checklists de uso: ≥ 80%.

11) Riscos & Mitigações
Risco	Mitigação
Baixa adesão digital	App intuitivo + treinamento rápido.
Falha de conectividade	Mobile com offline mode.
Fraude em retirada	Assinatura digital/biometria + QR/RFID.
EPIs vencidos em uso	Alertas automáticos + bloqueio de acesso.
Falha em auditoria	Relatórios NR-6 automáticos e logs imutáveis.

👉 Esse arquivo pode