# RhSensoERP — Manual de Geração de Entidades a partir de CREATE TABLE

**Versão:** 6.0 | **Atualizado:** 2026-03-03
**Sistema:** RhSensoERP — Sistema de Gestão de RH Multi-Tenant
**Stack:** .NET 8 · Clean Architecture · EF Core 8 · SQL Server · MediatR · AutoMapper

---

## 1. Visão Geral

Este documento descreve como gerar corretamente uma **entidade C# completa** para o RhSensoERP a partir de um script `CREATE TABLE` do SQL Server. Ele cobre desde a análise da DDL até a produção de todos os artefatos: Entity, DTOs, Validators, EF Config, Commands, Queries, Repository, API Controller, e Frontend (View + JavaScript).

> **Público-alvo:** Qualquer IA ou desenvolvedor que precise criar entidades neste projeto.

---

## 2. Arquitetura do Projeto

```
RhSensoERP/
├── src/
│   ├── Shared/
│   │   ├── Core/           → BaseEntity, Attributes, Interfaces
│   │   ├── Application/    → Interfaces compartilhadas
│   │   └── Infrastructure/ → DbContext compartilhado
│   ├── Modules/
│   │   ├── Identity/       → Módulo SEG (Segurança)
│   │   ├── GestaoDePessoas/ → Módulo RHU (RH)
│   │   ├── ControleDePonto/ → Módulo CPT
│   │   ├── GestaoTerceirosPrestadores/ → Módulo CAP
│   │   └── {Modulo}/
│   │       ├── Core/  (ou Domain/)
│   │       │   ├── Common/     → BaseEntity do módulo
│   │       │   └── Entities/   → ★ ENTIDADES GERADAS AQUI
│   │       ├── Application/
│   │       │   ├── DTOs/       → Dto, CreateRequest, UpdateRequest
│   │       │   ├── Commands/   → Create, Update, Delete (MediatR)
│   │       │   ├── Queries/    → GetById, GetPaged (MediatR)
│   │       │   ├── Validators/ → FluentValidation
│   │       │   └── Mappings/   → AutoMapper Profiles
│   │       └── Infrastructure/
│   │           ├── Persistence/ → EF Configurations
│   │           └── Repositories/
│   ├── API/                    → Controllers da API
│   └── Web/                    → Controllers MVC, Views, JS
```

---

## 3. Passo 1 — Analisar o CREATE TABLE

Dado um script SQL como input, extraia **todas** as informações abaixo:

### 3.1. Informações da Tabela

| Campo | Onde encontrar | Exemplo |
|-------|---------------|---------|
| **Nome da tabela** | `CREATE TABLE [schema].[nome]` | `dbo.cap_fornecedores` |
| **Schema** | Prefixo antes do nome | `dbo` |
| **Nome PascalCase** | Converter nome → PascalCase (remover prefixo `tb_` se houver) | `CapFornecedores` |

### 3.2. Informações de Cada Coluna

Para **cada coluna**, determine:

| Atributo | Como detectar | Impacto |
|----------|--------------|---------|
| **Nome** | Nome da coluna | Converter para PascalCase → nome da property |
| **Tipo SQL** | `varchar`, `int`, `uniqueidentifier`, etc. | Mapear para tipo C# (ver Seção 4) |
| **Tamanho** | `varchar(100)`, `decimal(18,2)` | `[StringLength]`, `[Column(TypeName)]` |
| **NOT NULL** | Ausência de `NULL` / presença de `NOT NULL` | `[Required]`, validação |
| **NULL** | Presença de `NULL` | Tipo nullable (`string?`, `int?`) |
| **PRIMARY KEY** | `PRIMARY KEY (col)` ou `CONSTRAINT PK_` | `[Key]`, HasKey no EF |
| **PK Composta** | `PRIMARY KEY (col1, col2)` | `HasKey(e => new { e.Col1, e.Col2 })` |
| **IDENTITY** | `IDENTITY(1,1)` | `[DatabaseGenerated(Identity)]` |
| **DEFAULT** | `DEFAULT newid()`, `DEFAULT 0`, `DEFAULT SYSUTCDATETIME()` | Valor padrão na entity |
| **FOREIGN KEY** | `REFERENCES [tabela](coluna)` | Navigation Property + `[ForeignKey]` |
| **UNIQUE** | `UNIQUE` ou `CONSTRAINT UX_` | `[Unique]` + Índice único no EF Config |
| **TRIGGERS** | `CREATE TRIGGER` referenciando a tabela | `[HasDatabaseTriggers]` |

### 3.3. Classificação da Chave Primária

Esta é a decisão **mais crítica**. Existem 4 cenários:

