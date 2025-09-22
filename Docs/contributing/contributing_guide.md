# 👨‍💻 Guia de Contribuição - RhSensoERP API

## 📋 Índice

- [Bem-vindo](#bem-vindo)
- [Configuração do Ambiente](#configuração-do-ambiente)
- [Padrões de Código](#padrões-de-código)
- [Fluxo de Desenvolvimento](#fluxo-de-desenvolvimento)
- [Testes](#testes)
- [Code Review](#code-review)
- [Debugging](#debugging)
- [Performance](#performance)

## 🎯 Bem-vindo

Este guia está baseado no **[Perfil do Desenvolvedor ERP](../perfil_desenvolvedor_erp_api.md)** - documento obrigatório que define o mindset e padrões do projeto.

### **🧠 Mindset Obrigatório**

Antes de começar, leia e internalize:
- ✅ **[perfil_desenvolvedor_erp_api.md](../perfil_desenvolvedor_erp_api.md)**

Você não é apenas um codificador. Você é um **Arquiteto de Soluções .NET** responsável pela qualidade, segurança e performance da aplicação.

### **⚡ Quick Start para Novos Desenvolvedores**

```bash
# 1. Clone o repositório
git clone https://github.com/empresa/rhsensoerp-api.git
cd rhsensoerp-api

# 2. Configure seu ambiente
./scripts/setup-dev-environment.sh

# 3. Configure secrets (obrigatório)
cd Src/API
dotnet user-secrets set "ConnectionStrings:Default" "SUA_CONNECTION_STRING"

# 4. Execute testes para validar setup
dotnet test

# 5. Execute a aplicação
dotnet run --project Src/API

# 6. Acesse https://localhost:57148/swagger
```

## 🛠️ Configuração do Ambiente

### **1. Pré-requisitos Obrigatórios**

| Ferramenta | Versão | Download | Configuração |
|------------|--------|----------|--------------|
| **.NET SDK** | 8.0+ | [Download](https://dotnet.microsoft.com/download) | `dotnet --version` |
| **Visual Studio** | 2022 17.8+ | [Download](https://visualstudio.microsoft.com/) | Workload: ASP.NET |
| **SQL Server** | 2019+ | [Download](https://www.microsoft.com/sql-server) | LocalDB ou Express |
| **Git** | 2.30+ | [Download](https://git-scm.com/) | Configurar nome e email |

### **2. Configuração do IDE**

#### **Visual Studio 2022**

```xml
<!-- .editorconfig (já existe no projeto) -->
root = true

[*.cs]
# Usar tabs de 4 espaços
indent_style = space
indent_size = 4

# Quebras de linha
end_of_line = crlf
insert_final_newline = true

# Estilo de código
dotnet_style_qualification_for_field = false:suggestion
dotnet_style_qualification_for_property = false:suggestion
csharp_new_line_before_open_brace = all
csharp_new_line_before_else = true
```

#### **Extensões Recomendadas**

- ✅ **SonarLint**: Análise de código em tempo real
- ✅ **CodeMaid**: Limpeza automática de código
- ✅ **Roslynator**: Refactoring avançado
- ✅ **GitLens**: Git integrado
- ✅ **REST Client**: Teste de APIs

#### **VS Code (Alternativo)**

```json
// .vscode/settings.json
{
    "dotnet.server.useOmnisharp": false,
    "omnisharp.enableRoslynAnalyzers": true,
    "editor.formatOnSave": true,
    "editor.codeActionsOnSave": {
        "source.organizeImports": true
    },
    "files.exclude": {
        "**/bin": true,
        "**/obj": true
    }
}
```

### **3. Configuração do Banco Local**

```bash
# Opção 1: SQL Server LocalDB (Recomendado para dev)
sqllocaldb create MSSQLLocalDB
sqllocaldb start MSSQLLocalDB

# Connection String
Server=(localdb)\MSSQLLocalDB;Database=bd_rhu_copenor;Integrated Security=true;

# Opção 2: SQL Server Express
# Connection String
Server=localhost\SQLEXPRESS;Database=bd_rhu_copenor;Integrated Security=true;

# Opção 3: Docker (se preferir)
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPassword123" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### **4. Scripts de Setup Automatizado**

```bash
#!/bin/bash
# scripts/setup-dev-environment.sh

echo "🚀 Configurando ambiente de desenvolvimento..."

# Verificar pré-requisitos
check_prerequisite() {
    if ! command -v $1 &> /dev/null; then
        echo "❌ $1 não encontrado. Instale antes de continuar."
        exit 1
    fi
    echo "✅ $1 encontrado"
}

check_prerequisite "dotnet"
check_prerequisite "git"
check_prerequisite "sqlcmd"

# Restaurar dependências
echo "📦 Restaurando dependências..."
dotnet restore

# Configurar User Secrets template
echo "🔐 Configurando User Secrets..."
cd Src/API

if [ ! -f "secrets.template.json" ]; then
cat > secrets.template.json << EOF
{
  "ConnectionStrings": {
    "Default": "SUBSTITUA_PELA_SUA_CONNECTION_STRING"
  },
  "Jwt": {
    "SecretKey": "MinhaChaveSecretaParaDev123456789AbCdEf",
    "Issuer": "RhSensoERP-Dev",
    "Audience": "RhSensoERP-Dev-Clients",
    "AccessTokenMinutes": 60
  }
}
EOF
fi

echo "📋 PRÓXIMOS PASSOS:"
echo "1. Configure sua connection string:"
echo "   dotnet user-secrets set \"ConnectionStrings:Default\" \"SUA_CONNECTION_STRING\""
echo "2. Execute os testes: dotnet test"
echo "3. Execute a aplicação: dotnet run --project Src/API"
echo ""
echo "🎯 Leia obrigatoriamente: perfil_desenvolvedor_erp_api.md"
```

## 🎨 Padrões de Código

### **1. Convenções de Nomenclatura**

```csharp
// ✅ CORRETO
public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IRepository<User> _userRepository;
    
    public async Task<UserDto> GetUserAsync(string cdUsuario, CancellationToken ct)
    {
        // Implementação
    }
}

// ❌ INCORRETO
public class userservice : iuserservice 
{
    public ILogger logger;
    public IRepository<User> userRepo;
    
    public UserDto GetUser(string cd_usuario)
    {
        // Implementação
    }
}
```

### **2. Estrutura de Classes**

```csharp
// Template obrigatório para Controllers
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ExemploController : ControllerBase
{
    private readonly IExemploService _exemploService;
    private readonly ILogger<ExemploController> _logger;

    public ExemploController(IExemploService exemploService, ILogger<ExemploController> logger)
    {
        _exemploService = exemploService;
        _logger = logger;
    }

    /// <summary>
    /// Descrição clara do endpoint
    /// </summary>
    /// <param name="request">Parâmetros de entrada</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Resposta tipada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ExemploResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<ExemploResponseDto>>> Action(
        [FromBody] ExemploRequestDto request,
        CancellationToken ct)
    {
        try
        {
            var result = await _exemploService.ExecuteAsync(request, ct);
            return Ok(ApiResponse<ExemploResponseDto>.Ok(result));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
```

### **3. DTOs e Records**

```csharp
// ✅ Use records para DTOs (imutáveis)
public record CreateUserRequestDto(
    string CdUsuario,
    string DcUsuario,
    string Email,
    char TpUsuario = 'U');

public record UserResponseDto(
    string CdUsuario,
    string DcUsuario,
    string Email,
    bool Active,
    DateTime CreatedAt);

// ✅ Validadores obrigatórios
public class CreateUserRequestValidator : AbstractValidator<CreateUserRequestDto>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.CdUsuario)
            .NotEmpty().WithMessage("Código do usuário é obrigatório")
            .Length(1, 30).WithMessage("Código deve ter entre 1 e 30 caracteres");
            
        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Email deve ser válido");
    }
}
```

### **4. Services e Use Cases**

```csharp
// ✅ Interface no Application
public interface IUserManagementService
{
    Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto request, CancellationToken ct);
    Task<UserResponseDto> GetUserAsync(string cdUsuario, CancellationToken ct);
    Task<bool> DeactivateUserAsync(string cdUsuario, CancellationToken ct);
}

// ✅ Implementação no Infrastructure
public class UserManagementService : IUserManagementService
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserManagementService> _logger;

    public UserManagementService(
        IRepository<User> userRepository,
        IUnitOfWork unitOfWork,
        ILogger<UserManagementService> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto request, CancellationToken ct)
    {
        _logger.LogInformation("Criando usuário {CdUsuario}", request.CdUsuario);
        
        // 1. Validar se usuário já existe
        var existingUser = await _userRepository
            .FirstOrDefaultAsync(u => u.CdUsuario == request.CdUsuario, ct);
            
        if (existingUser != null)
            throw new DomainException($"Usuário {request.CdUsuario} já existe");

        // 2. Criar entidade
        var user = new User
        {
            CdUsuario = request.CdUsuario,
            DcUsuario = request.DcUsuario,
            EmailUsuario = request.Email,
            TpUsuario = request.TpUsuario,
            FlAtivo = 'S'
        };

        // 3. Persistir
        await _userRepository.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Usuário {CdUsuario} criado com sucesso", user.CdUsuario);

        // 4. Retornar DTO
        return new UserResponseDto(
            user.CdUsuario,
            user.DcUsuario,
            user.EmailUsuario ?? string.Empty,
            user.Active,
            user.CreatedAt);
    }
}
```

## 🔄 Fluxo de Desenvolvimento

### **1. GitFlow Simplificado**

```mermaid
gitgraph
    commit id: "Initial"
    branch develop
    checkout develop
    commit id: "Setup"
    
    branch feature/auth-jwt
    checkout feature/auth-jwt
    commit id: "Add JWT"
    commit id: "Add tests"
    
    checkout develop
    merge feature/auth-jwt
    commit id: "Merge feature"
    
    checkout main
    merge develop
    commit id: "Release v1.0"
```

### **2. Comandos Git Essenciais**

```bash
# 1. Iniciar nova feature
git checkout develop
git pull origin develop
git checkout -b feature/nova-funcionalidade

# 2. Durante desenvolvimento
git add .
git commit -m "feat: adiciona endpoint de usuários"

# 3. Sync com develop
git checkout develop
git pull origin develop
git checkout feature/nova-funcionalidade
git rebase develop

# 4. Push e PR
git push origin feature/nova-funcionalidade
# Criar Pull Request via GitHub/Azure DevOps

# 5. Após merge, limpeza
git checkout develop
git pull origin develop
git branch -d feature/nova-funcionalidade
```

### **3. Conventional Commits (Obrigatório)**

```bash
# Tipos permitidos
feat:     # Nova funcionalidade
fix:      # Correção de bug
docs:     # Documentação
style:    # Formatação
refactor: # Refatoração
test:     # Testes
chore:    # Manutenção

# Exemplos
git commit -m "feat: adiciona endpoint de autenticação JWT"
git commit -m "fix: corrige validação de senha vazia"
git commit -m "docs: atualiza README com configuração"
git commit -m "test: adiciona testes unitários para UserService"
git commit -m "refactor: extrai lógica de validação para classe específica"
```

### **4. Processo de Feature**

#### **Fase 1: Planejamento**
- [ ] Definir requisitos funcionais
- [ ] Identificar impactos arquiteturais
- [ ] Estimar esforço
- [ ] Definir critérios de aceite

#### **Fase 2: Desenvolvimento**
```bash
# 1. Criar branch
git checkout -b feature/nome-da-feature

# 2. TDD (quando aplicável)
# Escrever testes ANTES da implementação
dotnet test --filter "TestName" # Deve falhar

# 3. Implementar
# Fazer testes passarem

# 4. Refatorar
# Melhorar código mantendo testes verdes
```

#### **Fase 3: Finalização**
- [ ] Todos os testes passando
- [ ] Code coverage adequado
- [ ] Documentação atualizada
- [ ] Auto-review completo
- [ ] PR criado com template

## 🧪 Testes

### **1. Pirâmide de Testes**

```
        /\
       /  \
      / UI \     ← Poucos (E2E)
     /______\
    /        \
   / Integration \  ← Moderados
  /______________\
 /                \
/ Unit Tests       \ ← Muitos (Rápidos)
/__________________\
```

### **2. Estrutura de Testes**

```csharp
// ✅ Template para testes unitários
namespace RhSensoERP.Tests.Unit.Services
{
    public class UserManagementServiceTests
    {
        private readonly Mock<IRepository<User>> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<UserManagementService>> _loggerMock;
        private readonly UserManagementService _service;

        public UserManagementServiceTests()
        {
            _userRepositoryMock = new Mock<IRepository<User>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<UserManagementService>>();
            
            _service = new UserManagementService(
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task CreateUserAsync_WithValidData_ShouldCreateUser()
        {
            // Arrange
            var request = new CreateUserRequestDto("test", "Test User", "test@test.com");
            _userRepositoryMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _service.CreateUserAsync(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.CdUsuario.Should().Be("test");
            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_WithExistingUser_ShouldThrowDomainException()
        {
            // Arrange
            var request = new CreateUserRequestDto("existing", "Existing User", "existing@test.com");
            var existingUser = new User { CdUsuario = "existing" };
            
            _userRepositoryMock
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            // Act & Assert
            var act = async () => await _service.CreateUserAsync(request, CancellationToken.None);
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("*já existe*");
        }
    }
}
```

### **3. Testes de Integração**

```csharp
// ✅ Template para testes de integração
public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var request = new LoginRequestDto("admin", "123456");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>();
        content!.Success.Should().BeTrue();
        content.Data!.AccessToken.Should().NotBeNullOrEmpty();
    }
}
```

### **4. Comandos de Teste**

```bash
# Executar todos os testes
dotnet test

# Testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Testes específicos
dotnet test --filter "UserManagementServiceTests"

# Testes em paralelo
dotnet test --parallel

# Output detalhado
dotnet test --verbosity normal

# Apenas testes unitários
dotnet test Tests/RhSensoERP.Tests.Unit/

# Apenas testes de integração
dotnet test Tests/RhSensoERP.Tests.Integration/

# Excluir testes de banco (se conexão indisponível)
dotnet test --filter "FullyQualifiedName!~DatabaseTests"
```

## 👥 Code Review

### **1. Template de Pull Request**

```markdown
## 📝 Descrição

Breve descrição das mudanças realizadas.

## 🎯 Tipo de Mudança

- [ ] 🐛 Bug fix
- [ ] ✨ Nova feature
- [ ] 💥 Breaking change
- [ ] 📚 Documentação
- [ ] 🎨 Melhoria de código
- [ ] ⚡ Performance
- [ ] 🧪 Testes

## 🧪 Testes

- [ ] Testes unitários adicionados/atualizados
- [ ] Testes de integração adicionados/atualizados
- [ ] Todos os testes estão passando
- [ ] Code coverage mantido/melhorado

## 📋 Checklist

- [ ] Código segue os padrões do [perfil do desenvolvedor](../perfil_desenvolvedor_erp_api.md)
- [ ] Documentação atualizada (se necessário)
- [ ] Não há TODOs ou FIXMEs no código
- [ ] Performance considerada
- [ ] Segurança revisada
- [ ] Logs apropriados adicionados

## 🔗 Issues Relacionadas

Fixes #123
Related to #456

## 📸 Screenshots (se aplicável)

[Adicionar screenshots se houver mudanças na UI]

## 📝 Notas Adicionais

Informações adicionais para os revisores.
```

### **2. Checklist do Revisor**

#### **Arquitetura e Design**
- [ ] ✅ Segue Clean Architecture
- [ ] ✅ Responsabilidades bem definidas
- [ ] ✅ Padrões do projeto seguidos
- [ ] ✅ Não há violação de dependências

#### **Código**
- [ ] ✅ Nomenclatura clara e consistente
- [ ] ✅ Funções pequenas e focadas
- [ ] ✅ Sem duplicação de código
- [ ] ✅ Tratamento adequado de erros

#### **Segurança**
- [ ] ✅ Validação de entrada
- [ ] ✅ Autorização adequada
- [ ] ✅ Não exposição de dados sensíveis
- [ ] ✅ Logs de segurança implementados

#### **Performance**
- [ ] ✅ Queries otimizadas
- [ ] ✅ Uso adequado de async/await
- [ ] ✅ Sem vazamentos de memória
- [ ] ✅ Cache considerado (se aplicável)

#### **Testes**
- [ ] ✅ Cobertura adequada
- [ ] ✅ Casos edge testados
- [ ] ✅ Mocks apropriados
- [ ] ✅ Testes rápidos e isolados

### **3. Comentários de Code Review**

```csharp
// ❌ COMENTÁRIO RUIM
// Este código está errado

// ✅ COMENTÁRIO BOM
// Sugestão: Use repository pattern aqui para melhor testabilidade
// Exemplo:
// var user = await _userRepository.GetByIdAsync(id, ct);

// ✅ COMENTÁRIO CONSTRUTIVO
// Nitpick: Considere extrair esta lógica para um método privado 
// para melhorar a legibilidade. Não é bloqueante.

// ✅ COMENTÁRIO DE SEGURANÇA
// Crítico: Esta query é vulnerável a SQL Injection. 
// Use parâmetros: cmd.Parameters.AddWithValue("@id", id)
```

## 🐛 Debugging

### **1. Setup de Debug**

#### **Visual Studio**
```json
// launchSettings.json - Configuração de debug
{
  "profiles": {
    "RhSensoERP.API": {
      "commandName": "Project",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ASPNETCORE_URLS": "https://localhost:57148;http://localhost:57149"
      }
    },
    "Docker": {
      "commandName": "Docker",
      "launchBrowser": true,
      "launchUrl": "{Scheme}://{ServiceHost}:{ServicePort}/swagger"
    }
  }
}
```

#### **VS Code**
```json
// .vscode/launch.json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Launch (web)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/Src/API/bin/Debug/net8.0/RhSensoERP.API.dll",
            "args": [],
            "cwd": "${workspaceFolder}/Src/API",
            "stopAtEntry": false,
            "serverReadyAction": {
                "action": "openExternally",
                "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
            },
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            }
        }
    ]
}
```

### **2. Debugging de Problemas Comuns**

#### **Problema: JWT não funciona**
```csharp
// Debug: Verificar configuração JWT
public class JwtDebugController : ControllerBase
{
    [HttpGet("debug/jwt-config")]
    public IActionResult GetJwtConfig([FromServices] IOptions<JwtOptions> jwtOptions)
    {
        var jwt = jwtOptions.Value;
        return Ok(new
        {
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            HasSecretKey = !string.IsNullOrEmpty(jwt.SecretKey),
            HasPublicKey = !string.IsNullOrEmpty(jwt.PublicKeyPem),
            AccessTokenMinutes = jwt.AccessTokenMinutes
        });
    }
}
```

#### **Problema: Entity Framework não carrega dados**
```csharp
// Debug: Verificar queries EF
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
    // ou
    optionsBuilder.EnableSensitiveDataLogging(); // APENAS EM DESENVOLVIMENTO
}
```

#### **Problema: Dependency Injection**
```csharp
// Debug: Verificar serviços registrados
public class DiagnosticsController : ControllerBase
{
    [HttpGet("debug/services")]
    public IActionResult GetRegisteredServices([FromServices] IServiceProvider serviceProvider)
    {
        // APENAS EM DESENVOLVIMENTO
        var services = serviceProvider.GetService<IServiceCollection>();
        return Ok(services?.Select(s => new { s.ServiceType.Name, s.Lifetime }));
    }
}
```

### **3. Logging para Debug**

```csharp
// ✅ Logs estruturados para debugging
public class UserService
{
    private readonly ILogger<UserService> _logger;

    public async Task<User> GetUserAsync(string cdUsuario)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["UserId"] = cdUsuario,
            ["Method"] = nameof(GetUserAsync),
            ["Timestamp"] = DateTime.UtcNow
        });

        _logger.LogDebug("Iniciando busca de usuário {CdUsuario}", cdUsuario);

        try
        {
            var user = await _userRepository.GetByCdUsuarioAsync(cdUsuario);
            
            if (user == null)
            {
                _logger.LogWarning("Usuário {CdUsuario} não encontrado", cdUsuario);
                return null;
            }

            _logger.LogDebug("Usuário {CdUsuario} encontrado com sucesso", cdUsuario);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário {CdUsuario}", cdUsuario);
            throw;
        }
    }
}
```

## ⚡ Performance

### **1. Profiling e Monitoramento**

```csharp
// ✅ Middleware para medir performance
public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMiddleware> _logger;

    public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        await _next(context);
        
        stopwatch.Stop();
        
        if (stopwatch.ElapsedMilliseconds > 1000) // Log requests > 1s
        {
            _logger.LogWarning("Slow request: {Method} {Path} took {ElapsedMs}ms",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
```

### **2. Otimizações de Banco**

```csharp
// ✅ Queries otimizadas
public class UserRepository
{
    // AsNoTracking para read-only queries
    public async Task<List<UserDto>> GetActiveUsersAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.FlAtivo == 'S')
            .Select(u => new UserDto(u.CdUsuario, u.DcUsuario)) // Projete apenas campos necessários
            .ToListAsync();
    }

    // Include explícito para evitar N+1
    public async Task<User> GetUserWithGroupsAsync(string cdUsuario)
    {
        return await _context.Users
            .Include(u => u.UserGroups)
                .ThenInclude(ug => ug.GrupoDeUsuario)
            .FirstOrDefaultAsync(u => u.CdUsuario == cdUsuario);
    }

    // Paginação eficiente
    public async Task<PagedResult<User>> GetUsersPagedAsync(int page, int pageSize)
    {
        var query = _context.Users.AsNoTracking();
        
        var total = await query.CountAsync();
        
        var items = await query
            .OrderBy(u => u.CdUsuario)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<User>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }
}
```

### **3. Caching**

```csharp
// ✅ Cache distribuído (Redis) para dados frequentemente acessados
public class CachedUserService : IUserService
{
    private readonly IUserService _userService;
    private readonly IDistributedCache _cache;
    private readonly ILogger<CachedUserService> _logger;

    public async Task<UserPermissions> GetUserPermissionsAsync(string cdUsuario, CancellationToken ct)
    {
        var cacheKey = $"user:permissions:{cdUsuario}";
        
        var cachedData = await _cache.GetStringAsync(cacheKey, ct);
        if (cachedData != null)
        {
            _logger.LogDebug("Cache hit for user permissions {CdUsuario}", cdUsuario);
            return JsonSerializer.Deserialize<UserPermissions>(cachedData)!;
        }

        var permissions = await _userService.GetUserPermissionsAsync(cdUsuario, ct);
        
        var serialized = JsonSerializer.Serialize(permissions);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };
        
        await _cache.SetStringAsync(cacheKey, serialized, options, ct);
        
        _logger.LogDebug("Cache miss for user permissions {CdUsuario}, data cached", cdUsuario);
        return permissions;
    }
}
```

### **4. Ferramentas de Performance**

```bash
# BenchmarkDotNet para micro-benchmarks
dotnet add package BenchmarkDotNet

