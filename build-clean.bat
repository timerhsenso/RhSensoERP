@echo off
echo ==================================================
echo   Limpando, Restaurando e Buildando a Solução
echo ==================================================

:: Vai para a pasta onde está este .bat
cd /d %~dp0

:: Limpar
echo [1/3] Limpando...
dotnet clean

:: Restaurar
echo [2/3] Restaurando pacotes...
dotnet restore

:: Compilar
echo [3/3] Compilando projeto...
dotnet build

echo ==================================================
echo   Processo concluído!
echo ==================================================
pause
