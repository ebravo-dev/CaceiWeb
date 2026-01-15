using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CaceiWeb.Data;
using CaceiWeb.Models;

namespace CaceiWeb.Admin
{
    public partial class Materias : Page
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

            if (!IsPostBack)
            {
                CargarAcademias();
                CargarMaterias();
            }
        }

        private void CargarAcademias()
        {
            List<Academia> academias = DatabaseHelper.ObtenerAcademias();
            ddlAcademia.Items.Clear();
            ddlAcademia.Items.Add(new ListItem("-- Sin academia --", ""));
            foreach (var academia in academias)
            {
                ddlAcademia.Items.Add(new ListItem(
                    string.Format("{0} ({1})", academia.Nombre, academia.Clave), 
                    academia.Id.ToString()));
            }
        }

        private void CargarMaterias()
        {
            List<Materia> materias = DatabaseHelper.ObtenerMaterias();
            
            if (materias.Count > 0)
            {
                rptMaterias.DataSource = materias;
                rptMaterias.DataBind();
                rptMaterias.Visible = true;
                pnlSinDatos.Visible = false;
            }
            else
            {
                rptMaterias.Visible = false;
                pnlSinDatos.Visible = true;
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MostrarError("El nombre es requerido.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtClave.Text))
            {
                MostrarError("La clave es requerida.");
                return;
            }

            try
            {
                int? academiaId = null;
                if (!string.IsNullOrEmpty(ddlAcademia.SelectedValue))
                    academiaId = Convert.ToInt32(ddlAcademia.SelectedValue);

                if (!string.IsNullOrEmpty(hfId.Value))
                {
                    // Actualizar
                    Materia materia = new Materia
                    {
                        Id = Convert.ToInt32(hfId.Value),
                        Nombre = txtNombre.Text.Trim(),
                        Clave = txtClave.Text.Trim().ToUpper(),
                        AcademiaId = academiaId,
                        Activo = true
                    };
                    DatabaseHelper.ActualizarMateria(materia);
                    MostrarMensaje("Materia actualizada exitosamente.", "success");
                }
                else
                {
                    // Crear nueva
                    Materia materia = new Materia
                    {
                        Nombre = txtNombre.Text.Trim(),
                        Clave = txtClave.Text.Trim().ToUpper(),
                        AcademiaId = academiaId,
                        Activo = true
                    };
                    DatabaseHelper.InsertarMateria(materia);
                    MostrarMensaje("Materia creada exitosamente.", "success");
                }

                LimpiarFormulario();
                CargarMaterias();
            }
            catch (Exception ex)
            {
                MostrarError("Error al guardar: " + ex.Message);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        protected void rptMaterias_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);
            
            if (e.CommandName == "Editar")
            {
                Materia materia = DatabaseHelper.ObtenerMateriaPorId(id);
                if (materia != null)
                {
                    hfId.Value = materia.Id.ToString();
                    txtNombre.Text = materia.Nombre;
                    txtClave.Text = materia.Clave;
                    ddlAcademia.SelectedValue = materia.AcademiaId.HasValue ? materia.AcademiaId.Value.ToString() : "";
                    litFormTitulo.Text = "Editar Materia";
                    btnCancelar.Visible = true;
                }
            }
            else if (e.CommandName == "Desactivar")
            {
                DatabaseHelper.EliminarMateria(id);
                MostrarMensaje("Materia desactivada.", "success");
                CargarMaterias();
            }
            else if (e.CommandName == "Activar")
            {
                Materia materia = DatabaseHelper.ObtenerMateriaPorId(id);
                if (materia != null)
                {
                    materia.Activo = true;
                    DatabaseHelper.ActualizarMateria(materia);
                    MostrarMensaje("Materia activada.", "success");
                    CargarMaterias();
                }
            }
        }

        private void LimpiarFormulario()
        {
            hfId.Value = "";
            txtNombre.Text = "";
            txtClave.Text = "";
            ddlAcademia.SelectedIndex = 0;
            litFormTitulo.Text = "Nueva Materia";
            btnCancelar.Visible = false;
            pnlError.Visible = false;
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            pnlMensaje.Visible = true;
            pnlMensaje.CssClass = "alert alert-" + tipo;
            litMensaje.Text = mensaje;
        }

        private void MostrarError(string mensaje)
        {
            pnlError.Visible = true;
            litError.Text = mensaje;
        }
    }
}
