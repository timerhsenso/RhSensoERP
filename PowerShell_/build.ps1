# build.ps1
Write-Host "🧹 Limpando solução..." -ForegroundColor Yellow
dotnet clean

Write-Host "📦 Restaurando pacotes..." -ForegroundColor Yellow
dotnet restore

Write-Host "🔨 Compilando na ordem correta..." -ForegroundColor Yellow

$projects = @(
    "src/Shared/Core/RhSensoERP.Shared.Core.csproj",
    "src/Shared/Application/RhSensoERP.Shared.Application.csproj",
    "src/Shared/Infrastructure/RhSensoERP.Shared.Infrastructure.csproj",
    "src/Identity/RhSensoERP.Identity.csproj",
    "src/Modules/Avaliacoes/RhSensoERP.Modules.Avaliacoes.csproj",
    "src/Modules/ControleDePonto/RhSensoERP.Modules.ControleDePonto.csproj",
    "src/Modules/GestaoDePessoas/RhSensoERP.Modules.GestaoDePessoas.csproj",
    "src/Modules/SaudeOcupacional/RhSensoERP.Modules.SaudeOcupacional.csproj",
    "src/Modules/Treinamentos/RhSensoERP.Modules.Treinamentos.csproj",
    "src/API/RhSensoERP.API.csproj",
    "src/Web/RhSensoERP.Web.csproj"
)

foreach ($project in $projects) {
    Write-Host "Building $project..." -ForegroundColor Cyan
    dotnet build $project --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Erro ao compilar $project" -ForegroundColor Red
        exit 1
    }
}

Write-Host "✅ Build concluído com sucesso!" -ForegroundColor Green