# Arquitetura do Sistema

## Visão Geral

O RhSensoERP segue os princípios de **Clean Architecture**, **Domain-Driven Design (DDD)** e **CQRS**.

## Camadas

### 1. Domain (Core)
- Entidades de negócio
- Value Objects
- Interfaces de repositórios
- Eventos de domínio
- Exceções de domínio

### 2. Application
- Use Cases (Commands e Queries)
- DTOs
- Validators (FluentValidation)
- Mappings (AutoMapper)
- Interfaces de serviços

### 3. Infrastructure
- Implementação de repositórios
- DbContext (Entity Framework Core)
- Serviços externos
- Cache (Redis)
- Background Jobs (Hangfire)

### 4. Presentation
- API (Controllers)
- Web (MVC/Razor Pages)
- Middlewares
- Filters

## Padrões Utilizados

- **Repository Pattern**
- **Unit of Work**
- **CQRS** (Command Query Responsibility Segregation)
- **Mediator Pattern** (MediatR)
- **Specification Pattern**
- **Factory Pattern**
- **Strategy Pattern**

## Módulos

- **GestaoDePessoas**: Cadastro e gestão de colaboradores
- **ControleDePonto**: Registro e controle de ponto
- **Treinamentos**: Gestão de treinamentos e capacitações
- **SaudeOcupacional**: Controle de saúde ocupacional
- **Avaliacoes**: Sistema de avaliação de desempenho

## Segurança

- Autenticação JWT
- Autorização baseada em roles e policies
- Rate Limiting
- CORS configurado
- HTTPS obrigatório em produção
- Data Protection para dados sensíveis

## Observabilidade

- **Logging**: Serilog com sinks para Console, File e Seq
- **Tracing**: OpenTelemetry
- **Metrics**: Application Insights
- **Health Checks**: Verificação de saúde de dependências
