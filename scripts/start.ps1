#!/usr/bin/env pwsh
# Script de inicialização rápida do RhSensoERP

Write-Host "🚀 Iniciando RhSensoERP..." -ForegroundColor Cyan

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
