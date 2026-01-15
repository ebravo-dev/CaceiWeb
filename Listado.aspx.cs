using System;
using System.Web.UI.WebControls;
using CaceiWeb.Data;

namespace CaceiWeb
{
    public partial class Listado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarRegistros();
            }
        }

        private void CargarRegistros()
        {
            try
            {
                DatabaseHelper.InitializeDatabase();
                var registros = DatabaseHelper.ObtenerRegistros();
                gvRegistros.DataSource = registros;
                gvRegistros.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar registros: {ex.Message}", false);
            }
        }

        protected void gvRegistros_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Eliminar")
            {
                try
                {
                    int id = Convert.ToInt32(e.CommandArgument);
                    DatabaseHelper.EliminarRegistro(id);
                    MostrarMensaje("✅ Registro eliminado correctamente.", true);
                    CargarRegistros();
                }
                catch (Exception ex)
                {
                    MostrarMensaje($"❌ Error al eliminar: {ex.Message}", false);
                }
            }
        }

        protected void btnRefrescar_Click(object sender, EventArgs e)
        {
            CargarRegistros();
            MostrarMensaje("✅ Listado actualizado.", true);
        }

        private void MostrarMensaje(string mensaje, bool esExito)
        {
            pnlMensaje.Visible = true;
            lblMensaje.Text = mensaje;
            lblMensaje.CssClass = esExito ? "alert alert-success" : "alert alert-error";
        }
    }
}
