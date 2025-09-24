Sistema de Portaria & Controle de Acesso Industrial — Especificação Funcional & Técnica

Documento de referência para um sistema corporativo de portaria/acesso voltado a fábricas e indústrias, incluindo logística pesada (caminhões, caminhões-tanque), fornecedores, visitantes, produtos químicos e validações de segurança (checklists, treinamentos, EPIs, permissões de trabalho).
Foco: robusto, flexível, auditável e integrado — não é para condomínios residenciais.

Stack sugerida: .NET 8 / ASP.NET Core + SQL Server + AdminLTE/Bootstrap (Web) + MAUI/Flutter (Mobile) + SignalR (tempo real) + Redis (cache) + RabbitMQ/Kafka (eventos/logística).

1) Visão Geral

Objetivo: Controlar entrada e saída de pessoas e veículos em ambientes industriais garantindo segurança operacional, compliance legal e fluidez logística (portaria, pátio, docas, balança/romaneio).

Personas:

Porteiro / Vigilante: triagem, registro, liberação, incidentes.

SSA/SESMT: validações de segurança, checklists, EPIs, treinamentos, incidentes.

Logística/Pátio: agendamentos, pátio/filas, docas, balança.

Suprimentos/Almoxarifado: controle de recebimento/expedição, notas/fiscal.

Manutenção/Operação: permissões de trabalho (PT), bloqueios/LOTO.

Visitantes / Motoristas / Terceiros: pré-cadastro, instruções de segurança.

Compliance/Auditoria: trilhas, relatórios, inspeções.

Benefícios:

Segurança: checagens automáticas (treinamentos, EPIs, PTs, SDS/MSDS).

Eficiência: filas e pátio otimizados, integração com balança e docas.

Compliance: NR-s, ambiental e rastreabilidade completa (quem/como/quando).

Integração: ERP/WMS/TMS, EPI, Treinamentos, Saúde Ocupacional.

2) Escopo Funcional
2.1 Identificação & Autenticação

Pessoas: crachá RFID/NFC, QR code, biometria (digital/facial), pré-check-in via link.

Veículos: LPR/ANPR (leitura de placas), TAG RFID (opcional), QR no para-brisa.

Motoristas & Terceiros: carteira, CNH, MOPP/NRs em dia, cadastro do veículo/reboque.

Por que: autenticação confiável evita fraude e agiliza o fluxo.

2.2 Fluxos de Acesso (Pessoas)

Visitantes corporativos: convite / pré-registro, termo de confidencialidade, vídeo de segurança, badge temporário.

Prestadores de serviço: vínculo a Ordem de Serviço e Permissão de Trabalho (PT); checar NRs e EPIs.

Funcionários: integração com RH (turnos, bloqueios, férias, afastamentos).

Diferencial: rotas condicionais por risco (área classificada, altura, espaço confinado).

2.3 Fluxos de Acesso (Veículos)

Visitantes/Pequenos: cadastro rápido, local de estacionamento, tempo de permanência.

Fornecedores (leves/pesados): doca atribuída, janela de agendamento, checklist de segurança.

Caminhões / Carretas / Caminhões-Tanque:

Checklists obrigatórios por tipo de carga (inflamável, corrosivo, refrigerado etc.).

Selo/Lacre (nº de lacres, ruptura), SDS/MSDS (Ficha de Segurança), EPI do motorista.

Integração com Balança (entrada/saída), ROMANEIO/CT-e/MDFe (se aplicável).

Rota interna (gate → pátio → doca/baldeio → balança → gate).

Diferencial: Gestão de pátio (Yard Management) com fila inteligente e painéis de docas.

2.4 Segurança & Compliance

Validações automáticas: treinamentos válidos (NR-s), exame médico/ASO, EPIs obrigatórios para a área, PT vigente.

Checklists digitais: por tipo de acesso/atividade (ex.: NR-20, NR-35, área classificada, inflamáveis).

Documentos obrigatórios: CRLV, ANTT, licenças ambientais, SDS/MSDS.

Incidentes/Quase Acidentes: registro com evidências (fotos, vídeos), bloqueios temporários e plano de ação.

Diferencial: Bloqueio automático de catraca / cancela se requisito crítico não atendido.

2.5 Logística & Pátio

Agendamento de janelas (docking appointment) com SLA e penalidades.

