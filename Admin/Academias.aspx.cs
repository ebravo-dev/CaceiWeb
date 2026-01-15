using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CaceiWeb.Data;
using CaceiWeb.Models;

namespace CaceiWeb.Admin
{
    public partial class Academias : Page
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
            }
        }

        private void CargarAcademias()
        {
            List<Academia> academias = DatabaseHelper.ObtenerTodasAcademias();
            
            if (academias.Count > 0)
            {
                rptAcademias.DataSource = academias;
                rptAcademias.DataBind();
                rptAcademias.Visible = true;
                pnlSinDatos.Visible = false;
            }
            else
            {
                rptAcademias.Visible = false;
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
                if (!string.IsNullOrEmpty(hfId.Value))
                {
                    // Actualizar
                    Academia academia = new Academia
                    {
                        Id = Convert.ToInt32(hfId.Value),
                        Nombre = txtNombre.Text.Trim(),
                        Clave = txtClave.Text.Trim().ToUpper(),
                        Activo = true
                    };
                    DatabaseHelper.ActualizarAcademia(academia);
                    MostrarMensaje("Academia actualizada exitosamente.", "success");
                }
                else
                {
                    // Crear nueva
                    Academia academia = new Academia
                    {
                        Nombre = txtNombre.Text.Trim(),
                        Clave = txtClave.Text.Trim().ToUpper(),
                        Activo = true
                    };
                    DatabaseHelper.InsertarAcademia(academia);
                    MostrarMensaje("Academia creada exitosamente.", "success");
                }

                LimpiarFormulario();
                CargarAcademias();
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

        protected void rptAcademias_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);
            
            if (e.CommandName == "Editar")
            {
                Academia academia = DatabaseHelper.ObtenerAcademiaPorId(id);
                if (academia != null)
                {
                    hfId.Value = academia.Id.ToString();
                    txtNombre.Text = academia.Nombre;
                    txtClave.Text = academia.Clave;
                    litFormTitulo.Text = "Editar Academia";
                    btnCancelar.Visible = true;
                }
            }
            else if (e.CommandName == "Desactivar")
            {
                DatabaseHelper.EliminarAcademia(id);
                MostrarMensaje("Academia desactivada.", "success");
                CargarAcademias();
            }
            else if (e.CommandName == "Activar")
            {
                Academia academia = DatabaseHelper.ObtenerAcademiaPorId(id);
                if (academia != null)
                {
                    academia.Activo = true;
                    DatabaseHelper.ActualizarAcademia(academia);
                    MostrarMensaje("Academia activada.", "success");
                    CargarAcademias();
                }
            }
        }

        private void LimpiarFormulario()
        {
            hfId.Value = "";
            txtNombre.Text = "";
            txtClave.Text = "";
            litFormTitulo.Text = "Nueva Academia";
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
