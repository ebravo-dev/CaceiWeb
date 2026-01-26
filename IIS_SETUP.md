# Configuración de CaceiWeb en IIS

## Requisitos Previos

- Windows Server o Windows 10/11 con IIS habilitado
- .NET Framework 4.5 instalado
- Visual Studio o MSBuild para compilar

## Pasos de Configuración

### 1. Instalar System.Data.SQLite

Descarga e instala el paquete NuGet o copia manualmente los DLLs:

```powershell
# En el directorio del proyecto
nuget restore
```

O descarga desde: https://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki

Necesitas copiar a la carpeta `bin`:
- `System.Data.SQLite.dll`
- `SQLite.Interop.dll` (x86 o x64 según tu AppPool)

### 2. Crear el Sitio en IIS

1. Abrir **IIS Manager** (`inetmgr`)
2. Click derecho en **Sites** → **Add Website**
3. Configurar:
   - **Site name**: `CaceiWeb`
   - **Physical path**: `C:\inetpub\wwwroot\CaceiWeb`
   - **Port**: 80 o el que desees

### 3. Configurar Application Pool

1. Ir a **Application Pools**
2. Selecciona el pool de CaceiWeb
3. Click derecho → **Advanced Settings**
4. Configurar:
   - **.NET CLR Version**: `v4.0`
   - **Managed Pipeline Mode**: `Integrated`
   - **Enable 32-Bit Applications**: `True` (si usas SQLite 32-bit)

### 4. Permisos de Carpeta App_Data

La base de datos SQLite necesita permisos de escritura:

```powershell
# En PowerShell como Administrador
icacls "C:\inetpub\wwwroot\CaceiWeb\App_Data" /grant "IIS AppPool\CaceiWeb:(OI)(CI)M"
```

### 5. Desplegar Archivos

Ejecuta el script de despliegue:

```cmd
deploy.bat
```

O copia manualmente los archivos al directorio IIS.

### 6. Verificar

1. Abre el navegador: `http://localhost/` o `http://tu-ip/`
2. Debería aparecer la página de login
3. Credenciales por defecto: `admin` / `potrillo`

## Solución de Problemas

### Error 500.19 - Configuration Error
- Verifica que .NET 4.5 está instalado
- Ejecuta: `aspnet_regiis -i` en el command prompt como Admin

### Error de SQLite - "Unable to load DLL 'SQLite.Interop.dll'"
- Verifica que tienes el DLL correcto (x86 o x64)
- Si el AppPool es 32-bit, usa el DLL x86

### Error de permisos en App_Data
- Ejecuta el comando icacls mencionado arriba
- O da permisos manualmente a `IIS_IUSRS`

### Ver errores detallados
- El Web.config ya tiene `customErrors mode="Off"`
- Revisa Event Viewer → Windows Logs → Application
