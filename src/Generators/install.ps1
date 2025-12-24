# =============================================================================
# RHSENSOERP GENERATOR v3.0 - SCRIPT DE INSTALAÇÃO
# =============================================================================
# Este script instala o Generator no projeto RhSensoERP.
# Execute com: .\install.ps1 -SolutionPath "C:\Dev\RhSensoERP"
# =============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$SolutionPath
)

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host " RhSensoERP Generator v3.0 - Instalador " -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se o caminho existe
if (-not (Test-Path $SolutionPath)) {
    Write-Host "❌ Caminho não encontrado: $SolutionPath" -ForegroundColor Red
    exit 1
}

# Caminhos de destino
$GeneratorsPath = Join-Path $SolutionPath "src\Generators\RhSensoERP.Generators"

# Criar estrutura de pastas
Write-Host "📁 Criando estrutura de pastas..." -ForegroundColor Yellow

$folders = @(
    $GeneratorsPath,
    "$GeneratorsPath\Attributes",
    "$GeneratorsPath\Models",
    "$GeneratorsPath\Extractors",
    "$GeneratorsPath\Templates",
    "$GeneratorsPath\Generators"
)

foreach ($folder in $folders) {
    if (-not (Test-Path $folder)) {
        New-Item -ItemType Directory -Path $folder -Force | Out-Null
        Write-Host "  ✅ Criado: $folder" -ForegroundColor Green
    } else {
        Write-Host "  ⏭️  Já existe: $folder" -ForegroundColor DarkGray
    }
}

# Copiar arquivos
Write-Host ""
Write-Host "📄 Copiando arquivos do Generator..." -ForegroundColor Yellow

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

$filesToCopy = @(
    @{ Source = "Attributes\GenerateCrudAttribute.cs"; Dest = "Attributes" },
    @{ Source = "Models\EntityInfo.cs"; Dest = "Models" },
    @{ Source = "Extractors\EntityInfoExtractor.cs"; Dest = "Extractors" },
    @{ Source = "Templates\DtoTemplate.cs"; Dest = "Templates" },
    @{ Source = "Templates\CommandsTemplate.cs"; Dest = "Templates" },
    @{ Source = "Templates\QueriesTemplate.cs"; Dest = "Templates" },
    @{ Source = "Templates\ValidatorsTemplate.cs"; Dest = "Templates" },
    @{ Source = "Templates\RepositoryTemplate.cs"; Dest = "Templates" },
    @{ Source = "Templates\MapperTemplate.cs"; Dest = "Templates" },
    @{ Source = "Templates\EfConfigTemplate.cs"; Dest = "Templates" },
    @{ Source = "Templates\ApiControllerTemplate.cs"; Dest = "Templates" },
    @{ Source = "Templates\WebControllerTemplate.cs"; Dest = "Templates" },
    @{ Source = "Templates\WebModelsTemplate.cs"; Dest = "Templates" },
    @{ Source = "Templates\WebServicesTemplate.cs"; Dest = "Templates" },
    @{ Source = "Generators\CrudGenerator.cs"; Dest = "Generators" },
    @{ Source = "RhSensoERP.Generators.csproj"; Dest = "" },
    @{ Source = "README.md"; Dest = "" }
)

foreach ($file in $filesToCopy) {
    $sourcePath = Join-Path $ScriptDir $file.Source
    $destPath = Join-Path $GeneratorsPath $file.Dest
    $destFile = Join-Path $destPath (Split-Path -Leaf $file.Source)
    
    if (Test-Path $sourcePath) {
        Copy-Item -Path $sourcePath -Destination $destFile -Force
        Write-Host "  ✅ Copiado: $($file.Source)" -ForegroundColor Green
    } else {
        Write-Host "  ❌ Não encontrado: $($file.Source)" -ForegroundColor Red
    }
}

# Copiar atributo para o projeto Shared.Core
Write-Host ""
Write-Host "📄 Copiando atributo para Shared.Core..." -ForegroundColor Yellow

$SharedCorePath = Join-Path $SolutionPath "src\Shared\RhSensoERP.Shared.Core\Attributes"
if (-not (Test-Path $SharedCorePath)) {
    New-Item -ItemType Directory -Path $SharedCorePath -Force | Out-Null
}

$attributeSource = Join-Path $ScriptDir "Attributes\GenerateCrudAttribute.cs"
$attributeDest = Join-Path $SharedCorePath "GenerateCrudAttribute.cs"

if (Test-Path $attributeSource) {
    Copy-Item -Path $attributeSource -Destination $attributeDest -Force
    Write-Host "  ✅ Atributo copiado para Shared.Core" -ForegroundColor Green
}

# Adicionar referência ao .sln (informativo)
Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host " PRÓXIMOS PASSOS (MANUAL)               " -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1️⃣  Adicione o projeto ao .sln:" -ForegroundColor White
Write-Host "    dotnet sln add src\Generators\RhSensoERP.Generators\RhSensoERP.Generators.csproj" -ForegroundColor Gray
Write-Host ""
Write-Host "2️⃣  Adicione referência nos projetos Domain:" -ForegroundColor White
Write-Host @"
    <ItemGroup>
      <ProjectReference Include="..\..\Generators\RhSensoERP.Generators\RhSensoERP.Generators.csproj" 
                        OutputItemType="Analyzer" 
                        ReferenceOutputAssembly="false" />
    </ItemGroup>
"@ -ForegroundColor Gray
Write-Host ""
Write-Host "3️⃣  Marque suas Entities com [GenerateCrud]:" -ForegroundColor White
Write-Host @"
    [GenerateCrud(
        TableName = "tsistema",
        DisplayName = "Sistema",
        CdSistema = "SEG",
        CdFuncao = "SEG_FM_TSISTEMA"
    )]
    public class Sistema { ... }
"@ -ForegroundColor Gray
Write-Host ""
Write-Host "4️⃣  Build o projeto:" -ForegroundColor White
Write-Host "    dotnet build" -ForegroundColor Gray
Write-Host ""
Write-Host "✅ Instalação concluída!" -ForegroundColor Green
Write-Host ""
