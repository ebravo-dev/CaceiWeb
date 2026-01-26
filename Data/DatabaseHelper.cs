using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Data.SQLite;
using CaceiWeb.Models;

namespace CaceiWeb.Data
{
    /// <summary>
    /// Helper para operaciones con SQLite usando System.Data.SQLite
    /// </summary>
    public static class DatabaseHelper
    {
        private static string GetConnectionString()
        {
            string dbPath = HttpContext.Current.Server.MapPath("~/App_Data/cacei.db");
            return string.Format("Data Source={0};Version=3;", dbPath);
        }

        private static string GetDbPath()
        {
            return HttpContext.Current.Server.MapPath("~/App_Data/cacei.db");
        }

        /// <summary>
        /// Inicializa la base de datos y crea las tablas si no existen
        /// </summary>
        public static void InitializeDatabase()
        {
            string dbPath = GetDbPath();
            string directory = Path.GetDirectoryName(dbPath);
            
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                
                // Tabla Roles
                ExecuteNonQuery(connection, @"
                    CREATE TABLE IF NOT EXISTS Roles (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nombre TEXT UNIQUE NOT NULL
                    )");

                // Tabla Usuarios
                ExecuteNonQuery(connection, @"
                    CREATE TABLE IF NOT EXISTS Usuarios (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nombre TEXT NOT NULL,
                        Correo TEXT UNIQUE NOT NULL,
                        Password TEXT NOT NULL,
                        RolId INTEGER NOT NULL,
                        Activo INTEGER DEFAULT 1,
                        FechaCreacion TEXT NOT NULL,
                        FOREIGN KEY (RolId) REFERENCES Roles(Id)
                    )");

                // Tabla Academias
                ExecuteNonQuery(connection, @"
                    CREATE TABLE IF NOT EXISTS Academias (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nombre TEXT NOT NULL,
                        Clave TEXT UNIQUE NOT NULL,
                        Activo INTEGER DEFAULT 1
                    )");

                // Tabla UsuarioAcademias (relación muchos a muchos)
                ExecuteNonQuery(connection, @"
                    CREATE TABLE IF NOT EXISTS UsuarioAcademias (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UsuarioId INTEGER NOT NULL,
                        AcademiaId INTEGER NOT NULL,
                        FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id),
                        FOREIGN KEY (AcademiaId) REFERENCES Academias(Id),
                        UNIQUE(UsuarioId, AcademiaId)
                    )");

                // Tabla Materias
                ExecuteNonQuery(connection, @"
                    CREATE TABLE IF NOT EXISTS Materias (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nombre TEXT NOT NULL,
                        Clave TEXT UNIQUE NOT NULL,
                        AcademiaId INTEGER,
                        Activo INTEGER DEFAULT 1,
                        FOREIGN KEY (AcademiaId) REFERENCES Academias(Id)
                    )");

                // Tabla ProfesorMaterias
                ExecuteNonQuery(connection, @"
                    CREATE TABLE IF NOT EXISTS ProfesorMaterias (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UsuarioId INTEGER NOT NULL,
                        MateriaId INTEGER NOT NULL,
                        FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id),
                        FOREIGN KEY (MateriaId) REFERENCES Materias(Id),
                        UNIQUE(UsuarioId, MateriaId)
                    )");

                // Tabla Registros (legacy)
                ExecuteNonQuery(connection, @"
                    CREATE TABLE IF NOT EXISTS Registros (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nombre TEXT NOT NULL,
                        Matricula TEXT,
                        Carrera TEXT,
                        Semestre TEXT,
                        Comentarios TEXT,
                        FechaRegistro TEXT NOT NULL,
                        Activo INTEGER DEFAULT 1
                    )");

                // Insertar roles por defecto
                InsertarRolSiNoExiste(connection, "admin");
                InsertarRolSiNoExiste(connection, "presidente");
                InsertarRolSiNoExiste(connection, "profesor");

                // Crear usuario admin por defecto
                CrearUsuarioAdminSiNoExiste(connection);
            }
        }

        private static void ExecuteNonQuery(SQLiteConnection connection, string sql)
        {
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static void InsertarRolSiNoExiste(SQLiteConnection connection, string nombre)
        {
            string checkSql = "SELECT COUNT(*) FROM Roles WHERE Nombre = @Nombre";
            using (var command = new SQLiteCommand(checkSql, connection))
            {
                command.Parameters.AddWithValue("@Nombre", nombre);
                int count = Convert.ToInt32(command.ExecuteScalar());
                if (count == 0)
                {
                    string insertSql = "INSERT INTO Roles (Nombre) VALUES (@Nombre)";
                    using (var insertCommand = new SQLiteCommand(insertSql, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Nombre", nombre);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void CrearUsuarioAdminSiNoExiste(SQLiteConnection connection)
        {
            string checkSql = "SELECT COUNT(*) FROM Usuarios WHERE Correo = 'admin'";
            using (var command = new SQLiteCommand(checkSql, connection))
            {
                int count = Convert.ToInt32(command.ExecuteScalar());
                if (count == 0)
                {
                    // Obtener rol admin
                    string getRolSql = "SELECT Id FROM Roles WHERE Nombre = 'admin'";
                    int rolId;
                    using (var getRolCommand = new SQLiteCommand(getRolSql, connection))
                    {
                        rolId = Convert.ToInt32(getRolCommand.ExecuteScalar());
                    }

                    string insertSql = @"
                        INSERT INTO Usuarios (Nombre, Correo, Password, RolId, Activo, FechaCreacion)
                        VALUES ('Administrador', 'admin', 'potrillo', @RolId, 1, @Fecha)";
                    using (var insertCommand = new SQLiteCommand(insertSql, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@RolId", rolId);
                        insertCommand.Parameters.AddWithValue("@Fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        #region Roles

        public static List<Rol> ObtenerRoles()
        {
            var roles = new List<Rol>();
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = "SELECT * FROM Roles ORDER BY Nombre";
                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add(new Rol
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString()
                        });
                    }
                }
            }
            return roles;
        }

        public static Rol ObtenerRolPorId(int id)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = "SELECT * FROM Roles WHERE Id = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Rol
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        #endregion

        #region Usuarios

        public static Usuario ValidarUsuario(string correo, string password)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = @"
                    SELECT u.*, r.Nombre as RolNombre 
                    FROM Usuarios u 
                    INNER JOIN Roles r ON u.RolId = r.Id 
                    WHERE u.Correo = @Correo AND u.Password = @Password AND u.Activo = 1";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Correo", correo);
                    command.Parameters.AddWithValue("@Password", password);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var usuario = new Usuario
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                RolId = Convert.ToInt32(reader["RolId"]),
                                RolNombre = reader["RolNombre"].ToString(),
                                Activo = Convert.ToInt32(reader["Activo"]) == 1,
                                FechaCreacion = DateTime.Parse(reader["FechaCreacion"].ToString())
                            };
                            usuario.Academias = ObtenerAcademiasDeUsuario(usuario.Id);
                            return usuario;
                        }
                    }
                }
            }
            return null;
        }

        public static List<Usuario> ObtenerUsuarios()
        {
            var usuarios = new List<Usuario>();
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = @"
                    SELECT u.*, r.Nombre as RolNombre 
                    FROM Usuarios u 
                    INNER JOIN Roles r ON u.RolId = r.Id 
                    ORDER BY u.Nombre";
                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var usuario = new Usuario
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            Correo = reader["Correo"].ToString(),
                            RolId = Convert.ToInt32(reader["RolId"]),
                            RolNombre = reader["RolNombre"].ToString(),
                            Activo = Convert.ToInt32(reader["Activo"]) == 1,
                            FechaCreacion = DateTime.Parse(reader["FechaCreacion"].ToString())
                        };
                        usuarios.Add(usuario);
                    }
                }
            }
            // Cargar academias para cada usuario
            foreach (var usuario in usuarios)
            {
                usuario.Academias = ObtenerAcademiasDeUsuario(usuario.Id);
            }
            return usuarios;
        }

        public static Usuario ObtenerUsuarioPorId(int id)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = @"
                    SELECT u.*, r.Nombre as RolNombre 
                    FROM Usuarios u 
                    INNER JOIN Roles r ON u.RolId = r.Id 
                    WHERE u.Id = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var usuario = new Usuario
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                Password = reader["Password"].ToString(),
                                RolId = Convert.ToInt32(reader["RolId"]),
                                RolNombre = reader["RolNombre"].ToString(),
                                Activo = Convert.ToInt32(reader["Activo"]) == 1,
                                FechaCreacion = DateTime.Parse(reader["FechaCreacion"].ToString())
                            };
                            usuario.Academias = ObtenerAcademiasDeUsuario(usuario.Id);
                            return usuario;
                        }
                    }
                }
            }
            return null;
        }

        public static int InsertarUsuario(Usuario usuario)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = @"
                    INSERT INTO Usuarios (Nombre, Correo, Password, RolId, Activo, FechaCreacion)
                    VALUES (@Nombre, @Correo, @Password, @RolId, @Activo, @FechaCreacion);
                    SELECT last_insert_rowid();";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@Correo", usuario.Correo);
                    command.Parameters.AddWithValue("@Password", usuario.Password);
                    command.Parameters.AddWithValue("@RolId", usuario.RolId);
                    command.Parameters.AddWithValue("@Activo", usuario.Activo ? 1 : 0);
                    command.Parameters.AddWithValue("@FechaCreacion", usuario.FechaCreacion.ToString("yyyy-MM-dd HH:mm:ss"));
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public static void ActualizarUsuario(Usuario usuario)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = @"
                    UPDATE Usuarios SET 
                        Nombre = @Nombre,
                        Correo = @Correo,
                        RolId = @RolId,
                        Activo = @Activo
                    WHERE Id = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@Correo", usuario.Correo);
                    command.Parameters.AddWithValue("@RolId", usuario.RolId);
                    command.Parameters.AddWithValue("@Activo", usuario.Activo ? 1 : 0);
                    command.Parameters.AddWithValue("@Id", usuario.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void ActualizarPasswordUsuario(int id, string password)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = "UPDATE Usuarios SET Password = @Password WHERE Id = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Password", password);
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DesactivarUsuario(int id)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = "UPDATE Usuarios SET Activo = 0 WHERE Id = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void ActivarUsuario(int id)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = "UPDATE Usuarios SET Activo = 1 WHERE Id = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool ExisteCorreo(string correo, int? exceptoId = null)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = exceptoId.HasValue 
                    ? "SELECT COUNT(*) FROM Usuarios WHERE Correo = @Correo AND Id != @Id"
                    : "SELECT COUNT(*) FROM Usuarios WHERE Correo = @Correo";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Correo", correo);
                    if (exceptoId.HasValue)
                        command.Parameters.AddWithValue("@Id", exceptoId.Value);
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        #endregion

        #region Academias

        public static List<Academia> ObtenerAcademias()
        {
            var academias = new List<Academia>();
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = "SELECT * FROM Academias WHERE Activo = 1 ORDER BY Nombre";
                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        academias.Add(new Academia
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            Clave = reader["Clave"].ToString(),
                            Activo = Convert.ToInt32(reader["Activo"]) == 1
                        });
                    }
                }
            }
            return academias;
        }

        public static List<Academia> ObtenerTodasAcademias()
        {
            var academias = new List<Academia>();
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = "SELECT * FROM Academias ORDER BY Nombre";
                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        academias.Add(new Academia
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            Clave = reader["Clave"].ToString(),
                            Activo = Convert.ToInt32(reader["Activo"]) == 1
                        });
                    }
                }
            }
            return academias;
        }

        public static Academia ObtenerAcademiaPorId(int id)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = "SELECT * FROM Academias WHERE Id = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Academia
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                Clave = reader["Clave"].ToString(),
                                Activo = Convert.ToInt32(reader["Activo"]) == 1
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static Academia ObtenerAcademiaPorClave(string clave)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = "SELECT * FROM Academias WHERE Clave = @Clave";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Clave", clave);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Academia
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                Clave = reader["Clave"].ToString(),
                                Activo = Convert.ToInt32(reader["Activo"]) == 1
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static int InsertarAcademia(Academia academia)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = @"
                    INSERT INTO Academias (Nombre, Clave, Activo)
                    VALUES (@Nombre, @Clave, @Activo);
                    SELECT last_insert_rowid();";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", academia.Nombre);
                    command.Parameters.AddWithValue("@Clave", academia.Clave);
                    command.Parameters.AddWithValue("@Activo", academia.Activo ? 1 : 0);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public static void ActualizarAcademia(Academia academia)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = @"
                    UPDATE Academias SET 
                        Nombre = @Nombre,
                        Clave = @Clave,
                        Activo = @Activo
                    WHERE Id = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", academia.Nombre);
                    command.Parameters.AddWithValue("@Clave", academia.Clave);
                    command.Parameters.AddWithValue("@Activo", academia.Activo ? 1 : 0);
                    command.Parameters.AddWithValue("@Id", academia.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void EliminarAcademia(int id)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = "UPDATE Academias SET Activo = 0 WHERE Id = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Usuario-Academia

        public static List<Academia> ObtenerAcademiasDeUsuario(int usuarioId)
        {
            var academias = new List<Academia>();
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = @"
                    SELECT a.* FROM Academias a
                    INNER JOIN UsuarioAcademias ua ON a.Id = ua.AcademiaId
                    WHERE ua.UsuarioId = @UsuarioId AND a.Activo = 1";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            academias.Add(new Academia
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                Clave = reader["Clave"].ToString(),
                                Activo = Convert.ToInt32(reader["Activo"]) == 1
                            });
                        }
                    }
                }
            }
            return academias;
        }

        public static void AsignarAcademiasAUsuario(int usuarioId, List<int> academiaIds)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                
                // Eliminar asignaciones actuales
                string deleteSql = "DELETE FROM UsuarioAcademias WHERE UsuarioId = @UsuarioId";
                using (var command = new SQLiteCommand(deleteSql, connection))
                {
                    command.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    command.ExecuteNonQuery();
                }

                // Insertar nuevas asignaciones
                foreach (int academiaId in academiaIds)
                {
                    string insertSql = "INSERT INTO UsuarioAcademias (UsuarioId, AcademiaId) VALUES (@UsuarioId, @AcademiaId)";
                    using (var command = new SQLiteCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        command.Parameters.AddWithValue("@AcademiaId", academiaId);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion

        #region Materias

        public static List<Materia> ObtenerMaterias()
        {
            var materias = new List<Materia>();
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = @"
                    SELECT m.*, a.Nombre as AcademiaNombre 
                    FROM Materias m 
                    LEFT JOIN Academias a ON m.AcademiaId = a.Id 
                    WHERE m.Activo = 1 
                    ORDER BY m.Nombre";
                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        materias.Add(new Materia
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            Clave = reader["Clave"].ToString(),
                            AcademiaId = reader["AcademiaId"] != DBNull.Value ? Convert.ToInt32(reader["AcademiaId"]) : (int?)null,
                            AcademiaNombre = reader["AcademiaNombre"]?.ToString(),
                            Activo = Convert.ToInt32(reader["Activo"]) == 1
                        });
                    }
                }
            }
            return materias;
        }

        public static Materia ObtenerMateriaPorId(int id)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = @"
                    SELECT m.*, a.Nombre as AcademiaNombre 
                    FROM Materias m 
                    LEFT JOIN Academias a ON m.AcademiaId = a.Id 
                    WHERE m.Id = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Materia
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                Clave = reader["Clave"].ToString(),
                                AcademiaId = reader["AcademiaId"] != DBNull.Value ? Convert.ToInt32(reader["AcademiaId"]) : (int?)null,
                                AcademiaNombre = reader["AcademiaNombre"]?.ToString(),
                                Activo = Convert.ToInt32(reader["Activo"]) == 1
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static int InsertarMateria(Materia materia)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = @"
                    INSERT INTO Materias (Nombre, Clave, AcademiaId, Activo)
                    VALUES (@Nombre, @Clave, @AcademiaId, @Activo);
                    SELECT last_insert_rowid();";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", materia.Nombre);
                    command.Parameters.AddWithValue("@Clave", materia.Clave);
                    command.Parameters.AddWithValue("@AcademiaId", materia.AcademiaId.HasValue ? (object)materia.AcademiaId.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", materia.Activo ? 1 : 0);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public static void ActualizarMateria(Materia materia)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = @"
                    UPDATE Materias SET 
                        Nombre = @Nombre,
                        Clave = @Clave,
                        AcademiaId = @AcademiaId,
                        Activo = @Activo
                    WHERE Id = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", materia.Nombre);
                    command.Parameters.AddWithValue("@Clave", materia.Clave);
                    command.Parameters.AddWithValue("@AcademiaId", materia.AcademiaId.HasValue ? (object)materia.AcademiaId.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", materia.Activo ? 1 : 0);
                    command.Parameters.AddWithValue("@Id", materia.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void EliminarMateria(int id)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = "UPDATE Materias SET Activo = 0 WHERE Id = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Registros (Legacy)

        public static int InsertarRegistro(Registro registro)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = @"
                    INSERT INTO Registros (Nombre, Matricula, Carrera, Semestre, Comentarios, FechaRegistro, Activo)
                    VALUES (@Nombre, @Matricula, @Carrera, @Semestre, @Comentarios, @FechaRegistro, @Activo);
                    SELECT last_insert_rowid();";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", registro.Nombre ?? "");
                    command.Parameters.AddWithValue("@Matricula", registro.Matricula ?? "");
                    command.Parameters.AddWithValue("@Carrera", registro.Carrera ?? "");
                    command.Parameters.AddWithValue("@Semestre", registro.Semestre ?? "");
                    command.Parameters.AddWithValue("@Comentarios", registro.Comentarios ?? "");
                    command.Parameters.AddWithValue("@FechaRegistro", registro.FechaRegistro.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@Activo", registro.Activo ? 1 : 0);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public static List<Registro> ObtenerRegistros()
        {
            var registros = new List<Registro>();
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = "SELECT * FROM Registros WHERE Activo = 1 ORDER BY FechaRegistro DESC";
                using (var command = new SQLiteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        registros.Add(new Registro
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            Matricula = reader["Matricula"].ToString(),
                            Carrera = reader["Carrera"].ToString(),
                            Semestre = reader["Semestre"].ToString(),
                            Comentarios = reader["Comentarios"].ToString(),
                            FechaRegistro = DateTime.Parse(reader["FechaRegistro"].ToString()),
                            Activo = Convert.ToInt32(reader["Activo"]) == 1
                        });
                    }
                }
            }
            return registros;
        }

        public static int ObtenerConteoRegistros()
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = "SELECT COUNT(*) FROM Registros WHERE Activo = 1";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public static void EliminarRegistro(int id)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = "UPDATE Registros SET Activo = 0 WHERE Id = @Id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Importación CSV

        public static Rol ObtenerRolPorNombre(string nombre)
        {
            using (var connection = new SQLiteConnection(GetConnectionString()))
            {
                connection.Open();
                string sql = "SELECT * FROM Roles WHERE LOWER(Nombre) = LOWER(@Nombre)";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", nombre);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Rol
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
