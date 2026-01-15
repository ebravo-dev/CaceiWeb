using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CaceiWeb.Data;
using CaceiWeb.Models;

namespace CaceiWeb.Admin
{
    public partial class ImportarUsuarios : Page
    {
        private const string DEFAULT_PASSWORD = "cacei2024";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar que sea admin
            if (Session["Usuario"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            Usuario usuario = Session["Usuario"] as Usuario;
            if (usuario.RolNombre.ToLower() != "admin")
            {
                Response.Redirect("~/Default.aspx");
                return;
            }
        }

        protected void btnAnalizar_Click(object sender, EventArgs e)
        {
            if (!fuArchivo.HasFile)
            {
                MostrarError("Selecciona un archivo CSV.");
                return;
            }

            try
            {
                string contenido;
                using (StreamReader reader = new StreamReader(fuArchivo.PostedFile.InputStream))
                {
                    contenido = reader.ReadToEnd();
                }

                List<UsuarioImport> registros = ParsearCSV(contenido);
                
                if (registros.Count == 0)
                {
                    MostrarError("El archivo está vacío o no tiene el formato correcto.");
                    return;
                }

                // Guardar en sesión para importar después
                Session["ImportData"] = registros;

                // Mostrar preview
                MostrarPreview(registros);
            }
            catch (Exception ex)
            {
                MostrarError("Error al procesar el archivo: " + ex.Message);
            }
        }

        private List<UsuarioImport> ParsearCSV(string contenido)
        {
            List<UsuarioImport> registros = new List<UsuarioImport>();
            string[] lineas = contenido.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            int lineaNum = 0;
            foreach (string linea in lineas)
            {
                lineaNum++;
                
                // Saltar encabezado
                if (lineaNum == 1 && linea.ToLower().Contains("nombre") && linea.ToLower().Contains("correo"))
                    continue;

                string[] campos = linea.Split(',');
                
                if (campos.Length < 3)
                {
                    registros.Add(new UsuarioImport
                    {
                        Linea = lineaNum,
                        EsValido = false,
                        Error = "Formato inválido"
                    });
                    continue;
                }

                UsuarioImport registro = new UsuarioImport
                {
                    Linea = lineaNum,
                    Nombre = campos[0].Trim().Trim('"'),
                    Correo = campos[1].Trim().Trim('"'),
                    Rol = campos[2].Trim().Trim('"').ToLower(),
                    // Las academias pueden estar separadas por punto y coma dentro del campo
                    Academias = campos.Length > 3 ? campos[3].Trim().Trim('"').Replace(";", ",") : "",
                    EsValido = true
                };

                // Validar nombre
                if (string.IsNullOrWhiteSpace(registro.Nombre))
                {
                    registro.EsValido = false;
                    registro.Error = "Nombre vacío";
                }
                // Validar correo
                else if (string.IsNullOrWhiteSpace(registro.Correo) || !registro.Correo.Contains("@"))
                {
                    registro.EsValido = false;
                    registro.Error = "Correo inválido";
                }
                // Validar rol
                else if (registro.Rol != "admin" && registro.Rol != "presidente" && registro.Rol != "profesor")
                {
                    registro.EsValido = false;
                    registro.Error = "Rol inválido";
                }
                // Verificar si correo ya existe
                else if (DatabaseHelper.ExisteCorreo(registro.Correo))
                {
                    registro.EsValido = false;
                    registro.Error = "Correo duplicado";
                }

                registros.Add(registro);
            }

            return registros;
        }

        private void MostrarPreview(List<UsuarioImport> registros)
        {
            pnlUpload.Visible = false;
            pnlPreview.Visible = true;

            rptPreview.DataSource = registros;
            rptPreview.DataBind();

            int total = registros.Count;
            int validos = registros.Count(r => r.EsValido);
            int errores = total - validos;

            litTotal.Text = total.ToString();
            litValidos.Text = validos.ToString();
            litErrores.Text = errores.ToString();

            if (errores > 0)
            {
                pnlWarnings.Visible = true;
                litWarnings.Text = string.Format(" {0} registro(s) con errores serán omitidos.", errores);
            }

            btnImportar.Enabled = validos > 0;
        }

        protected void btnImportar_Click(object sender, EventArgs e)
        {
            List<UsuarioImport> registros = Session["ImportData"] as List<UsuarioImport>;
            
            if (registros == null)
            {
                Response.Redirect("ImportarUsuarios.aspx");
                return;
            }

            int importados = 0;

            foreach (var registro in registros.Where(r => r.EsValido))
            {
                try
                {
                    // Obtener rol
                    Rol rol = DatabaseHelper.ObtenerRolPorNombre(registro.Rol);
                    if (rol == null) continue;

                    // Crear usuario
                    Usuario usuario = new Usuario
                    {
                        Nombre = registro.Nombre,
                        Correo = registro.Correo,
                        Password = DEFAULT_PASSWORD,
                        RolId = rol.Id,
                        Activo = true
                    };

                    int nuevoId = DatabaseHelper.InsertarUsuario(usuario);

                    // Asignar academias
                    if (!string.IsNullOrWhiteSpace(registro.Academias))
                    {
                        List<int> academiaIds = new List<int>();
                        string[] claves = registro.Academias.Split(',');
                        
                        foreach (string clave in claves)
                        {
                            Academia academia = DatabaseHelper.ObtenerAcademiaPorClave(clave.Trim());
                            if (academia != null)
                                academiaIds.Add(academia.Id);
                        }

                        if (academiaIds.Count > 0)
                            DatabaseHelper.AsignarAcademiasAUsuario(nuevoId, academiaIds);
                    }

                    importados++;
                }
                catch
                {
                    // Continuar con el siguiente
                }
            }

            Session.Remove("ImportData");

            pnlPreview.Visible = false;
            pnlResultado.Visible = true;
            litResultado.Text = string.Format("Se importaron {0} usuario(s) exitosamente. La contraseña por defecto es: <strong>{1}</strong>", 
                importados, DEFAULT_PASSWORD);
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Session.Remove("ImportData");
            Response.Redirect("ImportarUsuarios.aspx");
        }

        private void MostrarError(string mensaje)
        {
            pnlError.Visible = true;
            litError.Text = mensaje;
        }

        // Clase auxiliar para importación
        public class UsuarioImport
        {
            public int Linea { get; set; }
            public string Nombre { get; set; }
            public string Correo { get; set; }
            public string Rol { get; set; }
            public string Academias { get; set; }
            public bool EsValido { get; set; }
            public string Error { get; set; }
        }
    }
}
