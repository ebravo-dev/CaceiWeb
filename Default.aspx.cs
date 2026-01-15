using System;
using CaceiWeb.Data;
using CaceiWeb.Models;

namespace CaceiWeb
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    // Inicializar la base de datos si no existe
                    DatabaseHelper.InitializeDatabase();
                    
                    // Cargar estadísticas
                    // lblTotalRegistros.Text = DatabaseHelper.ObtenerConteoRegistros().ToString();
                    // lblRegistrosHoy.Text = "0";

                    // Mostrar panel de admin si el usuario es administrador
                    if (Session["Usuario"] != null)
                    {
                        Usuario usuario = Session["Usuario"] as Usuario;
                        if (usuario != null && usuario.RolNombre.ToLower() == "admin")
                        {
                            pnlAdmin.Visible = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // En caso de error, no hacer nada
                }
            }
        }
    }
}
