@echo off
REM =====================================================
REM Script de despliegue para CaceiWeb en IIS
REM =====================================================

SET IIS_PATH=C:\inetpub\wwwroot\CaceiWeb

echo Desplegando CaceiWeb a %IIS_PATH%...

REM Crear directorio si no existe
if not exist "%IIS_PATH%" mkdir "%IIS_PATH%"

REM Copiar archivos ASPX y code-behind
xcopy /Y "*.aspx" "%IIS_PATH%\"
xcopy /Y "*.aspx.cs" "%IIS_PATH%\"
xcopy /Y "*.Master" "%IIS_PATH%\"
xcopy /Y "*.Master.cs" "%IIS_PATH%\"
xcopy /Y "*.Master.designer.cs" "%IIS_PATH%\"
xcopy /Y "Web.config" "%IIS_PATH%\"

REM Copiar carpetas
xcopy /E /Y /I "Content" "%IIS_PATH%\Content"
xcopy /E /Y /I "bin" "%IIS_PATH%\bin"
xcopy /E /Y /I "App_Data" "%IIS_PATH%\App_Data"
xcopy /E /Y /I "Admin" "%IIS_PATH%\Admin"
xcopy /E /Y /I "Data" "%IIS_PATH%\Data"
xcopy /E /Y /I "Models" "%IIS_PATH%\Models"

echo.
echo =====================================================
echo Despliegue completado!
echo =====================================================
echo.
echo IMPORTANTE: Asegurate de:
echo 1. Configurar el Application Pool a .NET 4.0
echo 2. Dar permisos de escritura a App_Data
echo.
pause
