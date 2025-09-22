# 🔧 Guia de Configuração - RhSensoERP API

## 📋 Índice

- [Pré-requisitos](#pré-requisitos)
- [Configuração do Ambiente](#configuração-do-ambiente)
- [Banco de Dados](#banco-de-dados)
- [Autenticação JWT](#autenticação-jwt)
- [Variáveis de Ambiente](#variáveis-de-ambiente)
- [User Secrets (Desenvolvimento)](#user-secrets-desenvolvimento)
- [Configurações por Ambiente](#configurações-por-ambiente)
- [Troubleshooting](#troubleshooting)

## 🛠️ Pré-requisitos

### Software Obrigatório

| Componente | Versão Mínima | Recomendado | Download |
|------------|---------------|-------------|----------|
| .NET SDK | 8.0.0 | 8.0.x (latest) | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) |
| SQL Server | 2019 | 2022 | [microsoft.com/sql-server](https://www.microsoft.com/sql-server) |
| Visual Studio | 2022 17.8+ | 2022 (latest) | [visualstudio.microsoft.com](https://visualstudio.microsoft.com/) |

### Para Desenvolvimento Local

```bash
# Verificar versões instaladas
dotnet --version
sqlcmd -?
```

### Portas Utilizadas

| Serviço | Porta | Protocolo | Configurável |
|---------|-------|-----------|--------------|
| **HTTPS** | 57148 | HTTPS | ✅ `launchSettings.json` |
| **HTTP** | 57149 | HTTP | ✅ `launchSettings.json` |
| **SQL Server** | 1433 | TCP | ✅ Connection String |

## 🗄️ Banco de Dados

### 1. **SQL Server Local (Desenvolvimento)**

```bash
# Instalar LocalDB (se não tiver SQL Server completo)
# Vem com Visual Studio ou baixar SQL Server Express

# Verificar instância
sqllocaldb info
sqllocaldb start MSSQLLocalDB
```

### 2. **Connection String**

⚠️ **NUNCA commitar connection strings com credenciais!**

#### **Opção A: User Secrets (Recomendado para dev)**

```bash
# Configurar via User Secrets
cd Src/API
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost;Database=bd_rhu_copenor;Integrated Security=true;TrustServerCertificate=true;"

# Para SQL Server com usuário/senha
dotnet user-secrets set "ConnectionStrings:Default" "Server=SERVIDOR;Database=bd_rhu_copenor;User Id=usuario;Password=senha;TrustServerCertificate=true;"

# Verificar secrets configurados
dotnet user-secrets list
```

#### **Opção B: Variáveis de Ambiente**

```bash
# Windows
set ConnectionStrings__Default="Server=localhost;Database=bd_rhu_copenor;Integrated Security=true;TrustServerCertificate=true;"

# Linux/Mac
export ConnectionStrings__Default="Server=localhost;Database=bd_rhu_copenor;Integrated Security=true;TrustServerCertificate=true;"
```

### 3. **Verificar Conectividade**

```bash
# Executar testes de banco
dotnet test Tests/RhSensoERP.Tests.Unit/ --filter "DatabaseTests"

# Ou via API (após iniciar)
curl https://localhost:57148/health
curl https://localhost:57148/api/v1/test/banco
```

## 🔐 Autenticação JWT

### 1. **Configuração para Desenvolvimento (Chave Simétrica)**

```bash
# User Secrets (automático se chaves PEM não configuradas)
cd Src/API
dotnet user-secrets set "Jwt:SecretKey" "MinhaSuperChaveSecretaParaDesenvolvimento123456789"
dotnet user-secrets set "Jwt:Issuer" "RhSensoERP"
dotnet user-secrets set "Jwt:Audience" "RhSensoERP-Clients"
dotnet user-secrets set "Jwt:AccessTokenMinutes" "30"
```

### 2. **Configuração para Produção (Chaves RSA)**

```bash
# Gerar chaves RSA
openssl genrsa -out private.pem 2048
openssl rsa -in private.pem -pubout -out public.pem

# Configurar via variáveis de ambiente
export JWT__PublicKeyPem="$(cat public.pem)"
export JWT__PrivateKeyPem="$(cat private.pem)"
export JWT__Issuer="RhSensoERP"
export JWT__Audience="RhSensoERP-Clients"
```

### 3. **Testar JWT**

```bash
# Login para obter token
curl -X POST https://localhost:57148/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"cdUsuario":"admin","senha":"123456"}'

# Usar token em endpoint protegido
curl -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  https://localhost:57148/api/v1/auth/check-habilitacao?cdsistema=SEG&cdfuncao=USUARIO
```

## 🌍 Variáveis de Ambiente

### **Desenvolvimento (appsettings.Development.json)**

```json
{
  "ConnectionStrings": {
    "Default": "CONFIGURAR_VIA_USER_SECRETS"
  },
  "Jwt": {
    "Issuer": "RhSensoERP",
    "Audience": "RhSensoERP-Clients",
    "SecretKey": "CONFIGURAR_VIA_USER_SECRETS",
    "AccessTokenMinutes": 30
  },
  "Cors": {
    "AllowedOrigins": [
      "https://localhost:7050",
      "http://localhost:3000",
      "http://localhost:4200"
    ]
  },
  "Serilog": {
    "MinimumLevel": "Debug"
  }
}
```

### **Produção (Variáveis de Ambiente)**

```bash
# Banco de dados
export ConnectionStrings__Default="Server=prod-server;Database=bd_rhu_copenor;User Id=api_user;Password=***;TrustServerCertificate=false;"

# JWT (chaves RSA)
export Jwt__PublicKeyPem="-----BEGIN PUBLIC KEY-----\n...\n-----END PUBLIC KEY-----"
export Jwt__PrivateKeyPem="-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----"
export Jwt__Issuer="RhSensoERP-Production"
export Jwt__Audience="RhSensoERP-Clients"
export Jwt__AccessTokenMinutes="15"

# CORS (apenas origens autorizadas)
export Cors__AllowedOrigins__0="https://app.rhsenso.com"
export Cors__AllowedOrigins__1="https://admin.rhsenso.com"

# Logging
export Serilog__MinimumLevel="Warning"
export ASPNETCORE_ENVIRONMENT="Production"
```

## 🔒 User Secrets (Desenvolvimento)

### **Configuração Completa**

```bash
cd Src/API

# Connection String
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost;Database=bd_rhu_copenor;Integrated Security=true;TrustServerCertificate=true;"

# JWT
dotnet user-secrets set "Jwt:SecretKey" "MinhaChaveSecretaParaDev123456789AbCdEf"
dotnet user-secrets set "Jwt:Issuer" "RhSensoERP-Dev"
dotnet user-secrets set "Jwt:Audience" "RhSensoERP-Dev-Clients"
dotnet user-secrets set "Jwt:AccessTokenMinutes" "60"

# Verificar
dotnet user-secrets list

# Limpar (se necessário)
dotnet user-secrets clear
```

### **Arquivo de exemplo (.secrets-template)**

```json
{
  "ConnectionStrings": {
    "Default": "Server=SEU_SERVIDOR;Database=bd_rhu_copenor;Integrated Security=true;TrustServerCertificate=true;"
  },
  "Jwt": {
    "SecretKey": "SUA_CHAVE_SECRETA_AQUI_MIN_32_CHARS",
    "Issuer": "RhSensoERP-Dev",
    "Audience": "RhSensoERP-Dev-Clients",
    "AccessTokenMinutes": 60
  }
}
```

## 🚀 Configurações por Ambiente

### **Development**
- User Secrets para credenciais
- Logs detalhados (Debug)
- Swagger habilitado
- CORS permissivo
- Rate limiting relaxado

### **Staging**
- Variáveis de ambiente
- Logs moderados (Information)
- Swagger habilitado
- CORS restrito
- Rate limiting normal

### **Production**
- Variáveis de ambiente criptografadas
- Logs mínimos (Warning/Error)
- Swagger desabilitado
- CORS muito restrito
- Rate limiting rigoroso
- HTTPS obrigatório

## 🆘 Troubleshooting

### **1. Erro de Connection String**

```
❌ "Cannot open database bd_rhu_copenor"
```

**Soluções:**
```bash
# Verificar se SQL Server está rodando
sqlcmd -S localhost -E -Q "SELECT @@VERSION"

# Verificar se banco existe
sqlcmd -S localhost -E -Q "SELECT name FROM sys.databases WHERE name = 'bd_rhu_copenor'"

# Testar connection string manualmente
dotnet test --filter "Deve_Conectar_Com_Banco"
```

### **2. Erro de JWT**

```
❌ "IDX10503: Signature validation failed"
```

**Soluções:**
```bash
# Verificar configuração JWT
dotnet user-secrets list | grep Jwt

# Reconfigurar chave
dotnet user-secrets set "Jwt:SecretKey" "NovaChaveSecreta123456789AbCdEf"
```

### **3. Erro de Porta**

```
❌ "Failed to bind to address https://localhost:57148"
```

**Soluções:**
```bash
# Verificar portas em uso
netstat -an | findstr :57148

# Alterar porta no launchSettings.json
# ou usar porta diferente
dotnet run --urls="https://localhost:5001"
```

### **4. Erro de CORS**

```
❌ "CORS policy: No 'Access-Control-Allow-Origin' header"
```

**Soluções:**
```bash
# Adicionar origem no appsettings
# ou user secrets
dotnet user-secrets set "Cors:AllowedOrigins:0" "http://localhost:3000"
```

### **5. Problemas com Testes**

```bash
# Erro: banco não encontrado nos testes
# Os testes usam SQLite em memória, mas alguns usam SQL Server

# Para testes que precisam do banco real:
dotnet user-secrets set "ConnectionStrings:Default" "SUA_CONNECTION_STRING" --project Tests/RhSensoERP.Tests.Unit/

# Ou pular testes de banco:
dotnet test --filter "FullyQualifiedName!~DatabaseTests"
```

## ✅ Verificação Final

```bash
# 1. Verificar configuração
dotnet user-secrets list --project Src/API

# 2. Executar aplicação
dotnet run --project Src/API

# 3. Testar endpoints
curl https://localhost:57148/health
curl https://localhost:57148/swagger

# 4. Executar testes
dotnet test

# 5. Verificar logs
# Arquivos em: Src/API/logs/
```

---

💡 **Dica:** Mantenha um arquivo `.env.example` no repositório com exemplos de todas as variáveis necessárias (sem valores reais).