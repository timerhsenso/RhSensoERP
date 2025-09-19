# Testes do RhSensoERP – Guia de Uso e Boas Práticas

Este pacote documenta os testes criados/ajustados para o **RhSensoERP**.  
O objetivo é explicar **o que cada teste valida**, **como executar** e **boas práticas** seguidas.

---

## Estrutura

```
Tests/
└─ RhSensoERP.Tests.Unit/
   ├─ GlobalUsings.cs
   ├─ RhSensoERP.Tests.Unit.csproj
   ├─ Common/
   │  └─ CustomWebApplicationFactory.cs
   ├─ Api/
   │  └─ AuthControllerTests.cs
   ├─ Infrastructure/
   │  └─ DynamicAuthorizationPolicyProviderTests.cs
   └─ Modules/
      └─ Auth/
         └─ LoginCommandHandlerTests.cs
```

### Resumo dos Arquivos

- **GlobalUsings.cs**  
  Fornece _usings globais_ para `Xunit` e `FluentAssertions`.  
  **Por quê?** Evita repetição de `using` em todos os arquivos de teste e melhora a legibilidade.

- **Common/CustomWebApplicationFactory.cs**  
  Fábrica personalizada para hospedar a API em memória durante os testes de integração.  
  - Usa **SQLite In-Memory** (via `Microsoft.Data.Sqlite`) com **conexão aberta** durante os testes.  
  - Substitui `ILegacyAuthService` por uma implementação **fake** que libera autenticação previsível.  
  - Garante `db.Database.EnsureCreated()` para que o EF Core crie as tabelas.  
  **Valida:** que a API sobe num _host_ real e que endpoints podem ser acessados por `HttpClient` nos testes.

- **Api/AuthControllerTests.cs**  
  **Caso feliz:** credenciais válidas → `200 OK` + `ApiResponse<LoginResponseDto>` com `AccessToken`.  
  **Caso de erro:** credenciais inválidas → `400 BadRequest`.  
  **Boas práticas aplicadas:**  
  - Arrange/Act/Assert.  
  - Tipos reais do projeto (`LoginRequestDto`, `ApiResponse<LoginResponseDto>`).  
  - Sem acoplamento a detalhes de infraestrutura (token real): apenas verificações de contrato/semântica.

- **Infrastructure/DynamicAuthorizationPolicyProviderTests.cs**  
  **Valida:** que `DynamicAuthorizationPolicyProvider` cria _policies_ dinamicamente contendo `PermissionRequirement` com a permissão solicitada.  
  **Boas práticas aplicadas:**  
  - Evita _expression trees_ com pattern `is` (que causavam `CS8122`) usando `foreach` simples.  
  - Foco na **semântica** (a _policy_ contém o requisito certo).

- **Modules/Auth/LoginCommandHandlerTests.cs**  
  **Valida:** fluxo de autenticação no nível de serviço usando `ILegacyAuthService` **mockado**.  
  - **Sucesso:** retorna `AuthResult.Success == true` e `AccessToken` preenchido.  
  - **Falha:** retorna `Success == false` e mensagem `"INVALID_CREDENTIALS"`.  
  **Boas práticas:**  
  - _Mocks_ para limitar o escopo do teste (sem tocar DB/externos).  
  - Verificações explícitas (`Verify`) garantindo que a dependência foi chamada com os parâmetros esperados.

---

## Como Executar

1. **Restore & Build**
   ```bash
   dotnet restore
   dotnet build
   ```

2. **Testes**
   ```bash
   dotnet test
   ```

> Se você estiver usando **Central Package Management**, confirme que o arquivo `Directory.Packages.props` está presente na raiz da solução com as versões dos pacotes.

---

## Padrões e Boas Práticas Adotadas

- **AAA (Arrange, Act, Assert):** cada teste segue uma estrutura clara para facilitar leitura e manutenção.
- **Nomes Descritivos:** `Should_Resultado_Esperado_Quando_Condicao` ajudam a entender o objetivo sem abrir o corpo do teste.
- **Isolamento:**  
  - **Unitários:** `Moq` para simular dependências, evitando acessar IO/DB.  
  - **Integração (API):** `WebApplicationFactory` com SQLite In-Memory, evitando tocar recursos de produção.
- **Semântica > Implementação:** verificamos **contratos** (status code, DTOs, _claims_) ao invés de detalhes internos.
- **Test Doubles Estratégicos:** `FakeLegacyAuthService` injeta cenários previsíveis para endpoints de autenticação.
- **GlobalUsings:** reduz _boilerplate_ e mantém foco no que o teste realmente valida.

---

## Próximos Passos (opcional)

- **Cobertura de Middleware:** `ExceptionHandlingMiddleware`, `SecurityHeaders` e `RateLimiting`.  
- **Serviços de Domínio:** regras de negócios críticas (ex.: autorização por botão/ação).  
- **Relatórios de Cobertura:** integrar `coverlet` + `reportgenerator` para HTML com porcentagens por projeto/namespace.  
- **CI/CD:** pipeline de `dotnet restore/build/test` (GitHub Actions) para prevenir regressões.

---

> Qualquer arquivo pode receber comentários adicionais linha a linha — basta me dizer qual você quer detalhar mais.
