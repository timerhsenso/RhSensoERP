# 🚀 Fase 3.1 - Infrastructure Base para RhSensoERP

## ✅ Análise do Seu Código Existente

Analisei seus arquivos e criei código que **COMPLEMENTA** o que você já tem:

| Seu Arquivo | Status | Ação |
|-------------|--------|------|
| `Shared.Core/Common/Result.cs` | ✅ Mantido | Adicionei `ResultExtensions.cs` |
| `Shared.Core/Abstractions/IRepository.cs` | ✅ Mantido | Adicionei `IRepositoryExtended.cs` |
| `Shared.Contracts/Common/ApiResponse.cs` | ✅ Mantido | Adicionei `PaginationDtos.cs` |
| `Web/Models/Common/ApiResponse.cs` | ✅ Mantido | Nenhuma alteração |

---

## 📁 Mapeamento de Arquivos

### Para `RhSensoERP.Shared.Core`

| Arquivo ZIP | Destino | Descrição |
|-------------|---------|-----------|
| `Core/Common/ResultExtensions.cs` | `Common/ResultExtensions.cs` | Extensões para Result |
| `Core/Domain/Interfaces/IEntity.cs` | `Domain/Interfaces/IEntity.cs` | Interfaces de entidade |
| `Core/Domain/Entities/EntityBase.cs` | `Domain/Entities/EntityBase.cs` | Classes base |
| `Core/Abstractions/IRepositoryExtended.cs` | `Abstractions/IRepositoryExtended.cs` | Interfaces estendidas |

### Para `RhSensoERP.Shared.Contracts`

| Arquivo ZIP | Destino | Descrição |
|-------------|---------|-----------|
| `Contracts/DTOs/PaginationDtos.cs` | `DTOs/PaginationDtos.cs` | DTOs de paginação |

### Para `RhSensoERP.Shared.Application`

| Arquivo ZIP | Destino | Descrição |
|-------------|---------|-----------|
| `Application/Interfaces/IService.cs` | `Interfaces/IService.cs` | Interfaces de serviço |
| `Application/Services/GenericService.cs` | `Services/GenericService.cs` | Serviço genérico |
| `Application/Specifications/ISpecification.cs` | `Specifications/ISpecification.cs` | Padrão Specification |

### Para `RhSensoERP.Shared.Infrastructure`

| Arquivo ZIP | Destino | Descrição |
|-------------|---------|-----------|
| `Infrastructure/Persistence/GenericRepository.cs` | `Persistence/GenericRepository.cs` | Repositório genérico |
| `Infrastructure/Persistence/UnitOfWork.cs` | `Persistence/UnitOfWork.cs` | Unit of Work |
| `Infrastructure/Extensions/ServiceCollectionExtensions.cs` | `Extensions/ServiceCollectionExtensions.cs` | Extensões DI |

---

## 🔧 Passo a Passo

### 1. Extrair o ZIP

Extraia `fase3.1-final.zip` em uma pasta temporária.

### 2. Copiar Arquivos

```
fase3.1-final/Core/*           → src/Shared/RhSensoERP.Shared.Core/
fase3.1-final/Contracts/*      → src/Shared/RhSensoERP.Shared.Contracts/
fase3.1-final/Application/*    → src/Shared/RhSensoERP.Shared.Application/
fase3.1-final/Infrastructure/* → src/Shared/RhSensoERP.Shared.Infrastructure/
```

### 3. Verificar Referências entre Projetos

**Shared.Core.csproj** (sem dependências):
```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
</PropertyGroup>
```

**Shared.Contracts.csproj**:
```xml
<ItemGroup>
  <ProjectReference Include="..\RhSensoERP.Shared.Core\RhSensoERP.Shared.Core.csproj" />
</ItemGroup>
```

**Shared.Application.csproj**:
```xml
<ItemGroup>
  <ProjectReference Include="..\RhSensoERP.Shared.Core\RhSensoERP.Shared.Core.csproj" />
  <ProjectReference Include="..\RhSensoERP.Shared.Contracts\RhSensoERP.Shared.Contracts.csproj" />
  <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.0" />
</ItemGroup>
```

**Shared.Infrastructure.csproj**:
```xml
<ItemGroup>
  <ProjectReference Include="..\RhSensoERP.Shared.Core\RhSensoERP.Shared.Core.csproj" />
  <ProjectReference Include="..\RhSensoERP.Shared.Application\RhSensoERP.Shared.Application.csproj" />
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
  <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.0" />
  <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.0" />
</ItemGroup>
```

### 4. Compilar

```bash
dotnet build src/Shared/RhSensoERP.Shared.Core/
dotnet build src/Shared/RhSensoERP.Shared.Contracts/
dotnet build src/Shared/RhSensoERP.Shared.Application/
dotnet build src/Shared/RhSensoERP.Shared.Infrastructure/
```

---

## 📝 Exemplo de Uso

### Registrar no DI (Program.cs)

```csharp
using RhSensoERP.Shared.Infrastructure.Extensions;

// Configurar entidades para um DbContext
services.ConfigureEntities<GestaoDePessoasDbContext>()
    .AddEntity<Banco, string>()
    .AddEntity<Funcionario, int>()
    .Build();
```

### Criar um Serviço para Banco

```csharp
using RhSensoERP.Shared.Application.Services;
using RhSensoERP.Shared.Core.Abstractions;
using RhSensoERP.Shared.Core.Common;

public class BancoService : GenericService<Banco, string, BancoDto>
{
    public BancoService(
        IRepository<Banco, string> repository,
        IUnitOfWork unitOfWork,
        ILogger<BancoService> logger)
        : base(repository, unitOfWork, logger)
    {
    }

    protected override BancoDto MapToDto(Banco entity) => new()
    {
        CdBanco = entity.CdBanco,
        DcBanco = entity.DcBanco
    };

    protected override Banco MapToEntity(BancoDto dto) => new()
    {
        CdBanco = dto.CdBanco,
        DcBanco = dto.DcBanco
    };

    protected override void UpdateEntity(Banco entity, BancoDto dto)
    {
        entity.DcBanco = dto.DcBanco;
    }

    protected override string GetEntityKey(Banco entity) => entity.CdBanco;

    // Validação customizada
    protected override async Task<r> ValidateCreateAsync(BancoDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.CdBanco))
            return Result.Failure(new Error("Validation", "Código é obrigatório"));

        var exists = await Repository.AnyAsync(b => b.CdBanco == dto.CdBanco, ct);
        if (exists)
            return Result.Failure(new Error("Duplicate", $"Banco {dto.CdBanco} já existe"));

        return Result.Success();
    }
}
```

---

## ✅ Checklist

- [ ] Copiar arquivos para projetos corretos
- [ ] Verificar referências entre projetos
- [ ] Adicionar pacotes NuGet necessários
- [ ] Compilar todos os projetos
- [ ] Testar criação de um serviço

---

## ⚠️ Arquivos que NÃO devem ser sobrescritos

- `Shared.Core/Common/Result.cs` → Mantém o seu
- `Shared.Core/Common/Error.cs` → Mantém o seu (se existir)
- `Shared.Core/Abstractions/IRepository.cs` → Mantém o seu
- `Shared.Contracts/Common/ApiResponse.cs` → Mantém o seu
