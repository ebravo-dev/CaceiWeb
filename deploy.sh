#!/bin/bash

# Configuración del Servidor Windows
SERVER_IP="192.168.1.XXX"      # Cambia esto por la IP real
SERVER_USER="Administrator"    # Usuario de Windows
REMOTE_PATH="C:/inetpub/wwwroot/CaceiWeb" # Ruta en IIS

# Rutas Locales
PROJECT_PATH="/Users/ebravo/proyects/CaceiWeb"

echo "🚀 Iniciando despliegue a $SERVER_IP..."

# 1. Compilar el proyecto (opcional si subes los .cs, pero mejor compilar si usas DLLs)
# echo "🔨 Compilando..."
# xbuild /p:Configuration=Release CaceiWeb.csproj

# 2. Limpiar archivos basura antes de subir
echo "🧹 Limpiando archivos temporales..."
find . -name ".DS_Store" -delete

# 3. Subir archivos con SCP
echo "📤 Subiendo archivos al servidor..."
scp -r \
    "$PROJECT_PATH/Bin" \
    "$PROJECT_PATH/Content" \
    "$PROJECT_PATH/Scripts" \
    "$PROJECT_PATH/Admin" \
    "$PROJECT_PATH/App_Code" \
    "$PROJECT_PATH/Models" \
    "$PROJECT_PATH/Data" \
    "$PROJECT_PATH/Images" \
    "$PROJECT_PATH/*.aspx" \
    "$PROJECT_PATH/*.master" \
    "$PROJECT_PATH/Web.config" \
    "$SERVER_USER@$SERVER_IP:$REMOTE_PATH"

echo "✅ Despliegue completado."
echo "⚠️ Nota: Asegúrate de configurar 'DatabaseHelper.cs' y 'Web.config' en el servidor para Windows (ver guía)."
