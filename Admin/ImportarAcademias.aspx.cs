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
    public partial class ImportarAcademias : Page
    {
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

                List<AcademiaImport> registros = ParsearCSV(contenido);
                
                if (registros.Count == 0)
                {
                    MostrarError("El archivo está vacío o no tiene el formato correcto.");
                    return;
                }

                // Guardar en sesión para importar después
                Session["ImportAcademias"] = registros;

                // Mostrar preview
                MostrarPreview(registros);
            }
            catch (Exception ex)
            {
                MostrarError("Error al procesar el archivo: " + ex.Message);
            }
        }

        private List<AcademiaImport> ParsearCSV(string contenido)
        {
            List<AcademiaImport> registros = new List<AcademiaImport>();
            string[] lineas = contenido.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            int lineaNum = 0;
            foreach (string linea in lineas)
            {
                lineaNum++;
                
                // Saltar encabezado
                if (lineaNum == 1 && linea.ToLower().Contains("nombre") && linea.ToLower().Contains("clave"))
                    continue;

                string[] campos = linea.Split(',');
                
                if (campos.Length < 2)
                {
                    registros.Add(new AcademiaImport
                    {
                        Linea = lineaNum,
                        EsValido = false,
                        Error = "Formato inválido"
                    });
                    continue;
                }

                AcademiaImport registro = new AcademiaImport
                {
                    Linea = lineaNum,
                    Nombre = campos[0].Trim().Trim('"'),
                    Clave = campos[1].Trim().Trim('"').ToUpper(),
                    EsValido = true
                };

                // Validar nombre
                if (string.IsNullOrWhiteSpace(registro.Nombre))
                {
                    registro.EsValido = false;
                    registro.Error = "Nombre vacío";
                }
                // Validar clave
                else if (string.IsNullOrWhiteSpace(registro.Clave))
                {
                    registro.EsValido = false;
                    registro.Error = "Clave vacía";
                }
                // Verificar si clave ya existe
                else if (ExisteClaveAcademia(registro.Clave))
                {
                    registro.EsValido = false;
                    registro.Error = "Clave duplicada";
                }

                registros.Add(registro);
            }

            return registros;
        }

        private bool ExisteClaveAcademia(string clave)
        {
            Academia academia = DatabaseHelper.ObtenerAcademiaPorClave(clave);
            return academia != null;
        }

        private void MostrarPreview(List<AcademiaImport> registros)
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
            List<AcademiaImport> registros = Session["ImportAcademias"] as List<AcademiaImport>;
            
            if (registros == null)
            {
                Response.Redirect("ImportarAcademias.aspx");
                return;
            }

            int importados = 0;

            foreach (var registro in registros.Where(r => r.EsValido))
            {
                try
                {
                    Academia academia = new Academia
                    {
                        Nombre = registro.Nombre,
                        Clave = registro.Clave,
                        Activo = true
                    };

                    DatabaseHelper.InsertarAcademia(academia);
                    importados++;
                }
                catch
                {
                    // Continuar con el siguiente
                }
            }

            Session.Remove("ImportAcademias");

            pnlPreview.Visible = false;
            pnlResultado.Visible = true;
            litResultado.Text = string.Format("Se importaron {0} academia(s) exitosamente.", importados);
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Session.Remove("ImportAcademias");
            Response.Redirect("ImportarAcademias.aspx");
        }

        private void MostrarError(string mensaje)
        {
            pnlError.Visible = true;
            litError.Text = mensaje;
        }

        // Clase auxiliar para importación
        public class AcademiaImport
        {
            public int Linea { get; set; }
            public string Nombre { get; set; }
            public string Clave { get; set; }
            public bool EsValido { get; set; }
            public string Error { get; set; }
        }
    }
}
