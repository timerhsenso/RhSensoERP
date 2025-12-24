# powershell -ExecutionPolicy Bypass -File .\create-module.ps1 -ModuleName eSocial

param(
  [Parameter(Mandatory=$true)][string]$ModuleName,
  [string]$SolutionPath,
  [string]$ApiProject = "src\API\RhSensoERP.API.csproj",
  [string]$RootNamespace = "RhSensoERP.Modules"
)

function Info($m){Write-Host "[INFO] $m" -f Cyan}
function Warn($m){Write-Host "[WARN] $m" -f Yellow}
function Err ($m){Write-Host "[ERRO] $m" -f Red}

# Descobre .sln
if (-not $SolutionPath){
  $s = Get-ChildItem -Filter *.sln -File | Select-Object -First 1
  if(-not $s){ Err "Nenhum .sln na pasta atual"; exit 1 }
  $SolutionPath = $s.FullName
}
Info "Usando solução: $SolutionPath"

$moduleDir   = Join-Path "src\Modules" $ModuleName
$projectName = "$RootNamespace.$ModuleName"
$csprojPath  = Join-Path $moduleDir "$projectName.csproj"

# Cria pastas
$dirs = @(
  $moduleDir, "$moduleDir\Core", "$moduleDir\Application",
  "$moduleDir\Infrastructure", "$moduleDir\Infrastructure\Persistence",
  "$moduleDir\Infrastructure\Persistence\Configurations",
  "$moduleDir\Application\Services", "$moduleDir\Application\DTOs",
  "$moduleDir\Core\Entities", "$moduleDir\Core\ValueObjects",
  "$moduleDir\Core\Interfaces", "$moduleDir\Core\Enums"
)
$dirs | ForEach-Object { if(-not (Test-Path $_)){ New-Item -ItemType Directory $_ | Out-Null } }

# Cria o .csproj se não existir
if(-not (Test-Path $csprojPath)){
  pushd $moduleDir
  dotnet new classlib -n $projectName -f net8.0 | Out-Null
  popd
} else {
  Warn "Projeto já existe: $csprojPath"
}

# Ajusta o XML com DOM (seguro)
[xml]$xml = Get-Content $csprojPath
$pg = $xml.Project.PropertyGroup | Select-Object -First 1
if(-not $pg){
  $pg = $xml.CreateElement("PropertyGroup")
  $xml.Project.AppendChild($pg) | Out-Null
}
function EnsureNode($name,$value){
  $n = $pg.SelectSingleNode($name)
  if(-not $n){
    $n = $xml.CreateElement($name)
    $n.InnerText = $value
    $pg.AppendChild($n) | Out-Null
  } elseif([string]::IsNullOrWhiteSpace($n.InnerText)){
    $n.InnerText = $value
  }
}
EnsureNode "TargetFramework" "net8.0"
EnsureNode "Nullable" "enable"
EnsureNode "ImplicitUsings" "enable"
EnsureNode "RootNamespace" $projectName
$xml.Save($csprojPath)

# DI stubs
$appDI   = Join-Path $moduleDir "Application\DependencyInjection.cs"
$infraDI = Join-Path $moduleDir "Infrastructure\DependencyInjection.cs"
if(-not (Test-Path $appDI)){
@"
using Microsoft.Extensions.DependencyInjection;
namespace $projectName.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication$ModuleName(this IServiceCollection services)
        {
            return services;
        }
    }
}
"@ | Set-Content $appDI -Encoding UTF8
}
if(-not (Test-Path $infraDI)){
@"
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace $projectName.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure$ModuleName(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
    }
}
"@ | Set-Content $infraDI -Encoding UTF8
}

# Adiciona ao .sln
dotnet sln "$SolutionPath" add "$csprojPath" 2>$null | Out-Null

# Referência na API (se existir)
if(Test-Path $ApiProject){
  dotnet add "$ApiProject" reference "$csprojPath" 2>$null | Out-Null
  Info "API referenciada → $projectName"
} else {
  Warn "API não encontrada em '$ApiProject'."
}

Info "Módulo criado/atualizado: $projectName"