```
┌─────────────────────────────────────────────────────────────────────┐
│                   ÁRVORE DE DECISÃO DA PK                          │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  A tabela tem quantas colunas na PRIMARY KEY?                       │
│                                                                     │
│  ► UMA COLUNA (PK Simples):                                        │
│    │                                                                │
│    ├─ É IDENTITY(1,1)?                                              │
│    │   SIM → PK_SIMPLES_IDENTITY                                    │
│    │         Tipo: int/long, Auto-gerada pelo banco                 │
│    │         NÃO aparece no CreateRequest                           │
│    │         NÃO aparece no UpdateRequest                           │
│    │         NÃO aparece no formulário                              │
│    │         ✅ Usa [LookupKey] para Select2                       │
│    │                                                                │
│    ├─ É uniqueidentifier com DEFAULT newid()?                       │
│    │   SIM → PK_SIMPLES_GUID                                       │
│    │         Tipo: Guid, Auto-gerada pelo banco                     │
│    │         NÃO aparece no CreateRequest                           │
│    │         NÃO aparece no UpdateRequest                           │
│    │         NÃO aparece no formulário                              │
│    │                                                                │
│    └─ É varchar/int SEM identity?                                   │
│        SIM → PK_SIMPLES_MANUAL                                      │
│              Tipo: string/int, Informada pelo usuário               │
│              ✅ APARECE no CreateRequest (required!)                │
│              ❌ NÃO aparece no UpdateRequest (vem pela rota)        │
│              ✅ APARECE no formulário (required, readonly em edit)  │
│                                                                     │
│  ► DUAS+ COLUNAS (PK Composta):                                    │
│    → PK_COMPOSTA                                                    │
│      Tipo: string+string, int+string, etc.                          │
│      ✅ TODAS aparecem no CreateRequest (required!)                 │
│      ❌ NÃO aparecem no UpdateRequest (vêm pela rota)              │
│      ✅ TODAS aparecem no formulário (required, readonly em edit)   │
│      ⚠️  Campo Id (Guid) herdado de BaseEntity é IGNORADO          │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 4. Passo 2 — Mapeamento de Tipos SQL → C#

| Tipo SQL | Tipo C# (NOT NULL) | Tipo C# (NULL) | Notas |
|----------|-------------------|----------------|-------|
| `int` | `int` | `int?` | |
| `bigint` | `long` | `long?` | |
| `smallint` | `short` | `short?` | |
| `tinyint` | `byte` | `byte?` | |
| `bit` | `bool` | `bool?` | |
| `decimal(p,s)` | `decimal` | `decimal?` | Mapear `[Column(TypeName = "decimal(p,s)")]` |
| `numeric(p,s)` | `decimal` | `decimal?` | Idem |
| `float` | `double` | `double?` | |
| `real` | `float` | `float?` | |
| `varchar(n)` | `string` | `string?` | `[Column(TypeName = "VarChar(n)")]` + `[StringLength(n)]` |
| `nvarchar(n)` | `string` | `string?` | `[Column(TypeName = "nvarchar(n)")]` + `[StringLength(n)]` |
| `char(n)` | `string` | `string?` | `[Column(TypeName = "Char(n)")]` + `[StringLength(n)]` |
| `text` / `ntext` | `string` | `string?` | Legado — usar `nvarchar(max)` |
| `uniqueidentifier` | `Guid` | `Guid?` | |
| `datetime` | `DateTime` | `DateTime?` | |
| `datetime2` | `DateTime` | `DateTime?` | Mapear `[Column(TypeName = "datetime2(3)")]` |
| `date` | `DateTime` | `DateTime?` | **Atenção:** Mapeado para `DateTime`. A conversão para `DateOnly` deve ser feita na camada de apresentação (UI) ou em DTOs específicos. |
| `time` | `TimeSpan` | `TimeSpan?` | **Atenção:** Mapeado para `TimeSpan`. A conversão para `TimeOnly` deve ser feita na camada de apresentação (UI) ou em DTOs específicos. |
| `datetimeoffset` | `DateTimeOffset` | `DateTimeOffset?` | |
| `varbinary(n)` | `byte[]` | `byte[]?` | |
| `image` | `byte[]` | `byte[]?` | Legado |

### 4.1. Regra Especial para `string`

- `string` em C# é reference type → já pode ser `null`
- Se a coluna é `NOT NULL` → use `string` + `= string.Empty;` como default
- Se a coluna é `NULL` → use `string?` (sem default)
- **Não coloque `[Required]` em string PK** (a PK já é implicitamente required)

---

## 5. Passo 3 — Detectar Classe Base e Herança

O sistema suporta **dois padrões** de entidade:

### 5.1. Padrão COM Herança (BaseEntity)

```
┌─────────────────────────────────────────────────────────────────────┐
│  A tabela tem coluna "IdSaaS" (ou "idsaas")?                       │
│  SIM → Herda de BaseMultiTenantEntity                               │
│                                                                     │
│  A tabela tem colunas "DtCriacao" / "DtAtualizacao"?                │
│  SIM → Herda de BaseAuditableEntity                                 │
│                                                                     │
│  Nenhum dos anteriores?                                             │
│  → Herda de BaseEntity                                              │
└─────────────────────────────────────────────────────────────────────┘
```

**Importante:** `BaseEntity` injeta automaticamente um campo `Id` do tipo `Guid`. Em entidades com PK composta, esse `Id` existe na classe mas **NÃO é a PK real** — ele é ignorado nos DTOs e no formulário.

### 5.2. Padrão SEM Herança (Standalone Class)

Quando a entidade tem PK própria (`int IDENTITY`, por exemplo) e gerencia seus próprios campos de auditoria e TenantId diretamente:

```csharp
// SEM herança — classe standalone
public class CapFornecedores  // ← Sem ": BaseEntity"
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }          // ← PK própria (int Identity)

    [Required]
    public Guid TenantId { get; set; }   // ← TenantId declarado explicitamente

    // ... propriedades ...

    // Auditoria declarada explicitamente
    public DateTime CreatedAtUtc { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public Guid? UpdatedByUserId { get; set; }
}
```

### 5.3. Quando usar cada padrão?

| Cenário | Padrão | Motivo |
|---------|--------|--------|
| Tabela legada com `idsaas` + `Aud_*` | COM Herança (`BaseMultiTenantEntity`) | Compatibilidade |
| Tabela nova com PK `int IDENTITY` | SEM Herança | Controle total dos campos |
| Tabela com PK composta (varchar+varchar) | COM Herança (`BaseEntity`) | Id Guid é ignorado |
| Tabela nova com triggers de auditoria | SEM Herança + `[HasDatabaseTriggers]` | Banco controla auditoria |

---

## 6. Passo 4 — Detectar Campos Especiais

### 6.1. Campos de Auditoria

Existem **dois padrões** de auditoria no projeto:

**Padrão Legado (`Aud_*`):**

| Nome da Coluna | Papel | Tratamento |
|---------------|-------|------------|
| `Aud_DtCadastro` | Data de criação | Handler preenche automaticamente |
| `Aud_DtAlteracao` | Data de atualização | Handler preenche automaticamente |
| `Aud_IdUsuarioCadastro` | Usuário que criou | Handler preenche via `_currentUser.UserId` |
| `Aud_IdUsuarioAlteracao` | Usuário que alterou | Handler preenche via `_currentUser.UserId` |

**Padrão Novo (UTC):**

| Nome da Coluna | Papel | Tratamento |
|---------------|-------|------------|
| `CreatedAtUtc` | Data de criação (UTC) | Trigger SQL ou Handler |
| `UpdatedAtUtc` | Data de atualização (UTC) | Trigger SQL ou Handler |
| `CreatedByUserId` | Usuário que criou | Handler preenche |
| `UpdatedByUserId` | Usuário que alterou | Handler preenche |

**Regra:** Campos de auditoria **NUNCA** aparecem no CreateRequest, UpdateRequest ou formulário. São preenchidos automaticamente.

### 6.2. Campo TenantId

| Padrão | Tipo | Tratamento |
|--------|------|------------|
| `IdSaaS` | `Guid` ou `string` | Herdado de BaseMultiTenantEntity |
| `TenantId` | `Guid` | Declarado na entidade standalone |

**Regra:** TenantId **NUNCA** aparece nos DTOs ou formulário. É injetado no Handler do Create e validado no Update/Delete.

### 6.3. Campo Ativo

| Padrão | Tipo | Tratamento |
|--------|------|------------|
| `Ativo`, `IsAtivo` | `bool` | Toggle switch no formulário + endpoint `PATCH toggle-ativo` |

### 6.4. Database Triggers

Se a tabela tem **triggers** para auditoria automática (ex: `DEFAULT SYSUTCDATETIME()` ou triggers que populam `CreatedAtUtc`/`UpdatedAtUtc`), a entidade deve receber:

```csharp
[HasDatabaseTriggers("Auditoria automática de CreatedAt/UpdatedAt via triggers SQL Server")]
```

Isso informa o EF Core que a tabela tem triggers, evitando conflitos no `SaveChanges`.

---

## 7. Catálogo de Atributos Disponíveis

### 7.1. Atributos Padrão do .NET

| Atributo | Uso | Exemplo |
|----------|-----|---------|
| `[Key]` | Marca coluna como PK | `[Key] public int Id { get; set; }` |
| `[DatabaseGenerated(Identity)]` | PK auto-incremento | Com `IDENTITY(1,1)` |
| `[Required]` | NOT NULL (exceto string) | Campos obrigatórios |
| `[Column("nome", TypeName = "...")]` | Mapeia para coluna SQL | `[Column("cd_funcao", TypeName = "VarChar(20)")]` |
| `[StringLength(n)]` | Tamanho máximo | Strings com tamanho definido |
| `[Table("nome", Schema = "dbo")]` | Mapeia para tabela | No topo da classe |
| `[ForeignKey(nameof(Prop))]` | FK → Navigation | Em Navigation Properties |
| `[InverseProperty(nameof(...))]` | Navegação inversa (1:N) | Em ICollection<> |
| `[Display(Name = "Nome Bonito")]` | Nome amigável para UI | Em cada propriedade |

### 7.2. Atributos Customizados do RhSensoERP

| Atributo | Propósito | Quando Usar |
|----------|----------|-------------|
| `[GenerateCrud(...)]` | Aciona o Source Generator | Em toda entidade (ver Seção 8) |
| `[LookupKey]` | Define campo de valor do Select2 | Na PK (ex: `Id`) |
| `[LookupText]` | Define campo(s) de texto do Select2 | Em campos de exibição (ex: `RazaoSocial`, `NomeFantasia`) |
| `[Unique(scope, displayName)]` | Índice único + validação automática | Em campos que devem ser únicos |
| `[HasDatabaseTriggers("motivo")]` | Tabela tem triggers SQL | Quando banco controla auditoria |

### 7.3. Detalhamento: `[LookupKey]` e `[LookupText]`

Esses atributos controlam como a entidade aparece em **dropdowns Select2** de outras telas:

```csharp
[Key]
[LookupKey]                    // ← Este campo será o "value" do <option>
public int Id { get; set; }

