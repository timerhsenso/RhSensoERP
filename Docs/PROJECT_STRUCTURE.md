# Projeto .NET 8 — Estrutura, Convenções e Geração de CRUD
> **Este documento é um contrato de layout.**  
> Qualquer IA (ou pessoa) que gerar código para este repositório **deve** seguir exatamente a estrutura, nomes de pastas/arquivos e convenções abaixo. **Não** criar novas pastas além das listadas.

---

## 📁 Estrutura de Pastas (com comentários do propósito)
```
/ (raiz do repositório)
├─ RhSensoWebApi.sln                      # Solução .NET da aplicação inteira
├─ docs/                                   # Documentação do sistema (diagramas, decisões ADR, anotações)
├─ tests/                                  # Testes automatizados (unitários/integração)
│  └─ RhSensoWebApi.Tests/                 # Projeto de testes (xUnit/NUnit/etc.)
├─ src/                                    # TODO o código-fonte fica aqui (API, WEB, Shared, Core, Infra)
│  ├─ RhSenso.Shared/                      # DTOs e contratos compartilhados entre API e APP (NUNCA EF/infra)
│  │  ├─ Common/                           # Utilitários cross-cutting (paging, bulk, etc.)
│  │  │  ├─ Paging/                        # PagedQuery, PagedResult<T> (padrão de paginação)
│  │  │  └─ Bulk/                          # BulkDeleteRequest (padrão para exclusão em lote)
│  │  └─ {MÓDULO}/                         # Ex.: SEG, CAD, FIN... (áreas funcionais)
│  │     └─ {RECURSO}/                     # Ex.: Botoes, Sistemas, Usuarios...
│  │        ├─ {Recurso}ListDto.cs         # DTO usado em listagens/grids
│  │        ├─ {Recurso}FormDto.cs         # DTO para criar/editar (validação via FluentValidation)
│  │        └─ (opcional) {Recurso}DetailDto.cs  # DTO detalhado (quando precisar)
│  │
│  ├─ Core/                                # Regras e contratos de domínio (sem EF/infra)
│  │  ├─ Common/                           # Interfaces genéricas e contratos base
│  │  │  └─ ICrudService.cs                # Contrato CRUD genérico para qualquer recurso
│  │  └─ Abstractions/                     # Interfaces específicas por módulo/recurso (se houver extras)
│  │     └─ {MÓDULO}/
│  │        └─ {RECURSO}/
│  │           └─ I{Recurso}Service.cs     # Extensões do serviço além do CRUD genérico (opcional)
│  │
│  ├─ Infrastructure/                      # Acesso a dados, EF Core, serviços externos, cache
│  │  ├─ Data/
│  │  │  ├─ Context/                       # AppDbContext e configurações EF
│  │  │  │  └─ AppDbContext.cs
│  │  │  └─ Entities/                      # Entidades de persistência (se não houver Domain puro)
│  │  ├─ Repositories/                     # (Opcional) Implementações de repositório
│  │  ├─ Services/                         # Implementações de serviços de negócio (usam EF/externos)
│  │  │  └─ {MÓDULO}/
│  │  │     └─ {RECURSO}/
│  │  │        └─ {Recurso}Service.cs      # Implementa ICrudService<,> para o recurso
│  │  ├─ Cache/                             # Implementações de cache (IMemoryCache/Redis)
│  │  └─ ...                                # Outras integrações (e-mail, queue, storage), se houver
│  │
│  ├─ API/                                  # ASP.NET Core Web API (endpoints REST)
│  │  ├─ Common/
│  │  │  └─ Controllers/
│  │  │     └─ BaseCrudController.cs        # Controller genérico CRUD (herdado pelos recursos)
│  │  ├─ Controllers/
│  │  │  └─ {MÓDULO}/
│  │  │     └─ {Recurso}Controller.cs       # Controller fino que herda do BaseCrudController
│  │  ├─ Validators/                        # FluentValidation para *FormDto (somente validação de entrada)
│  │  │  └─ {MÓDULO}/
│  │  │     └─ {Recurso}FormValidator.cs
│  │  ├─ Middleware/                        # Middlewares (ExceptionHandling, RequestLogging, etc.)
│  │  ├─ Filters/                           # Filtros (Action/Exception) se usados
│  │  ├─ Swagger/                           # Schema filters e configs extras do Swagger
│  │  ├─ Properties/                        # launchSettings.json (profiles de execução)
│  │  ├─ logs/                              # Saída de logs (Serilog - rolling files)
│  │  ├─ Program.cs                         # Bootstrap/DI/pipeline da API (CORS, JWT, Versioning, etc.)
│  │  └─ RhSensoWebApi.API.csproj
│  │
│  └─ RhSensoWeb/                           # ASP.NET Core MVC App (front server-side)
│     ├─ Controllers/                       # Controllers MVC (consomem a API via HttpClient)
│     ├─ Views/                             # Views Razor (UI)
│     ├─ Models/                            # ViewModels do MVC (se realmente necessários)
│     ├─ Services/
│     │  └─ ApiClients/                     # Clients tipados para chamar a API (1 por recurso)
│     │     └─ {Recurso}Api.cs
│     ├─ Middleware/                        # Middlewares do MVC (se houver)
│     ├─ wwwroot/                           # Arquivos estáticos (css, js, imagens)
│     ├─ Program.cs                         # Bootstrap do MVC (resiliência http, cookies, sessão, rotas)
│     └─ RhSensoWeb.csproj
```
---

