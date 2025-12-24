# clean-rebuild.ps1
# Script para limpar e reconstruir a solução completamente

param(
    [switch]$Release = $false
)

$ErrorActionPreference = "Stop"

Write-Host "🧹 Limpando solução RhSensoERP..." -ForegroundColor Cyan

# Configuração
$Configuration = if ($Release) { "Release" } else { "Debug" }

Write-Host "Configuração: $Configuration" -ForegroundColor Yellow

# 1. Parar todos os processos dotnet
Write-Host "`n⏹️ Parando processos dotnet..." -ForegroundColor Yellow
Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

# 2. Limpar pastas bin e obj
Write-Host "`n🗑️ Removendo pastas bin e obj..." -ForegroundColor Yellow
Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | ForEach-Object {
    Write-Host "  Removendo: $($_.FullName)" -ForegroundColor DarkGray
    Remove-Item $_.FullName -Force -Recurse -ErrorAction SilentlyContinue
}

# 3. Limpar cache NuGet local
Write-Host "`n📦 Limpando cache NuGet local..." -ForegroundColor Yellow
dotnet nuget locals all --clear

# 4. Limpar solution
Write-Host "`n🧹 Executando dotnet clean..." -ForegroundColor Yellow
dotnet clean --configuration $Configuration

# 5. Restaurar pacotes
Write-Host "`n📥 Restaurando pacotes..." -ForegroundColor Yellow
dotnet restore --force

# 6. Reconstruir projeto API primeiro
Write-Host "`n🔨 Construindo RhSensoERP.API..." -ForegroundColor Yellow
dotnet build src/API/RhSensoERP.API.csproj --configuration $Configuration --no-restore

# 7. Construir solução completa
Write-Host "`n🏗️ Construindo solução completa..." -ForegroundColor Yellow
dotnet build --configuration $Configuration --no-restore

# 8. Verificar resultado
if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✅ Build concluído com sucesso!" -ForegroundColor Green
    
    # Listar DLLs geradas
    Write-Host "`n📁 DLLs geradas:" -ForegroundColor Cyan
    Get-ChildItem -Path src/API/bin/$Configuration/net8.0/*.dll | ForEach-Object {
        Write-Host "  ✓ $($_.Name)" -ForegroundColor DarkGreen
    }
} else {
    Write-Host "`n❌ Build falhou!" -ForegroundColor Red
    exit 1
}

Write-Host "`n📋 Próximos passos:" -ForegroundColor Yellow
Write-Host "1. Configure os secrets: .\init-secrets.ps1" -ForegroundColor White
Write-Host "2. Execute a aplicação: dotnet run --project src/API/RhSensoERP.API.csproj" -ForegroundColor White