# RhSensoERP API

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download)
[![License](https://img.shields.io/badge/license-Proprietary-red.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](docs/deployment/ci-cd.md)
[![Documentation](https://img.shields.io/badge/docs-available-brightgreen.svg)](docs/)

## 📋 Visão Geral

API REST moderna para o sistema ERP RhSenso, desenvolvida com Clean Architecture e .NET 8. Fornece funcionalidades de autenticação, autorização baseada em permissões granulares e integração com sistema legacy existente.

### 🎯 **Características Principais**

- ✅ **Clean Architecture** com separação clara de responsabilidades
- ✅ **Autenticação JWT** com suporte RSA/HMAC
- ✅ **Sistema de Permissões Granulares** baseado em grupos e funções
- ✅ **Integração Legacy** com banco SQL Server existente
- ✅ **Rate Limiting** e headers de segurança
- ✅ **Logging Estruturado** com Serilog
- ✅ **Testes Abrangentes** (Unitários + Integração)
- ✅ **Documentação OpenAPI/Swagger**

## 🚀 Quick Start

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB para desenvolvimento)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

### Instalação Rápida

```bash
# 1. Clone o repositório
git clone https://github.com/empresa/rhsensoerp-api.git
cd rhsensoerp-api

# 2. Configure connection string (User Secrets)
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost;Database=bd_rhu_copenor;Integrated Security=true;TrustServerCertificate=true;"

# 3. Execute a aplicação
dotnet run --project Src/API

# 4. Acesse a documentação
# https://localhost:57148/swagger
```

### Primeiro Teste

```bash
# Teste de conectividade
curl https://localhost:57148/health

# Login de exemplo (se dados existirem)
curl -X POST https://localhost:57148/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"cdUsuario":"admin","senha":"123456"}'
```

## 📚 Documentação

| Seção | Descrição | Link |
|-------|-----------|------|
| 🏗️ **Arquitetura** | Overview da Clean Architecture | [docs/architecture/](docs/architecture/) |
| ⚙️ **Configuração** | Setup completo e variáveis | [docs/getting-started/configuration.md](docs/getting-started/configuration.md) |
| 🔐 **Autenticação** | JWT, permissões e segurança | [docs/api/authentication.md](docs/api/authentication.md) |
| 🐳 **Deploy** | Docker, produção e ambientes | [docs/deployment/](docs/deployment/) |
| 🧪 **Testes** | Como executar e criar testes | [docs/contributing/testing-guide.md](docs/contributing/testing-guide.md) |
| 👨‍💻 **Contribuição** | Guia para desenvolvedores | [docs/contributing/](docs/contributing/) |

## 🏗️ Arquitetura

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Presentation  │    │   Application   │    │  Infrastructure │
│    (API)        │───▶│   (Use Cases)   │───▶│  (Data/External)│
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                │
                                ▼
                       ┌─────────────────┐
                       │      Domain     │
                       │     (Core)      │
                       └─────────────────┘
```

**Fluxo de Dependência:** `API` → `Application` → `Infrastructure` → `Core`

> 📖 **Detalhes:** [docs/architecture/overview.md](docs/architecture/overview.md)

## 🔐 Autenticação Legacy

A API integra com sistema legacy existente (tabelas `tuse1`, `usrh1`, `hbrh1`, etc.):

```csharp
// Exemplo de verificação de permissão
POST /api/v1/auth/check-habilitacao
{
  "cdsistema": "SEG",
  "cdfuncao": "USUARIO"
}
```

## 🧪 Testes

```bash
# Todos os testes
dotnet test

# Apenas testes unitários
dotnet test Tests/RhSensoERP.Tests.Unit/

# Testes de banco (requer configuração)
dotnet test --filter "DatabaseTests"

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 📊 Status do Projeto

| Componente | Status | Cobertura | Notas |
|------------|--------|-----------|-------|
| ✅ **Core/Domain** | Completo | 95%+ | Entidades e regras de negócio |
| ✅ **Authentication** | Completo | 90%+ | JWT + Legacy integration |
| ✅ **Infrastructure** | Completo | 85%+ | EF Core + SQL Server |
| 🚧 **API Endpoints** | Em desenvolvimento | 80%+ | CRUD básico implementado |
| 🚧 **Documentação** | Em andamento | - | README e estrutura |

## 🤝 Contribuição

1. **Fork** o projeto
2. **Crie uma branch** para sua feature (`git checkout -b feature/amazing-feature`)
3. **Siga o perfil de desenvolvedor** em [perfil_desenvolvedor_erp_api.md](perfil_desenvolvedor_erp_api.md)
4. **Execute os testes** (`dotnet test`)
5. **Commit suas mudanças** (`git commit -m 'feat: add amazing feature'`)
6. **Push para a branch** (`git push origin feature/amazing-feature`)
7. **Abra um Pull Request**

> 📋 **Guidelines:** [docs/contributing/development-guide.md](docs/contributing/development-guide.md)

## 📞 Suporte

- 📖 **Documentação:** [docs/](docs/)
- 🐛 **Issues:** [GitHub Issues](https://github.com/empresa/rhsensoerp-api/issues)
- 💬 **Discussões:** [GitHub Discussions](https://github.com/empresa/rhsensoerp-api/discussions)

## 📄 Licença

Este projeto é proprietário e confidencial. Todos os direitos reservados.

---

<p align="center">
  Desenvolvido com ❤️ pela equipe RhSenso
</p>