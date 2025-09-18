# Script corrigido para HTTPS
Write-Host "=== TESTE API RhSensoERP (HTTPS) ===" -ForegroundColor Green

# CORREÇÃO 1: URL HTTPS 
$ApiUrl = "https://localhost:57148"
$healthUrl = "$ApiUrl/health"
$loginUrl = "$ApiUrl/api/v1/auth/login"

# CORREÇÃO 2: Configurar TLS e SSL para desenvolvimento
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }

Write-Host "1. Testando health em $healthUrl..." -ForegroundColor Yellow

try {
    $healthResponse = Invoke-RestMethod -Uri $healthUrl -Method Get -TimeoutSec 10
    Write-Host "✓ API RODANDO!" -ForegroundColor Green
    Write-Host "  Response: $healthResponse" -ForegroundColor Gray
    Write-Host ""
    
    Write-Host "2. Testando login verusa/ABC..." -ForegroundColor Yellow
    
    $loginData = @{
        cdUsuario = "verusa"
        senha = "ABC"
    } | ConvertTo-Json
    
    $loginResponse = Invoke-RestMethod -Uri $loginUrl -Method Post -Body $loginData -ContentType "application/json" -TimeoutSec 10
    
    if ($loginResponse.success -eq $true) {
        Write-Host "✓ LOGIN SUCESSO!" -ForegroundColor Green
        Write-Host ""
        Write-Host "--- DADOS DO USUARIO ---" -ForegroundColor Cyan
        Write-Host "Usuario: $($loginResponse.data.userData.cdUsuario)" -ForegroundColor White
        Write-Host "Nome: $($loginResponse.data.userData.dcUsuario)" -ForegroundColor White  
        Write-Host "Email: $($loginResponse.data.userData.emailUsuario)" -ForegroundColor White
        Write-Host "Empresa: $($loginResponse.data.userData.cdEmpresa)" -ForegroundColor White
        Write-Host "Status: $($loginResponse.data.userData.flAtivo)" -ForegroundColor White
        Write-Host ""
        Write-Host "--- TOKEN ---" -ForegroundColor Cyan
        $token = $loginResponse.data.accessToken
        if ($token) {
            $preview = $token.Substring(0, [Math]::Min(50, $token.Length)) + "..."
            Write-Host "Token: $preview" -ForegroundColor Gray
        }
        Write-Host ""
        Write-Host "--- GRUPOS ---" -ForegroundColor Cyan
        if ($loginResponse.data.groups -and $loginResponse.data.groups.Count -gt 0) {
            foreach ($grupo in $loginResponse.data.groups) {
                Write-Host "Sistema: $($grupo.cdSistema) | Grupo: $($grupo.cdGrUser)" -ForegroundColor White
            }
        } else {
            Write-Host "Nenhum grupo encontrado" -ForegroundColor Yellow
        }
    } else {
        Write-Host "✗ LOGIN FALHOU" -ForegroundColor Red
        Write-Host "Erro: $($loginResponse.message)" -ForegroundColor Red
    }
    
} catch {
    Write-Host "✗ ERRO: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        Write-Host "Status HTTP: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "=== FIM TESTE ===" -ForegroundColor Green