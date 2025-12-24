# 🚀 RhSensoERP Source Generator v3.0

## ⚠️ IMPORTANTE

Este Source Generator gera **apenas código Backend**. Para Web/API Controllers e Services, use a **ferramenta CLI `RhSensoERP.CrudTool`**.

| Componente | Gerador | Onde gera |
|------------|---------|-----------|
| Backend (DTOs, Commands, Queries, Repository...) | **Source Generator** | Projeto da Entity |
| API/Web Controllers, Models, Services | **CrudTool CLI** | Projetos corretos |

---

## 📋 Arquivos Gerados (Backend)

| Categoria | Arquivos |
|-----------|----------|
| **DTOs** | EntityDto, CreateRequest, UpdateRequest |
| **Commands** | Create, Update, Delete, DeleteBatch |
| **Queries** | GetById, GetPaged |
| **Validators** | CreateValidator, UpdateValidator |
| **Repository** | Interface + Implementação |
| **Mapper** | AutoMapper Profile |
| **EF Config** | Entity Configuration |

**Total:** 15 arquivos gerados automaticamente

---

## 🎯 Uso

### 1. Marque a Entity

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Identity.Core.Entities;

[GenerateCrud(
    TableName = "tsistema",
    DisplayName = "Sistema",
    CdSistema = "SEG",
    CdFuncao = "SEG_FM_TSISTEMA",
    IsLegacyTable = true
)]
public class Sistema
{
    [Key]
    [Column("cdsistema")]           // Use [Column], NÃO [ColumnName]!
    [StringLength(10)]
    [FieldDisplayName("Código")]
    public string CdSistema { get; set; } = string.Empty;

    [Required]
    [Column("dcsistema")]
    [StringLength(100)]
    [FieldDisplayName("Descrição")]
    public string DcSistema { get; set; } = string.Empty;
    
    // ... navegações
    public virtual ICollection<Funcao> Funcoes { get; set; } = new List<Funcao>();
}
```

### 2. Build

```bash
dotnet build
```

### 3. Use o CrudTool CLI para Web/API

```bash
dotnet run --project src/Tools/RhSensoERP.CrudTool
```

---

## ⚙️ Flags de Geração

| Flag | Padrão | Descrição |
|------|--------|-----------|
| `GenerateDto` | ✅ | DTOs |
| `GenerateRequests` | ✅ | Create/Update Requests |
| `GenerateCommands` | ✅ | Commands CQRS |
| `GenerateQueries` | ✅ | Queries CQRS |
| `GenerateValidators` | ✅ | FluentValidation |
| `GenerateRepository` | ✅ | Repository |
| `GenerateMapper` | ✅ | AutoMapper Profile |
| `GenerateEfConfig` | ✅ | EF Configuration |
| `SupportsBatchDelete` | ✅ | Exclusão em lote |
| `IsLegacyTable` | ❌ | Tabela sem BaseEntity |
| `GenerateApiController` | ❌ | ⚠️ Use CrudTool |
| `GenerateWebController` | ❌ | ⚠️ Use CrudTool |
| `GenerateWebModels` | ❌ | ⚠️ Use CrudTool |
| `GenerateWebServices` | ❌ | ⚠️ Use CrudTool |

---

## 🔧 Instalação

### 1. Referência no projeto Domain

```xml
<ProjectReference Include="..\Generators\RhSensoERP.Generators.csproj"
                  OutputItemType="Analyzer"
                  ReferenceOutputAssembly="false" />
```

### 2. Atributos em Shared.Core

Copie `GenerateCrudAttribute.cs` para `src/Shared/RhSensoERP.Shared.Core/Attributes/`

---

## 📄 Licença

RhSenso Team © 2025