[LookupText]                   // ← Este campo será o "text" visível
public string RazaoSocial { get; set; } = string.Empty;

[LookupText]                   // ← Pode ter MÚLTIPLOS [LookupText]
public string? NomeFantasia { get; set; }
```

**Endpoint gerado:** `GET /api/{module}/{entity}/lookup?term=busca&page=1&pageSize=20`

**Response gerada:**
```json
{
  "results": [
    { "id": 1, "razaoSocial": "Empresa XPTO", "nomeFantasia": "XPTO" },
    { "id": 2, "razaoSocial": "Acme Corp", "nomeFantasia": "Acme" }
  ],
  "pagination": { "more": false }
}
```

**Regras de fallback (se nenhum atributo for colocado):**
- `[LookupKey]` ausente → usa campo com `[Key]`
- `[LookupText]` ausente → auto-detecta: `RazaoSocial`, `Nome`, `Descricao`, `NomeFantasia`, `Titulo`

### 7.4. Detalhamento: `[Unique]`

```csharp
// Único POR TENANT (mais comum):
[Unique(UniqueScope.Tenant, "CNPJ")]
public string? Cnpj { get; set; }

// Único GLOBALMENTE (raro):
[Unique(UniqueScope.Global, "Código")]
public string Codigo { get; set; } = string.Empty;
```

**O que o Source Generator faz automaticamente:**

1. **EF Config:** Cria índice único:
   ```csharp
   builder.HasIndex(e => new { e.TenantId, e.Cnpj }, "UX_CapFornecedores_Tenant_Cnpj")
       .IsUnique()
       .HasFilter("Cnpj IS NOT NULL");  // Se o campo é nullable
   ```

2. **Validação (Pipeline MediatR):** `UniqueValidationBehavior` verifica ANTES do `SaveChanges`

3. **Handler:** Captura `DbUpdateException` (SQL 2601/2627) → retorna HTTP 409 Conflict com mensagem amigável: _"Já existe um registro com este 'CNPJ'."_

---

## 8. Passo 5 — Gerar o Atributo [GenerateCrud]

Toda entidade precisa do atributo `[GenerateCrud]` para acionar o Source Generator:

```csharp
[GenerateCrud(
    // --- IDENTIFICAÇÃO ---
    TableName            = "cap_fornecedores",
    Schema               = "dbo",                   // Padrão: "dbo"
    DisplayName          = "CapFornecedores",

    // --- PERMISSÕES ---
    CdSistema            = "SEG",                    // Código do módulo pai
    CdFuncao             = "SEG_FM_CAPFORNECEDORES", // Código da função de permissão

    // --- ROTAS API ---
    ApiRoute             = "gestaoterceiros/capfornecedores",
    ApiGroup             = "Gestão de Terceiros",
    UsePluralRoute       = false,                    // false = singular (padrão)

    // --- FLAGS BACKEND ---
    GenerateDto          = true,
    GenerateRequests     = true,
    GenerateCommands     = true,
    GenerateQueries      = true,
    GenerateValidators   = true,
    GenerateRepository   = true,
    GenerateMapper       = true,
    GenerateEfConfig     = true,
    GenerateMetadata     = true,

    // --- FLAGS FRONTEND ---
    GenerateLookup       = true,                     // ★ Gera endpoint /lookup para Select2
    GenerateApiController = true,
    GenerateWebController = true,
    GenerateWebModels     = true,
    GenerateWebServices   = true,

    // --- COMPORTAMENTOS ---
    SupportsBatchDelete  = true,                     // false para PK composta
    ApiRequiresAuth      = true,
    IsLegacyTable        = false,
    InjectBaseAuditProperties = false
)]
```

### 8.1. Regras de Decisão do Atributo

| Cenário | Flag | Valor | Motivo |
|---------|------|-------|--------|
| PK simples (Guid/int) | `SupportsBatchDelete` | `true` | BatchDelete usa `List<string>` |
| PK composta | `SupportsBatchDelete` | `false` | Incompatível com `List<string>` |
| Tabela legada (padrão `Aud_*`) | `IsLegacyTable` | `true` | Sem TenantId/CurrentUser automáticos |
| Tabela nova | `IsLegacyTable` | `false` | Gera TenantId + CurrentUser + Auditoria |
| Tabela de referência (Tipo, UF, etc.) | `GenerateLookup` | `true` | Gera endpoint `/lookup` para Select2 |
| Tabela principal | `GenerateLookup` | `true` ou `false` | `true` se outra tela usa como dropdown |
| Entidade precisa de tela web | `GenerateWebController` + `WebModels` + `WebServices` | `true` | Gera frontend completo |

---

## 9. Passo 6 — Gerar a Entidade

### 9.1. Template Completo (Padrão Novo — Standalone)

Exemplo baseado no `CapFornecedores`:

```csharp
// =============================================================================
// RHSENSOERP - ENTITY {ENTITYNAME}
// =============================================================================
// Módulo: {NomeDoMódulo}
// Tabela: {nome_tabela}
// Schema: {schema}
// Multi-tenant: ✅ SIM (TenantId obrigatório)
//
// ✅ VALIDAÇÃO AUTOMÁTICA DE UNICIDADE:
// - {CampoUnico}: Único por tenant
// =============================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;
// using {namespace de entidades relacionadas para Navigation Properties};

namespace RhSensoERP.Modules.{Modulo}.Core.Entities;

