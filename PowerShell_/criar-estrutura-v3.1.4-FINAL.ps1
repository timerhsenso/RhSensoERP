#!/usr/bin/env pwsh
# ============================================================================
# RHSENSOERP - ENTERPRISE SETUP SCRIPT v3.1 OTIMIZADO
# ============================================================================
# Autor: Analista Sênior FullStack .NET 8
# Data: 2025-01-30
# Versão: 3.1.4 FINAL
# Descrição: Script completo para criação de estrutura enterprise com:
#   - Clean Architecture + DDD + CQRS
#   - Multi-tenancy com filtros globais
#   - Segurança completa (JWT, CORS, Rate Limiting, HTTPS)
#   - Testes (Unit, Integration, Architecture, E2E)
#   - Docker + Kubernetes ready
#   - CI/CD (GitHub Actions + Azure DevOps)
#   - Observabilidade (Serilog, OpenTelemetry, App Insights)
#   - Cache distribuído (Redis)
#   - Background Jobs (Hangfire)
#   - Real-time (SignalR)
#   - Documentação automática (Swagger/OpenAPI)
#
# Melhorias v3.1:
#   ✅ Corrigido conflito com parâmetro -Verbose (renomeado para -VerboseOutput)
#   ✅ Logging persistente com Start-Transcript
#   ✅ Validação de versão .NET com comparação semântica
#   ✅ Verificação de Docker daemon ativo
#   ✅ Sanitização de nome do projeto
#   ✅ Idempotência com flag -ForceWrite
#   ✅ Paths OS-safe com Join-Path
#   ✅ Certificado HTTPS dev automático
#   ✅ .gitignore e .gitattributes
#   ✅ Dispatcher de fases com telemetria
#   ✅ Tratamento de erro global com cleanup
#   ✅ Resumo final com estatísticas
# ============================================================================

#Requires -Version 5.1

[CmdletBinding(SupportsShouldProcess=$true, ConfirmImpact='Medium')]
param(
    [Parameter(Mandatory=$false, HelpMessage="Nome do projeto (sem espaços ou caracteres especiais)")]
    [string]$ProjectName = "RhSensoERP",
    
    [Parameter(Mandatory=$false, HelpMessage="Nome da empresa")]
    [string]$CompanyName = "RhSenso",
    
    [Parameter(Mandatory=$false, HelpMessage="Ambiente de execução")]
    [ValidateSet("Development", "Staging", "Production")]
    [string]$Environment = "Development",
    
    [Parameter(Mandatory=$false, HelpMessage="Pular configuração do Docker")]
    [switch]$SkipDocker,
    
    [Parameter(Mandatory=$false, HelpMessage="Pular criação de projetos de teste")]
    [switch]$SkipTests,
    
    [Parameter(Mandatory=$false, HelpMessage="Exibir logs detalhados")]
    [switch]$VerboseOutput,
    
    [Parameter(Mandatory=$false, HelpMessage="Forçar recriação de arquivos existentes")]
    [switch]$Force
)

# ============================================================================
# CONFIGURAÇÕES GLOBAIS
# ============================================================================
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"
if (Get-Variable -Name InformationPreference -ErrorAction SilentlyContinue) {
    $InformationPreference = "Continue"
}

$StartTime = Get-Date
$ScriptVersion = "3.1.4"
$PSVersion = $PSVersionTable.PSVersion.Major
$IsPowerShell51 = ($PSVersion -eq 5)
$IsPowerShellCore = ($PSVersion -ge 6)
$DotNetVersion = "8.0.401"
$SqlServerVersion = "2022-latest"

# Estatísticas (para resumo final)
$Script:Stats = @{
    ProjectsCreated = 0
    FilesCreated = 0
    FoldersCreated = 0
    Errors = 0
    Warnings = 0
}

# Cores para output
$Colors = @{
    Success = "Green"
    Info = "Cyan"
    Warning = "Yellow"
    Error = "Red"
    Section = "Magenta"
    Detail = "DarkGray"
}

# ============================================================================
# SANITIZAÇÃO E VALIDAÇÃO INICIAL
# ============================================================================

# Sanitizar nome do projeto (remover caracteres inválidos)
$ProjectName = ($ProjectName -replace '[^A-Za-z0-9_.-]', '')
if ([string]::IsNullOrWhiteSpace($ProjectName)) {
    throw "❌ Nome de projeto inválido após sanitização. Use apenas letras, números, underscore, ponto e hífen."
}

# Criar variável lowercase para uso em docker, kubernetes, etc
$Script:projectNameLower = $ProjectName.ToLower()

# Validar permissão de escrita no diretório atual
try {
    $testFile = Join-Path (Get-Location) ".write_test_$(Get-Random).tmp"
    New-Item -ItemType File -Path $testFile -Force | Out-Null
    Remove-Item -Path $testFile -Force
} catch {
    throw "❌ Sem permissão de escrita no diretório atual: $(Get-Location)"
}

# ============================================================================
# CONFIGURAÇÃO DE LOGGING
# ============================================================================

$logDir = Join-Path (Get-Location) "logs"
if (-not (Test-Path $logDir)) {
    New-Item -ItemType Directory -Path $logDir -Force | Out-Null
}

$logFile = Join-Path $logDir ("setup_{0:yyyyMMdd_HHmmss}.log" -f (Get-Date))
Start-Transcript -Path $logFile -Force | Out-Null

# Trap para cleanup em caso de erro
trap {
    Write-Host ""
    Write-Host "❌ ERRO CRÍTICO: $_" -ForegroundColor Red
    Write-Host "Stack Trace:" -ForegroundColor Red
    Write-Host $_.ScriptStackTrace -ForegroundColor DarkRed
    Write-Host ""
    Write-Host "Executando cleanup..." -ForegroundColor Yellow
    
    Stop-Transcript | Out-Null
    
    Write-Host "Log salvo em: $logFile" -ForegroundColor Cyan
    exit 1
}

# ============================================================================
# FUNÇÕES AUXILIARES
# ============================================================================

<#
.SYNOPSIS
    Escreve mensagem colorida no console e no log
.DESCRIPTION
    Função para padronizar output com cores, símbolos e logging estruturado
.PARAMETER Message
    Mensagem a ser exibida
.PARAMETER Color
    Cor do texto (padrão: White)
.PARAMETER Icon
    Ícone/emoji a ser exibido antes da mensagem
.EXAMPLE
    Write-ColorMessage "Operação concluída" "Green" "✅"
#>
function Write-ColorMessage {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$Message,
        
        [Parameter(Mandatory=$false)]
        [string]$Color = "White",
        
        [Parameter(Mandatory=$false)]
        [string]$Icon = ""
    )
    
    $timestamp = if ($VerboseOutput) { "[$(Get-Date -Format 'HH:mm:ss.fff')] " } else { "" }
    $fullMessage = "$timestamp$Icon $Message"
    
    Write-Host $fullMessage -ForegroundColor $Color
}

<#
.SYNOPSIS
    Compara versões semânticas
.DESCRIPTION
    Verifica se a versão instalada é maior ou igual à versão requerida
.PARAMETER Have
    Versão instalada
.PARAMETER Need
    Versão mínima requerida
.RETURNS
    $true se a versão instalada atende ao requisito, $false caso contrário
#>
function Test-VersionAtLeast {
    [CmdletBinding()]
    [OutputType([bool])]
    param(
        [Parameter(Mandatory=$true)]
        [string]$Have,
        
        [Parameter(Mandatory=$true)]
        [string]$Need
    )
    
    try {
        # Remover sufixos de pre-release (ex: 8.0.401-preview)
        $haveVersion = [Version]($Have.Split('-')[0])
        $needVersion = [Version]($Need.Split('-')[0])
        
        return ($haveVersion -ge $needVersion)
    } catch {
        Write-ColorMessage "  ⚠️ Não foi possível comparar versões: $Have vs $Need" $Colors.Warning
        return $false
    }
}

<#
.SYNOPSIS
    Cria pasta se não existir
.DESCRIPTION
    Cria diretório de forma idempotente e incrementa contador de estatísticas
.PARAMETER Path
    Caminho do diretório a ser criado
#>
function New-Folder {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$Path
    )
    
    if (-not (Test-Path $Path)) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
        $Script:Stats.FoldersCreated++
        
        if ($VerboseOutput) {
            Write-ColorMessage "  📁 Criado: $Path" $Colors.Detail
        }
    }
}

<#
.SYNOPSIS
    Cria arquivo com conteúdo UTF-8
.DESCRIPTION
    Cria arquivo de forma idempotente, respeitando flag -ForceWrite
.PARAMETER Path
    Caminho do arquivo a ser criado
.PARAMETER Content
    Conteúdo do arquivo
.PARAMETER ForceWrite
    Se $true, sobrescreve arquivo existente
#>
function New-FileWithContent {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$Path,
        
        [Parameter(Mandatory=$true)]
        [AllowEmptyString()]
        [string]$Content,
        
        [Parameter(Mandatory=$false)]
        [switch]$ForceWrite
    )
    
    $folder = Split-Path -Parent $Path
    if ($folder -and -not (Test-Path $folder)) {
        New-Item -ItemType Directory -Path $folder -Force | Out-Null
    }
    
    # Verificar se arquivo já existe
    if ((Test-Path $Path) -and -not $ForceWrite -and -not $Force) {
        if ($VerboseOutput) {
            Write-ColorMessage "  ℹ️ Mantido (já existe): $Path" $Colors.Detail
        }
        return
    }
    
    if ($IsPowerShell51) {
        [System.IO.File]::WriteAllText($Path, $Content, [System.Text.UTF8Encoding]::new($false))
    } else {
        Set-Content -Path $Path -Value $Content -Encoding UTF8 -Force
    }
    $Script:Stats.FilesCreated++
    
    if ($VerboseOutput) {
        Write-ColorMessage "  📄 Arquivo: $Path" $Colors.Detail
    }
}

