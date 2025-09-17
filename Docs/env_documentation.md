# 🔧 Variáveis de Ambiente - RhSensoERP

## 📋 **Configuração por Ambiente**

### 🔧 **Development (User Secrets)**

```bash
# Configurar User Secrets (apenas uma vez)
cd src/API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost;Database=RhSensoERP_Dev;Integrated Security=true;TrustServerCertificate=true"
dotnet user-secrets set "Jwt:SecretKey" "RhSensoERP-Super-Secret-Key-For-Development-Only-2024!"
```

### 🚀 **Production (Environment Variables)**

```bash
# Connection String
export ConnectionStrings__Default="Server=prod-sql;Database=RhSensoERP;User Id=rhsenso_user;Password=SECURE_PASSWORD;TrustServerCertificate=true"

# JWT Configuration
export Jwt__SecretKey="PRODUCTION_SECRET_KEY_256_BITS_MINIMUM"
export Jwt__PrivateKeyPem="-----BEGIN PRIVATE KEY-----...-----END PRIVATE KEY-----"
export Jwt__PublicKeyPem="-----BEGIN PUBLIC KEY-----...-----END PUBLIC KEY-----"
export Jwt__AccessTokenMinutes="15"

# CORS
export Cors__AllowedOrigins__0="https://app.rhsensoerp.com"
export Cors__AllowedOrigins__1="https://admin.rhsensoerp.com"

# Application
export ASPNETCORE_ENVIRONMENT="Production"
export ASPNETCORE_URLS="http://*:8080"
```

### 🐳 **Docker (docker-compose.yml)**

```yaml
environment:
  - ConnectionStrings__Default=Server=sql-server;Database=RhSensoERP;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true
  - Jwt__SecretKey=Docker-Secret-Key-For-Development-Only
  - ASPNETCORE_ENVIRONMENT=Development
```

---

## 📊 **Variáveis Obrigatórias**

| Variável | Ambiente | Obrigatória | Descrição |
|----------|-----------|-------------|-----------|
| `ConnectionStrings__Default` | Todos | ✅ | String de conexão SQL Server |
| `Jwt__SecretKey` | Dev/Test | ✅ | Chave simétrica para JWT (min 32 chars) |
| `Jwt__PrivateKeyPem` | Prod | ✅ | Chave privada RSA para JWT |
| `Jwt__PublicKeyPem` | Prod | ✅ | Chave pública RSA para JWT |

## 🔒 **Variáveis Opcionais**

| Variável | Padrão | Descrição |
|----------|---------|-----------|
| `Jwt__Issuer` | RhSensoERP | Emissor do token JWT |
| `Jwt__Audience` | RhSensoERP-Clients | Audiência do token |
| `Jwt__AccessTokenMinutes` | 30 | Tempo de vida do token (min) |
| `Cors__AllowedOrigins__0` | localhost:7050 | Origem permitida CORS |

---

## 🛡️ **Segurança**

### ❌ **NUNCA fazer:**
- Commitar secrets no Git
- Usar a mesma chave em dev/prod
- Chaves JWT menores que 256 bits
- Connection strings em appsettings.json

### ✅ **Sempre fazer:**
- User Secrets para desenvolvimento
- Variáveis de ambiente para produção
- Rotacionar chaves regularmente
- Usar Azure Key Vault ou equivalente

---

## 🧪 **Validação**

```bash
# Verificar se as variáveis estão carregadas
dotnet run --project src/API

# Se aparecer erro de connection string, as variáveis não estão configuradas
```

---

## 🆘 **Troubleshooting**

### Erro: "Connection string not found"
```bash
# Verificar User Secrets
dotnet user-secrets list --project src/API

# Reconfigurar se necessário
dotnet user-secrets set "ConnectionStrings:Default" "SUA_CONNECTION_STRING"
```

### Erro: "JWT Secret Key is too short"
```bash
# A chave deve ter pelo menos 32 caracteres
dotnet user-secrets set "Jwt:SecretKey" "RhSensoERP-Super-Secret-Key-For-Development-Only-2024!"
```