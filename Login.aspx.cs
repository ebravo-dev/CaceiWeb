using System;
using System.Web;
using System.Web.UI;
using CaceiWeb.Data;
using CaceiWeb.Models;

namespace CaceiWeb
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Inicializar base de datos
            DatabaseHelper.InitializeDatabase();
            
            // Si ya está logueado, redirigir
            if (Session["Usuario"] != null)
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string correo = txtUsuario.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(password))
            {
                MostrarError("Por favor ingresa usuario y contraseña.");
                return;
            }

            Usuario usuario = DatabaseHelper.ValidarUsuario(correo, password);

            if (usuario != null)
            {
                Session["Usuario"] = usuario;
                Session["UsuarioId"] = usuario.Id;
                Session["UsuarioNombre"] = usuario.Nombre;
                Session["UsuarioRol"] = usuario.RolNombre;
                
                Response.Redirect("~/Default.aspx");
            }
            else
            {
                MostrarError("Usuario o contraseña incorrectos.");
            }
        }

        private void MostrarError(string mensaje)
        {
            pnlError.Visible = true;
            litError.Text = mensaje;
        }
    }
}
