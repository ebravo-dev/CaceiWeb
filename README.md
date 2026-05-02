# 🏛️ CaceiWeb — Sistema de Evaluación para Acreditación

<div align="center">

[![ASP.NET](https://img.shields.io/badge/ASP.NET_Web_Forms-512BD4?style=for-the-badge&logo=.net&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite&logoColor=white)](https://www.sqlite.org/)

**Sistema web para la gestión y evaluación de atributos educativos en procesos de acreditación (CACEI).**

[Características](#-características) · [Arquitectura](#-arquitectura) · [Instalación](#-instalación)

</div>

---

## 💡 ¿Qué es CaceiWeb?

**CaceiWeb** es un sistema web desarrollado para apoyar los procesos de acreditación de programas educativos bajo el modelo **CACEI** (Consejo de Acreditación de la Enseñanza de la Ingeniería). El sistema permite gestionar la información académica, administrar usuarios con diferentes roles, y realizar la evaluación de atributos de calidad.

> 🎯 **Objetivo:** Centralizar la gestión de academias, materias, usuarios y registros para facilitar los procesos de acreditación institucional.

---

## ✨ Características

### 👥 Gestión de Usuarios y Roles
- **Roles definidos:** Administrador, Presidente de Academia, Profesor, Coordinador.
- Alta, edición, activación/desactivación de usuarios.
- Asignación de academias a usuarios según su rol.

### 🏛️ Gestión de Academias
- Registro de academias con nombre y clave.
- Importación masiva de academias desde archivo **CSV**.
- Listado y administración completa.

### 📚 Gestión de Materias
- Registro de materias asociadas a academias.
- Clave identificadora por materia.
- Importación masiva desde **CSV**.

### 📝 Captura de Registros
- Formulario de captura con información del alumno:
  - Nombre completo
  - Matrícula
  - Carrera
  - Semestre
  - Comentarios adicionales
- Listado de registros con opción de eliminación.
- Formato de fecha legible.

### 📁 Importación Masiva (CSV)
- Importación de **usuarios** desde CSV.
- Importación de **academias** desde CSV.
- Importación de **materias** desde CSV.

### 📊 Panel de Administración
- Dashboard centralizado con acceso rápido a todos los módulos.
- Interfaz responsive con menú de navegación.
- Sistema de alertas y mensajes de confirmación.

---

## 🏗️ Arquitectura

```text
CaceiWeb/
├── Admin/
│   ├── Usuarios.aspx              # Gestión de usuarios
│   ├── Academias.aspx             # Gestión de academias
│   ├── Materias.aspx              # Gestión de materias
│   ├── ImportarUsuarios.aspx      # Import CSV de usuarios
│   ├── ImportarAcademias.aspx     # Import CSV de academias
│   ├── ImportarMaterias.aspx      # Import CSV de materias
│   └── UsuarioForm.aspx           # Formulario de usuario
├── App_Code/
│   ├── Academia.cs
│   ├── Materia.cs
│   ├── Registro.cs
│   ├── Rol.cs
│   └── Usuario.cs
├── Models/
│   ├── Academia.cs                # Modelo de academia
│   ├── Materia.cs                 # Modelo de materia
│   ├── Registro.cs                # Modelo de registro/alumno
│   ├── Rol.cs                     # Modelo de rol
│   └── Usuario.cs                 # Modelo de usuario
├── Data/
│   └── DatabaseHelper.cs          # Helper de base de datos SQLite
├── Captura.aspx                   # Formulario de captura
├── Listado.aspx                   # Listado de registros
├── Default.aspx                   # Página principal / Dashboard
├── Login.aspx                     # Inicio de sesión
└── Site.Master                    # Master page (layout)
```

### Tech Stack
- **ASP.NET Web Forms** — Framework web de Microsoft
- **C#** — Lenguaje de programación backend
- **SQLite** — Base de datos local ligera
- **ADO.NET** — Acceso a datos
- **HTML/CSS/JS** — Frontend con Web Forms

---

## 🚀 Instalación

### Requisitos
- **IIS** (Internet Information Services) o servidor compatible con ASP.NET 4.5+
- **.NET Framework 4.5**
- **SQLite** (System.Data.SQLite)

### Pasos de instalación

```bash
# 1. Clonar el repositorio
git clone https://github.com/ebravo-dev/CaceiWeb.git

# 2. Publicar en IIS
# Copiar la carpeta del proyecto al directorio wwwroot de IIS
# Configurar el sitio web apuntando a la carpeta

# 3. Configurar la base de datos
# Asegurar que el archivo cacei.db exista en App_Data/
# Verificar permisos de lectura/escritura en la carpeta App_Data

# 4. Acceder al sistema
# http://localhost/CaceiWeb/Login.aspx
```

### Configuración de IIS
Ver archivo `IIS_SETUP.md` para instrucciones detalladas.

---

## 📊 Roles de Usuario

| Rol | Permisos |
|-----|----------|
| **Administrador** | Acceso total: usuarios, academias, materias, registros |
| **Presidente de Academia** | Gestión de su academia asignada |
| **Profesor** | Acceso a materias asignadas, captura de evaluaciones |
| **Coordinador** | Supervisión y reportes |

---

## 🛣️ Roadmap

- [x] Gestión de usuarios con roles
- [x] Gestión de academias y materias
- [x] Captura de registros de alumnos
- [x] Importación masiva por CSV
- [ ] Módulo de evaluación de atributos
- [ ] Indicadores y criterios de evaluación
- [ ] Gestión de grupos y ciclos escolares
- [ ] Exportación de evaluaciones por grupo
- [ ] Dashboard de reportes y estadísticas
- [ ] Integración con sistemas académicos externos

---

## 📄 Licencia

Proyecto institucional.  
Desarrollado por [Eder J. G. Bravo](https://github.com/ebravo-dev).

---

> *"Hecho para facilitar la acreditación educativa."* 🎓