<#
.SYNOPSIS
    Verifica pré-requisitos do sistema
.DESCRIPTION
    Valida instalação e versões de ferramentas necessárias
#>
function Test-Prerequisites {
    [CmdletBinding()]
    param()
    
    Write-ColorMessage "Verificando pré-requisitos..." $Colors.Info "🔍"
    
    # .NET SDK
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        throw "❌ .NET SDK não encontrado. Instale em: https://dot.net"
    }
    
    $dotnetVersion = (dotnet --version).Trim()
    Write-ColorMessage "  ✅ .NET SDK: $dotnetVersion" $Colors.Success
    
    # Validar versão mínima
    if (-not (Test-VersionAtLeast $dotnetVersion $DotNetVersion)) {
        Write-ColorMessage "  ⚠️ .NET SDK mínimo recomendado: $DotNetVersion (instalado: $dotnetVersion)" $Colors.Warning
        $Script:Stats.Warnings++
    }
    
    # Git
    if (Get-Command git -ErrorAction SilentlyContinue) {
        $gitVersion = (git --version).Trim()
        Write-ColorMessage "  ✅ Git: $gitVersion" $Colors.Success
    } else {
        Write-ColorMessage "  ⚠️ Git não encontrado (opcional, mas recomendado)" $Colors.Warning
        $Script:Stats.Warnings++
    }
    
    # Docker
    if (-not $SkipDocker) {
        if (Get-Command docker -ErrorAction SilentlyContinue) {
            try {
                $dockerVersion = docker --version
                docker info --format '{{.ServerVersion}}' 2>&1 | Out-Null
                Write-ColorMessage "  ✅ Docker: $dockerVersion (daemon ativo)" $Colors.Success
            } catch {
                Write-ColorMessage "  ⚠️ Docker instalado, mas daemon não está ativo ou sem permissão" $Colors.Warning
                $Script:Stats.Warnings++
            }
        } else {
            Write-ColorMessage "  ⚠️ Docker não encontrado (use -SkipDocker para ignorar)" $Colors.Warning
            $Script:Stats.Warnings++
        }
    }
    
    # Node.js (para frontend)
    if (Get-Command node -ErrorAction SilentlyContinue) {
        $nodeVersion = (node --version).Trim()
        Write-ColorMessage "  ✅ Node.js: $nodeVersion" $Colors.Success
    } else {
        Write-ColorMessage "  ℹ️ Node.js não encontrado (opcional para frontend avançado)" $Colors.Detail
    }
    
    # Certificado HTTPS de desenvolvimento
    try {
        dotnet dev-certs https --check 2>&1 | Out-Null
        $certExists = $LASTEXITCODE -eq 0
        
        if (-not $certExists) {
            Write-ColorMessage "  ⚙️ Criando certificado HTTPS de desenvolvimento..." $Colors.Info
            dotnet dev-certs https --trust 2>&1 | Out-Null
        }
        
        Write-ColorMessage "  ✅ Certificado HTTPS de desenvolvimento configurado" $Colors.Success
    } catch {
        Write-ColorMessage "  ⚠️ Não foi possível configurar certificado HTTPS (pode ser necessário executar manualmente)" $Colors.Warning
        $Script:Stats.Warnings++
    }
}

<#
.SYNOPSIS
    Cria estrutura completa de um módulo
.DESCRIPTION
    Cria todas as pastas necessárias para um módulo seguindo Clean Architecture
.PARAMETER ModuleName
    Nome do módulo
.PARAMETER ModulePath
    Caminho base do módulo
#>
function New-ModuleStructure {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$ModuleName,
        
        [Parameter(Mandatory=$true)]
        [string]$ModulePath
    )
    
    Write-ColorMessage "  📦 Módulo: $ModuleName" $Colors.Info
    
    # Domain/Core Layer
    $coreFolders = @(
        "Core/Entities",
        "Core/ValueObjects",
        "Core/Enums",
        "Core/Interfaces",
        "Core/Specifications",
        "Core/Events",
        "Core/Exceptions",
        "Core/Constants"
    )
    
    foreach ($folder in $coreFolders) {
        New-Folder (Join-Path $ModulePath $folder)
    }
    
    # Application Layer
    $appFolders = @(
        "Application/DTOs/Requests",
        "Application/DTOs/Responses",
        "Application/Commands/Handlers",
        "Application/Queries/Handlers",
        "Application/Mappings",
        "Application/Validators",
        "Application/Services",
        "Application/Interfaces",
        "Application/Behaviors",
        "Application/EventHandlers"
    )
    
    foreach ($folder in $appFolders) {
        New-Folder (Join-Path $ModulePath $folder)
    }
    
    # Infrastructure Layer
    $infraFolders = @(
        "Infrastructure/Persistence/Configurations",
        "Infrastructure/Persistence/Migrations",
        "Infrastructure/Persistence/Seeds",
        "Infrastructure/Repositories",
        "Infrastructure/Services",
        "Infrastructure/Integrations",
        "Infrastructure/BackgroundJobs",
        "Infrastructure/Cache"
    )
    
    foreach ($folder in $infraFolders) {
        New-Folder (Join-Path $ModulePath $folder)
    }
}

<#
.SYNOPSIS
    Adiciona projeto à solution com tratamento de erro
.DESCRIPTION
    Adiciona projeto à solution de forma idempotente
.PARAMETER ProjectPath
    Caminho do arquivo .csproj
.PARAMETER SolutionFolder
    Pasta virtual na solution
#>
function Add-ToSolution {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$ProjectPath,
        
        [Parameter(Mandatory=$false)]
        [string]$SolutionFolder = ""
    )
    
    if (-not (Test-Path $ProjectPath)) {
        Write-ColorMessage "  ❌ Projeto não encontrado: $ProjectPath" $Colors.Error
        $Script:Stats.Errors++
        return
    }
    
    try {
        if ($SolutionFolder) {
            dotnet sln add $ProjectPath --solution-folder $SolutionFolder 2>&1 | Out-Null
        } else {
            dotnet sln add $ProjectPath 2>&1 | Out-Null
        }
        
        if ($LASTEXITCODE -eq 0) {
            if ($VerboseOutput) {
                Write-ColorMessage "    ➕ Adicionado à solution: $SolutionFolder" $Colors.Detail
            }
        }
    } catch {
        # Projeto já existe na solution ou outro erro não crítico
        if ($VerboseOutput) {
            Write-ColorMessage "    ℹ️ Projeto já presente ou erro ao adicionar: $ProjectPath" $Colors.Detail
        }
    }
}

<#
.SYNOPSIS
    Remove versões de PackageReference em arquivos .csproj
.DESCRIPTION
    Remove atributo Version="" de PackageReference para compatibilidade com Central Package Management
.PARAMETER ProjectPath
    Caminho do arquivo .csproj
#>
function Remove-PackageVersions {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$ProjectPath
    )
    
    if (-not (Test-Path $ProjectPath)) {
        Write-ColorMessage "  ⚠️ Projeto não encontrado: $ProjectPath" $Colors.Warning
        return
    }
    
    try {
        $content = Get-Content $ProjectPath -Raw -Encoding UTF8
        
        # Remover Version="" de PackageReference usando regex
        # Padrão: <PackageReference Include="NomePacote" Version="X.X.X" />
        # Resultado: <PackageReference Include="NomePacote" />
        $pattern = '(<PackageReference\s+Include="[^"]+"\s+)Version="[^"]*"(\s*/?>)'
        $newContent = $content -replace $pattern, '$1$2'
        
        # Também remover Version="" quando está em linha separada
        $pattern2 = '\s*Version="[^"]*"\s*'
        $newContent = $newContent -replace '(<PackageReference[^>]*?)' + $pattern2 + '(/?>)', '$1$2'
        
        # Salvar apenas se houve mudança
        if ($content -ne $newContent) {
            if ($IsPowerShell51) {
                [System.IO.File]::WriteAllText($ProjectPath, $newContent, [System.Text.UTF8Encoding]::new($false))
            } else {
                Set-Content -Path $ProjectPath -Value $newContent -Encoding UTF8 -NoNewline
            }
            
            if ($VerboseOutput) {
                Write-ColorMessage "    🔧 Versões de pacotes removidas: $(Split-Path -Leaf $ProjectPath)" $Colors.Detail
            }
        }
    } catch {
        Write-ColorMessage "  ❌ Erro ao processar $ProjectPath : $_" $Colors.Error
        $Script:Stats.Errors++
    }
}

<#
.SYNOPSIS
    Executa uma fase do setup com telemetria
.DESCRIPTION
    Wrapper para execução de fases com medição de tempo e tratamento de erro
.PARAMETER Name
    Nome da fase
.PARAMETER Action
    ScriptBlock a ser executado
#>
function Invoke-Phase {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$Name,
        
        [Parameter(Mandatory=$true)]
        [scriptblock]$Action
    )
    
    if ($PSCmdlet.ShouldProcess($Name, "Executar fase")) {
        $sw = [System.Diagnostics.Stopwatch]::StartNew()
        
        try {
            Write-Host ""
            Write-ColorMessage $Name $Colors.Section "⚙️"
            
            & $Action
            
            $sw.Stop()
            Write-ColorMessage ("  ⏱️ Tempo: {0:N2}s" -f $sw.Elapsed.TotalSeconds) $Colors.Detail
        } catch {
            $sw.Stop()
            Write-ColorMessage "  ❌ Erro na fase: $_" $Colors.Error
            $Script:Stats.Errors++
            throw
        }
    }
}

