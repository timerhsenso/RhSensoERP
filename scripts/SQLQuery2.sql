# 📋 SGC - Consultas de Todas as Tabelas

> **Sistema de Gestão Corporativa - Controle de Portaria e Treinamento de Terceiros**  
> Total: **40 tabelas**  
> Gerado em: Dezembro/2025

---

## 📌 Índice

1. [Tabelas Auxiliares (Lookups)](#1-tabelas-auxiliares-lookups)
2. [Tabelas de Configuração](#2-tabelas-de-configuração)
3. [Tabelas de Cadastro - Fornecedores](#3-tabelas-de-cadastro---fornecedores)
4. [Tabelas de Cadastro - Pessoas](#4-tabelas-de-cadastro---pessoas)
5. [Tabelas de Cadastro - Veículos](#5-tabelas-de-cadastro---veículos)
6. [Tabelas de Cadastro - Treinamento](#6-tabelas-de-cadastro---treinamento)
7. [Tabelas de Cadastro - Checklist](#7-tabelas-de-cadastro---checklist)
8. [Tabelas de Contratos](#8-tabelas-de-contratos)
9. [Tabelas de Agendamento](#9-tabelas-de-agendamento)
10. [Tabelas de Recebimento](#10-tabelas-de-recebimento)
11. [Tabelas Transacionais - Acesso](#11-tabelas-transacionais---acesso)
12. [Tabelas de Ocorrências e Crachás](#12-tabelas-de-ocorrências-e-crachás)
13. [Tabelas de Alertas](#13-tabelas-de-alertas)

---

## 1. Tabelas Auxiliares (Lookups)

> Tabelas de domínio com valores pré-definidos. Raramente alteradas após implantação.

### SGC_TipoFornecedor
Classificação das empresas fornecedoras por tipo de serviço (Manutenção, Limpeza, Segurança, TI, etc).
```sql
SELECT * FROM SGC_TipoFornecedor ORDER BY Ordem;
```

---

### SGC_TipoVeiculo
Tipos de veículos com regras de negócio (exige checklist NR-20? exige pesagem? exige agendamento?).
```sql
SELECT * FROM SGC_TipoVeiculo ORDER BY Ordem;
```

---

### SGC_TipoPessoa
Classificação de pessoas para controle de acesso (Visitante, Prestador, Motorista, Candidato, etc).
```sql
SELECT * FROM SGC_TipoPessoa ORDER BY Ordem;
```

---

### SGC_TipoAso
Tipos de ASO (Atestado de Saúde Ocupacional) conforme NR-07 com validade padrão.
```sql
SELECT * FROM SGC_TipoAso ORDER BY Ordem;
```

---

### SGC_MotivoAcesso
Motivos de acesso à empresa (Entrega, Coleta, Serviço, Reunião, Auditoria, etc).
```sql
SELECT * FROM SGC_MotivoAcesso ORDER BY Ordem;
```

---

### SGC_StatusAcesso
Status do ciclo de vida de um registro de acesso (Aguardando, Entrada, Dentro, Saída, etc).
```sql
SELECT * FROM SGC_StatusAcesso ORDER BY Ordem;
```

---

### SGC_TipoChecklist
Tipos de checklist disponíveis (Veículo, NR-20, Segurança, Recebimento, etc).
```sql
SELECT * FROM SGC_TipoChecklist ORDER BY Ordem;
```

---

### SGC_TipoParentesco
Graus de parentesco para contatos de emergência e visitantes familiares.
```sql
SELECT * FROM SGC_TipoParentesco ORDER BY Ordem;
```

---

### SGC_TipoSanguineo
Tipos sanguíneos com informações de compatibilidade para emergências médicas.
```sql
SELECT * FROM SGC_TipoSanguineo ORDER BY Ordem;
```

---

### SGC_TipoDocumentoTerceiro
Catálogo de documentos controlados (ASO, CNH, NRs, Integração, MOPP) com validade padrão.
```sql
SELECT * FROM SGC_TipoDocumentoTerceiro ORDER BY Codigo;
```

---

### SGC_TipoTreinamento
Catálogo de treinamentos com carga horária e validade (NR-06, NR-10, NR-20, NR-33, NR-35, etc).
```sql
SELECT * FROM SGC_TipoTreinamento ORDER BY Codigo;
```

---

### SGC_MotivoRecusa
Motivos padronizados para recusa de acesso, carga, veículo ou documento.
```sql
SELECT * FROM SGC_MotivoRecusa ORDER BY Tipo, Ordem;
```

---

## 2. Tabelas de Configuração

> Configurações do sistema por empresa/filial. Definem comportamentos e regras de negócio.

### SGC_Portaria
Cadastro de portarias/guaritas da empresa (Principal, Carga, Emergência, etc).
```sql
SELECT * FROM SGC_Portaria ORDER BY CdEmpresa, CdFilial, Ordem;
```

---

### SGC_ConfiguracaoFilial
Parâmetros gerais por empresa/filial (exige agendamento? exige foto? permite recusa parcial?).
```sql
SELECT * FROM SGC_ConfiguracaoFilial ORDER BY CdEmpresa, CdFilial;
```

---

### SGC_ConfiguracaoObrigatoriedade
Regras de validação configuráveis por empresa (CNH obrigatória? ASO obrigatório? bloqueia se não atender?).
```sql
SELECT * FROM SGC_ConfiguracaoObrigatoriedade ORDER BY CdEmpresa, CdFilial, Contexto, Ordem;
```

---

## 3. Tabelas de Cadastro - Fornecedores

> Cadastro de empresas terceirizadas e prestadores de serviço.

### SGC_FornecedorEmpresa
Cadastro de empresas fornecedoras com dados cadastrais, contato principal e secundário.
```sql
SELECT * FROM SGC_FornecedorEmpresa ORDER BY RazaoSocial;
```

---

### SGC_FornecedorColaborador
Cadastro de colaboradores de empresas terceiras (dados pessoais, contato, emergência, CNH, MOPP).
```sql
SELECT * FROM SGC_FornecedorColaborador ORDER BY NomeCompleto;
```

---

## 4. Tabelas de Cadastro - Pessoas

> Cadastro de visitantes, instrutores e documentos de pessoas.

### SGC_Visitante
Cadastro de visitantes eventuais (não vinculados a empresas terceiras).
```sql
SELECT * FROM SGC_Visitante ORDER BY NomeCompleto;
```

---

### SGC_Instrutor
Cadastro de instrutores de treinamento (internos ou externos).
```sql
SELECT * FROM SGC_Instrutor ORDER BY NomeCompleto;
```

---

### SGC_PessoaDocumento
Documentos de funcionários e terceiros (ASO, CNH, certificados) com validade e arquivo digitalizado.
```sql
SELECT * FROM SGC_PessoaDocumento ORDER BY DataValidade;
```

---

## 5. Tabelas de Cadastro - Veículos

> Cadastro de veículos para controle de portaria e pesagem.

### SGC_Veiculo
Cadastro de veículos com dados, proprietário (empresa/funcionário/terceiro) e documentação.
```sql
SELECT * FROM SGC_Veiculo ORDER BY Placa;
```

---

## 6. Tabelas de Cadastro - Treinamento

> Gestão de turmas e participantes de treinamentos.

### SGC_LocalTreinamento
Cadastro de locais/salas para realização de treinamentos.
```sql
SELECT * FROM SGC_LocalTreinamento ORDER BY CdEmpresa, CdFilial, Nome;
```

---

### SGC_TreinamentoTurma
Turmas/sessões de treinamento com data, instrutor, local e status.
```sql
SELECT * FROM SGC_TreinamentoTurma ORDER BY DataInicio DESC;
```

---

### SGC_TreinamentoParticipante
Participantes de turmas (funcionários ou terceiros) com presença, aprovação e validade.
```sql
SELECT * FROM SGC_TreinamentoParticipante ORDER BY IdTreinamentoTurma;
```

---

## 7. Tabelas de Cadastro - Checklist

> Modelos de checklist e seus itens para aplicação em acessos e recebimentos.

### SGC_ChecklistModelo
Modelos de checklist por tipo (Veículo, NR-20, Segurança, etc).
```sql
SELECT * FROM SGC_ChecklistModelo ORDER BY CdEmpresa, CdFilial, Nome;
```

---

### SGC_ChecklistItem
Itens/perguntas de um modelo de checklist com tipo de resposta e se é bloqueante.
```sql
SELECT * FROM SGC_ChecklistItem ORDER BY IdChecklistModelo, Ordem;
```

---

## 8. Tabelas de Contratos

> Gestão de contratos com fornecedores, responsáveis e colaboradores autorizados.

### SGC_Contrato
Contratos com empresas fornecedoras (vigência, status, limite de colaboradores).
```sql
SELECT * FROM SGC_Contrato ORDER BY CdEmpresa, CdFilial, NumeroContrato;
```

---

### SGC_ContratoResponsavel
Responsáveis/gestores do contrato (gestor, suplente, fiscal, técnico) com permissões.
```sql
SELECT * FROM SGC_ContratoResponsavel ORDER BY IdContrato, TipoResponsavel;
```

---

### SGC_ContratoColaborador
Colaboradores terceiros vinculados a um contrato específico com horário e dias permitidos.
```sql
SELECT * FROM SGC_ContratoColaborador ORDER BY IdContrato, IdFornecedorColaborador;
```

---

## 9. Tabelas de Agendamento

> Pré-aviso de entregas e cargas com workflow de aprovação.

### SGC_AgendamentoCarga
Agendamentos de carga/entrega com fornecedor, motorista, veículo, produtos e status.
```sql
SELECT * FROM SGC_AgendamentoCarga ORDER BY DataPrevista DESC, HoraInicioPrevista;
```

---

### SGC_AgendamentoCargaProduto
Itens/produtos previstos no agendamento com controle de FISPQ para produtos químicos.
```sql
SELECT * FROM SGC_AgendamentoCargaProduto ORDER BY IdAgendamentoCarga, Ordem;
```

---

### SGC_AgendamentoAprovacao
Histórico de aprovações/rejeições do agendamento (workflow de aprovação).
```sql
SELECT * FROM SGC_AgendamentoAprovacao ORDER BY IdAgendamentoCarga, DataHoraAcao;
```

---

## 10. Tabelas de Recebimento

> Conferência e aceite de cargas recebidas.

### SGC_RecebimentoCarga
Registro do recebimento efetivo da carga (aceito total, parcial ou recusado).
```sql
SELECT * FROM SGC_RecebimentoCarga ORDER BY DataHoraInicioConferencia DESC;
```

---

### SGC_RecebimentoCargaProduto
Conferência item a item do recebimento com quantidade aceita/recusada e rastreabilidade.
```sql
SELECT * FROM SGC_RecebimentoCargaProduto ORDER BY IdRecebimentoCarga, Ordem;
```

---

## 11. Tabelas Transacionais - Acesso

> Registro de entrada e saída de pessoas na empresa.

### SGC_RegistroAcesso
Registro de entrada/saída com protocolo, status, veículo, carga, pesagem, crachá e checklist.
```sql
SELECT * FROM SGC_RegistroAcesso ORDER BY DataHoraEntrada DESC;
```

---

### SGC_RegistroAcessoChecklist
Respostas do checklist aplicado em um registro de acesso.
```sql
SELECT * FROM SGC_RegistroAcessoChecklist ORDER BY IdRegistroAcesso, IdChecklistItem;
```

---

### SGC_Autorizacao
Autorizações pré-aprovadas para acesso de terceiros/visitantes com vigência e restrições.
```sql
SELECT * FROM SGC_Autorizacao ORDER BY DataInicio DESC;
```

---

## 12. Tabelas de Ocorrências e Crachás

> Registro de incidentes e controle de crachás provisórios.

### SGC_Ocorrencia
Registro de incidentes, problemas e ocorrências com gravidade, workflow e bloqueio.
```sql
SELECT * FROM SGC_Ocorrencia ORDER BY DataHoraOcorrencia DESC;
```

---

### SGC_Cracha
Controle de crachás provisórios com status (disponível, em uso, extraviado).
```sql
SELECT * FROM SGC_Cracha ORDER BY CdEmpresa, CdFilial, NumeroCracha;
```

---

## 13. Tabelas de Alertas

> Alertas automáticos de vencimento de documentos e treinamentos.

### SGC_AlertaVencimento
Alertas de vencimento com workflow de leitura e resolução.
```sql
SELECT * FROM SGC_AlertaVencimento ORDER BY DataVencimento, DiasParaVencimento;
```

---

## 📊 Consultas Úteis

### Pessoas dentro da empresa agora
```sql
SELECT ra.*, 
       CASE ra.TipoPessoa 
           WHEN 'F' THEN 'Funcionário'
           WHEN 'T' THEN 'Terceiro'
           WHEN 'V' THEN 'Visitante'
       END AS TipoPessoaDesc
FROM SGC_RegistroAcesso ra
WHERE ra.DataHoraSaida IS NULL 
  AND ra.Ativo = 1
ORDER BY ra.DataHoraEntrada;
```

---

### Agendamentos de hoje pendentes
```sql
SELECT ac.*, fe.RazaoSocial AS Fornecedor
FROM SGC_AgendamentoCarga ac
LEFT JOIN SGC_FornecedorEmpresa fe ON ac.IdFornecedorEmpresa = fe.Id
WHERE ac.DataPrevista = CAST(GETDATE() AS DATE)
  AND ac.Status IN ('APROVADO', 'PENDENTE_APROVACAO')
ORDER BY ac.HoraInicioPrevista;
```

---

### Documentos vencendo nos próximos 30 dias
```sql
SELECT pd.*, tdt.Descricao AS TipoDocumento,
       CASE pd.EhFuncionario 
           WHEN 1 THEN 'Funcionário'
           ELSE fc.NomeCompleto
       END AS Pessoa
FROM SGC_PessoaDocumento pd
JOIN SGC_TipoDocumentoTerceiro tdt ON pd.IdTipoDocumentoTerceiro = tdt.Id
LEFT JOIN SGC_FornecedorColaborador fc ON pd.IdFornecedorColaborador = fc.Id
WHERE pd.DataValidade BETWEEN GETDATE() AND DATEADD(DAY, 30, GETDATE())
  AND pd.Ativo = 1
ORDER BY pd.DataValidade;
```

---

### Contratos vencendo nos próximos 60 dias
```sql
SELECT c.*, fe.RazaoSocial AS Fornecedor
FROM SGC_Contrato c
JOIN SGC_FornecedorEmpresa fe ON c.IdFornecedorEmpresa = fe.Id
WHERE c.Status = 'ATIVO'
  AND c.DataFim BETWEEN GETDATE() AND DATEADD(DAY, 60, GETDATE())
ORDER BY c.DataFim;
```

---

### Crachás disponíveis por portaria
```sql
SELECT p.Nome AS Portaria, 
       cr.Tipo,
       COUNT(*) AS Disponiveis
FROM SGC_Cracha cr
LEFT JOIN SGC_Portaria p ON cr.IdPortaria = p.Id
WHERE cr.Status = 'DISPONIVEL' 
  AND cr.Ativo = 1
GROUP BY p.Nome, cr.Tipo
ORDER BY p.Nome, cr.Tipo;
```

---

### Ocorrências abertas por gravidade
```sql
SELECT Gravidade, 
       COUNT(*) AS Quantidade
FROM SGC_Ocorrencia
WHERE Status IN ('ABERTA', 'EM_ANALISE')
  AND Ativo = 1
GROUP BY Gravidade
ORDER BY CASE Gravidade 
    WHEN 'CRITICA' THEN 1
    WHEN 'ALTA' THEN 2
    WHEN 'MEDIA' THEN 3
    WHEN 'BAIXA' THEN 4
END;
```

---

### Configurações de obrigatoriedade ativas
```sql
SELECT co.*, 
       CASE co.EhObrigatorio WHEN 1 THEN 'SIM' ELSE 'NÃO' END AS Obrigatorio,
       CASE co.BloqueiaSeNaoAtender WHEN 1 THEN 'SIM' ELSE 'NÃO' END AS Bloqueia
FROM SGC_ConfiguracaoObrigatoriedade co
WHERE co.Ativo = 1
ORDER BY co.CdEmpresa, co.CdFilial, co.Contexto, co.Ordem;
```

---

## 📈 Resumo de Tabelas por Categoria

| Categoria | Qtd | Tabelas |
|-----------|-----|---------|
| Auxiliares (Lookups) | 12 | TipoFornecedor, TipoVeiculo, TipoPessoa, TipoAso, MotivoAcesso, StatusAcesso, TipoChecklist, TipoParentesco, TipoSanguineo, TipoDocumentoTerceiro, TipoTreinamento, MotivoRecusa |
| Configuração | 3 | Portaria, ConfiguracaoFilial, ConfiguracaoObrigatoriedade |
| Cadastro - Fornecedores | 2 | FornecedorEmpresa, FornecedorColaborador |
| Cadastro - Pessoas | 3 | Visitante, Instrutor, PessoaDocumento |
| Cadastro - Veículos | 1 | Veiculo |
| Cadastro - Treinamento | 3 | LocalTreinamento, TreinamentoTurma, TreinamentoParticipante |
| Cadastro - Checklist | 2 | ChecklistModelo, ChecklistItem |
| Contratos | 3 | Contrato, ContratoResponsavel, ContratoColaborador |
| Agendamento | 3 | AgendamentoCarga, AgendamentoCargaProduto, AgendamentoAprovacao |
| Recebimento | 2 | RecebimentoCarga, RecebimentoCargaProduto |
| Transacionais - Acesso | 3 | RegistroAcesso, RegistroAcessoChecklist, Autorizacao |
| Ocorrências e Crachás | 2 | Ocorrencia, Cracha |
| Alertas | 1 | AlertaVencimento |
| **TOTAL** | **40** | |

---

> 📝 **Nota:** Todas as tabelas possuem prefixo `SGC_` (Sistema de Gestão Corporativa).  
> 📝 **Nota:** Campos de auditoria padrão: `Aud_CreatedAt`, `Aud_UpdatedAt`, `Aud_IdUsuarioCadastro`, `Aud_IdUsuarioAtualizacao`.