# PerfView para profiling detalhado (Windows)
# https://github.com/Microsoft/perfview

# dotMemory para análise de memória (JetBrains)
# https://www.jetbrains.com/dotmemory/

# Application Insights para monitoramento em produção
dotnet add package Microsoft.ApplicationInsights.AspNetCore
```

## 📚 Recursos e Referências

### **Documentação Interna**
- 📖 **[Perfil do Desenvolvedor](../perfil_desenvolvedor_erp_api.md)** - LEITURA OBRIGATÓRIA
- 🏗️ **[Arquitetura](../docs/architecture/overview.md)** - Overview da Clean Architecture
- 🔐 **[Autenticação](../docs/api/authentication.md)** - Sistema JWT e Legacy
- ⚙️ **[Configuração](../docs/getting-started/configuration.md)** - Setup completo

### **Ferramentas Essenciais**
- 🔍 **[Swagger UI](https://localhost:57148/swagger)** - Documentação da API
- 📊 **[Health Checks](https://localhost:57148/health)** - Status da aplicação
- 🧪 **[Testes via API](https://localhost:57148/api/v1/test/banco)** - Verificar banco

### **Recursos Externos**
- 📚 **[Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)** - Robert C. Martin
- 🎯 **[.NET Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/)** - Microsoft
- ✅ **[FluentValidation](https://docs.fluentvalidation.net/en/latest/)** - Documentação oficial
- 🧪 **[FluentAssertions](https://fluentassertions.com/introduction)** - Sintaxe de testes

### **Comunidade**
- 💬 **Slack**: #rhsenso-api-dev
- 📧 **Email**: dev-team@rhsenso.com
- 🎥 **Teams**: Reuniões de arquitetura (terças 14h)

## 🚨 Quando Pedir Ajuda

### **Antes de Pedir**
- [ ] ✅ Li a documentação relevante
- [ ] ✅ Busquei no histórico do Slack/Teams
- [ ] ✅ Tentei debuggar por pelo menos 30min
- [ ] ✅ Verifiquei logs e stack traces

### **Como Pedir Ajuda Efetivamente**

```markdown
## 🐛 Problema

