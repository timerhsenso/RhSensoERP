# 📚 RhSensoERP v2.0 - Documentação Completa

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL_Server-2019+-CC2927?style=for-the-badge&logo=microsoft-sql-server)
![License](https://img.shields.io/badge/License-Proprietary-red?style=for-the-badge)
![Build](https://img.shields.io/badge/Build-Passing-success?style=for-the-badge)

## 📋 Índice

- [1. Visão Geral](#1-visão-geral)
- [2. Arquitetura](#2-arquitetura)
- [3. Requisitos do Sistema](#3-requisitos-do-sistema)
- [4. Configuração - Desenvolvimento](#4-configuração---desenvolvimento)
- [5. Configuração - Produção](#5-configuração---produção)
- [6. Estrutura do Projeto](#6-estrutura-do-projeto)
- [7. Módulos do Sistema](#7-módulos-do-sistema)
- [8. Segurança e Autenticação](#8-segurança-e-autenticação)
- [9. API Documentation](#9-api-documentation)
- [10. Banco de Dados](#10-banco-de-dados)
- [11. Testes](#11-testes)
- [12. Deploy e DevOps](#12-deploy-e-devops)
- [13. Monitoramento](#13-monitoramento)
- [14. Troubleshooting](#14-troubleshooting)
- [15. Scripts Úteis](#15-scripts-úteis)
- [16. Contribuindo](#16-contribuindo)
- [17. Suporte](#17-suporte)

---

## 1. Visão Geral

**RhSensoERP** é um sistema completo de gestão de recursos humanos desenvolvido em **.NET 8** com arquitetura modular, projetado para empresas de médio e grande porte que necessitam de uma solução robusta e escalável para gestão de pessoas.

### 🎯 Principais Características

- ✅ **Multi-tenant**: Suporte para múltiplas empresas/filiais
- ✅ **Modular**: Arquitetura baseada em módulos independentes
- ✅ **Seguro**: Autenticação JWT com refresh tokens
- ✅ **Escalável**: Clean Architecture + DDD
- ✅ **Auditável**: Log completo de todas as operações
- ✅ **Integrável**: APIs RESTful documentadas
- ✅ **Performance**: Cache distribuído com Redis

### 📊 Números do Projeto

```
Linhas de Código: ~150.000
Tabelas no BD: 200+
APIs Endpoints: 300+
Testes Automatizados: 1.500+
Módulos: 8
```

---

## 2. Arquitetura

### 🏗️ Padrões e Princípios

```mermaid
graph TB
    A[Presentation Layer] --> B[Application Layer]
    B --> C[Domain Layer]
    B --> D[Infrastructure Layer]
    
    subgraph "Clean Architecture"
        C[Domain - Entidades e Regras de Negócio]
        B[Application - Casos de Uso]
        D[Infrastructure - Implementações]
        A[Presentation - Controllers/Views]
    end
```

### 📦 Stack Tecnológico

| Camada | Tecnologia | Versão | Propósito |
|--------|------------|--------|-----------|
| **Backend** | .NET / ASP.NET Core | 8.0 | Framework principal |
| **ORM** | Entity Framework Core | 8.0.11 | Acesso a dados |
| **Database** | SQL Server | 2019+ | Banco de dados principal |
| **Cache** | Redis | 7.0+ | Cache distribuído |
| **Auth** | JWT Bearer | - | Autenticação |
| **Docs** | Swagger/OpenAPI | 9.0.6 | Documentação da API |
| **Logs** | Serilog | 8.0.0 | Sistema de logs |
| **Tests** | xUnit + Moq | 2.9.2 | Testes unitários |
| **Monitoring** | OpenTelemetry | 1.9.0 | Observabilidade |

### 🎨 Padrões de Design

- **SOLID**: Princípios de design orientado a objetos
- **DDD**: Domain-Driven Design
- **CQRS**: Command Query Responsibility Segregation
- **Repository Pattern**: Abstração de acesso a dados
- **Unit of Work**: Transações atômicas
- **Specification Pattern**: Queries reutilizáveis
- **Mediator Pattern**: Desacoplamento via MediatR

---

## 3. Requisitos do Sistema

### 💻 Ambiente de Desenvolvimento

#### Software Obrigatório

| Software | Versão Mínima | Download |
|----------|---------------|----------|
| **.NET SDK** | 8.0.100+ | [Download](https://dotnet.microsoft.com/download/dotnet/8.0) |
| **Visual Studio** | 2022 (17.8+) | [Download](https://visualstudio.microsoft.com/) |
| **SQL Server** | 2019 Express+ | [Download](https://www.microsoft.com/sql-server/sql-server-downloads) |
| **Git** | 2.40+ | [Download](https://git-scm.com/) |
| **Node.js** | 18.0+ | [Download](https://nodejs.org/) |

#### Software Recomendado

| Software | Propósito |
|----------|-----------|
| **Docker Desktop** | Containers para Redis/SQL |
| **Postman/Insomnia** | Teste de APIs |
| **Azure Data Studio** | Gerenciamento de BD |
| **VS Code** | Editor alternativo |
| **Redis Insight** | Visualizar cache Redis |

### 🖥️ Ambiente de Produção

#### Requisitos Mínimos do Servidor

```yaml
Sistema Operacional:
  - Windows Server 2019+ (Recomendado)
  - Ubuntu Server 20.04 LTS
  - RHEL 8+

Hardware:
  - CPU: 4 cores (8 recomendado)
  - RAM: 8GB (16GB recomendado)
  - Disco: 100GB SSD
  - Rede: 100Mbps (1Gbps recomendado)

Software:
  - .NET 8.0 Runtime
  - ASP.NET Core Runtime 8.0
  - IIS 10+ (Windows) ou Nginx (Linux)
  - SQL Server 2019+
  - Redis Server 6.0+
  - SSL Certificate
```

---

## 4. Configuração - Desenvolvimento

### 🚀 Quick Start (Início Rápido)

```bash
# 1. Clonar repositório
git clone https://github.com/rhsenso/RhSensoERP.git
cd RhSensoERP

# 2. Restaurar pacotes
dotnet restore

# 3. Configurar secrets
cd src/API
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=bd_rhu_copenor_dev;User Id=sa;Password=SuaSenha123!;TrustServerCertificate=true"
dotnet user-secrets set "Jwt:Key" "ChaveSecretaComPeloMenos256BitsParaSegurancaJWT2024!@#$%"

# 4. Criar banco de dados
dotnet ef database update

# 5. Executar aplicação
dotnet run
```

### 🔧 Configuração Detalhada

#### Passo 1: Clonar e Preparar o Projeto

```bash
# Clone com submódulos (se houver)
git clone --recursive https://github.com/rhsenso/RhSensoERP.git
cd RhSensoERP

# Verificar branch
git branch -a
git checkout develop  # Para desenvolvimento

# Instalar ferramentas globais
dotnet tool install --global dotnet-ef
dotnet tool install --global dotnet-format
```

#### Passo 2: Configurar Banco de Dados

##### Opção A: SQL Server Local

```sql
-- Executar no SQL Server Management Studio
CREATE DATABASE bd_rhu_copenor_dev;
GO

USE bd_rhu_copenor_dev;
GO

-- Criar login se necessário
CREATE LOGIN [rhsenso_dev] WITH PASSWORD = 'DevPassword123!';
CREATE USER [rhsenso_dev] FOR LOGIN [rhsenso_dev];
ALTER ROLE db_owner ADD MEMBER [rhsenso_dev];
GO
```

##### Opção B: SQL Server via Docker

```bash
# Criar container SQL Server
docker run -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=YourStrong@Password123" \
  -p 1433:1433 \
  --name sqlserver-dev \
  -v sqldata:/var/opt/mssql \
  -d mcr.microsoft.com/mssql/server:2019-latest

# Criar banco via sqlcmd
docker exec -it sqlserver-dev /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Password123 \
  -Q "CREATE DATABASE bd_rhu_copenor_dev"
```

#### Passo 3: Configurar Redis (Opcional mas Recomendado)

```bash
# Via Docker
docker run --name redis-dev -p 6379:6379 -d redis:alpine

# Ou via instalação local (Windows)
# Download: https://github.com/microsoftarchive/redis/releases
```

#### Passo 4: User Secrets (Dados Sensíveis)

```bash
cd src/API

# Banco de Dados
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=bd_rhu_copenor_dev;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=true"

# JWT
dotnet user-secrets set "Jwt:Key" "Esta-E-Uma-Chave-Super-Secreta-Com-Mais-De-256-Bits-Para-JWT-2024!@#$%^&*()"

# Redis
dotnet user-secrets set "Redis:Configuration" "localhost:6379,abortConnect=false"

# Email (para testes)
dotnet user-secrets set "Diagnostics:Smtp:Host" "smtp.gmail.com"
dotnet user-secrets set "Diagnostics:Smtp:Port" "587"
dotnet user-secrets set "Diagnostics:Smtp:User" "seu-email@gmail.com"
dotnet user-secrets set "Diagnostics:Smtp:Pass" "sua-senha-de-app"

# Azure (se usar)
dotnet user-secrets set "AzureKeyVault:Url" "https://seu-keyvault.vault.azure.net/"
dotnet user-secrets set "AzureKeyVault:ClientId" "seu-client-id"
dotnet user-secrets set "AzureKeyVault:ClientSecret" "seu-client-secret"
```

#### Passo 5: Aplicar Migrations

```bash
# Navegar para pasta da API
cd src/API

# Criar/Atualizar banco de dados
dotnet ef database update --project ../Identity/RhSensoERP.Identity.csproj
dotnet ef database update --project ../Modules/GestaoDePessoas/RhSensoERP.Modules.GestaoDePessoas.csproj

# Verificar migrations aplicadas
dotnet ef migrations list --project ../Identity/RhSensoERP.Identity.csproj
```

#### Passo 6: Dados de Teste (Seed)

```sql
-- Executar script de seed
-- scripts/seed-development.sql

-- Usuário padrão para testes
INSERT INTO SEG_USUARIO (cdusuario, nmusuario, dsemail, dssenha, flativo)
VALUES 
  ('admin', 'Administrador', 'admin@rhsenso.local', '$2a$11$...', 1),
  ('teste', 'Usuario Teste', 'teste@rhsenso.local', '$2a$11$...', 1);
```

#### Passo 7: Executar a Aplicação

```bash
# Opção 1: CLI com hot reload
cd src/API
dotnet watch run

# Opção 2: Visual Studio
# 1. Abrir RhSensoERP.sln
# 2. Definir RhSensoERP.API como Startup Project
# 3. F5 (Debug) ou Ctrl+F5 (sem debug)

# Opção 3: VS Code
# 1. Abrir pasta raiz no VS Code
# 2. F5 para debug (configurar launch.json se necessário)
```

### 🌐 URLs de Desenvolvimento

| Serviço | URL | Descrição |
|---------|-----|-----------|
| **API** | https://localhost:7193 | Endpoint principal HTTPS |
| **API HTTP** | http://localhost:5174 | Endpoint HTTP (redirect) |
| **Swagger UI** | https://localhost:7193/swagger | Documentação interativa |
| **Health Check** | https://localhost:7193/health | Status da aplicação |
| **Health UI** | https://localhost:7193/health-ui | Dashboard de saúde |
| **Diagnostics** | https://localhost:7193/api/diagnostics/overview | Diagnóstico completo |

### 🧪 Testar a Instalação

```bash
# 1. Verificar API está rodando
curl https://localhost:7193/health

# 2. Testar autenticação
curl -X POST https://localhost:7193/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"usuario":"admin","senha":"admin123"}'

# 3. Verificar Swagger
start https://localhost:7193/swagger

# 4. Testar conexão com banco
curl https://localhost:7193/api/diagnostics/database
```

---

## 5. Configuração - Produção

### 🏭 Deploy em Windows Server + IIS

#### Preparação do Servidor

```powershell
# 1. Instalar IIS e recursos necessários
Enable-WindowsFeature -Name Web-Server
Enable-WindowsFeature -Name Web-Common-Http
Enable-WindowsFeature -Name Web-Static-Content
Enable-WindowsFeature -Name Web-Http-Errors
Enable-WindowsFeature -Name Web-Http-Redirect
Enable-WindowsFeature -Name Web-Net-Ext45
Enable-WindowsFeature -Name Web-Asp-Net45
Enable-WindowsFeature -Name Web-ISAPI-Ext
Enable-WindowsFeature -Name Web-ISAPI-Filter

# 2. Instalar .NET 8 Hosting Bundle
# Download: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
# Arquivo: dotnet-hosting-8.0.x-win.exe

# 3. Reiniciar IIS
iisreset

# 4. Criar estrutura de pastas
New-Item -Path "C:\inetpub\RhSensoERP" -ItemType Directory
New-Item -Path "C:\inetpub\RhSensoERP\logs" -ItemType Directory
New-Item -Path "C:\inetpub\RhSensoERP\uploads" -ItemType Directory
New-Item -Path "C:\inetpub\RhSensoERP\temp" -ItemType Directory

# 5. Configurar permissões
$acl = Get-Acl "C:\inetpub\RhSensoERP"
$permission = "IIS_IUSRS","FullControl","ContainerInherit,ObjectInherit","None","Allow"
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule $permission
$acl.SetAccessRule($accessRule)
Set-Acl "C:\inetpub\RhSensoERP" $acl
```

#### Configuração do Banco de Dados

```sql
-- Criar banco de produção
CREATE DATABASE bd_rhu_copenor
GO

-- Criar login para aplicação
CREATE LOGIN [rhsenso_app] WITH PASSWORD = 'Pr0duct10n$tr0ng!P@ssw0rd';
GO

USE bd_rhu_copenor;
GO

CREATE USER [rhsenso_app] FOR LOGIN [rhsenso_app];
ALTER ROLE db_datareader ADD MEMBER [rhsenso_app];
ALTER ROLE db_datawriter ADD MEMBER [rhsenso_app];
ALTER ROLE db_executor ADD MEMBER [rhsenso_app];
GO

-- Executar scripts de criação
-- scripts/01-create-tables.sql
-- scripts/02-create-procedures.sql
-- scripts/03-create-indexes.sql
-- scripts/04-seed-production.sql
```

#### Build e Publicação

```bash
# 1. Compilar em Release
dotnet build -c Release

# 2. Publicar aplicação
dotnet publish src/API/RhSensoERP.API.csproj \
  -c Release \
  -o C:\inetpub\RhSensoERP \
  --runtime win-x64 \
  --self-contained false

# 3. Ou usar perfil de publicação
dotnet publish -p:PublishProfile=IISProfile
```

#### Configuração IIS

```xml
<!-- C:\inetpub\RhSensoERP\web.config -->
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet"
                  arguments=".\RhSensoERP.API.dll"
                  stdoutLogEnabled="true"
                  stdoutLogFile=".\logs\stdout"
                  hostingModel="InProcess">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
          <environmentVariable name="ASPNETCORE_HTTPS_PORT" value="443" />
        </environmentVariables>
      </aspNetCore>
      <security>
        <requestFiltering>
          <requestLimits maxAllowedContentLength="52428800" />
        </requestFiltering>
      </security>
      <httpProtocol>
        <customHeaders>
          <remove name="X-Powered-By" />
          <add name="X-Frame-Options" value="SAMEORIGIN" />
          <add name="X-XSS-Protection" value="1; mode=block" />
          <add name="X-Content-Type-Options" value="nosniff" />
          <add name="Referrer-Policy" value="strict-origin-when-cross-origin" />
        </customHeaders>
      </httpProtocol>
      <rewrite>
        <rules>
          <rule name="Redirect to HTTPS" stopProcessing="true">
            <match url=".*" />
            <conditions>
              <add input="{HTTPS}" pattern="off" ignoreCase="true" />
            </conditions>
            <action type="Redirect" url="https://{HTTP_HOST}/{R:0}" redirectType="Permanent" />
          </rule>
        </rules>
      </rewrite>
    </system.webServer>
  </location>
</configuration>
```

#### Configurar Site no IIS

```powershell
# Via PowerShell
Import-Module WebAdministration

# Criar Application Pool
New-WebAppPool -Name "RhSensoERP_AppPool"
Set-ItemProperty -Path "IIS:\AppPools\RhSensoERP_AppPool" -Name processIdentity.identityType -Value ApplicationPoolIdentity
Set-ItemProperty -Path "IIS:\AppPools\RhSensoERP_AppPool" -Name recycling.periodicRestart.time -Value "00:00:00"
Set-ItemProperty -Path "IIS:\AppPools\RhSensoERP_AppPool" -Name managedRuntimeVersion -Value ""

# Criar Site
New-Website -Name "RhSensoERP" `
  -Port 443 `
  -Protocol https `
  -PhysicalPath "C:\inetpub\RhSensoERP" `
  -ApplicationPool "RhSensoERP_AppPool"

# Configurar SSL
$cert = Get-ChildItem -Path Cert:\LocalMachine\My | Where-Object {$_.Subject -match "rhsenso.com.br"}
New-WebBinding -Name "RhSensoERP" -Protocol https -Port 443 -HostHeader "api.rhsenso.com.br" -SslFlags 1
$binding = Get-WebBinding -Name "RhSensoERP" -Protocol https
$binding.AddSslCertificate($cert.Thumbprint, "My")
```

### 🐧 Deploy em Linux (Ubuntu/Docker)

#### Dockerfile

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["src/API/RhSensoERP.API.csproj", "API/"]
COPY ["src/Identity/RhSensoERP.Identity.csproj", "Identity/"]
COPY ["src/Shared/Core/RhSensoERP.Shared.Core.csproj", "Shared/Core/"]
COPY ["src/Shared/Application/RhSensoERP.Shared.Application.csproj", "Shared/Application/"]
COPY ["src/Shared/Infrastructure/RhSensoERP.Shared.Infrastructure.csproj", "Shared/Infrastructure/"]
COPY ["src/Shared/Contracts/RhSensoERP.Shared.Contracts.csproj", "Shared/Contracts/"]
COPY ["src/Modules/GestaoDePessoas/RhSensoERP.Modules.GestaoDePessoas.csproj", "Modules/GestaoDePessoas/"]

# Restaurar dependências
RUN dotnet restore "API/RhSensoERP.API.csproj"

# Copiar código fonte
COPY src/ .

# Build
WORKDIR "/src/API"
RUN dotnet build "RhSensoERP.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RhSensoERP.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Criar usuário não-root
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "RhSensoERP.API.dll"]
```

#### Docker Compose

```yaml
# docker-compose.yml
version: '3.8'

services:
  api:
    image: rhsenso/api:latest
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "8080:80"
      - "8443:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=https://+:443;http://+:80
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=bd_rhu_copenor;User Id=sa;Password=${DB_PASSWORD}
      - Redis__Configuration=redis:6379
      - Jwt__Key=${JWT_KEY}
    depends_on:
      - sqlserver
      - redis
    networks:
      - rhsenso-network
    volumes:
      - ./logs:/app/logs
      - ./uploads:/app/uploads

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2019-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=${DB_PASSWORD}
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql
    networks:
      - rhsenso-network

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redisdata:/data
    networks:
      - rhsenso-network

  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf
      - ./ssl:/etc/nginx/ssl
    depends_on:
      - api
    networks:
      - rhsenso-network

networks:
  rhsenso-network:
    driver: bridge

volumes:
  sqldata:
  redisdata:
```

#### Deploy com Docker

```bash
# 1. Build da imagem
docker build -t rhsenso/api:latest .

# 2. Criar arquivo .env
cat > .env <<EOF
DB_PASSWORD=Str0ngP@ssw0rd!
JWT_KEY=SuperSecretKeyWith256BitsOrMore2024!@#
EOF

# 3. Iniciar containers
docker-compose up -d

# 4. Verificar logs
docker-compose logs -f api

# 5. Aplicar migrations
docker exec -it rhsenso_api_1 dotnet ef database update
```

---

## 6. Estrutura do Projeto

### 📁 Organização de Pastas

```
RhSensoERP/
├── 📁 src/                              # Código fonte
│   ├── 📁 API/                          # API principal
│   │   ├── 📁 Configuration/            # Configurações (Swagger, etc)
│   │   ├── 📁 Controllers/              # Controllers da API
│   │   ├── 📁 Filters/                  # Action filters
│   │   ├── 📁 Middleware/               # Middlewares customizados
│   │   ├── 📄 Program.cs                # Entry point
│   │   ├── 📄 appsettings.json          # Configurações
│   │   └── 📄 RhSensoERP.API.csproj
│   │
│   ├── 📁 Web/                          # Interface Web (MVC)
│   │   ├── 📁 Areas/                    # Áreas do MVC
│   │   ├── 📁 wwwroot/                  # Arquivos estáticos
│   │   └── 📁 Views/                    # Views Razor
│   │
│   ├── 📁 Identity/                     # Módulo de autenticação
│   │   ├── 📁 Application/              # Serviços de autenticação
│   │   ├── 📁 Domain/                   # Entidades de usuário
│   │   └── 📁 Infrastructure/           # JWT, Identity
│   │
│   ├── 📁 Shared/                       # Código compartilhado
│   │   ├── 📁 Core/                     # Base entities, interfaces
│   │   ├── 📁 Application/              # DTOs, interfaces
│   │   ├── 📁 Infrastructure/           # Implementações comuns
│   │   └── 📁 Contracts/                # Contratos/DTOs públicos
│   │
│   └── 📁 Modules/                      # Módulos do sistema
│       ├── 📁 GestaoDePessoas/
│       │   ├── 📁 Application/          # Use cases, DTOs
│       │   ├── 📁 Domain/               # Entities, value objects
│       │   ├── 📁 Infrastructure/       # EF, repositories
│       │   └── 📁 Presentation/         # Controllers do módulo
│       │
│       ├── 📁 ControleDePonto/
│       ├── 📁 Treinamentos/
│       ├── 📁 SaudeOcupacional/
│       ├── 📁 Avaliacoes/
│       └── 📁 Esocial/
│
├── 📁 tests/                            # Testes automatizados
│   ├── 📁 Unit/                         # Testes unitários
│   ├── 📁 Integration/                  # Testes de integração
│   ├── 📁 Architecture/                 # Testes de arquitetura
│   └── 📁 E2E/                          # Testes end-to-end
│
├── 📁 scripts/                          # Scripts SQL e PowerShell
│   ├── 📄 01-create-tables.sql
│   ├── 📄 02-seed-data.sql
│   ├── 📄 03-stored-procedures.sql
│   └── 📄 reset-dev.ps1
│
├── 📁 docs/                             # Documentação
│   ├── 📄 README.md
│   ├── 📄 ARCHITECTURE.md
│   ├── 📄 API.md
│   └── 📄 CONTRIBUTING.md
│
├── 📁 .github/                          # GitHub Actions
│   └── 📁 workflows/
│       ├── 📄 build.yml
│       └── 📄 deploy.yml
│
├── 📄 .gitignore
├── 📄 .editorconfig
├── 📄 Directory.Build.props             # Configurações MSBuild globais
├── 📄 Directory.Packages.props          # Versionamento central de pacotes
├── 📄 global.json                       # Versão do SDK
└── 📄 RhSensoERP.sln                   # Solution file
```

### 📦 Projetos da Solution

| Projeto | Tipo | Descrição |
|---------|------|-----------|
| **RhSensoERP.API** | Web API | API RESTful principal |
| **RhSensoERP.Web** | MVC | Interface web administrativa |
| **RhSensoERP.Identity** | Class Library | Autenticação e autorização |
| **RhSensoERP.Shared.Core** | Class Library | Classes base e interfaces |
| **RhSensoERP.Shared.Application** | Class Library | DTOs e interfaces de aplicação |
| **RhSensoERP.Shared.Infrastructure** | Class Library | Implementações compartilhadas |
| **RhSensoERP.Shared.Contracts** | Class Library | Contratos públicos |
| **RhSensoERP.Modules.*** | Class Library | Módulos de negócio |
| **RhSensoERP.*Tests** | Test | Projetos de teste |

---

## 7. Módulos do Sistema

### 📋 Gestão de Pessoas

**Namespace**: `RhSensoERP.Modules.GestaoDePessoas`

Gerenciamento completo de colaboradores, dados pessoais, documentos e informações contratuais.

#### Principais Funcionalidades
- ✅ Cadastro de colaboradores
- ✅ Gestão de documentos
- ✅ Controle de dependentes
- ✅ Histórico profissional
- ✅ Gestão de contratos

#### APIs Principais
```http
GET    /api/pessoas                 # Listar pessoas
GET    /api/pessoas/{id}           # Obter pessoa
POST   /api/pessoas                 # Criar pessoa
PUT    /api/pessoas/{id}           # Atualizar pessoa
DELETE /api/pessoas/{id}           # Remover pessoa

GET    /api/pessoas/{id}/documentos     # Documentos da pessoa
GET    /api/pessoas/{id}/dependentes    # Dependentes
GET    /api/pessoas/{id}/contratos      # Contratos de trabalho
```

### ⏰ Controle de Ponto

**Namespace**: `RhSensoERP.Modules.ControleDePonto`

Sistema de registro e controle de ponto eletrônico conforme legislação.

#### Principais Funcionalidades
- ✅ Registro de ponto online
- ✅ Importação de relógios de ponto
- ✅ Gestão de horários e escalas
- ✅ Banco de horas
- ✅ Relatórios legais

#### APIs Principais
```http
POST   /api/ponto/registrar         # Registrar ponto
GET    /api/ponto/espelho/{mes}     # Espelho de ponto
POST   /api/ponto/justificativa     # Adicionar justificativa
GET    /api/ponto/banco-horas       # Consultar banco de horas
POST   /api/ponto/importar          # Importar arquivo AFD
```

### 🎓 Treinamentos

**Namespace**: `RhSensoERP.Modules.Treinamentos`

Gestão de capacitações, treinamentos e desenvolvimento de pessoas.

#### Principais Funcionalidades
- ✅ Catálogo de treinamentos
- ✅ Gestão de turmas
- ✅ Controle de presença
- ✅ Certificados
- ✅ Avaliações de reação

### 🏥 Saúde Ocupacional

**Namespace**: `RhSensoERP.Modules.SaudeOcupacional`

Medicina e segurança do trabalho, exames ocupacionais e controle de EPIs.

#### Principais Funcionalidades
- ✅ ASO - Atestado de Saúde Ocupacional
- ✅ Controle de exames
- ✅ Gestão de EPIs
- ✅ PCMSO e PPRA
- ✅ CAT - Comunicação de Acidente

### 📊 Avaliações

**Namespace**: `RhSensoERP.Modules.Avaliacoes`

Sistema de avaliação de desempenho e gestão por competências.

#### Principais Funcionalidades
- ✅ Avaliações 360°
- ✅ Gestão por competências
- ✅ PDI - Plano de Desenvolvimento
- ✅ Feedback contínuo
- ✅ Relatórios gerenciais

### 📤 eSocial

**Namespace**: `RhSensoERP.Modules.Esocial`

Integração com o eSocial do governo federal.

#### Principais Funcionalidades
- ✅ Geração de eventos
- ✅ Validação de layouts
- ✅ Envio em lote
- ✅ Consulta de protocolos
- ✅ Gestão de retificações

---

## 8. Segurança e Autenticação

### 🔐 Autenticação JWT

#### Configuração

```json
// appsettings.json
{
  "Jwt": {
    "Key": "USE_SECRET_MANAGER_OR_KEYVAULT",
    "Issuer": "https://api.rhsenso.com.br",
    "Audience": "RhSensoERP.Client",
    "ExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  }
}
```

#### Login Request

```http
POST /api/auth/login
Content-Type: application/json

{
  "usuario": "admin",
  "senha": "Admin@123"
}
```

#### Login Response

```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "dGhpcyBpcyBhIHJlZnJl...",
    "expiresIn": 3600,
    "usuario": {
      "id": "123",
      "nome": "Administrador",
      "email": "admin@rhsenso.com.br",
      "permissoes": ["admin", "rh.write", "rh.read"]
    }
  }
}
```

#### Uso do Token

```http
GET /api/pessoas
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

### 🛡️ Autorização e Permissões

#### Níveis de Permissão

| Nível | Código | Descrição |
|-------|--------|-----------|
| **Incluir** | I | Criar novos registros |
| **Alterar** | A | Modificar registros |
| **Excluir** | E | Remover registros |
| **Consultar** | C | Visualizar registros |

#### Exemplo de Autorização

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PessoasController : ControllerBase
{
    [HttpGet]
    [RequirePermission("RHU.PESSOAS", "C")] // Consultar
    public async Task<IActionResult> GetAll() { }
    
    [HttpPost]
    [RequirePermission("RHU.PESSOAS", "I")] // Incluir
    public async Task<IActionResult> Create() { }
    
    [HttpPut("{id}")]
    [RequirePermission("RHU.PESSOAS", "A")] // Alterar
    public async Task<IActionResult> Update() { }
    
    [HttpDelete("{id}")]
    [RequirePermission("RHU.PESSOAS", "E")] // Excluir
    public async Task<IActionResult> Delete() { }
}
```

### 🔒 Segurança da API

#### Headers de Segurança

```csharp
// Configurados automaticamente
X-Content-Type-Options: nosniff
X-Frame-Options: SAMEORIGIN
X-XSS-Protection: 1; mode=block
Strict-Transport-Security: max-age=31536000; includeSubDomains
Content-Security-Policy: default-src 'self'
```

#### Rate Limiting

```json
{
  "RateLimit": {
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 60
      },
      {
        "Endpoint": "POST:/api/auth/login",
        "Period": "15m",
        "Limit": 5
      }
    ]
  }
}
```

---

## 9. API Documentation

### 📖 Swagger/OpenAPI

#### Acessar Documentação

- **Desenvolvimento**: https://localhost:7193/swagger
- **Staging**: https://staging-api.rhsenso.com.br/swagger
- **Produção**: Desabilitado por segurança

#### Documentos Disponíveis

| Documento | Descrição | URL |
|-----------|-----------|-----|
| **Geral** | Todos os endpoints | `/swagger/v1/swagger.json` |
| **GestaoDePessoas** | APIs de RH | `/swagger/GestaoDePessoas/swagger.json` |
| **Identity** | Autenticação | `/swagger/Identity/swagger.json` |
| **ControleDePonto** | APIs de ponto | `/swagger/ControleDePonto/swagger.json` |

### 📡 Exemplos de Requisições

#### Listar Pessoas com Paginação

```http
GET /api/pessoas?page=1&pageSize=20&search=João
Authorization: Bearer {token}
```

```json
{
  "items": [
    {
      "id": 1,
      "nome": "João Silva",
      "cpf": "123.456.789-00",
      "email": "joao@empresa.com",
      "cargo": "Analista"
    }
  ],
  "totalCount": 150,
  "page": 1,
  "pageSize": 20,
  "totalPages": 8
}
```

#### Criar Novo Colaborador

```http
POST /api/pessoas
Authorization: Bearer {token}
Content-Type: application/json

{
  "nome": "Maria Santos",
  "cpf": "987.654.321-00",
  "dataNascimento": "1990-05-15",
  "email": "maria.santos@empresa.com",
  "telefone": "(11) 98765-4321",
  "endereco": {
    "logradouro": "Rua das Flores",
    "numero": "123",
    "bairro": "Centro",
    "cidade": "São Paulo",
    "uf": "SP",
    "cep": "01234-567"
  }
}
```

---

## 10. Banco de Dados

### 🗄️ Estrutura Principal

#### Diagrama ER Simplificado

```mermaid
erDiagram
    SEG_USUARIO ||--o{ SEG_USUARIO_GRUPO : pertence
    SEG_GRUPO ||--o{ SEG_USUARIO_GRUPO : contem
    SEG_GRUPO ||--o{ SEG_GRUPO_PERMISSAO : possui
    SEG_PERMISSAO ||--o{ SEG_GRUPO_PERMISSAO : atribuida
    
    RHU_PESSOA ||--o{ RHU_CONTRATO : possui
    RHU_PESSOA ||--o{ RHU_DOCUMENTO : tem
    RHU_PESSOA ||--o{ RHU_DEPENDENTE : tem
    RHU_CONTRATO ||--o{ CPT_REGISTRO_PONTO : gera
    
    RHU_PESSOA {
        int cdpessoa PK
        string nome
        string cpf UK
        date dataNascimento
        string email
    }
    
    SEG_USUARIO {
        string cdusuario PK
        string email UK
        string senha
        bool ativo
    }
```

### 📊 Tabelas Principais

#### Módulo Segurança (SEG)

| Tabela | Descrição | Chave Primária |
|--------|-----------|----------------|
| SEG_USUARIO | Usuários do sistema | cdusuario |
| SEG_GRUPO | Grupos de usuários | cdgrupo |
| SEG_PERMISSAO | Permissões | cdpermissao |
| SEG_MENU | Menus do sistema | cdmenu |
| SEG_AUDITORIA | Log de auditoria | cdauditoria |

#### Módulo RH (RHU)

| Tabela | Descrição | Chave Primária |
|--------|-----------|----------------|
| RHU_PESSOA | Dados pessoais | cdpessoa |
| RHU_CONTRATO | Contratos de trabalho | cdcontrato |
| RHU_CARGO | Cargos | cdcargo |
| RHU_DEPARTAMENTO | Departamentos | cddepartamento |
| RHU_DOCUMENTO | Documentos | cddocumento |

### 🔧 Procedures Principais

```sql
-- Autenticação
EXEC SP_SEG_ValidarLogin @usuario, @senha

-- Relatórios
EXEC SP_RHU_RelatorioQuadroFuncionarios @dataInicio, @dataFim

-- Processamento
EXEC SP_CPT_ProcessarPonto @competencia

-- Auditoria
EXEC SP_SEG_RegistrarAuditoria @tabela, @operacao, @usuario
```

### 📈 Índices e Performance

```sql
-- Índices principais
CREATE INDEX IX_RHU_PESSOA_CPF ON RHU_PESSOA(cpf);
CREATE INDEX IX_RHU_PESSOA_NOME ON RHU_PESSOA(nome);
CREATE INDEX IX_SEG_AUDITORIA_DATA ON SEG_AUDITORIA(dtoperacao);

-- Estatísticas
UPDATE STATISTICS RHU_PESSOA WITH FULLSCAN;
UPDATE STATISTICS RHU_CONTRATO WITH FULLSCAN;
```

---

## 11. Testes

### 🧪 Estratégia de Testes

| Tipo | Cobertura | Ferramenta | Localização |
|------|-----------|------------|-------------|
| **Unitários** | 80%+ | xUnit + Moq | `/tests/Unit` |
| **Integração** | APIs críticas | TestServer | `/tests/Integration` |
| **Arquitetura** | Regras DDD | ArchUnit | `/tests/Architecture` |
| **E2E** | Fluxos principais | Selenium | `/tests/E2E` |

### ✅ Executar Testes

```bash
# Todos os testes
dotnet test

# Com cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Específico
dotnet test --filter "FullyQualifiedName~GestaoDePessoas"

# Com relatório
dotnet test --logger "html;LogFileName=test-results.html"
```

### 📝 Exemplo de Teste Unitário

```csharp
public class PessoaServiceTests
{
    private readonly Mock<IPessoaRepository> _repositoryMock;
    private readonly PessoaService _service;
    
    public PessoaServiceTests()
    {
        _repositoryMock = new Mock<IPessoaRepository>();
        _service = new PessoaService(_repositoryMock.Object);
    }
    
    [Fact]
    public async Task CriarPessoa_ComDadosValidos_DeveRetornarSucesso()
    {
        // Arrange
        var dto = new CreatePessoaDto 
        { 
            Nome = "João Silva",
            Cpf = "12345678900"
        };
        
        _repositoryMock
            .Setup(x => x.ExistsByCpf(It.IsAny<string>()))
            .ReturnsAsync(false);
        
        // Act
        var result = await _service.CreateAsync(dto);
        
        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Pessoa>()), Times.Once);
    }
}
```

---

## 12. Deploy e DevOps

### 🚀 CI/CD Pipeline

#### GitHub Actions

```yaml
# .github/workflows/build-and-deploy.yml
name: Build and Deploy

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore -c Release
    
    - name: Test
      run: dotnet test --no-build --verbosity normal
    
    - name: Publish
      run: dotnet publish -c Release -o ./publish
    
    - name: Upload Artifacts
      uses: actions/upload-artifact@v3
      with:
        name: app
        path: ./publish

  deploy:
    needs: build
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    
    steps:
    - name: Download Artifacts
      uses: actions/download-artifact@v3
      
    - name: Deploy to IIS
      uses: SamKirkland/FTP-Deploy-Action@4.3.3
      with:
        server: ${{ secrets.FTP_SERVER }}
        username: ${{ secrets.FTP_USERNAME }}
        password: ${{ secrets.FTP_PASSWORD }}
```

### 📦 Versionamento

```xml
<!-- Directory.Build.props -->
<Project>
  <PropertyGroup>
    <Version>2.0.0</Version>
    <AssemblyVersion>2.0.0.0</AssemblyVersion>
    <FileVersion>2.0.0.$([System.DateTime]::UtcNow.ToString("yyMMdd"))</FileVersion>
  </PropertyGroup>
</Project>
```

---

## 13. Monitoramento

### 📊 Health Checks

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString, name: "database")
    .AddRedis(redisConnection, name: "cache")
    .AddCheck<CustomHealthCheck>("custom");
```

### 📈 Métricas e KPIs

| Métrica | Objetivo | Alerta |
|---------|----------|--------|
| **Uptime** | 99.9% | < 99.5% |
| **Response Time (P95)** | < 200ms | > 500ms |
| **Error Rate** | < 0.1% | > 1% |
| **CPU Usage** | < 70% | > 85% |
| **Memory Usage** | < 80% | > 90% |
| **Active Connections** | < 1000 | > 1500 |

### 🔍 Logs Estruturados

```csharp
Log.Information("Usuário {Usuario} realizou login às {Timestamp}", 
    usuario.Nome, DateTime.UtcNow);

Log.Error(ex, "Erro ao processar pedido {PedidoId}", pedidoId);
```

---

## 14. Troubleshooting

### ❌ Problemas Comuns

#### 1. Erro de Conexão com Banco

**Sintoma**: `SqlException: A network-related or instance-specific error`

**Soluções**:
```bash
# Verificar SQL Server está rodando
Get-Service MSSQLSERVER

# Testar conexão
Test-NetConnection -ComputerName localhost -Port 1433

# Verificar string de conexão
dotnet user-secrets list
```

#### 2. Token JWT Inválido

**Sintoma**: `401 Unauthorized - Invalid token`

**Soluções**:
```bash
# Verificar chave JWT
dotnet user-secrets set "Jwt:Key" "NovaChaveSegura256Bits"

# Verificar expiração
# Decodificar token em jwt.io
```

#### 3. Migrations Falhando

**Sintoma**: `The migration has already been applied to the database`

**Soluções**:
```sql
-- Verificar migrations aplicadas
SELECT * FROM __EFMigrationsHistory

-- Remover migration problemática
DELETE FROM __EFMigrationsHistory WHERE MigrationId = 'XXX'

-- Reexecutar
dotnet ef database update
```

#### 4. Performance Lenta

**Diagnóstico**:
```sql
-- Queries lentas
SELECT TOP 10 
    total_elapsed_time / execution_count AS avg_elapsed_time,
    total_worker_time / execution_count AS avg_worker_time,
    execution_count,
    SUBSTRING(st.text, (qs.statement_start_offset/2)+1, 100) AS query
FROM sys.dm_exec_query_stats AS qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) AS st
ORDER BY avg_elapsed_time DESC
```

---

## 15. Scripts Úteis

### 🔧 PowerShell Scripts

#### reset-dev.ps1

```powershell
# Script para resetar ambiente de desenvolvimento
param(
    [switch]$Database,
    [switch]$Cache,
    [switch]$All
)

Write-Host "=== Reset Development Environment ===" -ForegroundColor Cyan

if ($Database -or $All) {
    Write-Host "Resetting database..." -ForegroundColor Yellow
    sqlcmd -S localhost -Q "
        IF EXISTS(SELECT * FROM sys.databases WHERE name='bd_rhu_copenor_dev')
            DROP DATABASE bd_rhu_copenor_dev;
        CREATE DATABASE bd_rhu_copenor_dev;
    "
    
    dotnet ef database update --project src/Identity/RhSensoERP.Identity.csproj
    Write-Host "Database reset complete" -ForegroundColor Green
}

if ($Cache -or $All) {
    Write-Host "Clearing Redis cache..." -ForegroundColor Yellow
    redis-cli FLUSHALL
    Write-Host "Cache cleared" -ForegroundColor Green
}

Write-Host "Cleaning build artifacts..." -ForegroundColor Yellow
Get-ChildItem -Path . -Include bin,obj -Recurse | Remove-Item -Force -Recurse
Write-Host "Build artifacts cleaned" -ForegroundColor Green

Write-Host "=== Reset Complete ===" -ForegroundColor Cyan
```

#### backup-production.ps1

```powershell
# Backup do banco de produção
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupPath = "C:\Backups\RhSensoERP_$timestamp.bak"

$query = @"
BACKUP DATABASE [bd_rhu_copenor] 
TO DISK = N'$backupPath' 
WITH FORMAT, INIT, 
NAME = N'RhSensoERP-Full-$timestamp', 
SKIP, NOREWIND, NOUNLOAD, COMPRESSION, STATS = 10
"@

Invoke-Sqlcmd -ServerInstance "PROD-SQL" -Query $query

Write-Host "Backup completed: $backupPath" -ForegroundColor Green
```

### 🐧 Bash Scripts

#### deploy.sh

```bash
#!/bin/bash

# Deploy script para Linux
set -e

echo "=== Starting deployment ==="

# Build
echo "Building application..."
dotnet build -c Release

# Test
echo "Running tests..."
dotnet test --no-build -c Release

# Publish
echo "Publishing..."
dotnet publish -c Release -o ./dist

# Stop service
echo "Stopping service..."
sudo systemctl stop rhsenso-api

# Deploy files
echo "Deploying files..."
sudo cp -r ./dist/* /var/www/rhsenso/

# Start service
echo "Starting service..."
sudo systemctl start rhsenso-api

echo "=== Deployment complete ==="
```

---

## 16. Contribuindo

### 🤝 Como Contribuir

1. **Fork** o projeto
2. Crie uma **feature branch** (`git checkout -b feature/AmazingFeature`)
3. **Commit** suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. **Push** para a branch (`git push origin feature/AmazingFeature`)
5. Abra um **Pull Request**

### 📝 Padrões de Código

#### Naming Conventions

```csharp
// Classes: PascalCase
public class PessoaService { }

// Interfaces: IPascalCase
public interface IPessoaRepository { }

// Methods: PascalCase
public async Task<Result> CreateAsync() { }

// Variables: camelCase
private readonly ILogger<PessoaService> _logger;

// Constants: UPPER_CASE
public const int MAX_RETRY_COUNT = 3;
```

#### Estrutura de Commit

```
feat: adiciona nova funcionalidade de relatório
fix: corrige erro na validação de CPF
docs: atualiza README com instruções de deploy
test: adiciona testes para PessoaService
refactor: melhora performance da query de busca
```

### ✔️ Checklist para PR

- [ ] Código segue os padrões do projeto
- [ ] Testes unitários adicionados/atualizados
- [ ] Documentação atualizada
- [ ] Build passando sem warnings
- [ ] Code review aprovado

---

## 17. Suporte

### 📞 Contatos

| Canal | Contato | Horário |
|-------|---------|---------|
| **Email** | suporte@rhsenso.com.br | 24/7 |
| **Telefone** | (11) 3xxx-xxxx | Seg-Sex 8h-18h |
| **WhatsApp** | (11) 9xxxx-xxxx | Seg-Sex 8h-18h |
| **Teams** | equipe-rhsenso | Horário comercial |

### 🐛 Reportar Bugs

1. Acesse: https://github.com/rhsenso/RhSensoERP/issues
2. Clique em "New Issue"
3. Escolha o template "Bug Report"
4. Preencha com o máximo de detalhes

### 📚 Recursos Adicionais

- **Wiki**: https://wiki.rhsenso.com.br
- **API Docs**: https://api.rhsenso.com.br/swagger
- **Confluence**: https://rhsenso.atlassian.net
- **Treinamentos**: https://academy.rhsenso.com.br

### 🔄 Atualizações

Para receber atualizações:
- **Release Notes**: https://github.com/rhsenso/RhSensoERP/releases
- **Newsletter**: https://rhsenso.com.br/newsletter
- **Blog Técnico**: https://tech.rhsenso.com.br

---

## 📄 Licença

Copyright © 2024 RhSenso - Todos os direitos reservados.

Este é um software proprietário. Não é permitida a cópia, modificação ou distribuição sem autorização expressa.

---

## 🙏 Agradecimentos

- Time de desenvolvimento RhSenso
- Comunidade .NET
- Microsoft pela excelente documentação
- Todos os contribuidores do projeto

---

*Última atualização: Novembro 2024 - Versão 2.0.0*