Fila por prioridade (materiais críticos, perecíveis, químicos).

Mapa de pátio (slots), alocação de docas e status em tempo real (SignalR).

Balança/ROMANEIO: integração para peso de entrada/saída, tara/bruto/líquido; divergências.

Diferencial: Orquestração por eventos (arriving → waiting → in_dock → weighing → exit).

2.6 Monitoramento em Tempo Real

Painel de portaria: próximos a chegar, na fila, em área interna, saindo; semáforos.

CFTV/LPR: anexo automático de imagem/framing da passagem do veículo ou pessoa.

Alertas: vencimento de PT, tempo excedido em área, falha de checklist, bloqueio de rota.

2.7 Integrações

ERP/WMS/TMS: pedidos, notas fiscais, ordens de compra/serviço, romaneios.

EPI/Treinamentos/ASO: liberar/bloquear acesso conforme status.

Acesso Físico: catracas, cancelas, ACLs por área (Modbus/TCP, SDKs de controladoras).

Fiscal (quando aplicável): CT-e/MDFe/Manifestos via middleware.

2.8 Auditoria & Relatórios

Trilhas imutáveis: quem autorizou, quando, de onde (IP/Dispositivo), evidências.

Relatórios regulatórios: visitas, prestadores, cargas perigosas, tempos de pátio, não conformidades.

Dashboards: lead time gate→doca, ocupação de pátio, fila média, violações de segurança.

3) Requisitos Não-Funcionais
Categoria	Requisitos
Segurança	MFA/SSO, RBAC/ABAC por área, TLS, criptografia sensível, LGPD.
Confiabilidade	Idempotência em eventos de entrada/saída; tolerância a falhas com reprocesso.
Escalabilidade	Filas (RabbitMQ/Kafka) para eventos de pátio e balança; cache Redis.
Disponibilidade	Health checks, readiness/liveness, SLA ≥ 99,9% para gate operations.
Performance	Registro de passagem ≤ 1s P95; tela de pátio em tempo real (≤ 2s P95 refresh).
Observabilidade	Serilog + OpenTelemetry (traces/metrics/logs), painéis (Grafana/Seq).
Usabilidade	Telas “operacionais” (poucos cliques, alto contraste), mobile-first para fiscalização.
4) Arquitetura (Macro)

Domain/Core: Agregados: Acesso, Passagem, Checklist, PT, Agendamento, Pátio/Slot, Doca, Balança, Carga.

Application: Casos de uso (autorizar entrada, concluir checklist, alocar doca, pesar, liberar saída).

Infrastructure: EF Core, integrações (câmeras/LPR, controladoras, ERP/WMS/TMS, Balança), fila, cache.

API/Web: REST/SignalR, dashboards, postos operacionais (gate, pátio, doca, balança).

Mobile: fiscalização/checklist, fotos, leituras QR/RFID.

Padrões: Clean Architecture, event sourcing light para passagens, sagas para fluxos longos (gate→doca→balança→gate).

5) Modelo de Dados (Principais Entidades)

Pessoas & Credenciais

Pessoa (Id, Tipo: Funcionário/Visitante/Terceiro/Motorista, Documento, Validações)

Credencial (Id, PessoaId, Tipo: RFID/QR/Bio, Código, Status)

Veículos & Cargas

Veiculo (Id, Placa, Tipo: Leve/Pesado/Tanque, ProprietarioId, ANTT?, TagsRFID?)

Semireboque (Id, Placa, ProprietarioId)

Carga (Id, Tipo: Geral/Químico/Perigoso/Refrigerado, SDSId?, Risco, LacresJson)

Acesso & Passagens

Acesso (Id, PessoaId?, VeiculoId?, Motivo, AgendamentoId?, Status)

Passagem (Id, AcessoId, GateId, Direcao: Entrada/Saída, DataHora, EvidenciasJson, Device, OperadorId)

Segurança

Checklist (Id, Tipo, ItensJson, ObrigatorioPara: [Tanque/Químico/Altura])

ChecklistExecucao (Id, AcessoId/PassagemId, ItensMarcadosJson, Fotos, Resultado)

PermissaoTrabalho (Id, Tipo, Area, Vigencia, ResponsavelId, Status)

NaoConformidade (Id, Origem, Gravidade, Evidencias, AcaoImediata, Status)

