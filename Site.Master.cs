using System;
using System.Web.UI;
using CaceiWeb.Models;

namespace CaceiWeb
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] != null)
            {
                Usuario usuario = Session["Usuario"] as Usuario;
                
                // Mostrar barra de usuario
                pnlUserBar.Visible = true;
                litNombreUsuario.Text = usuario.Nombre;
                litRolUsuario.Text = usuario.RolNombre.ToUpper();
                
                // Ocultar link de login
                pnlLoginLink.Visible = false;
                
                // Mostrar menús según rol
                switch (usuario.RolNombre.ToLower())
                {
                    case "admin":
                        pnlMenuAdmin.Visible = true;
                        break;
                    case "presidente":
                        pnlMenuPresidente.Visible = true;
                        break;
                    case "profesor":
                        pnlMenuProfesor.Visible = true;
                        break;
                }
            }
            else
            {
                pnlUserBar.Visible = false;
                pnlLoginLink.Visible = true;
                pnlMenuAdmin.Visible = false;
                pnlMenuPresidente.Visible = false;
                pnlMenuProfesor.Visible = false;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}