## 📐 Regras Obrigatórias (para qualquer IA seguir)
1. **DTOs** ficam **sempre** em `RhSenso.Shared`. **Proibido** criar DTO em `API`, `Infrastructure` ou `RhSensoWeb`.
2. **Validação**: um `AbstractValidator` por `*FormDto` em `src/API/Validators/{MÓDULO}/{Recurso}FormValidator.cs`.
3. **Controllers de recurso (API)**: herdam de `BaseCrudController<TListDto, TFormDto, TKey>` e seguem rota:
   - `[ApiVersion("1.0")]`
   - `[Route("api/v{version:apiVersion}/{módulo}/{recurso}")]`
   - `[Tags("{Recurso}")]` para agrupar no Swagger
4. **Serviços (negócio/EF)**: em `Infrastructure/Services/{MÓDULO}/{RECURSO}/{Recurso}Service.cs`, implementam `ICrudService<TListDto, TFormDto, TKey>`.
5. **Rotas**: usar **segmento de versão** `/api/v{version}/...` (ex.: `/api/v1/seg/botoes`).
6. **Swagger**: agrupar por tag do recurso; documentar só endpoints públicos.
7. **EF Core**:
   - consultas de **leitura** com `AsNoTracking()`;
   - usar `CancellationToken` em todos os métodos `async`;
   - adicionar `RowVersion` (concorrência) quando fizer sentido.
8. **Erros**: respostas de **sucesso** sem envelopar; **falhas** via middleware/ProblemDetails.
9. **APP (MVC)**: chama a API via **Typed HttpClient** em `RhSensoWeb/Services/ApiClients`.
10. **Não criar novas pastas**. **Somente** as listadas neste documento.

---

## 🧩 Base Genérica (já existente/obrigatória)
- `src/RhSenso.Shared/Common/Paging/PagedQuery.cs` → `Page`, `PageSize`, `Q`, `Skip`, `Take`  
- `src/RhSenso.Shared/Common/Paging/PagedResult.cs` → `Total`, `Filtered`, `Items`
- `src/RhSenso.Shared/Common/Bulk/BulkDeleteRequest.cs` → `List<string> Codigos`
- `src/Core/Common/ICrudService.cs` → contrato CRUD genérico (List/Get/Create/Update/Delete/BulkDelete)
- `src/API/Common/Controllers/BaseCrudController.cs` → endpoints padrão:
  - `GET    /?page=&pageSize=&q=`       (listagem paginada)
  - `GET    /{id}`                      (1 item)
  - `POST   /`                          (criar → 201 + Location)
  - `PUT    /{id}`                      (atualizar → 204)
  - `DELETE /{id}`                      (remover 1 → 204/404)
  - `POST   /bulk-delete`               (remover vários → 200 com resumo)

> **Qualquer novo recurso deve reusar esses componentes.**

---

## 🛠️ Como gerar um CRUD novo ({MÓDULO}/{RECURSO})
> Substitua `{MÓDULO}` e `{RECURSO}` (ex.: `SEG/Botoes`). **Não mude sufixos** dos arquivos.

