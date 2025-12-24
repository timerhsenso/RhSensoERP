# init-secrets.ps1
# Script para inicializar User Secrets no RhSensoERP.API
# Execute com: .\init-secrets.ps1

param(
    [string]$Environment = "Development",
    [switch]$Force = $false
)

$ErrorActionPreference = "Stop"

# Configurações
$ProjectPath = "src\API\RhSensoERP.API.csproj"
$UserSecretsId = "e7f8fd86-8e3f-423d-ba9c-abb7aa6a2021"

# Cores para output
function Write-ColorHost {
    param([string]$Text, [ConsoleColor]$Color = "White")
    Write-Host $Text -ForegroundColor $Color
}

# Banner
Write-ColorHost "`n+--------------------------------------------------------+" "Cyan"
Write-ColorHost "¦           RhSensoERP - User Secrets Setup              ¦" "Cyan"
Write-ColorHost "+--------------------------------------------------------+`n" "Cyan"

Write-ColorHost "?? Ambiente: $Environment" "Yellow"
Write-ColorHost "?? Projeto: $ProjectPath`n" "Yellow"

# Verifica se o projeto existe
if (-not (Test-Path $ProjectPath)) {
    Write-ColorHost "? Erro: Projeto não encontrado em '$ProjectPath'" "Red"
    Write-ColorHost "??  Certifique-se de executar o script na raiz do repositório." "Yellow"
    exit 1
}

# Verifica se o dotnet está instalado
try {
    $dotnetVersion = dotnet --version
    Write-ColorHost "? .NET SDK encontrado: v$dotnetVersion" "Green"
} catch {
    Write-ColorHost "? Erro: .NET SDK não encontrado. Por favor, instale o .NET 8.0 ou superior." "Red"
    exit 1
}

# Função para verificar se secrets já existem
function Test-UserSecrets {
    $output = dotnet user-secrets list --project $ProjectPath 2>&1
    return $output -notmatch "No secrets configured"
}

# Verifica se já existem secrets configurados
if ((Test-UserSecrets) -and -not $Force) {
    Write-ColorHost "`n??  User Secrets já configurados!" "Yellow"
    Write-ColorHost "Use o parâmetro -Force para sobrescrever." "Yellow"
    
    $response = Read-Host "`nDeseja visualizar os secrets atuais? (S/N)"
    if ($response -eq 'S' -or $response -eq 's') {
        Write-ColorHost "`n?? Secrets atuais:" "Cyan"
        dotnet user-secrets list --project $ProjectPath
    }
    
    $response = Read-Host "`nDeseja sobrescrever os secrets existentes? (S/N)"
    if ($response -ne 'S' -and $response -ne 's') {
        Write-ColorHost "`nOperação cancelada." "Yellow"
        exit 0
    }
}

# Inicializa User Secrets
Write-ColorHost "`n?? Inicializando User Secrets..." "Cyan"
dotnet user-secrets init --project $ProjectPath --id $UserSecretsId
if ($LASTEXITCODE -ne 0) {
    Write-ColorHost "? Falha ao inicializar User Secrets" "Red"
    exit 1
}

# Função para definir secret com validação
function Set-Secret {
    param(
        [string]$Key,
        [string]$Value,
        [bool]$IsSensitive = $false
    )
    
    Write-Host -NoNewline "  Configurando "
    Write-Host -NoNewline "$Key" -ForegroundColor "White"
    
    if ($IsSensitive) {
        Write-Host " [SENSITIVE]" -ForegroundColor "DarkGray"
    } else {
        Write-Host ""
    }
    
    dotnet user-secrets set "$Key" "$Value" --project $ProjectPath > $null 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-ColorHost "    ??  Falha ao definir $Key" "Yellow"
    } else {
        Write-ColorHost "    ? Configurado" "DarkGreen"
    }
}

# Função para gerar senha segura
function New-SecurePassword {
    param([int]$Length = 32)
    
    $chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=[]{}|;:,.<>?"
    $password = ""
    $random = New-Object System.Random
    
    for ($i = 0; $i -lt $Length; $i++) {
        $password += $chars[$random.Next($chars.Length)]
    }
    
    return $password
}

