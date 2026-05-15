# Script para compilar o mod Allellopathy para Stardew Valley

# Configurações
$config = "Release"
$projectDir = $PSScriptRoot
$outputDir = Join-Path $projectDir "bin\$config"

# Determinar o sistema operacional
$isWindows = $PSVersionTable.PSVersion.ToString() -match "Windows"

if ($isWindows) {
    $gamePath = Join-Path $env:APPDATA "StardewValley\Mods\Allellopathy"
} else {
    # macOS
    $gamePath = Join-Path $HOME ".local/share/StardewValley/Mods/Allellopathy"
}

Write-Host "Iniciando compilação do mod Allellopathy..." -ForegroundColor Yellow

# Limpar compilações anteriores
Write-Host "Limpando compilações anteriores..." -ForegroundColor Yellow
if (Test-Path (Join-Path $projectDir "bin")) {
    Remove-Item -Path (Join-Path $projectDir "bin") -Recurse -Force
}
if (Test-Path (Join-Path $projectDir "obj")) {
    Remove-Item -Path (Join-Path $projectDir "obj") -Recurse -Force
}

# Compilar o projeto
Write-Host "Compilando o projeto..." -ForegroundColor Yellow
dotnet build -c $config

# Verificar se a compilação foi bem-sucedida
if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro na compilação! Verifique os erros acima." -ForegroundColor Red
    exit 1
}

Write-Host "Compilação concluída com sucesso!" -ForegroundColor Green

# Criar diretório de destino se não existir
if (-not (Test-Path $gamePath)) {
    Write-Host "Criando diretório de mods..." -ForegroundColor Yellow
    New-Item -Path $gamePath -ItemType Directory -Force | Out-Null
}

# Copiar arquivos para o diretório de mods
Write-Host "Copiando arquivos para o diretório de mods..." -ForegroundColor Yellow
Copy-Item -Path (Join-Path $outputDir "Allellopathy.dll") -Destination $gamePath -Force
Copy-Item -Path (Join-Path $projectDir "manifest.json") -Destination $gamePath -Force

# Copiar arquivos de tradução se existirem
$i18nDir = Join-Path $projectDir "i18n"
if (Test-Path $i18nDir) {
    Write-Host "Copiando arquivos de tradução..." -ForegroundColor Yellow
    $targetI18nDir = Join-Path $gamePath "i18n"
    if (-not (Test-Path $targetI18nDir)) {
        New-Item -Path $targetI18nDir -ItemType Directory -Force | Out-Null
    }
    Copy-Item -Path (Join-Path $i18nDir "*.json") -Destination $targetI18nDir -Force
}

Write-Host "Mod compilado e instalado com sucesso em:" -ForegroundColor Green
Write-Host $gamePath -ForegroundColor Green
Write-Host "Arquivo DLL gerado: $(Join-Path $outputDir "Allellopathy.dll")" -ForegroundColor Yellow