Logística

Agendamento (Id, Fornecedor/Cliente, JanelaIni/Fim, DocaPreferida, SLA, Status)

PatioSlot (Id, Mapa, Coordenadas, OcupadoPorVeiculoId?)

Doca (Id, Area, Capacidade, Status)

BalançaRegistro (Id, VeiculoId, EntradaPeso, SaidaPeso, Tara, Liquido, Divergencia)

Integrações & Auditoria

IntegracaoEvento (Id, Origem, Payload, Status, Tentativas, DeadLetter)

LogAuditoria (Id, Entidade, EntidadeId, Acao, Usuario, DataHora, Ip, Hash)

Multi-tenant (SaaS): adicionar IdSaas nas tabelas.

6) Regras & Validações (Exemplos)

Entrada de caminhão-tanque: exige ChecklistExecucao NR-20 OK, SDS/MSDS anexa, lacres conferidos, EPI do motorista válido; sem isso, cancela não abre.

Prestador de serviço em altura (NR-35): checar treinamento válido, PT aberta para a área, EPI liberado; sem conformidade → bloqueio.

Tempo máximo em pátio (SLA): excedeu? alerta + justificativa.

Divergência de balança: acima do tolerado → workflow de investigação (bloqueio de saída até tratativa).

7) Endpoints (Exemplos)

Autorização & Passagens

POST /api/access/authorize { pessoaId?, veiculoId?, motivo, agendamentoId? }

POST /api/access/checklist { acessoId, checklistId, respostas, fotos[] }

POST /api/gate/passage { acessoId, gateId, direcao, evidencias } ➜ idempotente

Logística

POST /api/scheduling (criar/alterar janela)

POST /api/yard/assign { veiculoId, slotId }

POST /api/dock/assign { veiculoId, docaId }

POST /api/scale/weigh { veiculoId, peso, tipo: entrada|saida }

Segurança

POST /api/pt (abrir PT), PUT /api/pt/{id}/status

POST /api/incidents (NC/Quase-acidente)

Relatórios

GET /api/reports/gate/day

GET /api/reports/yard/occupancy

GET /api/reports/hazmat (químicos/perigosos)

8) Telas & UX (Operação)

Console de Portaria (Gate):

Busca rápida por placa/documento/cracha; status de conformidade (verde/âmbar/vermelho).

Checklist inline com anexos (foto).

Botões grandes para Autorizar/Negar/Encaminhar.

Gestão de Pátio & Docas:

Mapa de slots + fila por prioridade; drag-and-drop de veículos.

Painel de docas (livre/ocupado/SLA).

Balança:

Tela simples: captura peso, compara com romaneio, marca divergências.

SSA/SESMT:

Painel de não-conformidades, PTs, vencimentos de treinamentos, EPIs.

Relatórios/Auditoria:

Linha do tempo de um acesso (da portaria à saída) com evidências e hashes.

9) Integrações Técnicas

Câmeras/LPR: via SDK/HTTP (snap + placa), vincular à Passagem.

Controladoras (catracas/cancelas): SDK ou Modbus/TCP/Wiegand/OSDP (middleware).

Balança: porta serial/TCP ➜ serviço leitor ligado ao endpoint /scale/weigh.

ERP/WMS/TMS: padrões (REST/CSV/SFTP), filas com retry e DLQ.

10) Segurança & Compliance

RBAC/ABAC (por área crítica).

MFA/SSO (AD/Azure AD/Keycloak).

LGPD: minimização, retenção, consentimento quando necessário.

Criptografia: at-rest (colunas sensíveis), in-transit (TLS), hash de evidências.

Auditoria imutável: encadeamento (hash chain) por acesso/período.

Rate Limit & Anti-tamper nos endpoints de gate/scale.

11) Observabilidade & Operação

Health Checks: DB, fila, cache, integrações (LPR/Controladoras/Balança).

Métricas-chave: throughput gate, tempo médio gate→doca, ocupação pátio, falhas checklist, divergências de balança, NC por gravidade.

Alertas: SLA de fila/ocupação, integração offline, tentativa de acesso bloqueado.

Playbooks: queda da balança, perda de comunicação com controladora, fila crítica.

12) KPIs & SLAs

Tempo de atendimento na portaria (P95): ≤ 60 s.