/// <summary>
/// {EntityName} - {Descrição da entidade}
/// Tabela: {nome_tabela} (Multi-tenant)
/// </summary>
[GenerateCrud(
    TableName = "{nome_tabela}",
    DisplayName = "{EntityName}",
    CdSistema = "{MOD}",
    CdFuncao = "{MOD_FM_ENTIDADE}",
    IsLegacyTable = false,
    GenerateApiController = true,
    GenerateLookup = true                // ← Se é tabela de referência
)]
[Table("{nome_tabela}")]
[HasDatabaseTriggers("Auditoria automática via triggers SQL Server")]  // ← Se tem triggers
public class {EntityName}
{
    // =========================================================================
    // CHAVE PRIMÁRIA
    // =========================================================================

    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "ID")]
    [LookupKey]                                        // ← Para Select2
    public int Id { get; set; }

    // =========================================================================
    // TENANT (Multi-tenant obrigatório)
    // =========================================================================

    [Required]
    [Column("TenantId")]
    [Display(Name = "Tenant ID")]
    public Guid TenantId { get; set; }

    // =========================================================================
    // CAMPOS DE NEGÓCIO
    // =========================================================================

    [Required]
    [Column("RazaoSocial", TypeName = "nvarchar(255)")]
    [StringLength(255)]
    [Display(Name = "Razão Social")]
    [LookupText]                                       // ← Texto do Select2
    public string RazaoSocial { get; set; } = string.Empty;

    [Column("NomeFantasia", TypeName = "nvarchar(255)")]
    [StringLength(255)]
    [Display(Name = "Nome Fantasia")]
    [LookupText]                                       // ← Múltiplos LookupText OK
    public string? NomeFantasia { get; set; }

    // =========================================================================
    // CAMPOS COM UNICIDADE
    // =========================================================================

    [Unique(UniqueScope.Tenant, "CNPJ")]               // ← Único por tenant!
    [Column("Cnpj", TypeName = "nvarchar(18)")]
    [StringLength(18)]
    [Display(Name = "CNPJ")]
    public string? Cnpj { get; set; }

    // =========================================================================
    // FOREIGN KEYS (campos de FK)
    // =========================================================================

    [Column("IdUf")]
    [Display(Name = "ID UF")]
    public int? IdUf { get; set; }                     // ← FK nullable

    // =========================================================================
    // CAMPO ATIVO (toggle)
    // =========================================================================

    [Required]
    [Column("Ativo")]
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;

    // =========================================================================
    // AUDITORIA (preenchida por triggers ou Handler)
    // =========================================================================

    [Required]
    [Column("CreatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Data Criação (UTC)")]
    public DateTime CreatedAtUtc { get; set; }

    [Column("CreatedByUserId")]
    [Display(Name = "Criado Por")]
    public Guid? CreatedByUserId { get; set; }

    [Required]
    [Column("UpdatedAtUtc", TypeName = "datetime2(3)")]
    [Display(Name = "Data Atualização (UTC)")]
    public DateTime UpdatedAtUtc { get; set; }

    [Column("UpdatedByUserId")]
    [Display(Name = "Atualizado Por")]
    public Guid? UpdatedByUserId { get; set; }

    // =========================================================================
    // NAVIGATION PROPERTIES (ManyToOne — esta entidade → outra)
    // =========================================================================

    /// <summary>
    /// Navegação para UF via IdUf.
    /// </summary>
    [ForeignKey(nameof(IdUf))]
    public virtual BasUfs? Uf { get; set; }

    // =========================================================================
    // INVERSE NAVIGATION (OneToMany — outras entidades → esta)
    // =========================================================================

    /// <summary>
    /// Colaboradores vinculados a este fornecedor.
    /// </summary>
    [InverseProperty(nameof(CapColaboradoresFornecedor.Fornecedor))]
    public virtual ICollection<CapColaboradoresFornecedor> Colaboradores { get; set; }
        = new List<CapColaboradoresFornecedor>();

    /// <summary>
    /// Contratos vinculados a este fornecedor.
    /// </summary>
    [InverseProperty(nameof(CapContratosFornecedor.Fornecedor))]
    public virtual ICollection<CapContratosFornecedor> Contratos { get; set; }
        = new List<CapContratosFornecedor>();
}
```

### 9.2. Template (Padrão Legado — Com Herança)

Para tabelas com padrão `idsaas` + `Aud_*`:

```csharp
[Table("tb_funcao_sistema", Schema = "dbo")]
[GenerateCrud(
    TableName = "tb_funcao_sistema",
    DisplayName = "Funções do Sistema",
    CdSistema = "SEG",
    CdFuncao = "SEG_FM_FUNCAOSISTEMA",
    IsLegacyTable = false,
    SupportsBatchDelete = false         // PK composta
)]
public class FuncaoSistema : BaseMultiTenantEntity  // ← Herda TenantId + Id (Guid)
{
    [Key]
    [Column("cd_sistema", TypeName = "VarChar(10)")]
    [StringLength(10)]
    public string CdSistema { get; set; } = string.Empty;

    [Key]
    [Column("cd_funcao", TypeName = "VarChar(20)")]
    [StringLength(20)]
    public string CdFuncao { get; set; } = string.Empty;

    // ... campos de negócio ...

    // ❌ NÃO declarar Id, TenantId, Aud_* (herdados da base)
}
```

---

## 10. Passo 7 — Navigation Properties (Guia Completo)

### 10.1. Tipos de Navegação

| Tipo | Direção | Property | Exemplo |
|------|---------|----------|---------|
| **ManyToOne** | Esta entidade → Outra | `public virtual Entidade? Nav { get; set; }` | Fornecedor → UF |
| **OneToMany (Inverse)** | Outra entidade → Esta | `public virtual ICollection<Entidade> Navs { get; set; }` | Fornecedor ← Colaboradores |

### 10.2. ManyToOne (FK nesta tabela)

Quando a tabela tem uma coluna FK que aponta para outra tabela:

```sql
-- SQL: FK na tabela atual
[IdUf] INT NULL REFERENCES [dbo].[bas_ufs]([Id])
```

```csharp
// C#: Propriedade FK + Navigation
[Column("IdUf")]
[Display(Name = "ID UF")]
public int? IdUf { get; set; }                // ← Propriedade FK

[ForeignKey(nameof(IdUf))]                     // ← Aponta para a FK
public virtual BasUfs? Uf { get; set; }        // ← Navigation Property
```

**Regras:**
- O nome da Navigation **NÃO** deve ser igual ao nome da FK (usar `Uf`, não `IdUf`)
- Se FK é `NULL` → Navigation é nullable (`BasUfs?`)
- Se FK é `NOT NULL` → Navigation pode ser não-nullable, mas usar `?` é mais seguro
- Adicionar `virtual` para lazy loading

### 10.3. OneToMany / Inverse Navigation (FK em OUTRA tabela)

Quando **outra** tabela tem uma FK que aponta para esta:

```sql
-- SQL: Em cap_colaboradores_fornecedor existe:
-- [IdFornecedor] INT NOT NULL REFERENCES [dbo].[cap_fornecedores]([Id])
```

```csharp
// C#: Na entidade CapFornecedores (o lado "One"):
[InverseProperty(nameof(CapColaboradoresFornecedor.Fornecedor))]
public virtual ICollection<CapColaboradoresFornecedor> Colaboradores { get; set; }
    = new List<CapColaboradoresFornecedor>();
