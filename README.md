# RhSensoERP

Sistema ERP de Recursos Humanos desenvolvido com .NET 8 e Clean Architecture.

## 🚀 Características

- **Clean Architecture** com DDD e CQRS
- **Multi-tenancy** com isolamento de dados
- **Segurança** com JWT, CORS, Rate Limiting
- **Observabilidade** com Serilog, OpenTelemetry e Application Insights
- **Cache distribuído** com Redis
- **Background Jobs** com Hangfire
- **Real-time** com SignalR
- **Testes** completos (Unit, Integration, Architecture, E2E)
- **Docker** e **Kubernetes** ready
- **CI/CD** com GitHub Actions e Azure DevOps

## 📋 Pré-requisitos

- .NET SDK 8.0 ou superior
- SQL Server 2019 ou superior
- Docker Desktop (opcional)
- Node.js 18+ (para frontend)

## 🛠️ Instalação

### Desenvolvimento Local

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/RhSensoERP.git
cd RhSensoERP
```

2. Restaure as dependências:
```bash
dotnet restore
```

3. Configure a connection string em `appsettings.Development.json`

4. Execute as migrations:
```bash
dotnet ef database update --project src/API
```

5. Execute a aplicação:
```bash
dotnet run --project src/API
```

### Docker

```bash
docker-compose up -d
```

## 🧪 Testes

```bash
# Todos os testes
dotnet test

# Apenas testes unitários
dotnet test tests/Unit

# Apenas testes de integração
dotnet test tests/Integration

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## 📚 Documentação

- [Arquitetura](docs/architecture.md)
- [Guia de Desenvolvimento](docs/development-guide.md)
- [API Documentation](docs/api-documentation.md)
- [Deployment](docs/deployment.md)

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 👥 Autores

- **RhSenso** - *Desenvolvimento Inicial*

## 🙏 Agradecimentos

- Equipe de desenvolvimento
- Comunidade .NET
