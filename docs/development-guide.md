# Guia de Desenvolvimento

## Configuração do Ambiente

### Ferramentas Necessárias

- Visual Studio 2022 ou VS Code
- .NET SDK 8.0
- SQL Server Developer Edition
- Docker Desktop
- Git

### Extensões Recomendadas (VS Code)

- C# Dev Kit
- Docker
- GitLens
- REST Client

## Padrões de Código

### Nomenclatura

- **Classes**: PascalCase
- **Métodos**: PascalCase
- **Propriedades**: PascalCase
- **Variáveis**: camelCase
- **Constantes**: UPPER_CASE
- **Interfaces**: IPascalCase

### Organização de Arquivos

- Um arquivo por classe
- Namespace deve refletir a estrutura de pastas
- Usar `#region` apenas quando necessário

### Commits

Seguir o padrão Conventional Commits:

- `feat:`: Nova funcionalidade
- `fix:`: Correção de bug
- `docs:`: Documentação
- `style:`: Formatação
- `refactor:`: Refatoração
- `test:`: Testes
- `chore:`: Tarefas de build/config

Exemplo:
```
feat(gestao-pessoas): adicionar endpoint de busca por CPF
```

## Criando um Novo Módulo

1. Criar estrutura de pastas no `src/Modules/NomeModulo`
2. Criar projeto classlib
3. Adicionar à solution
4. Implementar camadas (Core, Application, Infrastructure)
5. Adicionar testes
6. Documentar

## Executando Localmente

```bash
# Subir dependências (SQL Server, Redis, Seq)
docker-compose up -d sqlserver redis seq

# Executar migrations
dotnet ef database update --project src/API

# Executar API
dotnet run --project src/API

# Executar Web
dotnet run --project src/Web
```

## Debugging

- API: https://localhost:5001
- Web: https://localhost:7001
- Swagger: https://localhost:5001/swagger
- Seq: http://localhost:5341
- Hangfire: https://localhost:5001/hangfire

## Troubleshooting

### Erro de certificado HTTPS

```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

### Erro de conexão com SQL Server

Verificar se o container está rodando:
```bash
docker ps
```

### Erro de restore de pacotes

Limpar cache do NuGet:
```bash
dotnet nuget locals all --clear
dotnet restore
```