Write-ColorHost "`n?? Configurando secrets para ambiente: $Environment" "Yellow"
Write-ColorHost "----------------------------------------------------" "DarkGray"
# Configurações específicas por ambiente
switch ($Environment) {
    "Development" {
        # Database
        Set-Secret "ConnectionStrings:DefaultConnection" `
            "Server=localhost;Database=bd_rhu_copenor_dev;User Id=sa;Password=DevPass@2025!;TrustServerCertificate=true;Encrypt=false;MultipleActiveResultSets=true;Application Name=RhSensoERP_Dev"
        
        # Redis
        Set-Secret "Redis:Configuration" `
            "localhost:6379,password=RedisDevPass@2025,abortConnect=false,connectTimeout=5000,syncTimeout=5000"
        
        # JWT
        $jwtKey = New-SecurePassword -Length 64
        Set-Secret "Jwt:Key" $jwtKey -IsSensitive $true
        Set-Secret "Jwt:RefreshTokenKey" (New-SecurePassword -Length 48) -IsSensitive $true
        
        # SMTP (Mailhog para desenvolvimento)
        Set-Secret "Diagnostics:Smtp:Host" "localhost"
        Set-Secret "Diagnostics:Smtp:Port" "1025"
        Set-Secret "Diagnostics:Smtp:User" ""
        Set-Secret "Diagnostics:Smtp:Pass" ""
        
        # Aplicação
        Set-Secret "Application:AdminEmail" "admin@rhsenso.local"
        Set-Secret "Application:AdminPassword" "Admin@Dev2025!" -IsSensitive $true
    }
    
    "Staging" {
        # Database
        Set-Secret "ConnectionStrings:DefaultConnection" `
            "Server=staging-sql.rhsenso.local;Database=bd_rhu_copenor_staging;User Id=app_user;Password=StgPass@2025!;TrustServerCertificate=true;Encrypt=true;MultipleActiveResultSets=true;Application Name=RhSensoERP_Staging"
        
        # Redis
        Set-Secret "Redis:Configuration" `
            "redis-staging:6379,password=RedisStgPass@2025,ssl=true,abortConnect=false"
        
        # JWT
        $jwtKey = New-SecurePassword -Length 64
        Set-Secret "Jwt:Key" $jwtKey -IsSensitive $true
        Set-Secret "Jwt:RefreshTokenKey" (New-SecurePassword -Length 48) -IsSensitive $true
        
        # SMTP
        Set-Secret "Diagnostics:Smtp:User" "smtp-staging@rhsenso.com.br"
        Set-Secret "Diagnostics:Smtp:Pass" "SmtpStgPass@2025!" -IsSensitive $true
        
        # Application Insights
        Set-Secret "ApplicationInsights:InstrumentationKey" "YOUR-STAGING-KEY-HERE" -IsSensitive $true
        
        # Seq
        Set-Secret "Seq:ApiKey" "YOUR-SEQ-API-KEY-HERE" -IsSensitive $true
    }
    
    "Production" {
        Write-ColorHost "`n??  AVISO: Configuração de Production deve ser feita via Azure Key Vault!" "Yellow"
        Write-ColorHost "Este script não deve ser usado para configurar secrets de produção." "Yellow"
        exit 0
    }
    
    default {
        Write-ColorHost "? Ambiente '$Environment' não reconhecido." "Red"
        Write-ColorHost "Use: Development, Staging ou Production" "Yellow"
        exit 1
    }
}

# Configurações comuns a todos os ambientes
Write-ColorHost "`n?? Aplicando configurações comuns..." "Cyan"

# Criptografia
Set-Secret "DataProtection:ApplicationName" "RhSensoERP"
Set-Secret "DataProtection:KeyLifetime" "90"

# Segurança
Set-Secret "Security:EnableAudit" "true"
Set-Secret "Security:MaxLoginAttempts" "5"
Set-Secret "Security:LockoutDuration" "15"

# Multi-Tenant
Set-Secret "MultiTenant:DefaultTenant" "MASTER"
Set-Secret "MultiTenant:MasterConnectionString" `
    "Server=localhost;Database=bd_master;User Id=sa;Password=MasterPass@2025!;TrustServerCertificate=true" -IsSensitive $true

Write-ColorHost "`n? Todos os secrets foram configurados!" "Green"
Write-ColorHost "----------------------------------------------------" "DarkGray"

# Lista secrets configurados (sem valores)
Write-ColorHost "`n?? Secrets configurados (valores ocultos):" "Cyan"
$secrets = dotnet user-secrets list --project $ProjectPath 2>&1
$secrets | ForEach-Object {
    if ($_ -match "^(.+?)\s*=") {
        Write-ColorHost "  • $($Matches[1])" "DarkGray"
    }
}

# Instruções finais
Write-ColorHost "`n?? Próximos passos:" "Yellow"
Write-ColorHost "----------------------------------------------" "DarkGray"
Write-ColorHost "1. Execute a API:" "White"
Write-ColorHost "   dotnet run --project $ProjectPath" "Green"
Write-ColorHost "`n2. Acesse a documentação:" "White"
Write-ColorHost "   https://localhost:7001/swagger" "Green"
Write-ColorHost "`n3. Verifique os health checks:" "White"
Write-ColorHost "   https://localhost:7001/health" "Green"
Write-ColorHost "   https://localhost:7001/health-ui" "Green"

Write-ColorHost "`n??  IMPORTANTE:" "Red"
Write-ColorHost "----------------------------------------------" "DarkGray"
Write-ColorHost "• NUNCA commite o arquivo secrets.json!" "Yellow"
Write-ColorHost "• Os secrets são armazenados em:" "Yellow"
if ($IsWindows) {
    Write-ColorHost "  %APPDATA%\Microsoft\UserSecrets\$UserSecretsId\secrets.json" "DarkGray"
} else {
    Write-ColorHost "  ~/.microsoft/usersecrets/$UserSecretsId/secrets.json" "DarkGray"
}
Write-ColorHost "• Use variáveis de ambiente ou Azure Key Vault em produção" "Yellow"

Write-ColorHost "`n? Setup concluído com sucesso!`n" "Green"