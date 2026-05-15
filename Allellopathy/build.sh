#!/bin/bash

# Script para compilar o mod Allellopathy para Stardew Valley

# Cores para output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${YELLOW}Iniciando compilação do mod Allellopathy...${NC}"

# Diretório do projeto
PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$PROJECT_DIR"

# Configurações
CONFIG="Release"
OUTPUT_DIR="$PROJECT_DIR/bin/$CONFIG"
GAME_MODS_DIR="$HOME/.local/share/StardewValley/Mods/Allellopathy"

# Limpar compilações anteriores
echo -e "${YELLOW}Limpando compilações anteriores...${NC}"
rm -rf "$PROJECT_DIR/bin" "$PROJECT_DIR/obj"

# Compilar o projeto
echo -e "${YELLOW}Compilando o projeto...${NC}"
dotnet build -c $CONFIG

# Verificar se a compilação foi bem-sucedida
if [ $? -ne 0 ]; then
    echo -e "${RED}Erro na compilação! Verifique os erros acima.${NC}"
    exit 1
fi

echo -e "${GREEN}Compilação concluída com sucesso!${NC}"

# Criar diretório de destino se não existir
if [ ! -d "$GAME_MODS_DIR" ]; then
    echo -e "${YELLOW}Criando diretório de mods...${NC}"
    mkdir -p "$GAME_MODS_DIR"
fi

# Copiar arquivos para o diretório de mods
echo -e "${YELLOW}Copiando arquivos para o diretório de mods...${NC}"
cp -f "$OUTPUT_DIR/Allellopathy.dll" "$GAME_MODS_DIR/"
cp -f "$PROJECT_DIR/manifest.json" "$GAME_MODS_DIR/"

# Copiar arquivos de tradução se existirem
if [ -d "$PROJECT_DIR/i18n" ]; then
    echo -e "${YELLOW}Copiando arquivos de tradução...${NC}"
    mkdir -p "$GAME_MODS_DIR/i18n"
    cp -f "$PROJECT_DIR/i18n/"*.json "$GAME_MODS_DIR/i18n/"
fi

echo -e "${GREEN}Mod compilado e instalado com sucesso em:${NC}"
echo -e "${GREEN}$GAME_MODS_DIR${NC}"
echo -e "${YELLOW}Arquivo DLL gerado:${NC} $OUTPUT_DIR/Allellopathy.dll"