1) **DTOs (Shared)**
   - `src/RhSenso.Shared/{MÓDULO}/{RECURSO}/{Recurso}ListDto.cs`
   - `src/RhSenso.Shared/{MÓDULO}/{RECURSO}/{Recurso}FormDto.cs`
   - (opcional) `src/RhSenso.Shared/{MÓDULO}/{RECURSO}/{Recurso}DetailDto.cs`

2) **Validador (API)**
   - `src/API/Validators/{MÓDULO}/{Recurso}FormValidator.cs`  
     Classe: `{Recurso}FormValidator : AbstractValidator<{Recurso}FormDto>`

3) **Service (Infrastructure)**
   - `src/Infrastructure/Services/{MÓDULO}/{RECURSO}/{Recurso}Service.cs`  
     Implementa: `ICrudService<{Recurso}ListDto, {Recurso}FormDto, {TKey}>`  
     Usa: `AppDbContext` + `Entities` correspondentes

4) **Controller (API)**
   - `src/API/Controllers/{MÓDULO}/{Recurso}Controller.cs`  
     Herda de: `BaseCrudController<{Recurso}ListDto, {Recurso}FormDto, {TKey}>`  
     Atributos: `[ApiController]`, `[ApiVersion("1.0")]`, `[Tags("{Recurso}")]`,  
     `[Route("api/v{version:apiVersion}/{módulo}/{recurso}")]`

5) **Client (APP MVC)** (se a tela MVC consumir esse recurso)
   - `src/RhSensoWeb/Services/ApiClients/{Recurso}Api.cs`  
     Usa `HttpClient` tipado “Api” configurado no `Program.cs` do MVC

6) **Registro de DI (API)**
   - No `src/API/Program.cs`, adicionar:
     ```csharp
     builder.Services.AddScoped<
        ICrudService<{Recurso}ListDto, {Recurso}FormDto, {TKey}>,
        {Recurso}Service>();
     ```

---

## 🧭 Convenções de Nomes e Chaves
- `{Recurso}ListDto` → dados resumidos para listagens.
- `{Recurso}FormDto` → dados de criação/edição (validados).
- `{Recurso}DetailDto` → dados ricos (quando necessário).
- `{TKey}` pode ser `string`, `int` ou `Guid`. Para **chave composta**, padronizar uma **string de chave** (ex.: `"SIS:FUN:COD"`).

---

## 🚫 Não fazer (anti-padrões que a IA deve evitar)
- ❌ Não criar DTOs fora de `RhSenso.Shared`.
- ❌ Não acessar `DbContext` em controllers; toda lógica passa pelos **Services**.
- ❌ Não fugir do padrão de rota/versionamento (`api/v{version}/{módulo}/{recurso}`).
- ❌ Não duplicar validators; é **1 por `*FormDto`**.
- ❌ Não criar novas pastas/níveis além dos definidos aqui.

---

## ✅ Exemplo (SEG/Botoes) — checklist de geração
Crie **exatamente** estes arquivos:
- `src/RhSenso.Shared/SEG/Botoes/BotaoListDto.cs`
- `src/RhSenso.Shared/SEG/Botoes/BotaoFormDto.cs`
- `src/API/Validators/SEG/BotaoFormValidator.cs`
- `src/Infrastructure/Services/SEG/Botoes/BotoesService.cs`
- `src/API/Controllers/SEG/BotoesController.cs`
- (MVC) `src/RhSensoWeb/Services/ApiClients/BotoesApi.cs`

No `src/API/Program.cs`, registre o serviço:
```csharp
builder.Services.AddScoped<
  ICrudService<BotaoListDto, BotaoFormDto, string>,
  BotoesService>();
```

**Rota do controller**:
```
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/seg/botoes")]
[Tags("Botoes")]
```

---

## 🔐 Segurança e Configuração (referência rápida)
- **ConnectionStrings**: ler `ConnectionStrings:Default` via User Secrets/Env/KeyVault.
- **JWT**: `Issuer`, `Audience`, `Key` via secrets/env; `ClockSkew = TimeSpan.Zero`.
- **CORS**: política nomeada “Frontends”; em produção usar **origens explícitas**.
- **Health**: `/health` (liveness) e `/health/ready` (readiness com DB).

---

## 🏁 Resumo para a IA
> Para **qualquer recurso novo**, gere somente os arquivos indicados nos caminhos acima, herdando do `BaseCrudController` e implementando `ICrudService`.  
> **Nunca** crie DTOs fora do `RhSenso.Shared`. **Nunca** crie novas pastas. **Siga a rota e a versão.**