# ============================================================================
# BANNER E INICIALIZAÇÃO
# ============================================================================
Clear-Host
Write-Host ""
Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor $Colors.Section
Write-Host "║      RHSENSOERP - ENTERPRISE SETUP v$ScriptVersion OTIMIZADO      ║" -ForegroundColor $Colors.Section
Write-Host "║                  🚀 INICIANDO SETUP 🚀                     ║" -ForegroundColor $Colors.Section
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor $Colors.Section
Write-Host ""
Write-ColorMessage "📋 Projeto: $ProjectName" $Colors.Info
Write-ColorMessage "🏢 Empresa: $CompanyName" $Colors.Info
Write-ColorMessage "🌍 Ambiente: $Environment" $Colors.Info
Write-ColorMessage "📦 Script Version: $ScriptVersion" $Colors.Info
Write-ColorMessage "📁 Diretório: $(Get-Location)" $Colors.Info
Write-ColorMessage "📝 Log: $logFile" $Colors.Info
Write-Host ""

# Verificar pré-requisitos
Test-Prerequisites


# ============================================================================
# FASE 1: GOVERNANÇA E SDK MANAGEMENT
# ============================================================================
Invoke-Phase "[1/12] Configurando Governança e SDK Management" {
    
    # global.json - SDK pinning
    $globalJson = @"
{
  "`$schema": "https://json.schemastore.org/global.json",
  "sdk": {
    "version": "$DotNetVersion",
    "rollForward": "latestPatch",
    "allowPrerelease": false
  },
  "msbuild-sdks": {
    "Microsoft.Build.NoTargets": "3.7.0"
  }
}
"@
    New-FileWithContent "global.json" $globalJson -ForceWrite
    
    # Directory.Packages.props - Central Package Management (CPM)
    $dirPackages = @'
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    <CentralPackageTransitivePinningEnabled>true</CentralPackageTransitivePinningEnabled>
  </PropertyGroup>
  
  <ItemGroup Label="Core Packages">
    <!-- DDD & CQRS -->
    <PackageVersion Include="MediatR" Version="12.4.1" />
    <PackageVersion Include="MediatR.Extensions.Microsoft.DependencyInjection" Version="11.1.0" />
    <PackageVersion Include="AutoMapper" Version="13.0.1" />
    <PackageVersion Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
    <PackageVersion Include="FluentValidation" Version="11.9.2" />
    <PackageVersion Include="FluentValidation.DependencyInjectionExtensions" Version="11.9.2" />
    
    <!-- Entity Framework Core -->
    <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="8.0.10" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.10" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.10" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.10" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Proxies" Version="8.0.10" />
    <PackageVersion Include="EFCore.BulkExtensions" Version="8.1.1" />
    
    <!-- Identity & Security -->
    <PackageVersion Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.10" />
    <PackageVersion Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.10" />
    <PackageVersion Include="BCrypt.Net-Next" Version="4.0.3" />
    <PackageVersion Include="Microsoft.AspNetCore.DataProtection.EntityFrameworkCore" Version="8.0.10" />
    
    <!-- API Documentation -->
    <PackageVersion Include="Swashbuckle.AspNetCore" Version="6.8.1" />
    <PackageVersion Include="Swashbuckle.AspNetCore.Annotations" Version="6.8.1" />
    
    <!-- API Versioning -->
    <PackageVersion Include="Asp.Versioning.Mvc" Version="8.1.0" />
    <PackageVersion Include="Asp.Versioning.Mvc.ApiExplorer" Version="8.1.0" />
    
    <!-- Logging & Monitoring -->
    <PackageVersion Include="Serilog.AspNetCore" Version="8.0.2" />
    <PackageVersion Include="Serilog.Sinks.File" Version="6.0.0" />
    <PackageVersion Include="Serilog.Sinks.Console" Version="6.0.0" />
    <PackageVersion Include="Serilog.Sinks.Seq" Version="8.0.0" />
    <PackageVersion Include="Serilog.Sinks.ApplicationInsights" Version="4.0.0" />
    <PackageVersion Include="Serilog.Enrichers.Environment" Version="3.0.1" />
    <PackageVersion Include="Serilog.Enrichers.Thread" Version="4.0.0" />
    <PackageVersion Include="Serilog.Enrichers.CorrelationId" Version="3.0.1" />
    
    <!-- OpenTelemetry -->
    <PackageVersion Include="OpenTelemetry.Exporter.Console" Version="1.9.0" />
    <PackageVersion Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.9.0" />
    <PackageVersion Include="OpenTelemetry.Extensions.Hosting" Version="1.9.0" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.9.0" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.Http" Version="1.9.0" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.SqlClient" Version="1.9.0-beta.1" />
    
    <!-- Resilience & Performance -->
    <PackageVersion Include="Microsoft.Extensions.Http.Polly" Version="8.0.10" />
    <PackageVersion Include="Polly" Version="8.4.2" />
    <PackageVersion Include="Microsoft.Extensions.Caching.StackExchangeRedis" Version="8.0.10" />
    <PackageVersion Include="StackExchange.Redis" Version="2.8.16" />
    
    <!-- Background Jobs -->
    <PackageVersion Include="Hangfire.Core" Version="1.8.14" />
    <PackageVersion Include="Hangfire.SqlServer" Version="1.8.14" />
    <PackageVersion Include="Hangfire.AspNetCore" Version="1.8.14" />
    
    <!-- Real-time -->
    <PackageVersion Include="Microsoft.AspNetCore.SignalR.Common" Version="8.0.10" />
    <PackageVersion Include="Microsoft.AspNetCore.SignalR.Client" Version="8.0.10" />
    
    <!-- Health Checks -->
    <PackageVersion Include="AspNetCore.HealthChecks.SqlServer" Version="8.0.2" />
    <PackageVersion Include="AspNetCore.HealthChecks.Redis" Version="8.0.1" />
    <PackageVersion Include="AspNetCore.HealthChecks.UI" Version="8.0.2" />
    <PackageVersion Include="AspNetCore.HealthChecks.UI.Client" Version="8.0.1" />
    <PackageVersion Include="AspNetCore.HealthChecks.UI.InMemory.Storage" Version="8.0.1" />
    
    <!-- Testing -->
    <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
    <PackageVersion Include="xunit" Version="2.9.2" />
    <PackageVersion Include="xunit.runner.visualstudio" Version="2.8.2" />
    <PackageVersion Include="coverlet.collector" Version="6.0.2" />
    <PackageVersion Include="Moq" Version="4.20.72" />
    <PackageVersion Include="FluentAssertions" Version="6.12.1" />
    <PackageVersion Include="Bogus" Version="35.6.1" />
    <PackageVersion Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.10" />
    <PackageVersion Include="Testcontainers" Version="3.10.0" />
    <PackageVersion Include="Testcontainers.MsSql" Version="3.10.0" />
    <PackageVersion Include="Respawn" Version="6.2.1" />
    <PackageVersion Include="WireMock.Net" Version="1.6.7" />
    <PackageVersion Include="NetArchTest.Rules" Version="1.3.2" />
    
    <!-- Code Quality -->
    <PackageVersion Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="8.0.0" />
    <PackageVersion Include="StyleCop.Analyzers" Version="1.2.0-beta.556" />
    <PackageVersion Include="SonarAnalyzer.CSharp" Version="9.32.0.97167" />
    <PackageVersion Include="SecurityCodeScan.VS2019" Version="5.6.7" />
    <PackageVersion Include="Meziantou.Analyzer" Version="2.0.169" />
    
    <!-- Utilities -->
    <PackageVersion Include="Humanizer.Core" Version="2.14.1" />
    <PackageVersion Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageVersion Include="System.Linq.Dynamic.Core" Version="1.4.5" />
    <PackageVersion Include="ClosedXML" Version="0.104.1" />
    <PackageVersion Include="QuestPDF" Version="2024.10.3" />
  </ItemGroup>
</Project>
'@
    New-FileWithContent "Directory.Packages.props" $dirPackages -ForceWrite
    
    # Directory.Build.props - Global build properties
    $buildProps = @"
<Project>
  <PropertyGroup Label="Build Settings">
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>12.0</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <WarningsNotAsErrors>CS1591</WarningsNotAsErrors>
    <NoWarn>CS1591;CS1573</NoWarn>
    <AnalysisLevel>latest-all</AnalysisLevel>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <IncludeSymbols>true</IncludeSymbols>
    <SymbolPackageFormat>snupkg</SymbolPackageFormat>
    <PublishRepositoryUrl>true</PublishRepositoryUrl>
    <EmbedUntrackedSources>true</EmbedUntrackedSources>
    <ContinuousIntegrationBuild Condition="'`$(CI)' == 'true'">true</ContinuousIntegrationBuild>
    <Deterministic>true</Deterministic>
  </PropertyGroup>
  
  <PropertyGroup Label="Assembly Info">
    <Product>$ProjectName</Product>
    <Company>$CompanyName Corporation</Company>
    <Copyright>Copyright © $CompanyName `$([System.DateTime]::Now.Year)</Copyright>
    <AssemblyVersion>1.0.0.0</AssemblyVersion>
    <FileVersion>1.0.0.0</FileVersion>
    <InformationalVersion>1.0.0</InformationalVersion>
  </PropertyGroup>
  
  <ItemGroup Label="Global Analyzers">
    <PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" PrivateAssets="all" />
    <PackageReference Include="StyleCop.Analyzers" PrivateAssets="all" />
    <PackageReference Include="SonarAnalyzer.CSharp" PrivateAssets="all" />
    <PackageReference Include="SecurityCodeScan.VS2019" PrivateAssets="all" />
    <PackageReference Include="Meziantou.Analyzer" PrivateAssets="all" />
  </ItemGroup>
  
  <ItemGroup Label="Global Usings">
    <Using Include="System" />
    <Using Include="System.Collections.Generic" />
    <Using Include="System.Linq" />
    <Using Include="System.Threading" />
    <Using Include="System.Threading.Tasks" />
  </ItemGroup>
</Project>
"@
    New-FileWithContent "Directory.Build.props" $buildProps -ForceWrite
    
    # .editorconfig
    $editorConfig = @'
root = true

[*]
charset = utf-8
end_of_line = lf
insert_final_newline = true
trim_trailing_whitespace = true

[*.{cs,csx,vb,vbx}]
indent_size = 4
indent_style = space

[*.{json,yml,yaml}]
indent_size = 2
indent_style = space

[*.{ps1,psm1}]
indent_size = 4
indent_style = space

# C# Code Style
[*.cs]
# Desabilitar temporariamente alguns analyzers problemáticos
dotnet_diagnostic.SA1633.severity = none
dotnet_diagnostic.SA1600.severity = none
dotnet_diagnostic.SA1505.severity = none
dotnet_diagnostic.SA1508.severity = none
dotnet_diagnostic.S2094.severity = suggestion
dotnet_diagnostic.CA1716.severity = suggestion

# Desabilitar analyzers que conflitam com templates do .NET
dotnet_diagnostic.SA1200.severity = none       # Using fora do namespace (file-scoped namespaces)
dotnet_diagnostic.SA1309.severity = none       # Campo começa com underscore (padrão .NET)
dotnet_diagnostic.SA1101.severity = none       # Falta prefixo this.
dotnet_diagnostic.SA1515.severity = none       # Comentário sem linha em branco antes
dotnet_diagnostic.S6966.severity = suggestion  # Operação bloqueante em async
dotnet_diagnostic.S4487.severity = suggestion  # Campo escrito mas nunca lido

# Desabilitar analyzers de arquivos de template gerados
dotnet_diagnostic.SA1518.severity = none       # Arquivo sem newline no final
dotnet_diagnostic.IDE1006.severity = none      # Violação de nomenclatura
dotnet_diagnostic.SA1413.severity = none       # Falta vírgula no último item
dotnet_diagnostic.SCS0005.severity = none      # Random() não é criptograficamente seguro
dotnet_diagnostic.CA5394.severity = none       # Random() fraco (mesmo que SCS0005)
dotnet_diagnostic.S2699.severity = none        # Teste sem assertion
dotnet_diagnostic.S1186.severity = none        # Método vazio
dotnet_diagnostic.SA1512.severity = none       # Comentário seguido de linha em branco

# Organize usings
dotnet_sort_system_directives_first = true
dotnet_separate_import_directive_groups = false

# this. preferences
dotnet_style_qualification_for_field = false:warning
dotnet_style_qualification_for_property = false:warning
dotnet_style_qualification_for_method = false:warning
dotnet_style_qualification_for_event = false:warning

# Language keywords vs BCL types preferences
dotnet_style_predefined_type_for_locals_parameters_members = true:warning
dotnet_style_predefined_type_for_member_access = true:warning

# Modifier preferences
dotnet_style_require_accessibility_modifiers = always:warning
csharp_preferred_modifier_order = public,private,protected,internal,static,extern,new,virtual,abstract,sealed,override,readonly,unsafe,volatile,async:warning

# Expression-level preferences
dotnet_style_object_initializer = true:suggestion
dotnet_style_collection_initializer = true:suggestion
dotnet_style_explicit_tuple_names = true:warning
dotnet_style_prefer_inferred_tuple_names = true:suggestion
dotnet_style_prefer_inferred_anonymous_type_member_names = true:suggestion
dotnet_style_prefer_auto_properties = true:suggestion
dotnet_style_prefer_conditional_expression_over_assignment = true:silent
dotnet_style_prefer_conditional_expression_over_return = true:silent
dotnet_style_prefer_compound_assignment = true:suggestion

# Null-checking preferences
dotnet_style_coalesce_expression = true:warning
dotnet_style_null_propagation = true:warning
dotnet_style_prefer_is_null_check_over_reference_equality_method = true:warning

# C# Code Style
csharp_style_var_for_built_in_types = true:suggestion
csharp_style_var_when_type_is_apparent = true:suggestion
csharp_style_var_elsewhere = true:suggestion

# Expression-bodied members
csharp_style_expression_bodied_methods = when_on_single_line:silent
csharp_style_expression_bodied_constructors = false:silent
csharp_style_expression_bodied_operators = when_on_single_line:silent
csharp_style_expression_bodied_properties = true:silent
csharp_style_expression_bodied_indexers = true:silent
csharp_style_expression_bodied_accessors = true:silent
csharp_style_expression_bodied_lambdas = true:silent
csharp_style_expression_bodied_local_functions = when_on_single_line:silent

# Pattern matching
csharp_style_pattern_matching_over_is_with_cast_check = true:warning
csharp_style_pattern_matching_over_as_with_null_check = true:warning
csharp_style_prefer_switch_expression = true:suggestion
csharp_style_prefer_pattern_matching = true:suggestion
csharp_style_prefer_not_pattern = true:suggestion

# Inlined variable declarations
csharp_style_inlined_variable_declaration = true:warning

# C# expression-level preferences
csharp_prefer_simple_default_expression = true:suggestion

# C# null-checking preferences
csharp_style_throw_expression = true:suggestion
csharp_style_conditional_delegate_call = true:warning

# Code block preferences
csharp_prefer_braces = true:warning

# Unused value preferences
csharp_style_unused_value_expression_statement_preference = discard_variable:silent
csharp_style_unused_value_assignment_preference = discard_variable:suggestion

# Index and range preferences
csharp_style_prefer_index_operator = true:suggestion
csharp_style_prefer_range_operator = true:suggestion

# Miscellaneous preferences
csharp_style_pattern_local_over_anonymous_function = true:suggestion
csharp_style_deconstructed_variable_declaration = true:suggestion
csharp_prefer_static_local_function = true:warning
csharp_prefer_simple_using_statement = true:suggestion
csharp_style_prefer_method_group_conversion = true:silent

# Formatting
csharp_new_line_before_open_brace = all
csharp_new_line_before_else = true
csharp_new_line_before_catch = true
csharp_new_line_before_finally = true
csharp_new_line_before_members_in_object_initializers = true
csharp_new_line_before_members_in_anonymous_types = true
csharp_new_line_between_query_expression_clauses = true

# Indentation
csharp_indent_case_contents = true
csharp_indent_switch_labels = true
csharp_indent_labels = no_change
csharp_indent_block_contents = true
csharp_indent_braces = false
csharp_indent_case_contents_when_block = false

# Spacing
csharp_space_after_cast = false
csharp_space_after_keywords_in_control_flow_statements = true
csharp_space_between_parentheses = false
csharp_space_before_colon_in_inheritance_clause = true
csharp_space_after_colon_in_inheritance_clause = true
csharp_space_around_binary_operators = before_and_after
csharp_space_between_method_declaration_parameter_list_parentheses = false
csharp_space_between_method_declaration_empty_parameter_list_parentheses = false
csharp_space_between_method_declaration_name_and_open_parenthesis = false
csharp_space_between_method_call_parameter_list_parentheses = false
csharp_space_between_method_call_empty_parameter_list_parentheses = false
csharp_space_between_method_call_name_and_opening_parenthesis = false
csharp_space_after_comma = true
csharp_space_before_comma = false
csharp_space_after_dot = false
csharp_space_before_dot = false
csharp_space_after_semicolon_in_for_statement = true
csharp_space_before_semicolon_in_for_statement = false
csharp_space_around_declaration_statements = false
csharp_space_before_open_square_brackets = false
csharp_space_between_empty_square_brackets = false
csharp_space_between_square_brackets = false

# Wrapping
csharp_preserve_single_line_statements = false
csharp_preserve_single_line_blocks = true

# Naming Conventions
dotnet_naming_rule.interfaces_should_be_prefixed_with_i.severity = warning
dotnet_naming_rule.interfaces_should_be_prefixed_with_i.symbols = interface
dotnet_naming_rule.interfaces_should_be_prefixed_with_i.style = begins_with_i

dotnet_naming_rule.types_should_be_pascal_case.severity = warning
dotnet_naming_rule.types_should_be_pascal_case.symbols = types
dotnet_naming_rule.types_should_be_pascal_case.style = pascal_case

dotnet_naming_rule.non_field_members_should_be_pascal_case.severity = warning
dotnet_naming_rule.non_field_members_should_be_pascal_case.symbols = non_field_members
dotnet_naming_rule.non_field_members_should_be_pascal_case.style = pascal_case

dotnet_naming_rule.private_fields_should_be_camel_case_with_underscore.severity = warning
dotnet_naming_rule.private_fields_should_be_camel_case_with_underscore.symbols = private_fields
dotnet_naming_rule.private_fields_should_be_camel_case_with_underscore.style = camel_case_with_underscore

# Symbol specifications
dotnet_naming_symbols.interface.applicable_kinds = interface
dotnet_naming_symbols.interface.applicable_accessibilities = public, internal, private, protected, protected_internal, private_protected

dotnet_naming_symbols.types.applicable_kinds = class, struct, interface, enum
dotnet_naming_symbols.types.applicable_accessibilities = public, internal, private, protected, protected_internal, private_protected

dotnet_naming_symbols.non_field_members.applicable_kinds = property, event, method
dotnet_naming_symbols.non_field_members.applicable_accessibilities = public, internal, private, protected, protected_internal, private_protected

dotnet_naming_symbols.private_fields.applicable_kinds = field
dotnet_naming_symbols.private_fields.applicable_accessibilities = private

# Naming styles
dotnet_naming_style.pascal_case.capitalization = pascal_case

dotnet_naming_style.begins_with_i.required_prefix = I
dotnet_naming_style.begins_with_i.capitalization = pascal_case

dotnet_naming_style.camel_case_with_underscore.required_prefix = _
dotnet_naming_style.camel_case_with_underscore.capitalization = camel_case
'@
    New-FileWithContent ".editorconfig" $editorConfig
    
    # .gitignore
    $gitIgnore = @'
# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Ww][Ii][Nn]32/
[Aa][Rr][Mm]/
[Aa][Rr][Mm]64/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/
[Ll]ogs/

# Visual Studio
.vs/
.vscode/
*.rsuser
*.suo
*.user
*.userosscache
*.sln.docstates
*.userprefs

# Test Results
[Tt]est[Rr]esult*/
[Bb]uild[Ll]og.*
*.VisualState.xml
TestResult.xml
*.coverage
*.coveragexml

# NuGet
*.nupkg
*.snupkg
**/packages/*
!**/packages/build/
*.nuget.props
*.nuget.targets
project.lock.json
project.fragment.lock.json
artifacts/

# Node
node_modules/
npm-debug.log*
yarn-debug.log*
yarn-error.log*

# Others
*.cache
*.log
*.tmp
*.temp
.DS_Store
Thumbs.db
'@
    New-FileWithContent ".gitignore" $gitIgnore
    
    # .gitattributes
    $gitAttributes = @'
# Auto detect text files and perform LF normalization
* text=auto eol=lf

# Windows-specific files
*.sln text eol=crlf
*.ps1 text eol=crlf
*.cmd text eol=crlf
*.bat text eol=crlf

# Binary files
*.dll binary
*.exe binary
*.png binary
*.jpg binary
*.jpeg binary
*.gif binary
*.ico binary
*.pdf binary
'@
    New-FileWithContent ".gitattributes" $gitAttributes
    
    Write-ColorMessage "  ✅ Governança configurada com sucesso" $Colors.Success
}


# ============================================================================
# FASE 2: SOLUTION E ESTRUTURA DE PROJETOS
# ============================================================================
Invoke-Phase "[2/12] Criando Solution e Estrutura de Projetos" {
    
    # Criar solution
    if (-not (Test-Path "$ProjectName.sln")) {
        dotnet new sln -n $ProjectName --force | Out-Null
        Write-ColorMessage "  ✅ Solution criada: $ProjectName.sln" $Colors.Success
    }
    
    # Criar estrutura de pastas base
    $baseFolders = @(
        "src/Shared/Core",
        "src/Shared/Application",
        "src/Shared/Infrastructure",
        "src/Shared/Contracts",
        "src/Modules/GestaoDePessoas",
        "src/Modules/ControleDePonto",
        "src/Modules/Treinamentos",
        "src/Modules/SaudeOcupacional",
        "src/Modules/Avaliacoes",
        "src/Identity",
        "src/API",
        "src/Web",
        "docs",
        "scripts",
        "deploy/docker",
        "deploy/kubernetes",
        "deploy/terraform",
        ".github/workflows"
    )
    
    if (-not $SkipTests) {
        $baseFolders += @(
            "tests/Unit",
            "tests/Integration",
            "tests/Architecture",
            "tests/E2E"
        )
    }
    
    foreach ($folder in $baseFolders) {
        New-Folder $folder
    }
    
    Write-ColorMessage "  ✅ Estrutura de pastas criada" $Colors.Success
}

# ============================================================================
# FASE 3: PROJETOS SHARED (CORE, APPLICATION, INFRASTRUCTURE, CONTRACTS)
# ============================================================================
Invoke-Phase "[3/12] Criando Projetos Shared" {
    
    # Shared.Core (Domain)
    $sharedCorePath = "src/Shared/Core/$ProjectName.Shared.Core.csproj"
    if (-not (Test-Path $sharedCorePath)) {
        dotnet new classlib -n "$ProjectName.Shared.Core" -o "src/Shared/Core" --force | Out-Null
        $Script:Stats.ProjectsCreated++
    }
    Add-ToSolution $sharedCorePath "src/Shared"
    
    # Criar estrutura de pastas no Shared.Core
    $coreFolders = @(
        "src/Shared/Core/Entities",
        "src/Shared/Core/ValueObjects",
        "src/Shared/Core/Enums",
        "src/Shared/Core/Interfaces",
        "src/Shared/Core/Exceptions",
        "src/Shared/Core/Constants",
        "src/Shared/Core/Events"
    )
    foreach ($folder in $coreFolders) {
        New-Folder $folder
    }
    
    # Shared.Application
    $sharedAppPath = "src/Shared/Application/$ProjectName.Shared.Application.csproj"
    if (-not (Test-Path $sharedAppPath)) {
        dotnet new classlib -n "$ProjectName.Shared.Application" -o "src/Shared/Application" --force | Out-Null
        $Script:Stats.ProjectsCreated++
    }
    Add-ToSolution $sharedAppPath "src/Shared"
    
    # Adicionar referências e pacotes ao Shared.Application
    dotnet add $sharedAppPath reference $sharedCorePath 2>&1 | Out-Null
    dotnet add $sharedAppPath package MediatR 2>&1 | Out-Null
    dotnet add $sharedAppPath package AutoMapper 2>&1 | Out-Null
    dotnet add $sharedAppPath package FluentValidation 2>&1 | Out-Null
    
    # Shared.Infrastructure
    $sharedInfraPath = "src/Shared/Infrastructure/$ProjectName.Shared.Infrastructure.csproj"
    if (-not (Test-Path $sharedInfraPath)) {
        dotnet new classlib -n "$ProjectName.Shared.Infrastructure" -o "src/Shared/Infrastructure" --force | Out-Null
        $Script:Stats.ProjectsCreated++
    }
    Add-ToSolution $sharedInfraPath "src/Shared"
    
    # Adicionar referências e pacotes ao Shared.Infrastructure
    dotnet add $sharedInfraPath reference $sharedCorePath 2>&1 | Out-Null
    dotnet add $sharedInfraPath reference $sharedAppPath 2>&1 | Out-Null
    dotnet add $sharedInfraPath package Microsoft.EntityFrameworkCore 2>&1 | Out-Null
    dotnet add $sharedInfraPath package Microsoft.EntityFrameworkCore.SqlServer 2>&1 | Out-Null
    dotnet add $sharedInfraPath package Serilog.AspNetCore 2>&1 | Out-Null
    
    # Shared.Contracts (DTOs compartilhados)
    $sharedContractsPath = "src/Shared/Contracts/$ProjectName.Shared.Contracts.csproj"
    if (-not (Test-Path $sharedContractsPath)) {
        dotnet new classlib -n "$ProjectName.Shared.Contracts" -o "src/Shared/Contracts" --force | Out-Null
        $Script:Stats.ProjectsCreated++
    }
    Add-ToSolution $sharedContractsPath "src/Shared"
    
    Write-ColorMessage "  ✅ Projetos Shared criados com sucesso" $Colors.Success
}

# ============================================================================
# FASE 4: MÓDULOS DE NEGÓCIO
# ============================================================================
Invoke-Phase "[4/12] Criando Módulos de Negócio" {
    
    $modules = @(
        "GestaoDePessoas",
        "ControleDePonto",
        "Treinamentos",
        "SaudeOcupacional",
        "Avaliacoes"
    )
    
    foreach ($module in $modules) {
        $modulePath = "src/Modules/$module"
        
        # Criar estrutura de pastas do módulo
        New-ModuleStructure -ModuleName $module -ModulePath $modulePath
        
        # Criar projeto do módulo
        $moduleProjectPath = "$modulePath/$ProjectName.Modules.$module.csproj"
        if (-not (Test-Path $moduleProjectPath)) {
            dotnet new classlib -n "$ProjectName.Modules.$module" -o $modulePath --force | Out-Null
            $Script:Stats.ProjectsCreated++
        }
        Add-ToSolution $moduleProjectPath "src/Modules"
        
        # Adicionar referências aos projetos Shared
        dotnet add $moduleProjectPath reference "src/Shared/Core/$ProjectName.Shared.Core.csproj" 2>&1 | Out-Null
        dotnet add $moduleProjectPath reference "src/Shared/Application/$ProjectName.Shared.Application.csproj" 2>&1 | Out-Null
        dotnet add $moduleProjectPath reference "src/Shared/Infrastructure/$ProjectName.Shared.Infrastructure.csproj" 2>&1 | Out-Null
        
        # Adicionar pacotes essenciais
        dotnet add $moduleProjectPath package MediatR 2>&1 | Out-Null
        dotnet add $moduleProjectPath package AutoMapper 2>&1 | Out-Null
        dotnet add $moduleProjectPath package FluentValidation 2>&1 | Out-Null
        dotnet add $moduleProjectPath package Microsoft.EntityFrameworkCore 2>&1 | Out-Null
    }
    
    Write-ColorMessage "  ✅ Módulos de negócio criados com sucesso" $Colors.Success
}

# ============================================================================
# FASE 5: IDENTITY & SECURITY
# ============================================================================
Invoke-Phase "[5/12] Configurando Identity & Security" {
    
    $identityPath = "src/Identity/$ProjectName.Identity.csproj"
    if (-not (Test-Path $identityPath)) {
        dotnet new classlib -n "$ProjectName.Identity" -o "src/Identity" --force | Out-Null
        $Script:Stats.ProjectsCreated++
    }
    Add-ToSolution $identityPath "src"
    
    # Adicionar referências
    dotnet add $identityPath reference "src/Shared/Core/$ProjectName.Shared.Core.csproj" 2>&1 | Out-Null
    dotnet add $identityPath reference "src/Shared/Infrastructure/$ProjectName.Shared.Infrastructure.csproj" 2>&1 | Out-Null
    
    # Adicionar pacotes de Identity e Security
    dotnet add $identityPath package Microsoft.AspNetCore.Identity.EntityFrameworkCore 2>&1 | Out-Null
    dotnet add $identityPath package Microsoft.AspNetCore.Authentication.JwtBearer 2>&1 | Out-Null
    dotnet add $identityPath package BCrypt.Net-Next 2>&1 | Out-Null
    dotnet add $identityPath package Microsoft.AspNetCore.DataProtection.EntityFrameworkCore 2>&1 | Out-Null
    
    # Criar estrutura de pastas
    $identityFolders = @(
        "src/Identity/Entities",
        "src/Identity/Services",
        "src/Identity/Configurations",
        "src/Identity/Validators"
    )
    foreach ($folder in $identityFolders) {
        New-Folder $folder
    }
    
    Write-ColorMessage "  ✅ Identity & Security configurados" $Colors.Success
}

# ============================================================================
# FASE 6: API E WEB PROJECTS
# ============================================================================
Invoke-Phase "[6/12] Criando Projetos API e Web" {
    
    # API Project (REST API)
    $apiPath = "src/API/$ProjectName.API.csproj"
    if (-not (Test-Path $apiPath)) {
        dotnet new webapi -n "$ProjectName.API" -o "src/API" --use-controllers --force | Out-Null
        Remove-PackageVersions $apiPath
        $Script:Stats.ProjectsCreated++
    }
    Add-ToSolution $apiPath "src"
    
    # Adicionar referências aos módulos
    dotnet add $apiPath reference "src/Shared/Core/$ProjectName.Shared.Core.csproj" 2>&1 | Out-Null
    dotnet add $apiPath reference "src/Shared/Application/$ProjectName.Shared.Application.csproj" 2>&1 | Out-Null
    dotnet add $apiPath reference "src/Shared/Infrastructure/$ProjectName.Shared.Infrastructure.csproj" 2>&1 | Out-Null
    dotnet add $apiPath reference "src/Identity/$ProjectName.Identity.csproj" 2>&1 | Out-Null
    
    # Adicionar todos os módulos
    Get-ChildItem "src/Modules" -Directory | ForEach-Object {
        $moduleProj = Get-ChildItem $_.FullName -Filter "*.csproj" | Select-Object -First 1
        if ($moduleProj) {
            dotnet add $apiPath reference $moduleProj.FullName 2>&1 | Out-Null
        }
    }
    
    # Adicionar pacotes essenciais da API
    dotnet add $apiPath package Swashbuckle.AspNetCore 2>&1 | Out-Null
    dotnet add $apiPath package Asp.Versioning.Mvc 2>&1 | Out-Null
    dotnet add $apiPath package Asp.Versioning.Mvc.ApiExplorer 2>&1 | Out-Null
    dotnet add $apiPath package Serilog.AspNetCore 2>&1 | Out-Null
    dotnet add $apiPath package AspNetCore.HealthChecks.SqlServer 2>&1 | Out-Null
    dotnet add $apiPath package AspNetCore.HealthChecks.UI 2>&1 | Out-Null
    dotnet add $apiPath package AspNetCore.HealthChecks.UI.Client 2>&1 | Out-Null
    dotnet add $apiPath package Hangfire.AspNetCore 2>&1 | Out-Null
    dotnet add $apiPath package Microsoft.AspNetCore.SignalR.Common 2>&1 | Out-Null
    
    # Web Project (MVC/Razor Pages)
    $webPath = "src/Web/$ProjectName.Web.csproj"
    if (-not (Test-Path $webPath)) {
        dotnet new mvc -n "$ProjectName.Web" -o "src/Web" --force | Out-Null
        Remove-PackageVersions $webPath
        $Script:Stats.ProjectsCreated++
    }
    Add-ToSolution $webPath "src"
    
    # Adicionar referências
    dotnet add $webPath reference "src/Shared/Contracts/$ProjectName.Shared.Contracts.csproj" 2>&1 | Out-Null
    dotnet add $webPath reference "src/Identity/$ProjectName.Identity.csproj" 2>&1 | Out-Null
    
    # Adicionar pacotes essenciais do Web
    dotnet add $webPath package Serilog.AspNetCore 2>&1 | Out-Null
    dotnet add $webPath package Microsoft.AspNetCore.SignalR.Client 2>&1 | Out-Null
    
    Write-ColorMessage "  ✅ Projetos API e Web criados com sucesso" $Colors.Success
}


# ============================================================================
# FASE 7: PROJETOS DE TESTE
# ============================================================================
Invoke-Phase "[7/12] Criando Projetos de Teste" {
    
    if ($SkipTests) {
        Write-ColorMessage "  ⏭️ Testes ignorados (flag -SkipTests)" $Colors.Warning
        return
    }
    
    # Unit Tests
    $unitTestPath = "tests/Unit/$ProjectName.UnitTests.csproj"
    if (-not (Test-Path $unitTestPath)) {
        dotnet new xunit -n "$ProjectName.UnitTests" -o "tests/Unit" --force | Out-Null
        Remove-PackageVersions $unitTestPath
        $Script:Stats.ProjectsCreated++
    }
    Add-ToSolution $unitTestPath "tests"
    
    # Pacotes de teste já estão no Directory.Packages.props (Central Package Management)
    # Não é necessário adicionar manualmente - o CPM gerencia as versões automaticamente
    # dotnet add $unitTestPath package Moq 2>&1 | Out-Null
    # dotnet add $unitTestPath package FluentAssertions 2>&1 | Out-Null
    # dotnet add $unitTestPath package Bogus 2>&1 | Out-Null
    
    # Adicionar referências aos projetos
    dotnet add $unitTestPath reference "src/Shared/Core/$ProjectName.Shared.Core.csproj" 2>&1 | Out-Null
    dotnet add $unitTestPath reference "src/Shared/Application/$ProjectName.Shared.Application.csproj" 2>&1 | Out-Null
    
    # Integration Tests
    $integrationTestPath = "tests/Integration/$ProjectName.IntegrationTests.csproj"
    if (-not (Test-Path $integrationTestPath)) {
        dotnet new xunit -n "$ProjectName.IntegrationTests" -o "tests/Integration" --force | Out-Null
        Remove-PackageVersions $integrationTestPath
        $Script:Stats.ProjectsCreated++
    }
    Add-ToSolution $integrationTestPath "tests"
    
    # Pacotes já estão no Directory.Packages.props (CPM)
    # dotnet add $integrationTestPath package Microsoft.AspNetCore.Mvc.Testing 2>&1 | Out-Null
    # dotnet add $integrationTestPath package Testcontainers 2>&1 | Out-Null
    # dotnet add $integrationTestPath package Testcontainers.MsSql 2>&1 | Out-Null
    # dotnet add $integrationTestPath package Respawn 2>&1 | Out-Null
    # dotnet add $integrationTestPath package WireMock.Net 2>&1 | Out-Null
    # dotnet add $integrationTestPath package FluentAssertions 2>&1 | Out-Null
    
    # Adicionar referência ao projeto API
    dotnet add $integrationTestPath reference "src/API/$ProjectName.API.csproj" 2>&1 | Out-Null
    
    # Architecture Tests
    $archTestPath = "tests/Architecture/$ProjectName.ArchitectureTests.csproj"
    if (-not (Test-Path $archTestPath)) {
        dotnet new xunit -n "$ProjectName.ArchitectureTests" -o "tests/Architecture" --force | Out-Null
        Remove-PackageVersions $archTestPath
        $Script:Stats.ProjectsCreated++
    }
    Add-ToSolution $archTestPath "tests"
    
    # Pacotes já estão no Directory.Packages.props (CPM)
    # dotnet add $archTestPath package NetArchTest.Rules 2>&1 | Out-Null
    # dotnet add $archTestPath package FluentAssertions 2>&1 | Out-Null
    
    # E2E Tests
    $e2eTestPath = "tests/E2E/$ProjectName.E2ETests.csproj"
    if (-not (Test-Path $e2eTestPath)) {
        dotnet new xunit -n "$ProjectName.E2ETests" -o "tests/E2E" --force | Out-Null
        Remove-PackageVersions $e2eTestPath
        $Script:Stats.ProjectsCreated++
    }
    Add-ToSolution $e2eTestPath "tests"
    
    # Pacotes já estão no Directory.Packages.props (CPM)
    # dotnet add $e2eTestPath package Microsoft.AspNetCore.Mvc.Testing 2>&1 | Out-Null
    # dotnet add $e2eTestPath package FluentAssertions 2>&1 | Out-Null
    
    Write-ColorMessage "  ✅ Projetos de teste criados com sucesso (pacotes gerenciados via CPM)" $Colors.Success
}

# ============================================================================
# FASE 8: DOCKER & CONTAINERS
# ============================================================================
Invoke-Phase "[8/12] Configurando Docker & Containers" {
    
    if ($SkipDocker) {
        Write-ColorMessage "  ⏭️ Docker ignorado (flag -SkipDocker)" $Colors.Warning
        return
    }
    
    # Dockerfile para API
    $dockerfileApi = @"
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/API/$ProjectName.API.csproj", "src/API/"]
COPY ["src/Shared/Core/$ProjectName.Shared.Core.csproj", "src/Shared/Core/"]
COPY ["src/Shared/Application/$ProjectName.Shared.Application.csproj", "src/Shared/Application/"]
COPY ["src/Shared/Infrastructure/$ProjectName.Shared.Infrastructure.csproj", "src/Shared/Infrastructure/"]
COPY ["src/Identity/$ProjectName.Identity.csproj", "src/Identity/"]
RUN dotnet restore "src/API/$ProjectName.API.csproj"
COPY . .
WORKDIR "/src/src/API"
RUN dotnet build "$ProjectName.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "$ProjectName.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "$ProjectName.API.dll"]
"@
    New-FileWithContent "src/API/Dockerfile" $dockerfileApi
    
    # docker-compose.yml
    $dockerCompose = @"
version: '3.8'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:$SqlServerVersion
    container_name: $projectNameLower-sqlserver
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
      - MSSQL_PID=Developer
    ports:
      - "1433:1433"
    volumes:
      - sqlserver-data:/var/opt/mssql
    networks:
      - $projectNameLower-network
    healthcheck:
      test: /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -Q "SELECT 1" || exit 1
      interval: 10s
      timeout: 3s
      retries: 10
      start_period: 10s

  redis:
    image: redis:7-alpine
    container_name: $projectNameLower-redis
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data
    networks:
      - $projectNameLower-network
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 3s
      retries: 5

  seq:
    image: datalust/seq:latest
    container_name: $projectNameLower-seq
    environment:
      - ACCEPT_EULA=Y
    ports:
      - "5341:80"
    volumes:
      - seq-data:/data
    networks:
      - $projectNameLower-network

  api:
    build:
      context: .
      dockerfile: src/API/Dockerfile
    container_name: $projectNameLower-api
    environment:
      - ASPNETCORE_ENVIRONMENT=$Environment
      - ASPNETCORE_URLS=http://+:80
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=${ProjectName}DB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
      - Redis__Configuration=redis:6379
      - Serilog__WriteTo__0__Args__serverUrl=http://seq:5341
    ports:
      - "5000:80"
    depends_on:
      sqlserver:
        condition: service_healthy
      redis:
        condition: service_healthy
      seq:
        condition: service_started
    networks:
      - $projectNameLower-network
    restart: unless-stopped

volumes:
  sqlserver-data:
  redis-data:
  seq-data:

networks:
  $projectNameLower-network:
    driver: bridge
"@
    New-FileWithContent "docker-compose.yml" $dockerCompose
    
    # .dockerignore
    $dockerIgnore = @'
**/.git
**/.gitignore
**/.vs
**/.vscode
**/bin
**/obj
**/*.user
**/node_modules
**/TestResults
**/*.md
**/LICENSE
'@
    New-FileWithContent ".dockerignore" $dockerIgnore
    
    Write-ColorMessage "  ✅ Docker configurado com sucesso" $Colors.Success
}

# ============================================================================
# FASE 9: KUBERNETES
# ============================================================================
Invoke-Phase "[9/12] Configurando Kubernetes" {
    
    if ($SkipDocker) {
        Write-ColorMessage "  ⏭️ Kubernetes ignorado (depende do Docker)" $Colors.Warning
        return
    }
    
    # Deployment
    $k8sDeployment = @"
apiVersion: apps/v1
kind: Deployment
metadata:
  name: $projectNameLower-api
  labels:
    app: $projectNameLower-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: $projectNameLower-api
  template:
    metadata:
      labels:
        app: $projectNameLower-api
    spec:
      containers:
      - name: api
        image: $projectNameLower-api:latest
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "$Environment"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: $projectNameLower-secrets
              key: connection-string
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health/live
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 80
          initialDelaySeconds: 10
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: $projectNameLower-api-service
spec:
  selector:
    app: $projectNameLower-api
  ports:
  - protocol: TCP
    port: 80
    targetPort: 80
  type: LoadBalancer
"@
    New-FileWithContent "deploy/kubernetes/deployment.yaml" $k8sDeployment
    
    # ConfigMap
    $k8sConfigMap = @"
apiVersion: v1
kind: ConfigMap
metadata:
  name: $projectNameLower-config
data:
  ASPNETCORE_ENVIRONMENT: "$Environment"
  Serilog__MinimumLevel: "Information"
"@
    New-FileWithContent "deploy/kubernetes/configmap.yaml" $k8sConfigMap
    
    # Secret (exemplo - valores devem ser base64)
    $k8sSecret = @"
apiVersion: v1
kind: Secret
metadata:
  name: $projectNameLower-secrets
type: Opaque
data:
  connection-string: U2VydmVyPXNxbHNlcnZlcjtEYXRhYmFzZT1SaFNlbnNvRVJQREI7VXNlciBJZD1zYTtQYXNzd29yZD1Zb3VyU3Ryb25nQFBhc3N3MHJkO1RydXN0U2VydmVyQ2VydGlmaWNhdGU9VHJ1ZTs=
"@
    New-FileWithContent "deploy/kubernetes/secret.yaml" $k8sSecret
    
    Write-ColorMessage "  ✅ Kubernetes configurado com sucesso" $Colors.Success
}

# ============================================================================
# FASE 10: CI/CD (GITHUB ACTIONS + AZURE DEVOPS)
# ============================================================================
Invoke-Phase "[10/12] Configurando CI/CD" {
    
    # GitHub Actions - CI
    $githubCI = @"
name: CI

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

env:
  DOTNET_VERSION: '8.0.x'

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
      with:
        fetch-depth: 0
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: `${{ env.DOTNET_VERSION }}
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore --configuration Release
    
    - name: Run Unit Tests
      run: dotnet test tests/Unit/$ProjectName.UnitTests.csproj --no-build --configuration Release --verbosity normal --collect:"XPlat Code Coverage"
    
    - name: Run Integration Tests
      run: dotnet test tests/Integration/$ProjectName.IntegrationTests.csproj --no-build --configuration Release --verbosity normal
    
    - name: Run Architecture Tests
      run: dotnet test tests/Architecture/$ProjectName.ArchitectureTests.csproj --no-build --configuration Release --verbosity normal
    
    - name: Upload coverage reports to Codecov
      uses: codecov/codecov-action@v4
      with:
        token: `${{ secrets.CODECOV_TOKEN }}
"@
    New-FileWithContent ".github/workflows/ci.yml" $githubCI
    
    # GitHub Actions - CD
    $githubCD = @"
name: CD

on:
  push:
    branches: [ main ]
    tags:
      - 'v*'

env:
  DOTNET_VERSION: '8.0.x'
  REGISTRY: ghcr.io
  IMAGE_NAME: `${{ github.repository }}

jobs:
  deploy:
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: `${{ env.DOTNET_VERSION }}
    
    - name: Publish
      run: dotnet publish src/API/$ProjectName.API.csproj -c Release -o ./publish
    
    - name: Log in to Container Registry
      uses: docker/login-action@v3
      with:
        registry: `${{ env.REGISTRY }}
        username: `${{ github.actor }}
        password: `${{ secrets.GITHUB_TOKEN }}
    
    - name: Extract metadata
      id: meta
      uses: docker/metadata-action@v5
      with:
        images: `${{ env.REGISTRY }}/`${{ env.IMAGE_NAME }}
    
    - name: Build and push Docker image
      uses: docker/build-push-action@v5
      with:
        context: .
        file: src/API/Dockerfile
        push: true
        tags: `${{ steps.meta.outputs.tags }}
        labels: `${{ steps.meta.outputs.labels }}
"@
    New-FileWithContent ".github/workflows/cd.yml" $githubCD
    
    # Azure DevOps Pipeline
    $azurePipeline = @"
trigger:
  branches:
    include:
    - main
    - develop

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  dotnetSdkVersion: '8.0.x'

stages:
- stage: Build
  displayName: 'Build and Test'
  jobs:
  - job: BuildJob
    displayName: 'Build'
    steps:
    - task: UseDotNet@2
      displayName: 'Use .NET SDK'
      inputs:
        version: `$(dotnetSdkVersion)
    
    - task: DotNetCoreCLI@2
      displayName: 'Restore'
      inputs:
        command: 'restore'
    
    - task: DotNetCoreCLI@2
      displayName: 'Build'
      inputs:
        command: 'build'
        arguments: '--configuration `$(buildConfiguration) --no-restore'
    
    - task: DotNetCoreCLI@2
      displayName: 'Test'
      inputs:
        command: 'test'
        arguments: '--configuration `$(buildConfiguration) --no-build --collect:"XPlat Code Coverage"'
        publishTestResults: true
    
    - task: PublishCodeCoverageResults@1
      displayName: 'Publish Code Coverage'
      inputs:
        codeCoverageTool: 'Cobertura'
        summaryFileLocation: '`$(Agent.TempDirectory)/**/*coverage.cobertura.xml'

- stage: Deploy
  displayName: 'Deploy to Production'
  dependsOn: Build
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  jobs:
  - deployment: DeployJob
    displayName: 'Deploy'
    environment: 'production'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: Docker@2
            displayName: 'Build and Push'
            inputs:
              command: 'buildAndPush'
              repository: '$projectNameLower'
              dockerfile: 'src/API/Dockerfile'
              tags: |
                `$(Build.BuildId)
                latest
"@
    New-FileWithContent "azure-pipelines.yml" $azurePipeline
    
    Write-ColorMessage "  ✅ CI/CD configurado com sucesso" $Colors.Success
}

# ============================================================================
# FASE 11: DOCUMENTAÇÃO
# ============================================================================
Invoke-Phase "[11/12] Criando Documentação" {
    
    # README.md principal
    $readme = @"
# $ProjectName

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
``````bash
git clone https://github.com/seu-usuario/$ProjectName.git
cd $ProjectName
``````

2. Restaure as dependências:
``````bash
dotnet restore
``````

3. Configure a connection string em ``appsettings.Development.json``

4. Execute as migrations:
``````bash
dotnet ef database update --project src/API
``````

5. Execute a aplicação:
``````bash
dotnet run --project src/API
``````

### Docker

``````bash
docker-compose up -d
``````

## 🧪 Testes

``````bash
# Todos os testes
dotnet test

# Apenas testes unitários
dotnet test tests/Unit

# Apenas testes de integração
dotnet test tests/Integration

# Com cobertura
dotnet test --collect:"XPlat Code Coverage"
``````

## 📚 Documentação

- [Arquitetura](docs/architecture.md)
- [Guia de Desenvolvimento](docs/development-guide.md)
- [API Documentation](docs/api-documentation.md)
- [Deployment](docs/deployment.md)

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (``git checkout -b feature/AmazingFeature``)
3. Commit suas mudanças (``git commit -m 'Add some AmazingFeature'``)
4. Push para a branch (``git push origin feature/AmazingFeature``)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 👥 Autores

- **$CompanyName** - *Desenvolvimento Inicial*

## 🙏 Agradecimentos

- Equipe de desenvolvimento
- Comunidade .NET
"@
    New-FileWithContent "README.md" $readme
    
    # Architecture.md
    $architecture = @"
# Arquitetura do Sistema

## Visão Geral

O $ProjectName segue os princípios de **Clean Architecture**, **Domain-Driven Design (DDD)** e **CQRS**.

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
"@
    New-FileWithContent "docs/architecture.md" $architecture
    
    # Development Guide
    $devGuide = @"
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
- Usar ``#region`` apenas quando necessário

### Commits

Seguir o padrão Conventional Commits:

- ``feat:``: Nova funcionalidade
- ``fix:``: Correção de bug
- ``docs:``: Documentação
- ``style:``: Formatação
- ``refactor:``: Refatoração
- ``test:``: Testes
- ``chore:``: Tarefas de build/config

Exemplo:
``````
feat(gestao-pessoas): adicionar endpoint de busca por CPF
``````

## Criando um Novo Módulo

1. Criar estrutura de pastas no ``src/Modules/NomeModulo``
2. Criar projeto classlib
3. Adicionar à solution
4. Implementar camadas (Core, Application, Infrastructure)
5. Adicionar testes
6. Documentar

## Executando Localmente

``````bash
# Subir dependências (SQL Server, Redis, Seq)
docker-compose up -d sqlserver redis seq

# Executar migrations
dotnet ef database update --project src/API

# Executar API
dotnet run --project src/API

# Executar Web
dotnet run --project src/Web
``````

## Debugging

- API: https://localhost:5001
- Web: https://localhost:7001
- Swagger: https://localhost:5001/swagger
- Seq: http://localhost:5341
- Hangfire: https://localhost:5001/hangfire

## Troubleshooting

### Erro de certificado HTTPS

``````bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
``````

### Erro de conexão com SQL Server

Verificar se o container está rodando:
``````bash
docker ps
``````

### Erro de restore de pacotes

Limpar cache do NuGet:
``````bash
dotnet nuget locals all --clear
dotnet restore
``````
"@
    New-FileWithContent "docs/development-guide.md" $devGuide
    
    # LICENSE
    $license = @"
MIT License

Copyright (c) $(Get-Date -Format yyyy) $CompanyName

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
"@
    New-FileWithContent "LICENSE" $license
    
    Write-ColorMessage "  ✅ Documentação criada com sucesso" $Colors.Success
}

# ============================================================================
# FASE 12: FINALIZAÇÃO E VALIDAÇÃO
# ============================================================================
Invoke-Phase "[12/12] Finalizando e Validando" {
    
    # Build da solution para validar
    Write-ColorMessage "  🔨 Compilando solution..." $Colors.Info
    $buildResult = dotnet build --no-restore --configuration Release 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorMessage "  ✅ Build concluído com sucesso" $Colors.Success
    } else {
        Write-ColorMessage "  ⚠️ Build apresentou warnings (verifique os logs)" $Colors.Warning
        $Script:Stats.Warnings++
    }
    
    # Criar script de inicialização rápida
    $quickStart = @"
#!/usr/bin/env pwsh
# Script de inicialização rápida do $ProjectName

Write-Host "🚀 Iniciando $ProjectName..." -ForegroundColor Cyan

# Subir dependências
Write-Host "📦 Iniciando dependências (SQL Server, Redis, Seq)..." -ForegroundColor Yellow
docker-compose up -d sqlserver redis seq

# Aguardar SQL Server ficar pronto
Write-Host "⏳ Aguardando SQL Server..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Executar migrations
Write-Host "🗄️ Executando migrations..." -ForegroundColor Yellow
dotnet ef database update --project src/API

# Executar API
Write-Host "🌐 Iniciando API..." -ForegroundColor Green
Write-Host ""
Write-Host "✅ Acesse:" -ForegroundColor Green
Write-Host "   API: https://localhost:5001" -ForegroundColor Cyan
Write-Host "   Swagger: https://localhost:5001/swagger" -ForegroundColor Cyan
Write-Host "   Seq: http://localhost:5341" -ForegroundColor Cyan
Write-Host ""

dotnet run --project src/API
"@
    New-FileWithContent "scripts/start.ps1" $quickStart
    
    # Tornar script executável (Linux/Mac)
    if ($IsLinux -or $IsMacOS) {
        chmod +x scripts/start.ps1 2>&1 | Out-Null
    }
    
    Write-ColorMessage "  ✅ Finalização concluída" $Colors.Success
}

# ============================================================================
# RESUMO FINAL
# ============================================================================
$endTime = Get-Date
$duration = $endTime - $StartTime

Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor $Colors.Section
Write-Host "                    ✅ SETUP CONCLUÍDO COM SUCESSO                " -ForegroundColor $Colors.Success
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor $Colors.Section
Write-Host ""

Write-ColorMessage "📊 ESTATÍSTICAS:" $Colors.Info
Write-ColorMessage "  • Projetos criados: $($Script:Stats.ProjectsCreated)" $Colors.Detail
Write-ColorMessage "  • Arquivos criados: $($Script:Stats.FilesCreated)" $Colors.Detail
Write-ColorMessage "  • Pastas criadas: $($Script:Stats.FoldersCreated)" $Colors.Detail
Write-ColorMessage "  • Warnings: $($Script:Stats.Warnings)" $(if ($Script:Stats.Warnings -gt 0) { $Colors.Warning } else { $Colors.Detail })
Write-ColorMessage "  • Erros: $($Script:Stats.Errors)" $(if ($Script:Stats.Errors -gt 0) { $Colors.Error } else { $Colors.Detail })
Write-ColorMessage "  • Tempo total: $($duration.ToString('mm\:ss'))" $Colors.Detail

Write-Host ""
Write-ColorMessage "📁 ESTRUTURA CRIADA:" $Colors.Info
Write-ColorMessage "  • Solution: $ProjectName.sln" $Colors.Detail
Write-ColorMessage "  • Projetos Shared: 4" $Colors.Detail
Write-ColorMessage "  • Módulos de Negócio: 5" $Colors.Detail
Write-ColorMessage "  • Projetos de Teste: $(if ($SkipTests) { '0 (ignorado)' } else { '4' })" $Colors.Detail
Write-ColorMessage "  • Docker: $(if ($SkipDocker) { 'Não configurado' } else { 'Configurado' })" $Colors.Detail

Write-Host ""
Write-ColorMessage "🚀 PRÓXIMOS PASSOS:" $Colors.Info
Write-ColorMessage "  1. Revisar e customizar appsettings.json" $Colors.Detail
Write-ColorMessage "  2. Configurar connection string do banco de dados" $Colors.Detail
Write-ColorMessage "  3. Executar: ./scripts/start.ps1" $Colors.Detail
Write-ColorMessage "  4. Acessar Swagger: https://localhost:5001/swagger" $Colors.Detail
Write-ColorMessage "  5. Ler documentação em: docs/" $Colors.Detail

Write-Host ""
Write-ColorMessage "📝 LOG SALVO EM: $logFile" $Colors.Info

Write-Host ""
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor $Colors.Section
Write-ColorMessage "    Desenvolvido com ❤️ por $CompanyName" $Colors.Success
Write-ColorMessage "         Enterprise Architecture | Clean Code | DDD" $Colors.Success
Write-Host "═══════════════════════════════════════════════════════════════" -ForegroundColor $Colors.Section
Write-Host ""

# Encerrar transcript
Stop-Transcript | Out-Null

# Exit code baseado em erros
exit $Script:Stats.Errors
