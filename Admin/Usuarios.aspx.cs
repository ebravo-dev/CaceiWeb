using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CaceiWeb.Data;
using CaceiWeb.Models;

namespace CaceiWeb.Admin
{
    public partial class Usuarios : Page
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
                CargarUsuarios();
                
                // Mostrar mensaje si existe
                if (Request.QueryString["msg"] != null)
                {
                    string msg = Request.QueryString["msg"];
                    if (msg == "created")
                        MostrarMensaje("Usuario creado exitosamente.", "success");
                    else if (msg == "updated")
                        MostrarMensaje("Usuario actualizado exitosamente.", "success");
                }
            }
        }

        private void CargarUsuarios()
        {
            List<Usuario> usuarios = DatabaseHelper.ObtenerUsuarios();
            
            if (usuarios.Count > 0)
            {
                rptUsuarios.DataSource = usuarios;
                rptUsuarios.DataBind();
                pnlSinDatos.Visible = false;
            }
            else
            {
                rptUsuarios.Visible = false;
                pnlSinDatos.Visible = true;
            }
        }

        protected string GetAcademiasString(Usuario usuario)
        {
            if (usuario.Academias == null || usuario.Academias.Count == 0)
                return "<span class='text-muted'>-</span>";
            
            return string.Join(", ", usuario.Academias.Select(a => a.Clave));
        }

        protected void rptUsuarios_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);
            
            if (e.CommandName == "Desactivar")
            {
                DatabaseHelper.DesactivarUsuario(id);
                MostrarMensaje("Usuario desactivado.", "success");
            }
            else if (e.CommandName == "Activar")
            {
                DatabaseHelper.ActivarUsuario(id);
                MostrarMensaje("Usuario activado.", "success");
            }
            
            CargarUsuarios();
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            pnlMensaje.Visible = true;
            pnlMensaje.CssClass = "alert alert-" + tipo;
            litMensaje.Text = mensaje;
        }
    }
}
