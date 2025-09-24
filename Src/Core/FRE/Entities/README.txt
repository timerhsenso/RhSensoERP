# FRE.Entities (mapeamento das tabelas de frequência)

Este pacote contém **uma entidade por tabela** do módulo de frequência, espelhando fielmente os campos do SQL.
Cada classe usa `[Table]`, `[Column]`, tamanhos e chaves primárias conforme o script original.

- **BancoHoras** — contém os lançamentos do banco de horas (débito/crédito).
- **BATIDAS** — registra as batidas de ponto do colaborador.
- **COMP1 / COMP2** — cabeçalho e períodos de compensação de horas.
- **FALTASANTECIPADAS** — faltas/saídas antecipadas e justificativas.
- **freq1 / freq2 / freq3 / FREQ4** — ocorrências e consolidações de frequência por dia.
- **chor1 / CHOR2** — horários administrativos e grade semanal.
- **hjor1** — histórico de alterações de jornada por colaborador.
- **jornada / jtpa1** — carga horária prevista por mês e por ano.
- **sitc2** — situação de processamento da frequência por colaborador/dia.
- **mfre1 / mfre2** — motivos de ocorrência e escopo por filial.

> Observação: algumas tabelas não possuem PK explícita no banco. Para o EF Core funcionar corretamente, adotamos como chave o campo `id` quando presente, ou uma chave composta natural (via `[PrimaryKey]`). Isso **não altera** o schema do banco; apenas orienta o EF para rastrear entidades.

Namespace: `RhSensoERP.Core.FRE.Entities`
