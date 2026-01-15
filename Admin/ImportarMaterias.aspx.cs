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
    public partial class ImportarMaterias : Page
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

                List<MateriaImport> registros = ParsearCSV(contenido);
                
                if (registros.Count == 0)
                {
                    MostrarError("El archivo está vacío o no tiene el formato correcto.");
                    return;
                }

                // Guardar en sesión para importar después
                Session["ImportMaterias"] = registros;

                // Mostrar preview
                MostrarPreview(registros);
            }
            catch (Exception ex)
            {
                MostrarError("Error al procesar el archivo: " + ex.Message);
            }
        }

        private List<MateriaImport> ParsearCSV(string contenido)
        {
            List<MateriaImport> registros = new List<MateriaImport>();
            string[] lineas = contenido.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            int lineaNum = 0;
            foreach (string linea in lineas)
            {
                lineaNum++;
                
                // Saltar encabezado
                if (lineaNum == 1 && linea.ToLower().Contains("nombre") && linea.ToLower().Contains("clave"))
                    continue;

                string[] campos = linea.Split(',');
                
                if (campos.Length < 3)
                {
                    registros.Add(new MateriaImport
                    {
                        Linea = lineaNum,
                        EsValido = false,
                        Error = "Formato inválido"
                    });
                    continue;
                }

                MateriaImport registro = new MateriaImport
                {
                    Linea = lineaNum,
                    Nombre = campos[0].Trim().Trim('"'),
                    Clave = campos[1].Trim().Trim('"').ToUpper(),
                    AcademiaClave = campos[2].Trim().Trim('"').ToUpper(),
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
                // Validar academia
                else if (string.IsNullOrWhiteSpace(registro.AcademiaClave))
                {
                    registro.EsValido = false;
                    registro.Error = "Academia vacía";
                }
                // Verificar si academia existe
                else
                {
                    Academia academia = DatabaseHelper.ObtenerAcademiaPorClave(registro.AcademiaClave);
                    if (academia == null)
                    {
                        registro.EsValido = false;
                        registro.Error = "Academia no existe";
                    }
                    else
                    {
                        registro.AcademiaId = academia.Id;
                    }
                }
                // Verificar si clave ya existe
                if (registro.EsValido && DatabaseHelper.ExisteClaveMATERIA(registro.Clave))
                {
                    registro.EsValido = false;
                    registro.Error = "Clave duplicada";
                }

                registros.Add(registro);
            }

            return registros;
        }

        private void MostrarPreview(List<MateriaImport> registros)
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
            List<MateriaImport> registros = Session["ImportMaterias"] as List<MateriaImport>;
            
            if (registros == null)
            {
                Response.Redirect("ImportarMaterias.aspx");
                return;
            }

            int importados = 0;

            foreach (var registro in registros.Where(r => r.EsValido))
            {
                try
                {
                    Materia materia = new Materia
                    {
                        Nombre = registro.Nombre,
                        Clave = registro.Clave,
                        AcademiaId = registro.AcademiaId,
                        Activo = true
                    };

                    DatabaseHelper.InsertarMateria(materia);
                    importados++;
                }
                catch
                {
                    // Continuar con el siguiente
                }
            }

            Session.Remove("ImportMaterias");

            pnlPreview.Visible = false;
            pnlResultado.Visible = true;
            litResultado.Text = string.Format("Se importaron {0} materia(s) exitosamente.", importados);
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Session.Remove("ImportMaterias");
            Response.Redirect("ImportarMaterias.aspx");
        }

        private void MostrarError(string mensaje)
        {
            pnlError.Visible = true;
            litError.Text = mensaje;
        }

        // Clase auxiliar para importación
        public class MateriaImport
        {
            public int Linea { get; set; }
            public string Nombre { get; set; }
            public string Clave { get; set; }
            public string AcademiaClave { get; set; }
            public int AcademiaId { get; set; }
            public bool EsValido { get; set; }
            public string Error { get; set; }
        }
    }
}
