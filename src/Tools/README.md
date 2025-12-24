# RhSensoERP CrudTool v2.0

Ferramenta CLI para geração automática de código **Frontend (Web)** compatível com o backend gerado pelo **Source Generator**.

## 📁 Estrutura de Arquivos Gerados

```
src/Web/
├── Controllers/
│   └── {PluralName}Controller.cs      ← Herda de BaseCrudController
├── Models/{PluralName}/
│   ├── {Name}Dto.cs                   ← DTO de leitura
│   ├── Create{Name}Request.cs         ← Request de criação
│   ├── Update{Name}Request.cs         ← Request de atualização
│   └── {PluralName}ListViewModel.cs   ← ViewModel (herda BaseListViewModel)
├── Services/{PluralName}/
│   ├── I{Name}ApiService.cs           ← Interface (herda IApiService)
│   └── {Name}ApiService.cs            ← Implementação
├── Views/{PluralName}/
│   └── Index.cshtml                   ← View Razor
└── wwwroot/js/{pluralnamelower}/
    └── {namelower}.js                 ← JS que estende CrudBase
```

## 🚀 Como Usar

### 1. Copiar para o projeto

Copie a pasta `CrudTool` para dentro do diretório `tools/` da sua solution:

```
RhSensoERP/
├── src/
├── tools/
│   └── CrudTool/           ← Copie aqui
└── RhSensoERP.sln
```

### 2. Criar arquivo de configuração

Crie um arquivo `crud-config.json` na raiz da solution (ou use `crud-config-example.json` como base):

```json
{
  "solutionRoot": ".",
  "webProject": "src/Web",
  "entities": [
    {
      "name": "Sitc2",
      "displayName": "Situação de Frequência",
      "pluralName": "Sitc2s",
      "module": "ControleDePonto",
      "moduleRoute": "controledeponto",
      "cdSistema": "FRE",
      "cdFuncao": "CPT_FM_SITC2",
      "primaryKey": {
        "property": "Id",
        "type": "Guid"
      },
      "properties": [...],
      "generate": {
        "webController": true,
        "webModels": true,
        "webServices": true,
        "view": true,
        "javascript": true
      }
    }
  ]
}
```

### 3. Executar o gerador

```bash
cd tools/CrudTool
dotnet run -- ../../crud-config.json
```

Ou instale como tool global:

```bash
dotnet pack
dotnet tool install --global --add-source ./nupkg RhSensoERP.CrudTool
rhsenso-crud crud-config.json
```

### 4. Registrar o Service no DI

Adicione em `Program.cs` ou `ServiceCollectionExtensions.cs`:

```csharp
// Em ConfigureServices ou AddApplicationServices
services.AddHttpClient<ISitc2ApiService, Sitc2ApiService>(client =>
{
    client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]!);
});
```

### 5. Adicionar rota no menu

Adicione o link no menu de navegação (`_Layout.cshtml` ou similar).

## ✅ Compatibilidade

O código gerado é 100% compatível com:

| Componente | Classe Base Existente |
|------------|----------------------|
| Controller | `BaseCrudController<TDto, TCreate, TUpdate, TKey>` |
| Service Interface | `IApiService<TDto, TCreate, TUpdate, TKey>` |
| Batch Delete | `IBatchDeleteService<TKey>` |
| ViewModel | `BaseListViewModel` |
| JavaScript | `CrudBase` (crud-base.js) |
| Responses | `ApiResponse<T>`, `BatchDeleteResultDto` |

## 📋 Propriedades do JSON

### Entity Config

| Propriedade | Descrição |
|-------------|-----------|
| `name` | Nome da entidade (PascalCase) |
| `displayName` | Nome amigável para exibição |
| `pluralName` | Nome no plural |
| `module` | Módulo do backend (ex: ControleDePonto) |
| `moduleRoute` | Rota da API (ex: controledeponto) |
| `cdSistema` | Código do sistema para permissões |
| `cdFuncao` | Código da função para permissões |

### Property Config

| Propriedade | Descrição |
|-------------|-----------|
| `name` | Nome da propriedade |
| `type` | Tipo C# (string, int, Guid, DateTime?, etc) |
| `displayName` | Nome amigável |
| `required` | Se é obrigatório |
| `maxLength` | Tamanho máximo (para strings) |
| `list.show` | Exibir na tabela |
| `list.format` | Formato: text, date, datetime, boolean, currency |
| `form.show` | Exibir no formulário |
| `form.inputType` | Tipo de input: text, number, date, checkbox, select |
| `form.colSize` | Tamanho da coluna (1-12 Bootstrap) |

## 🔧 Fluxo de Comunicação

```
┌─────────────┐     ┌──────────────────┐     ┌─────────────────┐     ┌─────────────┐
│  CrudBase   │────▶│  WebController   │────▶│   ApiService    │────▶│   API       │
│  (JS)       │     │  (BaseCrud)      │     │  (IApiService)  │     │  Backend    │
└─────────────┘     └──────────────────┘     └─────────────────┘     └─────────────┘
      │                     │                        │                      │
      │  {success, data}    │   JsonSuccess()       │   ApiResponse<T>     │  Result<T>
      │◀────────────────────│◀──────────────────────│◀─────────────────────│
```

## 📝 Exemplo de Uso

Veja o arquivo `crud-config-example.json` para um exemplo completo de configuração.

---

**Versão:** 2.0  
**Compatível com:** RhSensoERP Web + Backend Source Generator v3.x
