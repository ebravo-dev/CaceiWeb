using System;
using CaceiWeb.Data;
using CaceiWeb.Models;

namespace CaceiWeb
{
    public partial class Captura : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Inicializar la base de datos si no existe
                DatabaseHelper.InitializeDatabase();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar campo requerido
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MostrarMensaje("El nombre es requerido.", false);
                    return;
                }

                // Crear el registro
                var registro = new Registro
                {
                    Nombre = txtNombre.Text.Trim(),
                    Matricula = txtMatricula.Text.Trim(),
                    Carrera = txtCarrera.Text.Trim(),
                    Semestre = ddlSemestre.SelectedValue,
                    Comentarios = txtComentarios.Text.Trim(),
                    FechaRegistro = DateTime.Now,
                    Activo = true
                };

                // Guardar en la base de datos
                int id = DatabaseHelper.InsertarRegistro(registro);

                // Limpiar formulario y mostrar mensaje
                LimpiarFormulario();
                MostrarMensaje($"✅ Registro guardado exitosamente (ID: {id})", true);
            }
            catch (Exception ex)
            {
                MostrarMensaje($"❌ Error al guardar: {ex.Message}", false);
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            pnlMensaje.Visible = false;
        }

        private void LimpiarFormulario()
        {
            txtNombre.Text = "";
            txtMatricula.Text = "";
            txtCarrera.Text = "";
            ddlSemestre.SelectedIndex = 0;
            txtComentarios.Text = "";
        }

        private void MostrarMensaje(string mensaje, bool esExito)
        {
            pnlMensaje.Visible = true;
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = esExito ? "alert alert-success" : "alert alert-error";
        }
    }
}