Descrição clara e concisa do problema.

## 🎯 Objetivo

O que você está tentando alcançar.

## 🔄 Tentativas

1. Tentei X, resultado Y
2. Tentei Z, resultado W

## 💻 Código Relevante

```csharp
// Cole aqui o código com problema
```

## 📊 Logs/Erros

```
[Stack trace ou mensagens de erro]
```

## 🌍 Ambiente

- SO: Windows 11 / Ubuntu 22.04
- IDE: Visual Studio 2022 / VS Code
- .NET: 8.0.1
- Banco: SQL Server LocalDB
```

## 🎉 Conclusão

Lembre-se: **Você é um Arquiteto de Soluções .NET**, não apenas um codificador. Cada linha de código que você escreve impacta:

- ✅ **Qualidade** do produto
- ✅ **Segurança** dos dados
- ✅ **Performance** da aplicação
- ✅ **Manutenibilidade** do código
- ✅ **Experiência** do usuário

**Próximos passos:**
1. 📚 Leia o [perfil_desenvolvedor_erp_api.md](../perfil_desenvolvedor_erp_api.md)
2. ⚙️ Configure seu ambiente de desenvolvimento
3. 🧪 Execute todos os testes
4. 🚀 Comece sua primeira feature

---

💡 **Dica Final**: Quando em dúvida, sempre prefira simplicidade e clareza. Código que funciona hoje e é mantível amanhã é melhor que código "elegante" que ninguém entende.