```

**Regras:**
- `[InverseProperty]` é **obrigatório** quando existem múltiplas FKs para a mesma tabela
- O `nameof(...)` aponta para a Navigation Property na entidade filho
- Sempre inicializar com `= new List<T>()`
- Usar `virtual` para lazy loading

### 10.4. Quando Gerar cada tipo?

| Informação no CREATE TABLE | O que gerar | Na entidade atual |
|---------------------------|-------------|-------------------|
| `FOREIGN KEY (IdUf) REFERENCES bas_ufs(Id)` | ManyToOne | `public virtual BasUfs? Uf { get; set; }` |
| Outra tabela tem `REFERENCES cap_fornecedores(Id)` | OneToMany (Inverse) | `public virtual ICollection<T> Items { get; set; }` |
| FK composta (múltiplas colunas) | Ignorar | Não gerar (complexo demais para auto-gen) |

### 10.5. Naming Conventions para Navigation

| Tipo de FK | Nome da Navigation | Exemplo |
|-----------|-------------------|---------|
| `IdUf` → `bas_ufs` | Remove prefixo `Id` → `Uf` | `public virtual BasUfs? Uf { get; set; }` |
| `IdFornecedor` → `cap_fornecedores` | Remove prefixo `Id` → `Fornecedor` | `public virtual CapFornecedores? Fornecedor { get; set; }` |
| `CdSistema` → `tb_sistema` | Remove prefixo `Cd` → `Sistema` | `public virtual Sistema? Sistema { get; set; }` |
| `FkCliente` → `clientes` | Remove prefixo `Fk` → `Cliente` | `public virtual Cliente? Cliente { get; set; }` |

---

## 11. Passo 8 — Gerar DTOs

### 11.1. DTO de Leitura (`{Entity}Dto`)

Contém **TODAS** as propriedades visíveis, incluindo PKs e campos de navegação:

```csharp
public sealed class CapFornecedoresDto
{
    public int Id { get; set; }                            // PK
    public Guid TenantId { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }
    public string? Cnpj { get; set; }
    public string? Cpf { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public int? IdUf { get; set; }
    public string? Cep { get; set; }
    public string? Contato { get; set; }
    public string? ContatoTelefone { get; set; }
    public string? ContatoEmail { get; set; }
    public bool Ativo { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    // =========================================================================
    // PROPRIEDADES DE NAVEGAÇÃO (campos de entidades relacionadas)
    // =========================================================================
    public string? UfNome { get; set; }                    // ← Vem do JOIN com BasUfs
    public string? UfSigla { get; set; }                   // ← Vem do JOIN com BasUfs
}
```

### 11.2. CreateRequest

| Campo | Incluir? | Condição |
|-------|---------|----------|
| PK IDENTITY / GUID | ❌ NÃO | Auto-gerada |
| PK Manual (string/int sem identity) | ✅ SIM | Informada pelo usuário, **required** |
| PK Composta (cada coluna) | ✅ SIM | Todas as colunas, **required** |
| Id (Guid herdado de BaseEntity) em PK composta | ❌ NÃO | Auto-gerado, ignorado |
| TenantId / IdSaaS | ❌ NÃO | Auto-preenchido |
| Campos de auditoria (`Aud_*`, `CreatedAtUtc`, etc.) | ❌ NÃO | Auto-preenchidos |
| `CreatedByUserId` / `UpdatedByUserId` | ❌ NÃO | Auto-preenchidos |
| Campos normais NOT NULL | ✅ SIM | Com validação required |
| Campos normais NULL | ✅ SIM | Sem validação required |
| Navigation Properties (`virtual`) | ❌ NÃO | Somente no Dto de leitura |
| Inverse Navigation (`ICollection<>`) | ❌ NÃO | Somente no Dto de leitura |

```csharp
public sealed class CreateCapFornecedoresRequest
{
    // ❌ Sem Id (IDENTITY auto-gerado)
    // ❌ Sem TenantId (auto-injetado)

    // ✅ Campos de negócio
    [Required]
    public string RazaoSocial { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }
    public string? Cnpj { get; set; }
    public string? Cpf { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public int? IdUf { get; set; }
    public string? Cep { get; set; }
    public string? Contato { get; set; }
    public string? ContatoTelefone { get; set; }
    public string? ContatoEmail { get; set; }
    public bool Ativo { get; set; } = true;

    // ❌ Sem CreatedAtUtc, UpdatedAtUtc (triggers ou Handler)
    // ❌ Sem CreatedByUserId, UpdatedByUserId (Handler)
}
```

### 11.3. UpdateRequest

| Campo | Incluir? | Motivo |
|-------|---------|--------|
| PK (qualquer tipo) | ❌ NÃO | Vem pela rota da API |
| Id (Guid herdado) | ❌ NÃO | Causa bug no AutoMapper |
| TenantId | ❌ NÃO | Imutável |
| Campos de auditoria | ❌ NÃO | Auto-preenchidos |
| Campos editáveis | ✅ SIM | O que o usuário pode alterar |

```csharp
public sealed class UpdateCapFornecedoresRequest
{
    // ❌ SEM Id, TenantId, auditoria

    // ✅ Apenas campos editáveis
    public string RazaoSocial { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }
    public string? Cnpj { get; set; }
    // ... demais campos editáveis ...
    public bool Ativo { get; set; }
}
```

> **⚠️ ATENÇÃO CRÍTICA:** Se o `Id` (Guid ou int) entrar no UpdateRequest, o AutoMapper pode sobrescrever `entity.Id` com o valor default (0 para int, Guid.Empty para Guid), causando `DbUpdateException`. Isto é uma armadilha de value types: `0 != null` e `Guid.Empty != null` ambos retornam `true`, então a condição `.Condition((src, dest, srcMember) => srcMember != null)` do AutoMapper **NÃO protege** value types.

---

## 12. Passo 9 — Gerar Validators (FluentValidation)

### 12.1. CreateValidator

```csharp
public sealed class CreateCapFornecedoresRequestValidator
    : AbstractValidator<CreateCapFornecedoresRequest>
{
    public CreateCapFornecedoresRequestValidator()
    {
        // ✅ Campos NOT NULL obrigatórios
        RuleFor(x => x.RazaoSocial)
            .NotEmpty().WithMessage("Razão Social é obrigatória.")
            .MaximumLength(255);

        // ✅ Campos NULL: apenas tamanho máximo
        RuleFor(x => x.NomeFantasia)
            .MaximumLength(255)
            .When(x => x.NomeFantasia != null);

        RuleFor(x => x.Cnpj)
            .MaximumLength(18)
            .When(x => x.Cnpj != null);

        RuleFor(x => x.Email)
            .MaximumLength(100)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));

        // ✅ FK nullable: valida apenas se preenchida
        RuleFor(x => x.IdUf)
            .GreaterThan(0).When(x => x.IdUf.HasValue)
            .WithMessage("UF inválida.");
    }
}
```

### 12.2. Regras de Validação por Tipo

| Tipo C# | NOT NULL | NULL | Exemplo |
|---------|----------|------|---------|
| `string` NOT NULL | `.NotEmpty().MaximumLength(n)` | — | RazaoSocial |
| `string` NULL | — | `.MaximumLength(n).When(x => x.Prop != null)` | NomeFantasia |
| `string` Email | `.EmailAddress().When(x => !IsNullOrEmpty(x.Email))` | — | Email |
| `int` FK NOT NULL | `.GreaterThan(0)` | — | FK obrigatória |
| `int?` FK NULL | — | `.GreaterThan(0).When(x => x.Prop.HasValue)` | IdUf |
| `Guid` FK NOT NULL | `.NotEqual(Guid.Empty)` | — | FK Guid obrigatória |
| `decimal` NOT NULL | `.GreaterThanOrEqualTo(0)` | — | Financeiro |
| `bool` | Sem validação | — | Ativo |
| `DateTime` NOT NULL | `.NotEmpty()` | — | Data obrigatória |
| PK Manual (Create) | `.NotEmpty()` | — | CdFuncao |

---

## 13. Passo 10 — Gerar AutoMapper Profile

```csharp
public sealed class CapFornecedoresProfile : Profile
{
    public CapFornecedoresProfile()
    {
        // DTO de leitura (com campos de navegação)
        CreateMap<CapFornecedores, CapFornecedoresDto>()
            .ForMember(d => d.UfNome, opt => opt.MapFrom(s => s.Uf != null ? s.Uf.Nome : null))
            .ForMember(d => d.UfSigla, opt => opt.MapFrom(s => s.Uf != null ? s.Uf.Sigla : null));

        // Create: Request → Entity
        CreateMap<CreateCapFornecedoresRequest, CapFornecedores>()
            .ForMember(d => d.Id, opt => opt.Ignore())             // ← PK Identity
            .ForMember(d => d.TenantId, opt => opt.Ignore())       // ← Tenant
            .ForMember(d => d.CreatedAtUtc, opt => opt.Ignore())   // ← Auditoria
            .ForMember(d => d.CreatedByUserId, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(d => d.UpdatedByUserId, opt => opt.Ignore())
            .ForMember(d => d.Uf, opt => opt.Ignore())             // ← Navigation
            .ForMember(d => d.Colaboradores, opt => opt.Ignore())  // ← Inverse Navigation
            .ForMember(d => d.Contratos, opt => opt.Ignore());     // ← Inverse Navigation

        // Update: Request → Entity (PATCH semântico)
        CreateMap<UpdateCapFornecedoresRequest, CapFornecedores>()
            .ForMember(d => d.Id, opt => opt.Ignore())             // ← CRÍTICO
            .ForMember(d => d.TenantId, opt => opt.Ignore())
            .ForMember(d => d.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(d => d.CreatedByUserId, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(d => d.UpdatedByUserId, opt => opt.Ignore())
            .ForMember(d => d.Uf, opt => opt.Ignore())
            .ForMember(d => d.Colaboradores, opt => opt.Ignore())
            .ForMember(d => d.Contratos, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition(
                (src, dest, srcMember) => srcMember != null));     // ← PATCH
    }
}
```

### 13.1. Campos que SEMPRE devem ser ignorados no Mapper

| Campo | Onde ignorar | Motivo |
|-------|-------------|--------|
| `Id` (qualquer tipo) | Create + Update | Auto-gerado (Identity/Guid) |
| `TenantId` / `IdSaaS` | Create + Update | Injetado no Handler |
| Campos `Aud_*`, `CreatedAtUtc`, etc. | Create + Update | Preenchidos no Handler ou Trigger |
| `CreatedByUserId`, `UpdatedByUserId` | Create + Update | Preenchidos no Handler |
| Navigation Properties (`virtual`) | Create + Update | EF gerencia relacionamentos via FK |
| Inverse Navigation (`ICollection<>`) | Create + Update | Coleções não vêm do Request |
| PKs compostas | **Somente** Update | No Create vêm do Request; no Update vêm da rota |

---

## 14. Passo 11 — Gerar EF Configuration

```csharp
public sealed class CapFornecedoresConfiguration
    : IEntityTypeConfiguration<CapFornecedores>
{
    public void Configure(EntityTypeBuilder<CapFornecedores> builder)
    {
        // Tabela
        builder.ToTable("cap_fornecedores", "dbo");

        // Chave Primária
        builder.HasKey(e => e.Id);

        // Propriedades
        builder.Property(e => e.RazaoSocial)
            .HasColumnName("RazaoSocial")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Cnpj)
            .HasColumnName("Cnpj")
            .HasMaxLength(18);

        // ... demais propriedades ...

        // ★ Índice Único por Tenant (gerado pelo [Unique])
        builder.HasIndex(e => new { e.TenantId, e.Cnpj },
            "UX_CapFornecedores_Tenant_Cnpj")
            .IsUnique()
            .HasFilter("Cnpj IS NOT NULL");      // ← Porque Cnpj é nullable

        // ★ Relacionamentos
        builder.HasOne(e => e.Uf)
            .WithMany()
            .HasForeignKey(e => e.IdUf)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

### 14.1. PK Composta no EF Config

```csharp
// PK Simples:
builder.HasKey(e => e.Id);

// PK Composta (ORDEM importa — deve coincidir com a PK do banco):
builder.HasKey(e => new { e.CdSistema, e.CdFuncao });
```

---

## 15. Passo 12 — Gerar Repository

### 15.1. Regras Críticas

```csharp
// ❌ NUNCA:
_context.Entry(entity).State = EntityState.Modified;  // Força UPDATE em tudo → erro

// ✅ CORRETO:
await _context.SaveChangesAsync(ct);  // Change Tracker detecta automaticamente
```

### 15.2. GetByIdAsync

```csharp
// PK Simples (int Identity):
public async Task<CapFornecedores?> GetByIdAsync(int id, CancellationToken ct)
    => await _dbSet.FindAsync(new object[] { id }, ct);

// PK Simples com navegação (Include):
public async Task<CapFornecedores?> GetByIdAsync(int id, CancellationToken ct)
    => await _dbSet
        .Include(e => e.Uf)
        .FirstOrDefaultAsync(e => e.Id == id, ct);

// PK Composta (ordem deve coincidir com HasKey):
public async Task<FuncaoSistema?> GetByIdAsync(
    string cdSistema, string cdFuncao, CancellationToken ct)
    => await _dbSet.FindAsync(new object[] { cdSistema, cdFuncao }, ct);
```

---

## 16. Passo 13 — Gerar Frontend (Formulário)

### 16.1. Regras do Formulário HTML

| Tipo de campo | `required`? | `readonly` em edit? | Badge | Aparece? |
|--------------|------------|--------------------|----|---------|
| PK auto (IDENTITY/GUID) | N/A | N/A | N/A | ❌ NÃO (hidden) |
| PK manual (string/int) | ✅ **SEMPRE** | ✅ SIM (via JS) | `PK` | ✅ SIM |
| PK composta (cada coluna) | ✅ **SEMPRE** | ✅ SIM (via JS) | `PK` | ✅ SIM |
| Campo NOT NULL | ✅ SIM | ❌ | | ✅ SIM |
| Campo NULL | ❌ NÃO | ❌ | | ✅ SIM |
| TenantId | N/A | N/A | N/A | ❌ NÃO |
| Campos de auditoria | N/A | N/A | N/A | ❌ NÃO |
| Id (Guid herdado) | N/A | N/A | N/A | ❌ NÃO (hidden) |
| FK com Select2 | ❌/✅ (depende) | ❌ | | ✅ SIM (dropdown) |
| Bool (Ativo) | ❌ | ❌ | | ✅ SIM (toggle) |

### 16.2. Select2 para FKs (Lookup)

Quando um campo é FK e a entidade referenciada tem `GenerateLookup = true`:

```html
<!-- Select2 Ajax para FK -->
<div class="col-md-6 mb-3">
    <label for="IdUf" class="form-label">
        UF
    </label>
    <select class="form-control select2-ajax" id="IdUf" name="IdUf"
            data-select2-url="/api/common/basufs/lookup"
            data-value-field="id"
            data-text-field="nome"
            style="width: 100%">
        <option value="">Selecione...</option>
    </select>
</div>
```

**JavaScript do Select2:**

```javascript
// Inicialização automática no CrudBase.js
$('.select2-ajax').each(function() {
    var $select = $(this);
    var endpoint = $select.data('select2-url');   // ← URL do lookup
    var valueField = $select.data('value-field'); // ← "id"
    var textField = $select.data('text-field');   // ← "nome" ou "razaoSocial"

    $select.select2({
        ajax: {
            url: window.AppConfig.buildApiUrl(endpoint),
            dataType: 'json',
            delay: 300,
            data: function(params) {
                return { term: params.term, page: params.page || 1 };
            },
            processResults: function(data) {
                return {
                    results: data.results.map(function(item) {
                        return { id: item[valueField], text: item[textField] };
                    }),
                    pagination: { more: data.pagination.more }
                };
            }
        },
        minimumInputLength: 0,
        allowClear: true,
        placeholder: 'Selecione...'
    });
});
```

---

## 17. Passo 14 — Rotas da API

### 17.1. PK Simples (int/Guid)

```
GET    /api/{module}/{entity}              → GetPaged (com filtros)
GET    /api/{module}/{entity}/{id}         → GetById
POST   /api/{module}/{entity}              → Create (body: CreateRequest)
PUT    /api/{module}/{entity}/{id}         → Update (body: UpdateRequest)
DELETE /api/{module}/{entity}/{id}         → Delete
POST   /api/{module}/{entity}/batch-delete → BatchDelete (body: List<string>)
GET    /api/{module}/{entity}/lookup       → Lookup (para Select2)
PATCH  /api/{module}/{entity}/{id}/toggle-ativo → ToggleAtivo (se tem campo Ativo)
```

### 17.2. PK Composta

```
GET    /api/{module}/{entity}                      → GetPaged
GET    /api/{module}/{entity}/{pk1}/{pk2}           → GetById
POST   /api/{module}/{entity}                      → Create (body inclui PKs)
PUT    /api/{module}/{entity}/{pk1}/{pk2}           → Update (body SEM PKs)
DELETE /api/{module}/{entity}/{pk1}/{pk2}           → Delete
❌     BatchDelete NÃO disponível para PK composta
PATCH  /api/{module}/{entity}/{pk1}/{pk2}/toggle-ativo → ToggleAtivo
```

---

## 18. Exemplo Completo A — PK Identity + Navegação + Lookup

### 18.1. Input: CREATE TABLE

```sql
CREATE TABLE [dbo].[cap_fornecedores] (
    [Id]               INT              NOT NULL IDENTITY(1,1),
    [TenantId]         UNIQUEIDENTIFIER NOT NULL,
    [RazaoSocial]      NVARCHAR(255)    NOT NULL,
    [NomeFantasia]     NVARCHAR(255)    NULL,
    [Cnpj]             NVARCHAR(18)     NULL,
    [Cpf]              NVARCHAR(14)     NULL,
    [Email]            NVARCHAR(100)    NULL,
    [Telefone]         NVARCHAR(20)     NULL,
    [Endereco]         NVARCHAR(500)    NULL,
    [Numero]           NVARCHAR(20)     NULL,
    [Complemento]      NVARCHAR(200)    NULL,
    [Bairro]           NVARCHAR(100)    NULL,
    [Cidade]           NVARCHAR(100)    NULL,
    [IdUf]             INT              NULL,
    [Cep]              NVARCHAR(10)     NULL,
    [Contato]          NVARCHAR(100)    NULL,
    [ContatoTelefone]  NVARCHAR(20)     NULL,
    [ContatoEmail]     NVARCHAR(100)    NULL,
    [Ativo]            BIT              NOT NULL DEFAULT 1,
    [CreatedAtUtc]     DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME(),
    [CreatedByUserId]  UNIQUEIDENTIFIER NULL,
    [UpdatedAtUtc]     DATETIME2(3)     NOT NULL DEFAULT SYSUTCDATETIME(),
    [UpdatedByUserId]  UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_cap_fornecedores] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_cap_fornecedores_uf] FOREIGN KEY ([IdUf])
        REFERENCES [dbo].[bas_ufs]([Id]),
    CONSTRAINT [UX_cap_fornecedores_tenant_cnpj]
        UNIQUE ([TenantId], [Cnpj])
);

-- Trigger de auditoria existe
CREATE TRIGGER trg_cap_fornecedores_audit ON [dbo].[cap_fornecedores] ...
```

### 18.2. Análise

| Item | Valor | Decisão |
|------|-------|---------|
| PK | `Id INT IDENTITY(1,1)` | **PK_SIMPLES_IDENTITY** → Não aparece nos DTOs |
| Herança | Sem `idsaas` / `Aud_*` | **Standalone** (sem BaseEntity) |
| TenantId | Sim, explícito | Declarar na entidade |
| Auditoria | `CreatedAtUtc`, `UpdatedAtUtc` + trigger | `[HasDatabaseTriggers]` |
| Unique | `UNIQUE (TenantId, Cnpj)` | `[Unique(UniqueScope.Tenant, "CNPJ")]` |
| FK | `IdUf → bas_ufs` | Navigation `Uf` + `[ForeignKey]` |
| Lookup | Tabela de referência | `GenerateLookup = true` + `[LookupKey]` + `[LookupText]` |
| Ativo | `BIT NOT NULL DEFAULT 1` | Toggle endpoint |
| BatchDelete | PK simples | `SupportsBatchDelete = true` |

### 18.3. Output: Entidade Gerada → Ver CapFornecedores na Seção 9.1

---

## 19. Exemplo Completo B — PK Composta (Legado)

### 19.1. Input: CREATE TABLE

```sql
CREATE TABLE [dbo].[tb_funcao_sistema] (
    [cd_funcao]         VARCHAR(20)     NOT NULL,
    [cd_sistema]        VARCHAR(10)     NOT NULL,
    [dc_funcao]         VARCHAR(100)    NULL,
    [dc_modulo]         VARCHAR(50)     NULL,
    [descricao_modulo]  NVARCHAR(200)   NULL,
    [idsaas]            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [aud_dt_cadastro]   DATETIME        NULL DEFAULT GETDATE(),
    [aud_dt_alteracao]  DATETIME        NULL,
    CONSTRAINT [PK_tb_funcao_sistema] PRIMARY KEY ([cd_sistema], [cd_funcao]),
    CONSTRAINT [FK_funcao_sistema] FOREIGN KEY ([cd_sistema])
        REFERENCES [dbo].[tb_sistema]([cd_sistema])
);
```

### 19.2. Análise

| Item | Valor | Decisão |
|------|-------|---------|
| PK | `cd_sistema` + `cd_funcao` | **PK_COMPOSTA** |
| Herança | Tem `idsaas` | `BaseMultiTenantEntity` |
| Auditoria | `aud_dt_cadastro`, `aud_dt_alteracao` | Excluir dos DTOs |
| FK | `cd_sistema → tb_sistema` | Navigation: `Sistema` |
| BatchDelete | PK composta | `SupportsBatchDelete = false` |
| Formulário | PKs no form | `required` + `data-is-pk="true"` + readonly em edição |

### 19.3. Output

```csharp
[Table("tb_funcao_sistema", Schema = "dbo")]
[GenerateCrud(
    TableName = "tb_funcao_sistema",
    DisplayName = "Funções do Sistema",
    CdSistema = "SEG",
    CdFuncao = "SEG_FM_FUNCAOSISTEMA",
    SupportsBatchDelete = false,
    GenerateLookup = true
)]
public class FuncaoSistema : BaseMultiTenantEntity
{
    [Key]
    [Column("cd_sistema", TypeName = "VarChar(10)")]
    [StringLength(10)]
    [Display(Name = "Código do Sistema")]
    public string CdSistema { get; set; } = string.Empty;

    [Key]
    [Column("cd_funcao", TypeName = "VarChar(20)")]
    [StringLength(20)]
    [Display(Name = "Código da Função")]
    public string CdFuncao { get; set; } = string.Empty;

    [Column("dc_funcao", TypeName = "VarChar(100)")]
    [StringLength(100)]
    [Display(Name = "Descrição da Função")]
    public string? DcFuncao { get; set; }

    [Column("dc_modulo", TypeName = "VarChar(50)")]
    [StringLength(50)]
    [Display(Name = "Descrição do Módulo")]
    public string? DcModulo { get; set; }

    [Column("descricao_modulo", TypeName = "NVarChar(200)")]
    [StringLength(200)]
    [Display(Name = "Descrição Completa do Módulo")]
    public string? DescricaoModulo { get; set; }

    // ❌ idsaas → herdado de BaseMultiTenantEntity
    // ❌ aud_dt_cadastro → auditoria (Handler)
    // ❌ aud_dt_alteracao → auditoria (Handler)

    // =========================================================================
    // Navigation Properties
    // =========================================================================

    [ForeignKey(nameof(CdSistema))]
    public virtual Sistema? Sistema { get; set; }
}
```

---

## 20. Checklist de Validação

Após gerar todos os artefatos, verifique:

### Entidade
- [ ] Classe usa herança correta OU é standalone
- [ ] `[Table]` com nome e schema corretos
- [ ] `[Key]` em todas as colunas PK
- [ ] `[DatabaseGenerated(Identity)]` se é IDENTITY
- [ ] `[Column]` com nome original + TypeName em todas as propriedades
- [ ] `[StringLength]` em todos os campos texto com tamanho definido
- [ ] `[Display(Name = "...")]` em todas as propriedades
- [ ] `[LookupKey]` na PK (se `GenerateLookup = true`)
- [ ] `[LookupText]` nos campos de exibição (se `GenerateLookup = true`)
- [ ] `[Unique(...)]` nos campos com UNIQUE constraint
- [ ] `[HasDatabaseTriggers]` se a tabela tem triggers
- [ ] Tipos C# corretos para cada coluna
- [ ] Campos de auditoria e TenantId **NÃO duplicados** (se herdados)
- [ ] Navigation Properties com `[ForeignKey]` e `virtual`
- [ ] Inverse Navigation com `[InverseProperty]` e `ICollection<>`

### DTOs
- [ ] CreateRequest exclui PK auto-gerada, TenantId, auditoria, navigations
- [ ] CreateRequest inclui PKs manuais/compostas
- [ ] UpdateRequest exclui TODA PK, Id, TenantId, auditoria, navigations
- [ ] Dto inclui campos de navegação (UfNome, etc.)

### Validators
- [ ] CreateValidator valida PKs compostas como required
- [ ] UpdateValidator **NÃO** valida PKs
- [ ] Strings NOT NULL: `.NotEmpty()`
- [ ] Strings NULL: `.MaximumLength().When()`
- [ ] FKs nullable: `.GreaterThan(0).When(x => x.Prop.HasValue)`

### AutoMapper
- [ ] Dto map inclui `.ForMember` para campos de navegação
- [ ] Create ignora Id, TenantId, auditoria, navigations, inverse navigations
- [ ] Update ignora Id, TenantId, PKs compostas, auditoria, navigations, inverse
- [ ] Update tem `.ForAllMembers` com condition null-check

### EF Config
- [ ] HasKey correto (simples ou composto)
- [ ] Propriedades com `IsRequired()` e `HasMaxLength()` corretos
- [ ] Índices únicos configurados (com `.HasFilter` se campo nullable)
- [ ] Relacionamentos com `OnDelete(DeleteBehavior.Restrict)`

### Repository
- [ ] `UpdateAsync` **NÃO** usa `EntityState.Modified`
- [ ] `GetByIdAsync` recebe parâmetros corretos
- [ ] Includes para Navigation Properties quando necessário

### Frontend
- [ ] PKs manuais/compostas com `required` e `data-is-pk="true"`
- [ ] PKs auto-geradas **NÃO** aparecem no formulário
- [ ] FKs com Select2 (`data-select2-url`, `data-value-field`, `data-text-field`)
- [ ] Campo Ativo com toggle switch
- [ ] JavaScript torna PKs readonly em edição
- [ ] Validação de campos vazios no `beforeSubmit`

---

## 21. Erros Comuns (Armadilhas)

| Erro | Causa | Solução |
|------|-------|---------|
| `DbUpdateException` no Update | AutoMapper sobrescreve `Id` com default (0/Guid.Empty) | Ignorar `Id` no mapper do Update |
| `EntityState.Modified` causa erro | Força UPDATE em PK/Id | Usar Change Tracker nativo |
| PK composta salva vazia | Falta `required` no HTML + validação no JS | Adicionar `required` + `data-is-pk` |
| TenantId aparece no form | Não foi excluído dos DTOs | Filtrar campos especiais |
| Campo Aud_* editável | Não foi excluído dos DTOs | Filtrar campos de auditoria |
| `ToHashSet()` não compila | Source Generator roda em .NET Standard 2.0 | Usar `new HashSet<string>(...)` |
| Select2 não carrega dados | URL sem CamelCase no body | `PropertyNamingPolicy = CamelCase` |
| Navigation Property null | Falta `.Include()` no Repository | Adicionar `.Include(e => e.Uf)` |
| Inverse Navigation ciclo infinito | JSON serialization loop | Ignorar inverse no DTO / usar `.ReferenceLoopHandling` |
| Trigger + EF Core conflito | EF não sabe que trigger modifica dados | Adicionar `[HasDatabaseTriggers]` |
| Unique constraint viola ao editar | Validação não exclui registro atual | `UniqueValidationBehavior` recebe `EntityId` para excluir self |
| Select2 não mostra texto correto | `data-text-field` não bate com JSON | Usar nomes camelCase que coincidem com Dto |

---

## 22. Glossário de Termos

| Termo | Significado |
|-------|------------|
| **PK** | Primary Key — chave primária da tabela |
| **PK Composta** | Primary Key com 2+ colunas |
| **IDENTITY** | Coluna com auto-incremento (1, 2, 3...) |
| **FK** | Foreign Key — chave estrangeira |
| **DTO** | Data Transfer Object — objeto para transferência de dados |
| **BaseEntity** | Classe base que injeta `Id` (Guid) automaticamente |
| **BaseMultiTenantEntity** | BaseEntity + `TenantId` (idsaas) |
| **TenantId / IdSaaS** | Identificador da empresa no multi-tenant |
| **IAEC** | Include, Alter, Exclude, Consult — padrão de permissões |
| **CrudTool** | Ferramenta que gera frontend (View + JS) |
| **Source Generator** | Roslyn analyzer que gera código C# em tempo de compilação |
| **Lookup** | Endpoint que retorna dados simplificados para Select2/dropdown |
| **Select2** | Plugin jQuery para dropdowns com busca Ajax |
| **Navigation Property** | Propriedade de navegação EF Core (ManyToOne) |
| **Inverse Navigation** | Coleção de navegação inversa EF Core (OneToMany) |
| **[LookupKey]** | Atributo que marca campo como valor do Select2 |
| **[LookupText]** | Atributo que marca campo como texto do Select2 |
| **[Unique]** | Atributo que cria índice único + validação automática |
| **[HasDatabaseTriggers]** | Informa EF Core sobre triggers no banco |

---

*Fim do manual. Versão 6.0 — Março 2026.*