Acurácia LPR: ≥ 95% (condições padrão).

Conformidades automáticas resolvidas no gate: ≥ 90%.

Redução de tempo de pátio (mês): ≥ 20%.

Registros com evidência completa: ≥ 99%.

Disponibilidade gate ops: ≥ 99,9%.

13) Roadmap

MVP (0–3 meses)

Passagens (entrada/saída) pessoas/veículos, cadastro básico, checklists, relatórios diários, integrações mínimas (RH, EPI, Treinamentos), painel de portaria.

V2 (3–6 meses)

Gestão de pátio & docas, agendamentos, integração com balança e romaneio, LPR integrado, auditoria avançada, dashboards tempo real.

V3 (6–12 meses)

Sagas/Orquestração logística completa, PTs integradas por área, bloqueios automáticos físicos, KPI avançado, mobile fiscalização (offline), IA para previsão de filas e janelas ótimas.

14) Diferenciais Competitivos (por que superior)

Segurança convergente: pessoas + veículos + área + documento + EPI + treinamento + PT → uma decisão automática de acesso.

Operação em tempo real: yard & dock com fila inteligente, painéis, SignalR.

Compliance forte: SDS/MSDS, NR-s, auditoria imutável, lacres, trilhas completas.

Integrações industriais: balança, controladoras, LPR, ERP/WMS/TMS.

Escalabilidade: arquitetura por eventos, cache, filas, idempotência.

UX operacional: telas “grandes”, poucos cliques, foco no trabalho de campo.

15) Exemplos de Checklists (resumo)

Caminhão-Tanque (NR-20):

SDS/MSDS anexada? Lacres conferidos? Aterramento OK? Extintor? Condutor com MOPP válido? EPI (luvas, óculos, respirador) OK?

Trabalho em Altura (NR-35):

PT aberta? Linha de vida? Cinto/ talabarte inspecionados? Condições climáticas?

Área Classificada:

Equip. antiexplosão? Detecção de gás? Autorização de entrada?

Visitante técnico:

Vídeo de segurança assistido? Termo assinado? Acompanhante designado?

16) Pseudocódigos de Casos-Chave

Decisão de Liberação no Gate

var acesso = await AcessoService.ObterContextoAsync(placa, documento, cracha);

var requisitos = await ComplianceService.VerificarAsync(new() {
    PessoaId = acesso.PessoaId,
    VeiculoId = acesso.VeiculoId,
    AreaDestino = acesso.AreaDestino,
    TipoCarga = acesso.TipoCarga
});

if (!requisitos.Ok)
  return Deny(requisitos.Motivos);

var checklistOk = await ChecklistService.ExecutarOuValidarAsync(acesso, requisitos.ChecklistObrigatorio);
if (!checklistOk) return Deny("Checklist inválido");

await GateHardware.AbrirCancelaAsync(acesso.GateId);
return Approve(acesso);


Evento de Balança (idempotente)

public async Task RegistrarPesoAsync(WeighDto dto) {
  using var scope = new TxScope();
  if (await Repo.PesoJaRegistrado(dto.IdempotencyKey)) return;

  await Repo.SalvarPeso(dto);
  await Broker.PublishAsync("weigh.registered", dto);
  scope.Complete();
}

17) Configurações de Política (exemplo)
{
  "AccessPolicy": {
    "RequireTrainingValid": true,
    "RequireEpiByArea": true,
    "RequirePTForHighRisk": true,
    "AutoDenyOnChecklistFail": true,
    "Hazmat": {
      "RequireSDS": true,
      "LacresMandatory": true,
      "BalanceWeightToleranceKg": 50
    }
  },
  "Yard": {
    "MaxQueueMinutes": 90,
    "PriorityRules": ["Perishable","Hazmat","CriticalPO"]
  }
}

18) Riscos & Mitigações
Risco	Mitigação
Integrações de hardware heterogêneas	Camada de “drivers”/middleware com abstrações + testbeds.
LPR em baixa acurácia	Combinar com TAG/QR; túnel iluminado; fallback manual.
Gargalo na portaria	Pré-check-in, agendamento, checklists curtos & direcionados.
Falhas de rede industrial	Buffer local + reprocesso; alertas; plano de contingência manual.
Falhas de compliance	Regras bloqueantes claras + auditoria imutável + relatórios prontos.