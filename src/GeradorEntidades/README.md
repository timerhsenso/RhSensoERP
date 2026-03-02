# GeradorFrontendEnterprise

Gerador automatizado de CRUD frontend para o **RhSensoERP**, transformando schemas de tabelas SQL Server em código pronto para uso: Controllers MVC, ViewModels, Views Razor, JavaScript (DataTables) e CSS — reduzindo o desenvolvimento de um CRUD completo de **~2 dias para ~30 minutos**.

O app funciona como uma aplicação web local com um wizard de 5 passos que guia o desenvolvedor desde a seleção da tabela até o download do ZIP com todos os arquivos gerados.

---

## Sumário

- [Pré-requisitos](#pré-requisitos)
- [Instalação e Configuração](#instalação-e-configuração)
- [Como Rodar](#como-rodar)
- [Fluxo do Wizard (Passo a Passo)](#fluxo-do-wizard-passo-a-passo)
- [Exemplo Completo: Gerando uma Entidade](#exemplo-completo-gerando-uma-entidade)
- [Arquitetura do Projeto](#arquitetura-do-projeto)
- [Estrutura de Diretórios](#estrutura-de-diretórios)
- [Templates Scriban](#templates-scriban)
- [Configuração da API de Manifesto](#configuração-da-api-de-manifesto)
- [Testes](#testes)
- [Troubleshooting](#troubleshooting)
- [Roadmap](#roadmap)

---

## Pré-requisitos

| Dependência         | Versão Mínima | Observação                                    |
|---------------------|---------------|-----------------------------------------------|
| .NET SDK            | 8.0           | `dotnet --version` para verificar              |
| SQL Server          | 2019+         | Instância local ou remota com acesso de leitura |
| Visual Studio 2022  | 17.8+         | Ou VS Code com C# Dev Kit                     |
| Git                 | 2.x           | Para clonar o repositório                      |

### Pacotes NuGet utilizados

| Pacote                       | Versão  | Finalidade                                |
|------------------------------|---------|-------------------------------------------|
| `Scriban`                    | 6.5.2   | Motor de templates para geração de código |
| `System.Data.SqlClient`     | 4.9.0   | Conexão com SQL Server (leitura de schema)|
| `Newtonsoft.Json`            | 13.0.4  | Serialização JSON (ManifestClient)        |
| `xUnit`                     | 2.9.3   | Framework de testes unitários             |
| `xunit.runner.visualstudio` | 3.1.5   | Runner de testes para Visual Studio       |
| `Moq`                       | 4.20.72 | Mocking para testes unitários             |

---

## Instalação e Configuração

### 1. Clonar o repositório

```bash
git clone https://github.com/timerhsenso/GeradorFrontendEnterprise.git
cd GeradorFrontendEnterprise
```

### 2. Restaurar pacotes

```bash
dotnet restore
```

### 3. Configurar a connection string do SQL Server

Edite o `appsettings.Development.json` e adicione a seção `ConnectionStrings` e `ManifestApi`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=SUA_BASE;User Id=sa;Password=SUA_SENHA;TrustServerCertificate=true;"
  },
  "ManifestApi": {
    "BaseUrl": "https://localhost:7086"
  }
}
```

> **Nota:** Se a `ManifestApi:BaseUrl` apontar para `localhost` ou estiver vazia, o sistema usará dados mock automaticamente. Isso é útil para testes sem a API do RhSensoERP rodando.

### 4. Compilar

```bash
dotnet build
```

---

## Como Rodar

### Via CLI

```bash
dotnet run
```

O app inicia em **http://localhost:5201** (HTTP) ou **https://localhost:7086** (HTTPS).

### Via Visual Studio

1. Abrir `GeradorFrontendEnterprise.sln`
2. Definir como projeto de inicialização
3. Pressionar `F5` (Debug) ou `Ctrl+F5` (sem debug)

### Via IIS Express

O `launchSettings.json` já está configurado com IIS Express na porta `44321` (HTTPS) e `38426` (HTTP).

---

## Fluxo do Wizard (Passo a Passo)

Acesse `http://localhost:5201` e clique em **"Iniciar Wizard"**. O processo segue 5 passos:

```
  ┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐
  │  Step 1  │───→│  Step 2  │───→│  Step 3  │───→│  Step 4  │───→│  Step 5  │
  │ Selecionar│   │ Resolver │    │Configurar│    │  Gerar   │    │ Download │
  │ Entidade │    │Conflitos │    │  Layout  │    │  Código  │    │   ZIP    │
  └──────────┘    └──────────┘    └──────────┘    └──────────┘    └──────────┘
       │                │                │                │               │
    Manifest +       Compara          Define grid      Renderiza       Empacota
    Schema DB       DB vs API       e formulário     templates        e baixa
```

### Step 1 — Selecionar Entidade

**URL:** `/wizard/step1`

- Digite o identificador da entidade (ex: `TreTiposTreinamento`)
- O sistema busca o manifesto da entidade na API (ou usa mock em dev)
- Também lê o schema da tabela diretamente do SQL Server via `INFORMATION_SCHEMA`

**O que acontece internamente:**

1. `OrchestratorService.InitializeWizardAsync()` é chamado
2. `HttpManifestClient.GetEntityManifestAsync()` busca metadados da API
3. `SqlServerSchemaReader.ReadTableSchemaAsync()` lê colunas, PKs, FKs, índices e extended properties
4. Uma configuração padrão (`WizardConfig`) é sugerida

### Step 2 — Resolver Conflitos

**URL:** `/wizard/step2`

- O sistema compara campo a campo entre o que o banco de dados tem e o que o manifesto da API diz
- Se existirem divergências, elas são listadas em uma tabela com opções de resolução
- Se não houver conflitos, o step é pulado automaticamente

**Tipos de conflitos detectados:**

| Tipo                    | Descrição                                              |
|-------------------------|--------------------------------------------------------|
| `FieldNotInDatabase`    | Campo existe no manifesto mas não na tabela            |
| `FieldNotInManifest`    | Coluna existe na tabela mas não no manifesto           |
| `TypeMismatch`          | Tipo CLR divergente entre banco e manifesto            |
| `NullabilityMismatch`   | Banco aceita NULL mas manifesto diz Required, ou vice-versa |
| `PrimaryKeyMismatch`    | Definição de PK divergente                             |
| `ForeignKeyMismatch`    | Definição de FK divergente                             |

**Opções de resolução para cada conflito:**

| Opção            | Comportamento                                           |
|------------------|---------------------------------------------------------|
| Usar Banco       | Prevalece a definição do SQL Server                     |
| Usar Manifesto   | Prevalece a definição da API                            |
| Ignorar          | O campo é excluído da geração                           |
| Revisão Manual   | Marcado para revisão posterior (gera com warning)       |

### Step 3 — Configurar Layout

**URL:** `/wizard/step3`

- Define o nome da entidade, módulo (ex: `TRE`, `RHU`, `SEG`) e nome de exibição
- Configura quais colunas aparecem na grid (DataTable)
- Configura quais campos aparecem no formulário
- Define tipos de input, obrigatoriedade, largura de colunas, etc.

### Step 4 — Gerar Código

**URL:** `/wizard/step4`

- Revisão final antes da geração
- Ao clicar em **"Gerar Código"**, o `GeneratorService.GenerateAsync()` executa:
  1. Gera o **Controller** (`.generated.cs`)
  2. Gera o **ViewModel** (`.generated.cs`)
  3. Gera a **Razor View** (`Index.generated.cshtml`)
  4. Gera o **JavaScript** (`.generated.js`) com configuração do DataTable
  5. Gera o **CSS** (`.generated.css`)
  6. Gera arquivos **customizáveis** (`.custom.cs`, `.custom.js`) — partial classes que não são sobrescritas na regeneração

### Step 5 — Download

**URL:** `/wizard/step5`

- Exibe mensagem de sucesso
- Disponibiliza o botão **"Baixar Código (ZIP)"** com todos os arquivos gerados

---

## Exemplo Completo: Gerando uma Entidade

### Cenário

Gerar o CRUD frontend para a tabela `tre_tipos_treinamento` do módulo **Treinamento (TRE)**.

### Passo a passo

**1. Inicie o app:**

```bash
cd GeradorFrontendEnterprise
dotnet run
```

**2. Acesse o browser:** `http://localhost:5201`

**3. Step 1:** Digite `TreTiposTreinamento` no campo "Entidade" e clique em "Próximo"

**4. Step 2:** Se houver conflitos entre banco e manifesto, resolva cada um e avance. Se não houver conflitos, avance direto.

**5. Step 3:** Configure:
   - **Módulo:** `TRE`
   - **Nome da Entidade:** `TreTiposTreinamento`
   - Selecione colunas da grid e campos do formulário

**6. Step 4:** Clique em "Gerar Código"

**7. Step 5:** Clique em "Baixar Código (ZIP)"

### Resultado esperado no ZIP

```
TreTiposTreinamento/
├── TreTiposTreinamentoController.generated.cs    ← Controller com CRUD actions
├── TreTiposTreinamentoController.custom.cs        ← Partial class para customizações
├── TreTiposTreinamentoViewModel.generated.cs      ← ViewModel com propriedades e validação
├── TreTiposTreinamentoViewModel.custom.cs         ← Partial class para props extras
├── Index.generated.cshtml                          ← View com DataTable e formulário
├── tretipostreinamento.generated.js                ← JS: DataTable init, CRUD via AJAX
├── tretipostreinamento.generated.css               ← CSS: estilos da entidade
└── tretipostreinamento.custom.js                   ← JS para customizações
```

### Integração com o RhSensoERP

Após o download, copie os arquivos para a estrutura do projeto destino:

```
src/RhSensoERP.Web/
├── Areas/TRE/
│   ├── Controllers/
│   │   └── TreTiposTreinamentoController.cs
│   ├── Models/
│   │   └── TreTiposTreinamentoViewModel.cs
│   └── Views/TreTiposTreinamento/
│       └── Index.cshtml
└── wwwroot/
    ├── js/tre/
    │   └── tretipostreinamento.js
    └── css/tre/
        └── tretipostreinamento.css
```

---

## Arquitetura do Projeto

O projeto segue **Clean Architecture** com separação clara entre camadas:

```
┌─────────────────────────────────────────────┐
│                  Controllers                │  ← Apresentação (Wizard UI)
│            WizardController.cs              │
├─────────────────────────────────────────────┤
│                  Services                   │  ← Lógica de negócio
│    OrchestratorService  │  GeneratorService │
├─────────────────────────────────────────────┤
│                Core (Models + Contracts)     │  ← Domínio (sem dependências)
│   TableSchema, EntityManifest, WizardConfig │
│   ISchemaReader, IManifestClient, etc.      │
├─────────────────────────────────────────────┤
│               Infrastructure                │  ← Implementações concretas
│  SqlServerSchemaReader │ HttpManifestClient  │
│         ScribanTemplateEngine               │
└─────────────────────────────────────────────┘
```

### Fluxo de dados

```
[SQL Server] ───→ SchemaReader ───→ TableSchema ──┐
                                                   ├─→ OrchestratorService ─→ GeneratorService ─→ ZIP
[API Manifesto] ─→ ManifestClient ─→ EntityManifest┘
                                                        ↑
[Wizard UI] ─────→ WizardController ─→ WizardConfig ───┘
```

### Interfaces (Contratos)

| Interface              | Implementação              | Responsabilidade                              |
|------------------------|----------------------------|-----------------------------------------------|
| `ISchemaReader`        | `SqlServerSchemaReader`    | Lê schema de tabelas do SQL Server            |
| `IManifestClient`      | `HttpManifestClient`       | Obtém metadados de entidades via HTTP          |
| `ITemplateEngine`      | `ScribanTemplateEngine`    | Renderiza templates Scriban                   |
| `IGeneratorService`    | `GeneratorService`         | Gera e empacota arquivos de código            |
| `IOrchestratorService` | `OrchestratorService`      | Coordena o fluxo completo do wizard           |

---

## Estrutura de Diretórios

```
GeradorFrontendEnterprise/
│
├── Core/                            # Domínio — zero dependências externas
│   ├── Contracts/                   # Interfaces
│   │   ├── ISchemaReader.cs         #   Leitura de schema SQL Server
│   │   ├── IManifestClient.cs       #   Comunicação com API de manifesto
│   │   ├── ITemplateEngine.cs       #   Motor de templates Scriban
│   │   ├── IGeneratorService.cs     #   Geração de código
│   │   └── IOrchestratorService.cs  #   Orquestração do wizard
│   ├── Enums/
│   │   └── SqlDataType.cs           # SqlDataType, ConflictType, FormInputType, etc.
│   └── Models/
│       ├── TableSchema.cs           # Schema completo de tabela (colunas, PKs, FKs)
│       ├── EntityManifest.cs        # Metadados da entidade via API
│       ├── WizardConfig.cs          # Configuração do wizard (grid + form layout)
│       ├── FieldConfigs.cs          # GridFieldConfig, FormFieldConfig, layouts
│       ├── GenerationResult.cs      # Resultado da geração (arquivos, erros, stats)
│       └── Conflict.cs              # Conflitos DB vs Manifesto
│
├── Infrastructure/                  # Implementações concretas
│   ├── ManifestClient/
│   │   └── HttpManifestClient.cs    # Consome API REST (com fallback para mock)
│   ├── SchemaReader/
│   │   └── SqlServerSchemaReader.cs # Lê INFORMATION_SCHEMA + Extended Properties
│   └── TemplateEngine/
│       └── ScribanTemplateEngine.cs # Renderiza templates .scriban
│
├── Services/                        # Lógica de negócio
│   ├── Generator/
│   │   └── GeneratorService.cs      # Gera Controller, ViewModel, View, JS, CSS + ZIP
│   └── Orchestrator/
│       └── OrchestratorService.cs   # Coordena wizard: init, conflitos, validação, geração
│
├── Controllers/
│   ├── HomeController.cs            # Página inicial
│   └── WizardController.cs          # Wizard Steps 1-5 + Download
│
├── Views/
│   ├── Home/Index.cshtml            # Landing page com botão "Iniciar Wizard"
│   ├── Wizard/
│   │   ├── Step1.cshtml             # Seleção de entidade
│   │   ├── Step2.cshtml             # Resolução de conflitos
│   │   ├── Step3.cshtml             # Configuração de layout
│   │   ├── Step4.cshtml             # Confirmação e geração
│   │   └── Step5.cshtml             # Download do ZIP
│   └── Shared/_Layout.cshtml        # Layout base
│
├── Templates/                       # Templates Scriban
│   ├── ControllerTemplate.scriban
│   ├── ViewModelTemplate.scriban
│   ├── RazorViewTemplate.scriban
│   └── JavaScriptTemplate.scriban
│
├── Tests/Unit/                      # Testes unitários (xUnit + Moq)
│   ├── SchemaReaderTests.cs         # 271 linhas
│   ├── GeneratorServiceTests.cs     # 124 linhas
│   └── OrchestratorServiceTests.cs  # 136 linhas
│
├── Program.cs                       # Configuração DI + pipeline HTTP
├── appsettings.json
├── appsettings.Development.json
├── GeradorFrontendEnterprise.csproj
└── GeradorFrontendEnterprise.sln
```

---

## Templates Scriban

Os templates ficam em `Templates/` e usam a sintaxe [Scriban](https://github.com/scriban/scriban).

### Variáveis disponíveis

| Variável                      | Tipo     | Descrição                                  |
|-------------------------------|----------|--------------------------------------------|
| `{{ namespace }}`             | string   | Namespace do código gerado                 |
| `{{ entity_name }}`           | string   | Nome da entidade (PascalCase)              |
| `{{ entity_name_lower }}`     | string   | Nome da entidade (lowercase)               |
| `{{ entity_display_name }}`   | string   | Nome amigável para exibição                |
| `{{ generation_date }}`       | datetime | Data/hora da geração                       |
| `{{ fields }}`                | array    | Lista de campos da entidade                |

### Propriedades de cada `field`

| Propriedade       | Tipo   | Descrição                        |
|-------------------|--------|----------------------------------|
| `field.name`      | string | Nome do campo (PascalCase)       |
| `field.label`     | string | Rótulo amigável                  |
| `field.type`      | string | Tipo CLR (string, int, etc.)     |
| `field.required`  | bool   | Se é obrigatório                 |

### Exemplo de uso em template

```scriban
{{ for field in fields }}
{{ if field.required }}
[Required(ErrorMessage = "{{ field.label }} é obrigatório")]
{{ end }}
public {{ field.type }} {{ field.name }} { get; set; }
{{ end }}
```

### Customizando templates

Edite os arquivos `.scriban` diretamente. A aplicação recarrega os templates a cada geração — não precisa reiniciar o app.

---

## Configuração da API de Manifesto

O `HttpManifestClient` consome a API REST do RhSensoERP para obter metadados das entidades.

### appsettings.json

```json
{
  "ManifestApi": {
    "BaseUrl": "https://seu-servidor:porta"
  }
}
```

### Endpoint consumido

```
GET {BaseUrl}/api/manifesto/entidades/{entityId}
```

### Response esperado

```json
{
  "entityId": "TreTiposTreinamento",
  "entityName": "Tipos de Treinamento",
  "module": "Treinamento",
  "cdSistema": 1,
  "cdFuncao": 150,
  "databaseSchema": "dbo",
  "tableName": "tre_tipos_treinamento",
  "routes": {
    "list": "/api/treinamento/tipos-treinamento",
    "getById": "/api/treinamento/tipos-treinamento/{id}",
    "create": "/api/treinamento/tipos-treinamento",
    "update": "/api/treinamento/tipos-treinamento/{id}",
    "delete": "/api/treinamento/tipos-treinamento/{id}"
  },
  "fields": [
    {
      "fieldName": "IdTipoTreinamento",
      "label": "ID",
      "clrType": "int",
      "isPrimaryKey": true,
      "isIdentity": true,
      "isRequired": true
    },
    {
      "fieldName": "DsTipoTreinamento",
      "label": "Descrição",
      "clrType": "string",
      "maxLength": 100,
      "isRequired": true,
      "suggestedInputType": "Text"
    }
  ]
}
```

### Modo mock (desenvolvimento sem API)

Se `ManifestApi:BaseUrl` contiver `localhost` ou estiver vazio, o sistema retorna dados mock automaticamente. Isso permite usar o gerador sem a API do RhSensoERP rodando.

---

## Testes

### Rodar todos os testes

```bash
dotnet test
```

### Rodar com verbosidade

```bash
dotnet test --verbosity detailed
```

### Rodar testes específicos

```bash
dotnet test --filter "FullyQualifiedName~SchemaReaderTests"
dotnet test --filter "FullyQualifiedName~GeneratorServiceTests"
dotnet test --filter "FullyQualifiedName~OrchestratorServiceTests"
```

### Cobertura dos testes

| Arquivo                       | Linhas | O que cobre                                    |
|-------------------------------|--------|------------------------------------------------|
| `SchemaReaderTests.cs`        | 271    | Leitura de colunas, PKs, FKs, mapeamento SQL→CLR |
| `GeneratorServiceTests.cs`    | 124    | Geração de arquivos, criação de ZIP, validação    |
| `OrchestratorServiceTests.cs` | 136    | Fluxo do wizard, detecção de conflitos, config    |

---

## Troubleshooting

### Erro de conexão com SQL Server

```
A connection was successfully established with the server, but then an error occurred
```

Verifique se `TrustServerCertificate=true` está na connection string (necessário para certificados auto-assinados).

### ManifestClient sempre retorna mock

Comportamento esperado em desenvolvimento local. Para dados reais, configure `ManifestApi:BaseUrl` com a URL da API do RhSensoERP e garanta que ela está acessível.

### ZIP gerado com poucos arquivos

Verifique o log do console. Se a geração falhou parcialmente, os erros aparecem no output e no `GenerationResult.Errors`.

### Session expirada (redirect para Step1)

O wizard usa session com timeout de 30 minutos (`Program.cs`). Se o timeout expirar, o wizard redireciona para o Step 1. Para aumentar:

```csharp
// Program.cs
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Aumentar conforme necessário
});
```

### Porta em uso

Se a porta 5201 estiver ocupada:

```bash
dotnet run --urls "http://localhost:5300"
```

Ou altere em `Properties/launchSettings.json`.

---

## Roadmap

### Pendente

- [ ] Migrar geração para usar templates `.scriban` (atualmente hardcoded via StringBuilder)
- [ ] Gerar MVC Controller com `[Area]` e `[Authorize]` (em vez de ApiController)
- [ ] Gerar ZIP com estrutura de pastas `Areas/{Module}/`
- [ ] Templates adicionais: ApiService, ServiceRegistration, FluentValidation
- [ ] Integrar com sistema de permissões IAEC
- [ ] Migrar para `Microsoft.Data.SqlClient`
- [ ] Substituir `Newtonsoft.Json` por `System.Text.Json`
- [ ] Separar testes em projeto próprio
- [ ] Suporte a lookups/autocomplete para campos FK
- [ ] Preview em tempo real do código antes da geração

### Concluído

- [x] Arquitetura Clean Architecture com separação de camadas
- [x] Leitura completa de schema SQL Server (colunas, PKs, FKs, índices, extended properties)
- [x] Detecção de conflitos entre banco e manifesto
- [x] Wizard funcional de 5 steps
- [x] Geração de Controller, ViewModel, View, JS e CSS
- [x] Empacotamento em ZIP para download
- [x] Testes unitários com xUnit + Moq
- [x] Suporte a partial classes (`.generated` + `.custom`)

---

## Métricas

| Métrica            | Valor   |
|--------------------|---------|
| Linhas C#          | ~5.630  |
| Linhas Scriban     | ~371    |
| Linhas Razor       | ~343    |
| Arquivos .cs       | 16      |
| Templates .scriban | 4       |
| Testes (linhas)    | 531     |

---

## Licença

Projeto interno — uso restrito à equipe de desenvolvimento do RhSensoERP.
